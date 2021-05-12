#region License Terms

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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Horizon.XmlRpc.Server;
using RosSharp.Master;
using RosSharp.Topic;
using RosSharp.Transport;
using RosSharp.Utility;

namespace RosSharp.Slave
{
    /// <summary>
    ///   XML-RPC Server for Slave API
    /// </summary>
    public sealed class SlaveServer : XmlRpcListenerService, ISlave, IDisposable
    {
        private bool stop;
        private SlaveServer that;
        private HttpListener listener;
        // private readonly HttpServerChannel _channel;
        private readonly Dictionary<string, TcpRosListener> _tcpRosListener;
        private readonly TopicContainer _topicContainer;
        public string NodeId { get; private set; }

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        internal SlaveServer(string nodeId, int portNumber, TopicContainer topicContainer)
        {
            that = this;
            NodeId = nodeId;

            _topicContainer = topicContainer;
            _tcpRosListener = new Dictionary<string, TcpRosListener>();
            int port = GetRandomUnusedPort();
            string slaveName = "slave" + Guid.NewGuid().ToString("N");
            listener = new HttpListener();
            string url = string.Format("http://{0}:{1}/{2}/", Ros.HostName, port, slaveName);
            listener.Prefixes.Add(url);
            listener.Start();
            new Thread(() =>
            {
                while (!stop)
                {
                    var context = listener.GetContext();
                    that.ProcessRequest(context);
                }
            }).Start();
            SlaveUri = new Uri(url);
        }

        public Uri SlaveUri { get; private set; }

        internal void AddListener(string topic, TcpRosListener listener)
        {
            if (_tcpRosListener.ContainsKey(topic))
            {
                throw new InvalidOperationException("Already registered listener.");
            }

            _tcpRosListener.Add(topic, listener);
        }

        internal void RemoveListener(string topic)
        {
            _tcpRosListener[topic].Dispose();
            _tcpRosListener.Remove(topic);
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                stop = true;
                listener.Stop();
                listener.Close();
            }
            catch
            {
            }
            _tcpRosListener.Values.ToList().ForEach(x => x.Dispose());
            _tcpRosListener.Clear();
        }

        #endregion

        #region ISlave Members

        /// <summary>
        ///   Retrieve transport/topic statistics.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br />
        /// [1] = str: status message <br />
        /// [2] = stats: [publishStats, subscribeStats, serviceStats] <br /> 
        ///   publishStats: [[topicName, messageDataSent, pubConnectionData]...] <br />
        ///   subscribeStats: [[topicName, subConnectionData]...] <br />
        ///   serviceStats: (proposed) [numRequests, bytesReceived, bytesSent] <br />
        ///     pubConnectionData: [connectionId, bytesSent, numSent, connected]* <br />
        ///     subConnectionData: [connectionId, bytesReceived, numSent, dropEstimate, connected]*
        /// </returns>
        public object[] GetBusStats(string callerId)
        {
            LogHelper.DebugFormat("GetBusStats(callerId={0})", callerId);
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Retrieve transport/topic connection information.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message<br/>
        /// [2] = businfo: [[connectionId1, destinationId1, direction1, transport1, topic1, connected1]... ] <br/>
        ///   connectionId is defined by the node and is opaque. <br/>
        ///   destinationId is the XMLRPC URI of the destination. <br/>
        ///   direction is one of 'i', 'o', or 'b' (in, out, both). <br/>
        ///   transport is the transport type (e.g. 'TCPROS'). <br/>
        ///   topic is the topic name. <br/>
        ///   connected1 indicates connection status.
        /// </returns>
        public object[] GetBusInfo(string callerId)
        {
            LogHelper.DebugFormat("GetBusInfo(callerId={0})", callerId);
            List<object[]> busList = new List<object[]>();
            List<Node> nodeList = new List<Node>();
            //所有的节点

            return new object[]
           {
                (int)StatusCode.Success,
                "",
                Ros.BusList.ToArray()
           };
        }

        /// <summary>
        ///   Get the URI of the master node.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/> 
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the master 
        /// </returns>
        public object[] GetMasterUri(string callerId)
        {
            LogHelper.DebugFormat("GetMasterUri(callerId={0})", callerId);
            return new object[]
            {
                (int)StatusCode.Success,
                "",
                Ros.MasterUri
            };
        }

