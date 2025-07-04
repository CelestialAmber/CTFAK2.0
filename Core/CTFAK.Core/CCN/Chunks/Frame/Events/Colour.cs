﻿using CTFAK.Memory;
using CTFAK.Core.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace CTFAK.MMFParser.EXE.Loaders.Events.Parameters
{
    public class Colour : ParameterCommon
    {
        public Rgba32 Value;

        public override void Read(ByteReader reader)
        {
            var bytes = reader.ReadBytes(4);
            Value = new Rgba32(bytes[0], bytes[1], bytes[2]);
        }

        public override void Write(ByteWriter Writer)
        {
            Writer.WriteInt8(Value.R);
            Writer.WriteInt8(Value.G);
            Writer.WriteInt8(Value.B);
            Writer.WriteInt8(255);
        }
    }
}
