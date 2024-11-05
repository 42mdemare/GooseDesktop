using System;
using System.Text;

namespace TwitchDesktopShared
{
	[Serializable]
	public class MMFMessage
	{
		public enum MessageType
		{
			AttachToTwitch,
			StartTask
		}

		public MessageType type;

		public string args;

		public MMFMessage(MessageType t, string a)
		{
			type = t;
			args = a;
		}

		public MMFMessage(MessageType t, string[] args)
		{
			type = t;
			StringBuilder stringBuilder = new StringBuilder(4095);
			for (int i = 0; i < args.Length; i++)
			{
				stringBuilder.Append(args[i] + " ");
			}
		}

		public MMFMessage(string s)
		{
			int num = s.IndexOf(" ");
			int result = -1;
			int.TryParse(s.Substring(0, num), out result);
			type = (MessageType)result;
			args = s.Substring(num);
		}

		public MMFMessage(byte[] bytes)
			: this(Encoding.UTF8.GetString(bytes))
		{
		}

		public byte[] GetBytes()
		{
			Encoding uTF = Encoding.UTF8;
			int num = (int)type;
			return uTF.GetBytes(num + " " + args);
		}
	}
}
