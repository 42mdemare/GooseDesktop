using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
//using GooseDesktop.Properties;
using SamEngine;

namespace GooseDesktop
{
	internal static class Sound
	{
		public class Mp3Player
		{
			public bool loop;

			private string alias;

			public Mp3Player(string filename, string playerAlias)
			{
				alias = playerAlias;
				mciSendString($"open \"{filename}\" type MPEGVideo alias {alias}", null, 0, IntPtr.Zero);
			}

			public void Play()
			{
				if (!GooseConfig.settings.SilenceSounds)
				{
					string format = "play {0}";
					format = string.Format(format, alias);
					if (loop)
					{
						format += " REPEAT";
					}
					mciSendString(format, null, 0, IntPtr.Zero);
				}
			}

			public void Pause()
			{
				mciSendString($"stop {alias}", null, 0, IntPtr.Zero);
			}

			public void SetVolume(float volume)
			{
				int num = (int)Math.Max(Math.Min(volume * 1000f, 1000f), 0f);
				mciSendString($"setaudio {alias} volume to {num}", null, 0, IntPtr.Zero);
			}

			public void Dispose()
			{
				mciSendString($"close {alias}", null, 0, IntPtr.Zero);
			}

			public void ChangeFile(string newFilePath)
			{
				mciSendString($"open \"{newFilePath}\" type MPEGVideo alias {alias}", null, 0, IntPtr.Zero);
			}

			public void Restart()
			{
				mciSendString($"seek {alias} to start", null, 0, IntPtr.Zero);
			}

			[DllImport("winmm.dll")]
			private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hWndCallback);
		}

		public static Mp3Player honkBiteSoundPlayer;

		public static Mp3Player musicPlayer;

		public static Mp3Player environmentSoundsPlayer;

		//private static readonly Stream[] patSources = new Stream[3]
		//{
		//	Resources.Pat1,
		//	Resources.Pat2,
		//	Resources.Pat3
		//};

		private static SoundPlayer[] patSoundPool;

		private static readonly string[] honkSources = new string[4]
		{
			Program.GetPathToFileInAssembly("Assets/Sound/NotEmbedded/Honk1.mp3"),
			Program.GetPathToFileInAssembly("Assets/Sound/NotEmbedded/Honk2.mp3"),
			Program.GetPathToFileInAssembly("Assets/Sound/NotEmbedded/Honk3.mp3"),
			Program.GetPathToFileInAssembly("Assets/Sound/NotEmbedded/Honk4.mp3")
		};

		private static readonly string biteSource = Program.GetPathToFileInAssembly("Assets/Sound/NotEmbedded/BITE.mp3");

		public static void Init()
		{
			honkBiteSoundPlayer = new Mp3Player(honkSources[0], "honkPlayer");
			//patSoundPool = new SoundPlayer[patSources.Length];
			//for (int i = 0; i < patSources.Length; i++)
			//{
			//	patSoundPool[i] = new SoundPlayer(patSources[i]);
			//	patSoundPool[i].Load();
			//}
			environmentSoundsPlayer = new Mp3Player(Program.GetPathToFileInAssembly("Assets/Sound/NotEmbedded/MudSquith.mp3"), "assortedEnvironment");
			string pathToFileInAssembly = Program.GetPathToFileInAssembly("Assets/Sound/Music/Music.mp3");
			if (File.Exists(pathToFileInAssembly))
			{
				musicPlayer = new Mp3Player(pathToFileInAssembly, "musicPlayer");
				musicPlayer.loop = true;
				musicPlayer.SetVolume(0.5f);
				musicPlayer.Play();
			}
		}

		public static void PlayPat()
		{
			if (!GooseConfig.settings.SilenceSounds && patSoundPool != null && patSoundPool.Length > 0)
			{
				int num = (int)(SamMath.Rand.NextDouble() * (double)patSoundPool.Length);
				SoundPlayer soundPlayer = patSoundPool[num];

				if (soundPlayer != null && soundPlayer.Stream != null && soundPlayer.Stream.CanSeek)
				{
					soundPlayer.Stream.Seek(0L, SeekOrigin.Begin);
				}

				soundPlayer?.Play();
			}
		}


		public static void HONCC()
		{
			int num = (int)(SamMath.Rand.NextDouble() * (double)honkSources.Length);
			honkBiteSoundPlayer.Pause();
			honkBiteSoundPlayer.Dispose();
			honkBiteSoundPlayer.ChangeFile(honkSources[num]);
			honkBiteSoundPlayer.SetVolume(0.8f);
			honkBiteSoundPlayer.Play();
		}

		public static void CHOMP()
		{
			honkBiteSoundPlayer.Pause();
			honkBiteSoundPlayer.Dispose();
			honkBiteSoundPlayer.ChangeFile(biteSource);
			honkBiteSoundPlayer.SetVolume(0.07f);
			honkBiteSoundPlayer.Play();
		}

		public static void PlayMudSquith()
		{
			environmentSoundsPlayer.Restart();
			environmentSoundsPlayer.Play();
		}
	}
}
