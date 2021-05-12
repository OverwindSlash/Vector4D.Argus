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
    public class Pose : IMessage
    {
        ///<exclude/>
        public Pose()
        {
            position = new Point();
            orientation = new Quaternion();
        }
        ///<exclude/>
        public Pose(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public Point position { get; set; }
        ///<exclude/>
        public Quaternion orientation { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "geometry_msgs/Pose"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "e45d45a5a1ce597b249e23fb30fc871f"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "Point position\nQuaternion orientation"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            position.Serialize(bw);
            orientation.Serialize(bw);
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            position = new Point(br);
            orientation = new Quaternion(br);
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return position.SerializeLength + orientation.SerializeLength; }
        }
        ///<exclude/>
        public bool Equals(Pose other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.position.Equals(position) && other.orientation.Equals(orientation);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Pose)) return false;
            return Equals((Pose)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                result = (result * 397) ^ position.GetHashCode();
                result = (result * 397) ^ orientation.GetHashCode();
                return result;
            }
        }
    }
}

