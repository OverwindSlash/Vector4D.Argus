//----------------------------------------------------------------
// <auto-generated>
//     This code was generated by the GenMsg. Version: 0.1.0.0
//     Don't change it manually.
//     2012-06-23T22:05:33+09:00
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
    public class Int8MultiArray : IMessage
    {
        ///<exclude/>
        public Int8MultiArray()
        {
            layout = new MultiArrayLayout();
            data = new List<sbyte>();
        }
        ///<exclude/>
        public Int8MultiArray(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public MultiArrayLayout layout { get; set; }
        ///<exclude/>
        public List<sbyte> data { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "std_msgs/Int8MultiArray"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "d7c1af35a1b4781bbe79e03dd94b7c13"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "MultiArrayLayout layout\nint8[] data"; }
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
            data = new List<sbyte>(br.ReadInt32()); for(int i=0; i<data.Capacity; i++) { var x = br.ReadSByte();data.Add(x);}
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return layout.SerializeLength + 4 + data.Sum(x => 1); }
        }
        ///<exclude/>
        public bool Equals(Int8MultiArray other)
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
            if (obj.GetType() != typeof(Int8MultiArray)) return false;
            return Equals((Int8MultiArray)obj);
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
