//----------------------------------------------------------------
// <auto-generated>
//     This code was generated by the GenMsg. Version: 0.1.0.0
//     Don't change it manually.
//     2012-06-23T22:05:34+09:00
// </auto-generated>
//----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.std_msgs;
namespace RosSharp.std_msgs
{
    ///<exclude/>
    public class String : IMessage
    {
        ///<exclude/>
        public String()
        {
            data = string.Empty;
        }
        ///<exclude/>
        public String(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public string data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/String"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "992ce8a1687cec8c8bd883ec73ca41d1"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "string data"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            bw.WriteUtf8String(data);
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            data = br.ReadUtf8String();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 4 + data.Length; }
        }
      
    }
}