        /// <summary>
        ///   Stop this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="msg"> A message describing why the node is being shutdown. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        public object[] Shutdown(string callerId, string msg)
        {
            LogHelper.DebugFormat("Shutdown(callerId={0},msg={1})", callerId, msg);
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Get the PID of this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: server process pid
        /// </returns>
        public object[] GetPid(string callerId)
        {
            LogHelper.DebugFormat("GetPid(callerId={0})", callerId);
            return new object[]
            {
                (int)StatusCode.Success,
                "",
                Process.GetCurrentProcess().Id
            };
        }

        /// <summary>
        ///   Retrieve a list of topics that this node subscribes to
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = topicList is a list of topics this node subscribes to and is of the form [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        public object[] GetSubscriptions(string callerId)
        {
            LogHelper.DebugFormat("GetSubscriptions(callerId={0})", callerId);
            return new object[]
            {
                (int)StatusCode.Success,
                "Success",
                _topicContainer.GetSubscribers().Select(x => new object[] {x.TopicName, x.MessageType}).ToArray()
            };
        }

        /// <summary>
        ///   Retrieve a list of topics that this node publishes.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = topicList is a list of topics published by this node and is of the form [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        public object[] GetPublications(string callerId)
        {
            LogHelper.DebugFormat("GetPublications(callerId={0})", callerId);
            return new object[]
            {
                (int)StatusCode.Success,
                "Success",
                _topicContainer.GetPublishers().Select(x => new object[] {x.TopicName, x.MessageType}).ToArray()
            };
        }

        /// <summary>
        ///   Callback from master with updated value of subscribed parameter.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="parameterKey"> Parameter name, globally resolved. </param>
        /// <param name="parameterValue"> New parameter value. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        public object[] ParamUpdate(string callerId, string parameterKey, object parameterValue)
        {
            LogHelper.DebugFormat("ParamUpdate(callerId={0},parameterKey={1},parameterValue={2})", callerId, parameterKey, parameterValue);
            var handler = ParameterUpdated;
            if (handler != null)
            {
                Task.Factory.FromAsync(handler.BeginInvoke, handler.EndInvoke, parameterKey, parameterValue, null)
                    .ContinueWith(task => LogHelper.Error("PramUpdateError", task.Exception)
                                  , TaskContinuationOptions.OnlyOnFaulted);
            }

            return new object[]
            {
                (int)StatusCode.Success,
                "parameter update [" + parameterKey + "]",
                0
            };
        }

        /// <summary>
        ///   Callback from master of current publisher list for specified topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="topic"> Topic name. </param>
        /// <param name="publishers"> List of current publishers for topic in the form of XMLRPC URIs </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        public object[] PublisherUpdate(string callerId, string topic, string[] publishers)
        {
            LogHelper.DebugFormat("PublisherUpdate(callerId={0},topic={1},publishers={2})", callerId, topic, publishers);
            new Thread(() =>
            {
                if (_topicContainer.HasSubscriber(topic))
                {
                    var subs = _topicContainer.GetSubscribers().First(s => s.TopicName == topic);
                    subs.UpdatePublishers(publishers.Select(x => new Uri(x)).ToList());
                }
            }).Start();
            return new object[]
            {
                (int)StatusCode.Success,
                "Publisher update received.",
                0
            };
        }

        /// <summary>
        ///   Publisher node API method called by a subscriber node. <br/>
        ///   This requests that source allocate a channel for communication. <br/>
        ///   Subscriber provides a list of desired protocols for communication. <br/>
        ///   Publisher returns the selected protocol along with any additional params required for establishing connection. <br/>
        ///   For example, for a TCP/IP-based connection, the source node may return a port number of TCP/IP server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="topic"> Topic name. </param>
        /// <param name="protocols"> List of desired protocols for communication in order of preference. Each protocol is a list of the form [ProtocolName, ProtocolParam1, ProtocolParam2...N] </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = protocolParams may be an empty list if there are no compatible protocols. 
        /// </returns>
        public object[] RequestTopic(string callerId, string topic, object[] protocols)
        {
            LogHelper.DebugFormat("RequestTopic(callerId={0},topic={1},protocols={2})", callerId, topic, protocols);
            if (!_topicContainer.HasPublisher(topic))
            {
                LogHelper.WarnFormat("No publishers for topic: {0}", topic);
                return new object[]
                {
                    (int)(StatusCode.Error),
                    "No publishers for topic: " + topic,
                    "null"
                };
            }

            foreach (string[] protocol in protocols)
            {
                string protocolName = protocol[0];

                if (protocolName != "TCPROS")
                {
                    continue;
                }

                if (!_tcpRosListener.ContainsKey(topic))
                {
                    LogHelper.WarnFormat("No publishers for topic: {0}", topic);
                    return new object[]
                    {
                        (int)(StatusCode.Error),
                        "No publishers for topic: " + topic,
                        "null"
                    };
                }
                var listener = _tcpRosListener[topic];
                var address = listener.EndPoint;

                return new object[]
                {
                   (int)(StatusCode.Success),
                    "Protocol<" + protocolName + ", AdvertiseAddress<" + Ros.HostName + ", " + address.Port + ">>",
                    new object[]
                    {
                        protocolName,
                        Ros.HostName,
                        address.Port
                    }
                };
            }

            LogHelper.Warn("No supported protocols specified.");
            return new object[]
            {
                (int)(StatusCode.Error),
                "No supported protocols specified.",
                "null"
            };
        }
        #endregion

        internal event Action<string, object> ParameterUpdated;
    }
}