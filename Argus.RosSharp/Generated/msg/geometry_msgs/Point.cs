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
    public class Point : IMessage
    {
        ///<exclude/>
        public Point()
        {
        }
        ///<exclude/>
        public Point(BinaryReader br)
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
        public string MessageType
        {
            get { return "geometry_msgs/Point"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "4a842b65f413084dc2b10fb484ea7f17"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "float64 x\nfloat64 y\nfloat64 z"; }
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
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            x = br.ReadDouble();
            y = br.ReadDouble();
            z = br.ReadDouble();
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 8 + 8 + 8; }
        }
        ///<exclude/>
        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.x.Equals(x) && other.y.Equals(y) && other.z.Equals(z);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Point)) return false;
            return Equals((Point)obj);
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
                return result;
            }
        }
    }
}
