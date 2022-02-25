using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using System;
using System.IO;
using System.Linq;

namespace NAudioTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FFMpegMix.Mix();
            
        }
    }
}
