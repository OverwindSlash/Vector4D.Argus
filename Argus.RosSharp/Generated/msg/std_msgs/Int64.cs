//----------------------------------------------------------------
// <auto-generated>
//     This code was generated by the GenMsg. Version: 0.1.0.0
//     Don't change it manually.
//     2012-06-23T22:05:32+09:00
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
    public class Int64 : IMessage
    {
        ///<exclude/>
        public Int64()
        {
        }
        ///<exclude/>
        public Int64(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public long data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/Int64"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "34add168574510e6e17f5d23ecc077ef"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "int64 data"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            bw.Write(data);
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            data = br.ReadInt64();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 8; }
        }
        ///<exclude/>
        public bool Equals(Int64 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.data.Equals(data);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Int64)) return false;
            return Equals((Int64)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                result = (result * 397) ^ data.GetHashCode();
                return result;
            }
        }
    }
}
