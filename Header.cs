using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFSTools
{
    internal class Header
    {
        public string magic { get; set; } 
        public int filesize { get; set; } 
        public int count { get; set; } 

        public Header(BinaryReader reader)
        {
            reader.ReadInt32();
            int head1Size = reader.ReadInt32();
            reader.BaseStream.Position = head1Size;
            magic = Encoding.UTF8.GetString(reader.ReadBytes(4));
            filesize = reader.ReadInt32();
            reader.BaseStream.Position += 4;
            count = reader.ReadInt32();
        }
    }
}
