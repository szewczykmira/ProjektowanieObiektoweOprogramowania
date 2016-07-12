using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Zestaw_5
{
    public class CaesarStream
    {
        private int Rotation;
        private Stream Stream;
        public CaesarStream(Stream stream, int rotation)
        {
            Rotation = rotation;
            Stream = stream;
        }
        private void RotateBytes(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[i + offset] = (byte)((buffer[i] + Rotation) % 256);
            }
        }
        public void Write(byte[] buffer, int offset, int count)
        {
            RotateBytes(buffer, offset, count);
            Stream.Write(buffer, offset, count);
        }
        public int Read(byte[] buffer, int offset, int count)
        {
            int n = Stream.Read(buffer, offset, count);
            RotateBytes(buffer, offset, count);
            return n;
        }
    }


    [TestFixture]
    public class CaesarTest
    {
        [Test]
        public void Caesar()
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] str = encoding.GetBytes("loremipsum");

            MemoryStream ms = new MemoryStream();
            CaesarStream cs = new CaesarStream(ms, 5);
            cs.Write(str, 0, 10);

            byte[] buf = new byte[10];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(buf, 0, 10);
            ms.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual("qtwjrnuxzr", encoding.GetString(buf));

            cs = new CaesarStream(ms, -5);
            cs.Read(buf, 0, 10);
            Assert.AreEqual("loremipsum", encoding.GetString(buf));
        }
    }


}
