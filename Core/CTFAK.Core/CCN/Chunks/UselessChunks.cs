using CTFAK.CCN.Chunks;
using CTFAK.Memory;
using CTFAK.Core.Utils;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace CTFAK.Core.CCN.Chunks
{
    public class AppIcon
    {
        public static Bitmap Icon;
        public static List<Rgba32> Palette;

        public static Bitmap ReadIcon(ByteReader reader)
        {
            reader.Seek(reader.PeekInt32());
            Palette = new();
            for (int i = 0; i < 16 * 16; i++)
            {
                var b = reader.ReadByte();
                var g = reader.ReadByte();
                var r = reader.ReadByte();
                var a = reader.ReadByte();
                var newColor = new Rgba32(r, g, b, 255);
                Palette.Add(newColor);
            }

            Icon = new Bitmap(16, 16);

            for (int h = 0; h < Icon.height; h++)
                for (int w = 0; w < Icon.width; w++)
                    Icon.SetPixel(w, Icon.width - 1 - h, Palette[reader.ReadByte()]);

            var BitmapSize = Icon.width * Icon.height;
            for (int y = 0; y < Icon.height; ++y)
                for (int x = 0; x < Icon.width; x += 8)
                {
                    byte Mask = reader.ReadByte();
                    for (int i = 0; i < 8; ++i)
                        if ((1 & (Mask >> (7 - i))) != 0)
                        {
                            Rgba32 get = Icon.GetPixel(x + i, y);
                            Icon.SetPixel(x + i, y, new Rgba32(get.R, get.G, get.B, 0));
                        }
                        else
                        {
                            Rgba32 get = Icon.GetPixel(x + i, y);
                            Icon.SetPixel(x + i, y, new Rgba32(get.R, get.G, get.B, 255));
                        }
                }
            return Icon;
        }
    }

    public class ExeOnly
    {
        public static bool ReadBool(ByteReader reader)
        {
            return reader.ReadByte() != 0;
        }
    }

    public class ExtendedHeader : ChunkLoader
    {
        public BitDict Flags = new BitDict(new string[]
        {
            "KeepScreenRatio", "1",
            "AntiAliasingWhenResizing", "2", "3",
            "RightToLeftReading", "4",
            "RightToLeftLayout", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18",
            "DontOptimizeStrings", "19", "20", "21",
            "DontIgnoreDestroy",
            "DisableIME",
            "ReduceCPUUsage", "22",
            "PremultipliedAlpha",
            "OptimizePlaySample"
        });

        public BitDict CompressionFlags = new BitDict(new string[]
        {
            "CompressionLevelMax",
            "CompressSounds",
            "IncludeExternalFiles",
            "NoAutoImageFilters",
            "NoAutoSoundFilters", "1", "2", "3",
            "DontDisplayBuildWarning",
            "OptimizeImageSize",
        });

        public BitDict ViewFlags = new BitDict(new string[]
        {
            "1"
        });

        public BitDict NewFlags = new BitDict(new string[]
        {
            "1"
        });

        public byte BuildType;
        public int ScreenRatio;
        public int ScreenAngle;

        public override void Read(ByteReader reader)
        {
            Flags.flag = (uint)reader.ReadInt32();
            BuildType = reader.ReadByte();
            Settings.BuildType = BuildType;
            switch (BuildType)
            {
                case 0:  // Windows EXE Application
                case 1:  // Windows Screen Saver
                case 2:  // Sub-Application
                case 3:  // Java Sub-Application
                case 4:  // Java Application
                case 5:  // Java Internet Applet
                case 6:  // Java Web Start
                case 7:  // Java for Mobile Devices
                case 9:  // Java Mac Application
                case 10: // Adobe Flash
                case 11: // Java for BlackBerry
                case 18: // XNA Windows Project
                case 19: // XNA Xbox Project
                case 20: // XNA Phone Project
                case 27: // HTML5 Development
                case 28: // HTML5 Final Project
                case 33: // UWP Project
                    break;
                case 12: // Android / OUYA Application
                case 34: // Android App Bundle
                    Settings.gameType = Settings.GameType.ANDROID;
                    break;
                case 13: // iOS Application
                case 14: // iOS Xcode Project
                case 15: // Final iOS Xcode Project
                    Settings.gameType = Settings.GameType.IOS;
                    break;
                case 74: // Nintendo Switch
                case 75: // Xbox One
                case 78: // Playstation
                    Settings.gameType = Settings.GameType.F3;
                    break;
                default:
                    Logger.LogWarning("Unknown Build Type: " + BuildType);
                    break;
            }
            reader.Skip(3);
            CompressionFlags.flag = (uint)reader.ReadInt32();
            ScreenRatio = reader.ReadInt16();
            ScreenAngle = reader.ReadInt16();
            ViewFlags.flag = (uint)reader.ReadInt16();
            NewFlags.flag = (uint)reader.ReadInt16();
        }

        public override void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageShapes : ChunkLoader
    {
        public int Count;
        public List<ImageShape> Shapes = new List<ImageShape>();

        public override void Read(ByteReader reader)
        {
            Count = reader.ReadInt32();
            for (int i = 0; i < Count; i++) 
            {
                var shape = new ImageShape();
                shape.Read(reader);
                Shapes.Add(shape);
            }
        }

        public override void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public class ImageShape : ChunkLoader
    {
        public int[] xArray;
        public int[] yArray;
        public short Image;
        public int Count;

        public override void Read(ByteReader reader)
        {
            Image = (short)reader.ReadInt32();
            Count = reader.ReadInt32();
            if (Count != 0)
            {
                xArray = new int[Count];
                yArray = new int[Count];
                for (int i = 0; i < Count; i++)
                {
                    xArray[i] = reader.ReadInt32();
                    yArray[i] = reader.ReadInt32();
                }
            }
        }

        public override void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }

    public class ChunkOffsets : ChunkLoader
    {
        public int ImageBankOffset;
        public int FontBankOffset;
        public int SoundBankOffset;
        public int LastOffset;

        public override void Read(ByteReader reader)
        {
            ImageBankOffset = reader.ReadInt32();
            FontBankOffset = reader.ReadInt32();
            SoundBankOffset = reader.ReadInt32();
            LastOffset = reader.ReadInt32();
        }

        public override void Write(ByteWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
