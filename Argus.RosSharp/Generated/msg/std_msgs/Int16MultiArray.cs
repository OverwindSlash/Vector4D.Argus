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
    public class Int16MultiArray : IMessage
    {
        ///<exclude/>
        public Int16MultiArray()
        {
            layout = new MultiArrayLayout();
            data = new List<short>();
        }
        ///<exclude/>
        public Int16MultiArray(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public MultiArrayLayout layout { get; set; }
        ///<exclude/>
        public List<short> data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/Int16MultiArray"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "d9338d7f523fcb692fae9d0a0e9f067c"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "MultiArrayLayout layout\nint16[] data"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            layout.Serialize(bw);
            bw.Write(data.Count); for(int i=0; i<data.Count; i++) { bw.Write(data[i]);}
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            layout = new MultiArrayLayout(br);
            data = new List<short>(br.ReadInt32()); for(int i=0; i<data.Capacity; i++) { var x = br.ReadInt16();data.Add(x);}
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return layout.SerializeLength + 4 + data.Sum(x => 2); }
        }
        ///<exclude/>
        public bool Equals(Int16MultiArray other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.layout.Equals(layout) && other.data.SequenceEqual(data);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Int16MultiArray)) return false;
            return Equals((Int16MultiArray)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                result = (result * 397) ^ layout.GetHashCode();
                result = (result * 397) ^ data.GetHashCode();
                return result;
            }
        }
    }
}
