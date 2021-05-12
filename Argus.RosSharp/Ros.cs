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

using RosSharp.Slave;
using RosSharp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RosSharp
{
    /// <summary>
    ///   ROS Manager
    /// </summary>
    public static class Ros
    {
        private static readonly Dictionary<string, Node> _nodes = new Dictionary<string, Node>();

        /// <summary>
        ///   XML-RPC URI of the Master
        /// </summary>
        /// <example>
        ///   http://192.168.1.10:11311/
        /// </example>
        public static Uri MasterUri { get; set; }

        /// <summary>
        ///   local network address of a ROS Node
        /// </summary>
        /// <example>
        ///   192.168.1.10
        /// </example>
        public static string HostName { get; set; }

        /// <summary>
        ///   Timeout in milliseconds on a XML-RPC proxy method call
        /// </summary>
        public static int XmlRpcTimeout { get; set; }

        /// <summary>
        ///   Timeout in milliseconds on a ROS TOPIC
        /// </summary>
        public static int TopicTimeout { get; set; }

        /// <summary>
        ///   Initialize Setting
        /// </summary>
        static Ros()
        {
            Console.CancelKeyPress += (sender, args) => Dispose();
        }

        /// <summary>
        ///   Dispose all nodes
        /// </summary>
        public static void Dispose()
        {
            DisposeAsync().Wait();
        }

        /// <summary>
        ///   Asynchronous dispose all nodes
        /// </summary>
        public static Task DisposeAsync()
        {
            var nodes = GetNodes();
            var tasks = nodes.Select(node => node.DisposeAsync());

            return Task.Factory.StartNew(() => Task.WaitAll(tasks.ToArray()));
        }
        /// <summary>
        ///   Create a ROS Node
        /// </summary>
        /// <param name="nodeName"> ROS Node name </param>
        /// <param name="enableLogger"> if true, enable RosOut Logger </param>
        /// <param name="anonymous"> if true, named to an anonymous name (append a random number to the node name) </param>
        /// <returns> created Node </returns>
        public static Task<Node> InitNodeAsync(string nodeName)
        {
            lock (_nodes)
            {
                var node = new Node(nodeName);

                var tcs = new TaskCompletionSource<Node>();

                var initTask = node.InitializeAsync();

                initTask.ContinueWith(t =>
                {
                    node.Disposing += DisposeNode;
                    _nodes.Add(nodeName, node);
                    tcs.SetResult(node);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                initTask.ContinueWith(
                    t => tcs.SetException(t.Exception.InnerException),
                    TaskContinuationOptions.OnlyOnFaulted);
                return tcs.Task;
            }
        }

        /// <summary>
        ///   Get all nodes
        /// </summary>
        /// <returns> all nodes </returns>
        public static List<Node> GetNodes()
        {
            List<Node> nodes;
            lock (_nodes)
            {
                nodes = new List<Node>(_nodes.Values);
            }
            return nodes;
        }

        public static Node GetNode(string nodeId)
        {
            lock (_nodes)
            {
                return _nodes[nodeId];
            }
        }

        private static Task DisposeNode(string nodeId)
        {
            return Task.Factory.StartNew(() =>
            {
                lock (_nodes)
                {
                    if (_nodes.ContainsKey(nodeId))
                    {
                        _nodes.Remove(nodeId);
                    }
                }
            });
        }
        private static object _lock = new object();
        public static int id = 1;
        public static List<object[]> BusList = new List<object[]>();
        public static void AddBus(BusInformation busInfo)
        {
            lock (_lock)
            {
                object[] bus = new object[] { id, busInfo.DestinationId, busInfo.Direction, busInfo.Transport, busInfo.Topic, busInfo.Connected };
                BusList.Add(bus);
            }
        }
    }
}