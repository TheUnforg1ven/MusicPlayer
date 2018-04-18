using System;
using System.IO;

namespace TB_App
{
	class MPlayer
	{
		static private int iter;
		static public int Iter { get { return iter; } set { iter = value; } }

		static private string textBoxSongName;
		static public string  TextBoxSongName { get { return textBoxSongName; } set { textBoxSongName = value; } }

		public MPlayer()
		{
			iter = 1;
			textBoxSongName = String.Empty;
		}

		// write into file
		public void Save(string musicFilePath, string[] songArray)
		{
			using(StreamWriter writer = new StreamWriter(musicFilePath))
			{
				foreach (string item in songArray)
				{
					writer.WriteLine(item);
				}
			}
		}

		// read from file
		public string[] OpenFile(string musicFilePath, string[] songArray)
		{
			songArray = File.ReadAllLines(musicFilePath);

			return songArray;
		}
	}
}
