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

using System.IO;

namespace RosSharp.Message
{
    /// <summary>
    ///   Defines interface for Topic Message
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Message Type Name
        /// </summary>
        string MessageType { get; }

        /// <summary>
        /// MD5 Sum of this Message
        /// </summary>
        string Md5Sum { get; }

        /// <summary>
        /// Raw Message Definition
        /// </summary>
        string MessageDefinition { get; }

        /// <summary>
        /// Has Header
        /// </summary>
        bool HasHeader { get; }

        /// <summary>
        /// Message Length for Serialize
        /// </summary>
        int SerializeLength { get; }

        /// <summary>
        /// Serialize Message
        /// </summary>
        /// <param name="stream">Serialized Binary Data</param>
        void Serialize(BinaryWriter stream);

        /// <summary>
        /// Deserialize Message
        /// </summary>
        /// <param name="stream">Binary Data for Desirialize</param>
        void Deserialize(BinaryReader stream);
    }
}