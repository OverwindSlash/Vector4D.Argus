﻿#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Transport;
using RosSharp.Utility;

namespace RosSharp.Topic
{
    internal sealed class RosTopicServer<TMessage> : IDisposable where TMessage : IMessage, new()
    {
        private TcpRosClient _client;
        public Uri SlaveUri { get; private set; }


        public RosTopicServer(string nodeId, string topicName, Uri slaveUri)
        {
            NodeId = nodeId;
            TopicName = topicName;
            SlaveUri = slaveUri;
        }

        public string NodeId { get; private set; }
        public string TopicName { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            _client.Dispose();
        }

        #endregion

        public Task<IObservable<TMessage>> StartAsync(TopicParam param, bool nodelay = true)
        {
            _client = new TcpRosClient();

            var tcs = new TaskCompletionSource<IObservable<TMessage>>();
            _client.ConnectAsync(param.HostName, param.PortNumber)
                .ContinueWith(connectTask =>
                {
                    LogHelper.Debug("StartAsync Connected");
                    if (connectTask.Status == TaskStatus.RanToCompletion)
                    {
                        try
                        {
                            ConnectToPublisherAsync(nodelay).ContinueWith(connectedTask =>
                            {
                                LogHelper.Debug("StartAsync ConnectToPublisherAsync");
                                if (connectedTask.Status == TaskStatus.RanToCompletion)
                                {
                                    tcs.SetResult(connectedTask.Result);
                                }
                                else if (connectedTask.Status == TaskStatus.Faulted)
                                {
                                    tcs.SetException(connectedTask.Exception.InnerException);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("Connect Error", ex);
                            tcs.SetException(ex);
                        }
                    }
                    else if (connectTask.Status == TaskStatus.Faulted)
                    {
                        tcs.SetException(connectTask.Exception.InnerException);
                    }
                });

            return tcs.Task;
        }


        private Task<IObservable<TMessage>> ConnectToPublisherAsync(bool nodelay)
        {
            var last = _client.ReceiveAsync()
                .Take(1)
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .PublishLast();

            last.Connect();

            var dummy = new TMessage();
            var sendHeader = new
            {
                callerid = NodeId,
                topic = TopicName,
                md5sum = dummy.Md5Sum,
                type = dummy.MessageType,
                tcp_nodelay = nodelay ? "1" : "0"
            };

            var stream = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(stream, sendHeader);

            var tcs = new TaskCompletionSource<IObservable<TMessage>>();
            _client.SendAsync(stream.ToArray())
                .ContinueWith(task =>
                {
                    LogHelper.Debug("ConnectToPublisherAsync Sent");
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        try
                        {
                            var recvHeader = last.Timeout(TimeSpan.FromMilliseconds(Ros.TopicTimeout)).First();
                            tcs.SetResult(CreateSubscriber(recvHeader));
                        }
                        catch (TimeoutException ex)
                        {
                            LogHelper.Error("Receive Header Timeout Error", ex);
                            tcs.SetException(ex);
                        }
                        catch (RosTopicException ex)
                        {
                            LogHelper.Error("Header Deserialize Error", ex);
                            tcs.SetException(ex);
                        }
                    }
                    else if (task.Status == TaskStatus.Faulted)
                    {
                        tcs.SetException(task.Exception.InnerException);
                    }
                });

            return tcs.Task;
        }

        private IObservable<TMessage> CreateSubscriber(dynamic header)
        {
            LogHelper.Debug("CreateSubscriber");
            var dummy = new TMessage();

            /* roscpp don't have a topic member */
            if (header.HasMember("topic") && header.topic != TopicName)
            {
                LogHelper.ErrorFormat( "TopicName mismatch error, expected={0} actual={1}", TopicName, header.topic);
                throw new RosTopicException("TopicName mismatch error");
            }
            if (header.type != dummy.MessageType)
            {
                LogHelper.ErrorFormat( "TopicType mismatch error, expected={0} actual={1}", dummy.MessageType, header.type);
                throw new RosTopicException("TopicType mismatch error");
            }
            if (header.md5sum != "*" && header.md5sum != dummy.Md5Sum)
            {
                LogHelper.ErrorFormat( "MD5Sum mismatch error, expected={0} actual={1}", dummy.Md5Sum, header.md5sum);
                throw new RosTopicException("MD5Sum mismatch error");
            }

            return _client.ReceiveAsync().Select(Deserialize);
        }

        private TMessage Deserialize(byte[] x)
        {
            var data = new TMessage();
            var br = new BinaryReader(new MemoryStream(x));
            var len = br.ReadInt32();
            if (br.BaseStream.Length != len + 4)
            {
                LogHelper.Error("Received Invalid Message");
                throw new RosTopicException("Received Invalid Message");
            }
            data.Deserialize(br);
            return data;
        }
    }
}