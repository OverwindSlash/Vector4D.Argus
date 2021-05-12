using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.std_msgs;

namespace RosSharp.geometry_msgs
{
    public class Twist : IMessage
    {
        ///<exclude/>
        public Twist()
        {
            linear = new Vector3();
            angular = new Vector3();
        }
        ///<exclude/>
        public Twist(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public Vector3 linear { get; set; }
        ///<exclude/>
        public Vector3 angular { get; set; }
        ///<exclude/>
        public string MessageType
        {
            get { return "geometry_msgs/Twist"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "9f195f881246fdfa2798d1d3eebca84a"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return "Vector3 linear\nVector3 angular"; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
            linear.Serialize(bw);
            angular.Serialize(bw);
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
            linear = new Vector3(br);
            angular = new Vector3(br);
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return linear.SerializeLength + angular.SerializeLength; }
        }
        ///<exclude/>
        public bool Equals(Twist other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.linear.Equals(linear) && other.angular.Equals(angular);
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Twist)) return false;
            return Equals((Twist)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;
                result = (result * 397) ^ linear.GetHashCode();
                result = (result * 397) ^ angular.GetHashCode();
                return result;
            }
        }
    }
}
