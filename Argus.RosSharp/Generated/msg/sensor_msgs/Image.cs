using RosSharp.Message;
using RosSharp.std_msgs;
using System;
using System.Collections.Generic;
using System.IO;

namespace RosSharp.sensor_msgs
{
    public class Image : IMessage
    {
        public Header header { get; set; }
        public uint height { get; set; }
        public uint width { get; set; }
        public string encoding { get; set; }
        public byte is_bigendian { get; set; }
        public uint step { get; set; }
        public List<byte> data { get; set; }

        public string MessageType
        {
            get { return "sensor_msgs/Image"; }
        }

        public string Md5Sum
        {
            get { return "060021388200f6f0f447d0fcd9c64743"; }
        }

        public string MessageDefinition
        {
            get { return "uint32 height\nuint32 width\nstring encoding\nuint8 is_bigendian\nuint32 step\nuint8[] data"; }
        }

        public bool HasHeader
        {
            get { return true; }
        }

        public int SerializeLength
        {
            get
            {
                return 4 + 4 + 4 + encoding.Length + 1 + 4 + data.Count;
            }
        }

        public Image()
        {
            header = new Header();
            data = new List<byte>();
        }

        public Image(BinaryReader br)
            : this()
        {
            Deserialize(br);
        }

        public void Serialize(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(BinaryReader br)
        {
            header.Deserialize(br);
            height = br.ReadUInt32();
            width = br.ReadUInt32();
            encoding = br.ReadUtf8String();
            is_bigendian = br.ReadByte();
            step = br.ReadUInt32();

            data = new List<byte>((int)step * (int)height);
            for (int i = 0; i < data.Capacity; i++)
            {
                var x = br.ReadByte();
                data.Add(x);
            }
        }
    }
}
