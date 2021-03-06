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
    public class UInt16 : IMessage
    {
        ///<exclude/>
        public UInt16()
        {
        }
        ///<exclude/>
        public UInt16(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public ushort data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/UInt16"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "1df79edf208b629fe6b81923a544552d"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "uint16 data"; }
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
            data = br.ReadUInt16();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 2; }
        }
        ///<exclude/>
        public bool Equals(UInt16 other)
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
            if (obj.GetType() != typeof(UInt16)) return false;
            return Equals((UInt16)obj);
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
