using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Synthesis;

namespace TTS2Audio
{
  class Program {
    public readonly SpeechSynthesizer synth = new SpeechSynthesizer();
    public string voiceName = null;
    public int volume = 100;
    public int rate = 1;

    static void Main(string[] args) {
      Program prog = new Program();
      var paths = new OptionSet()
        {
          {
            "l|list",
            v => {
              Console.WriteLine("Available voices: ");
              foreach (var vc in prog.synth.GetInstalledVoices()) {
                Console.WriteLine(vc.VoiceInfo.Name);
              }
              System.Environment.Exit(0);
            }
          },
          {
            "h|help",
            v => {
              Console.WriteLine("Usage");
              System.Environment.Exit(0);
            }
          },
          {
            "r|rate=",
            v => {
              prog.rate = int.Parse(v);
              if (prog.rate < -10 || prog.rate > 10) {
                throw new ArgumentException();
              }
            }
          },
          {
            "vol|volume=",
            v => {
              prog.volume = int.Parse(v);
              if (prog.volume < 0 || prog.volume > 100) {
                throw new ArgumentException();
              }
            }
          },
          {
            "vc|voice=",
            v => { prog.voiceName = v; }
          },
        }.Parse(args);
      if (paths.Count > 0) prog.Wav(paths);
    }

    void Wav(List<string> paths) {
      if (this.voiceName != null) {
        this.synth.SelectVoice(this.voiceName);
      }
      this.synth.Volume = this.volume;
      this.synth.Rate = this.rate;
      foreach (var path in paths) {
        try {
          string wavpath = Path.GetFileNameWithoutExtension(path) + ".wav";
          this.synth.SetOutputToWaveFile(wavpath);
          this.synth.Speak(File.ReadAllText(path));
          Console.WriteLine("Wrote: " + wavpath);
        } catch (Exception err) {
          Console.WriteLine(err.Message);
        }
      }
    }
  }
}
