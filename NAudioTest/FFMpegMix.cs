using FFMpegCore;
using FFMpegCore.Pipes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest
{
    public static class FFMpegMix
    {
        public static void Mix()
        {
            try
            {
                var options = new FFOptions
                {
                    BinaryFolder = @"C:\ProgramData\chocolatey\lib\ffmpeg\tools\ffmpeg\bin",
                    TemporaryFilesFolder = @"C:\Windows\Temp"
                };

                var file = File.ReadAllBytes(@"C:\Windows\Temp\teste1.mp3");
                var file2 = File.ReadAllBytes(@"C:\Windows\Temp\teste2.mp3");


                var audioStream1 = new MemoryStream(file);
                var audioStream2 = new MemoryStream(file2);


                var mediaAnalisys = FFProbe.Analyse(audioStream1, int.MaxValue, options);
                var mediaAnalisys2 = FFProbe.Analyse(audioStream2, int.MaxValue, options);


                var durationAudio1 = mediaAnalisys.Duration;
                var durationAudio2 = mediaAnalisys2.Duration;


                var d = new TimeSpan();
                if (durationAudio1 > durationAudio2) d = durationAudio1;
                else if (durationAudio1 < durationAudio2) d = durationAudio2;

                audioStream1.Position = 0;
                audioStream2.Position = 0;

                var outStream = new MemoryStream();

                FFMpegArguments
                    .FromPipeInput(new StreamPipeSource(audioStream1), options =>
                    {
                        options.WithDuration(durationAudio1);
                    })
                    .AddPipeInput(new StreamPipeSource(audioStream2), options =>
                    {
                        options.WithDuration(durationAudio2);
                    })
                    .OutputToPipe(new StreamPipeSink(outStream), options =>
                    {
                        options.ForceFormat("opus");
                        options.WithCustomArgument(@"-filter_complex amix=inputs=2");
                        options.WithDuration(d);
                    })
                    .NotifyOnOutput((str, dt) =>
                    {
                        Console.WriteLine(str);
                    })
                    .ProcessSynchronously(true, options);

                using FileStream fl = File.Create(@"C:\Users\PremierSoft\Downloads\result3.opus");
                fl.Write(outStream.GetBuffer(), 0, (int)outStream.Length);
                fl.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
