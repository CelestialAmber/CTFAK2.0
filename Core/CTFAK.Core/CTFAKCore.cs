using System;
using System.IO;
using CTFAK.FileReaders;
using CTFAK.Core.Utils;
using Joveler.Compression.ZLib;
using System.Runtime.InteropServices;

namespace CTFAK
{
    public class CTFAKCore
    {
        public delegate void SaveHandler(int index, int all);

        public static IFileReader currentReader;
        public static string parameters;
        public static string path;
        public static void Init()
        {

            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                Console.WriteLine(e.ExceptionObject.GetType());
                //NativeLib.MessageBox((IntPtr)0, $"{e.Exception.ToString()}", "ERROR", 0);



            };

            InitZLibLibrary();

        }

        public static void InitZLibLibrary()
        {
            string libDir = "runtimes";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                libDir = Path.Combine(libDir, "win-");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libDir = Path.Combine(libDir, "linux-");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libDir = Path.Combine(libDir, "osx-");

            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    libDir += "x86";
                    break;
                case Architecture.X64:
                    libDir += "x64";
                    break;
                case Architecture.Arm:
                    libDir += "arm";
                    break;
                case Architecture.Arm64:
                    libDir += "arm64";
                    break;
            }
            libDir = Path.Combine(libDir, "native");

            string libPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                libPath = Path.Combine(libDir, "zlib1.dll");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libPath = Path.Combine(libDir, "libz.so");
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libPath = Path.Combine(libDir, "libz.dylib");

            if (libPath == null)
                throw new PlatformNotSupportedException($"Unable to find native library.");
            if (!File.Exists(libPath))
                throw new PlatformNotSupportedException($"Unable to find native library [{libPath}].");

            ZLibInit.GlobalInit(libPath, new ZLibInitOptions());
        }
    }
}