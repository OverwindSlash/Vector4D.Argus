using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.std_msgs;
namespace RosSharp.geometry_msgs
{
    ///<exclude/>
    public class Quaternion : IMessage
    {
        ///<exclude/>
        public Quaternion()
        {
        }
        ///<exclude/>
        public Quaternion(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public double x { get; set; }
        ///<exclude/>
        public double y { get; set; }
        ///<exclude/>
        public double z { get; set; }
        ///<exclude/>
        public double w { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "geometry_msgs/Quaternion"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "a779879fadf0160734f906b8c19c7004"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "float64 x\nfloat64 y\nfloat64 z\nfloat64 w"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            bw.Write(x);
            bw.Write(y);
            bw.Write(z);
            bw.Write(w);
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            x = br.ReadDouble();
            y = br.ReadDouble();
            z = br.ReadDouble();
            w = br.ReadDouble();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 8 + 8 + 8 + 8; }
        }
        ///<exclude/>
        public bool Equals(Quaternion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.x.Equals(x) && other.y.Equals(y) && other.z.Equals(z) && other.w.Equals(w);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Quaternion)) return false;
            return Equals((Quaternion)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                result = (result * 397) ^ x.GetHashCode();
                result = (result * 397) ^ y.GetHashCode();
                result = (result * 397) ^ z.GetHashCode();
                result = (result * 397) ^ w.GetHashCode();
                return result;
            }
        }
    }
}

