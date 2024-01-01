using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFSTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Contains(".txt"))
            {
                Rebuild(args[0]);
            }
            else
            {
                Extract(args[0]);
            }
        }
        public static void Extract(string file)
        {
            var reader = new BinaryReader(File.OpenRead(file));
            Header header = new Header(reader);
            int[] pointers = new int[header.count];
            string[] strings = new string[header.count];
            for (int i = 0; i < header.count; i++)
            {
                pointers[i] = reader.ReadInt32() + 0x14;
                int pred = (int)reader.BaseStream.Position;
                reader.BaseStream.Position = pointers[i];
                strings[i] = Utils.ReadString(reader, Encoding.Unicode);
                reader.BaseStream.Position = pred;
                strings[i] = strings[i].Replace("\n","<lf>").Replace("\r", "<br>");
            }
            File.WriteAllLines(Path.GetFileNameWithoutExtension(file) + ".txt", strings);
        }
        public static void Rebuild(string txt)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(Path.GetFileNameWithoutExtension(txt) + ".loc")))
            {
                string[] strings = File.ReadAllLines(txt);
                int[] pointers = new int[strings.Length];
                writer.Write(Encoding.UTF8.GetBytes("LOCH"));
                writer.Write(new byte[16]);
                writer.Write(Encoding.UTF8.GetBytes("LOCL"));
                writer.Write(new byte[strings.Length * 4 + 12]);
                for (int i = 0; i < strings.Length; i++)
                {
                    pointers[i] = (int)writer.BaseStream.Position - 0x14;
                    strings[i] = strings[i].Replace("<lf>","\n").Replace("<br>", "\r");
                    writer.Write(Encoding.Unicode.GetBytes(strings[i]));
                    writer.Write(new byte());
                    Utils.AddPadding(writer, (int)writer.BaseStream.Position - 1);
                }
                writer.BaseStream.Position = 4;
                writer.Write(0x14);
                writer.BaseStream.Position += 4;
                writer.Write(1);
                writer.Write(0x14);
                writer.BaseStream.Position += 4;
                writer.Write(writer.BaseStream.Length - 0x14);
                writer.Write(strings.Length);
                for (int i = 0; i < strings.Length; i++)
                {
                    writer.Write(pointers[i]);
                }
            }
        }
    }
}
