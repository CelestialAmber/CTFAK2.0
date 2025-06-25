#define USE_IONIC
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Ionic.Zlib;
using Joveler.Compression.ZLib;



namespace CTFAK.Memory
{
    public static class Decompressor
    {
        public static ByteWriter Compress(byte[] buffer)
        {
            var writer = new ByteWriter(new MemoryStream());
            var compressed = CompressBlock(buffer);
            writer.WriteInt32(buffer.Length);
            writer.WriteInt32(compressed.Length);
            writer.WriteBytes(compressed);
            return writer;
        }

        public static byte[] Decompress(ByteReader exeReader, out int decompressed)
        {
            var decompSize = exeReader.ReadInt32();
            var compSize = exeReader.ReadInt32();
            decompressed = decompSize;
            return DecompressBlock(exeReader, compSize);
        }

        public static ByteReader DecompressAsReader(ByteReader exeReader, out int decompressed)
        {
            return new ByteReader(Decompress(exeReader, out decompressed));
        }

        public static byte[] DecompressBlock(byte[] data)
        {
            return ZlibStream.UncompressBuffer(data);
        }

        public static byte[] DecompressBlock(ByteReader reader, int size)
        {
            return ZlibStream.UncompressBuffer(reader.ReadBytes(size));
        }

        public static byte[] DecompressOld(ByteReader reader)
        {
            var decompressedSize = reader.PeekInt32() != -1 ? reader.ReadInt32() : 0;
            var start = reader.Tell();
            var compressedSize = reader.Size();
            var buffer = reader.ReadBytes((int)compressedSize);
            int actualSize;
            var data = DecompressOldBlock(buffer, (int)compressedSize, decompressedSize, out actualSize);
            reader.Seek(start + actualSize);
            return data;
        }

        public static byte[] DecompressOldBlock(byte[] buff, int size, int decompSize, out int actual_size)
        {
            byte[] originalBuff = buff;
            byte[] outputBuff = new byte[decompSize];
            actual_size = decompressOld(buff, ref outputBuff);
            return outputBuff;
        }

        public static int decompressOld(byte[] source, ref byte[] output)
        {
            MemoryStream compressedStream = new MemoryStream(source);
            MemoryStream decompressedStream = new MemoryStream(output);
            System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(compressedStream, System.IO.Compression.CompressionMode.Decompress);
            deflateStream.CopyTo(decompressedStream);
            return (int)decompressedStream.Length;
        }

        public static byte[] CompressBlock(byte[] data)
        {
            var compOpts = new ZLibCompressOptions();
            compOpts.Level = ZLibCompLevel.Default;
            var decompressedStream = new MemoryStream(data);
            var compressedStream = new MemoryStream();
            byte[] compressedData = null;
            var zs = new ZLibStream(compressedStream, compOpts);
            decompressedStream.CopyTo(zs);
            zs.Close();

            compressedData = compressedStream.ToArray();

            return compressedData;
        }
    }
}