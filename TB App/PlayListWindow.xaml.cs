using System.Collections.Generic;
using System.Windows;
using WMPLib;
using System.Windows.Controls;

namespace TB_App
{
	public partial class PlayListWindow : Window
	{
		private WindowsMediaPlayer IplayList;
		private List<string> playPaths = new List<string>(); // list to save Paths of music files

		public PlayListWindow()
		{
			InitializeComponent();
		}

		// method to load paths from main form to List
		public void LoadPath(string[] paths)
		{
			playPaths.Clear();
			foreach (string item in paths)
				playPaths.Add(item);
		}

		// method to clear ListBox
		public void ClearListBox()
		{
			playListBox.Items.Clear();
		}

		// method to fill ListBox with Paths
		public void FillListBox()
		{
			foreach (string item in playPaths)
				playListBox.Items.Add(StringFix(item));
		}

		// Fix string to show
		protected string StringFix(string strToFix)
		{
			string[] words = strToFix.Split(new char[] { '\\' });
			string newSongName = words[2].Replace(".mp3", "");

			return newSongName;
		}

		// method to fill ListBox with Paths on first load
		private void grid_loaded(object sender, RoutedEventArgs e)
		{
			playListBox.Items.Clear();
			FillListBox();
		}

		// if close second window - just hide it
		private void playClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Visibility = Visibility.Hidden;
		}

		// method to set PlayList
		public void setIplayList(WindowsMediaPlayer IplayList)
		{
			this.IplayList = IplayList;
		}

		// play selected in ListBox song & show name in textBox
		private void playListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (playListBox.SelectedIndex >= 0)
			{
				int songNumber = playListBox.SelectedIndex;
				string tempSong = playPaths[songNumber];

				MPlayer.Iter = playListBox.SelectedIndex + 1;
				MPlayer.TextBoxSongName = tempSong;

				MainWindow mw = (MainWindow)Application.Current.MainWindow;  // use current MAINWINDOW !!!
				mw.SetTextBoxProperty = StringFix(tempSong); // set fixed str into songName (textBox)

				IplayList.URL = tempSong;
				IplayList.controls.play();
			}
		}
	}
}
