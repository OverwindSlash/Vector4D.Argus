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
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using RosSharp.Message;
using System.Linq;
using System.Net;
using System.Threading;
using RosSharp.Utility;
using System.IO;
using RosSharp.Slave;

namespace RosSharp.Topic
{
    /// <summary>
    ///   Publishes message on a ROS Topic
    /// </summary>
    /// <typeparam name="TMessage"> Message Type </typeparam>
    public sealed class Publisher<TMessage> : IPublisher, IObserver<TMessage> where TMessage : IMessage, new()
    {
        private readonly BehaviorSubject<int> _connectionCounterSubject = new BehaviorSubject<int>(0);
        private readonly bool _latching;
        private readonly List<RosTopicClient<TMessage>> _rosTopicClients = new List<RosTopicClient<TMessage>>();
        private readonly List<RosTopicServer<TMessage>> _rosTopicServers = new List<RosTopicServer<TMessage>>();
        private TMessage _lastPublishedMessage;
        private string slaveServerUrl;


        internal Publisher(string topicName, string nodeId, string slaveServerUrl, bool latching = false)
        {
            NodeId = nodeId;
            var dummy = new TMessage();
            TopicName = topicName;
            MessageType = dummy.MessageType;
            this.slaveServerUrl = slaveServerUrl;
            _latching = latching;
        }

        public string NodeId { get; private set; }

        #region IObserver<TMessage> Members

        public void OnNext(TMessage value)
        {
            lock (_rosTopicClients) //ロック範囲が広い？
            {
                var failedClients = new List<RosTopicClient<TMessage>>();

                foreach (var client in _rosTopicClients)
                {
                    try
                    {
                        client.SendAsync(value).Wait();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("SendError", ex);
                        failedClients.Add(client);
                    }
                }

                foreach (var c in failedClients)
                {
                    _rosTopicClients.Remove(c);
                }

                _lastPublishedMessage = value;
            }
        }

        public void OnError(Exception error)
        {
            LogHelper.Error("OnError", error);
        }

        public void OnCompleted()
        {
            LogHelper.Info("OnCompleted");
        }

        #endregion

        #region IPublisher Members

        public Task DisposeAsync()
        {
            lock (_rosTopicClients)
            {
                foreach (var topic in _rosTopicClients)
                {
                    topic.Dispose();
                }
                _rosTopicClients.Clear();
            }

            var handler = Disposing;
            Disposing = null;
            return handler(TopicName);
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        public string TopicName { get; private set; }

        public string MessageType { get; private set; }
        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        #endregion

        public IObservable<int> ConnectionChangedAsObservable()
        {
            return _connectionCounterSubject;
        }

        public void WaitForConnection()
        {
            ConnectionChangedAsObservable().Where(x => x > 0).First();
        }

        public void WaitForDisconnection()
        {
            ConnectionChangedAsObservable().Where(x => x == 0).First();
        }

        public void WaitForConnection(TimeSpan timeout)
        {
            ConnectionChangedAsObservable().Where(x => x > 0).Timeout(timeout).First();
        }

        public void WaitForDisconnection(TimeSpan timeout)
        {
            ConnectionChangedAsObservable().Where(x => x == 0).Timeout(timeout).First();
        }


        internal Task AddTopic(Socket socket)
        {
            var rosTopicClient = new RosTopicClient<TMessage>(NodeId, TopicName);

            return rosTopicClient.StartAsync(socket, _latching).ContinueWith(startTask =>
              {
                  if (startTask.Status == TaskStatus.RanToCompletion)
                  {
                      LogHelper.Debug("AddTopic: Started");
                      startTask.Result.Subscribe(
                          _ => { },
                          ex =>
                          {
                              lock (_rosTopicClients)
                              {
                                  _rosTopicClients.Remove(rosTopicClient);
                                  _connectionCounterSubject.OnNext(_rosTopicClients.Count);
                              }
                          },
                          () =>
                          {
                              lock (_rosTopicClients)
                              {
                                  _rosTopicClients.Remove(rosTopicClient);
                                  _connectionCounterSubject.OnNext(_rosTopicClients.Count);
                              }
                          }
                          );

                      lock (_rosTopicClients)
                      {
                          _rosTopicClients.Add(rosTopicClient);
                      }

                      if (_latching && _lastPublishedMessage != null)
                      {
                          OnNext(_lastPublishedMessage);
                      }
                      LogHelper.Debug("OnConnected");
                      _connectionCounterSubject.OnNext(_rosTopicClients.Count);
                  }
                  else if (startTask.Status == TaskStatus.Faulted)
                  {
                      LogHelper.Error("AddTopic: Failure", startTask.Exception.InnerException);
                      throw startTask.Exception.InnerException;
                  }
              });
        }

        internal void UpdateSubscribers(List<Uri> subscribers)
        {
            //通知订阅者，我上线了
            List<SlaveClient> slaves;
            lock (_rosTopicServers)
            {
                slaves = subscribers
                    .Select(x => new SlaveClient(x))
                    .ToList();
            }
            foreach (var slaveClient in slaves)
            {
                var uri = slaveClient.SlaveUri;
                object[] bus = new object[] {
                  Guid.NewGuid().ToString("N"),
                  uri,
                  'o',
                  "TCPROS",
                  TopicName,
                  true
                };
                Ros.AddBus(new BusInformation() { DestinationId = uri.ToString(), Direction = "o", Transport = "TCPROS", Topic = TopicName, Connected = true });
                var requestTask = slaveClient.PublisherUpdateAsync(NodeId, TopicName, new string[] { slaveServerUrl });
                requestTask.ContinueWith(t =>
                {
                    Console.WriteLine(t.Exception.Message);
                }, TaskContinuationOptions.OnlyOnFaulted);
            }


        }
    }
}