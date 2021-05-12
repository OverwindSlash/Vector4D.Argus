//----------------------------------------------------------------
// <auto-generated>
//     This code was generated by the GenMsg. Version: 0.1.0.0
//     Don't change it manually.
//     2012-06-23T22:05:26+09:00
// </auto-generated>
//----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.std_msgs;
namespace RosSharp.roscpp
{
    ///<exclude/>
    public class Empty : ServiceBase<Empty.Request,Empty.Response>
    {
        ///<exclude/>
        public Empty()
        {
        }
        ///<exclude/>
        public Empty(Func<Request,Response> action)
            :base(action)
        {
        }
        ///<exclude/>
        public override string ServiceType
        {
            get { return "roscpp/Empty"; }
        }
        ///<exclude/>
        public override string Md5Sum
        {
            get { return "d41d8cd98f00b204e9800998ecf8427e"; }
        }
        ///<exclude/>
        public override string ServiceDefinition
        {
            get { return "---\n"; }
        }
    ///<exclude/>
    public class Request : IMessage
    {
        ///<exclude/>
        public Request()
        {
        }
        ///<exclude/>
        public Request(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public string MessageType
        {
            get { return "EmptyRequest"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "d41d8cd98f00b204e9800998ecf8427e"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return ""; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 0; }
        }
        ///<exclude/>
        public bool Equals(Request other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return true;
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Request)) return false;
            return Equals((Request)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;

                return result;
            }
        }
    }
    ///<exclude/>
    public class Response : IMessage
    {
        ///<exclude/>
        public Response()
        {
        }
        ///<exclude/>
        public Response(BinaryReader br)
        {
            Deserialize(br);
        }
        ///<exclude/>
        public string MessageType
        {
            get { return "EmptyResponse"; }
        }
        ///<exclude/>
        public string Md5Sum
        {
            get { return "d41d8cd98f00b204e9800998ecf8427e"; }
        }
        ///<exclude/>
        public string MessageDefinition
        {
            get { return ""; }
        }
        ///<exclude/>
        public bool HasHeader
        {
            get { return false; }
        }
        ///<exclude/>
        public void Serialize(BinaryWriter bw)
        {
        }
        ///<exclude/>
        public void Deserialize(BinaryReader br)
        {
        }
        ///<exclude/>
        public int SerializeLength
        {
            get { return 0; }
        }
        ///<exclude/>
        public bool Equals(Response other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return true;
        }
        ///<exclude/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Response)) return false;
            return Equals((Response)obj);
        }
        ///<exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 0;

                return result;
            }
        }
    }
    }
}
