using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using WMPLib;
using System.IO;

namespace TB_App
{
	public partial class MainWindow : Window
	{
		protected WindowsMediaPlayer Chose = new WindowsMediaPlayer();

		MPlayer mplayer = new MPlayer(); 
		PlayListWindow playWindow;
		DispatcherTimer timer = null;

		protected string[] paths;
		private bool isVoiceOn = true;
		private string musicPath = @"songs.txt";

		public MainWindow()
		{
			InitializeComponent();
			playWindow = new PlayListWindow(); // initialize playlist window
			playWindow.setIplayList(Chose);

			//open file
			paths = fillPathsArr(paths);


			// init timer and EventHandler
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(10);
			timer.Tick += new EventHandler(timer_Tick);
			
			Chose.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Chose_PlayStateChange);
		}

		// property to set textBox - songName, cause it's PRIVATE
		public string SetTextBoxProperty
		{
			get
			{
				return songName.Text;
			}
			set
			{
				songName.Text = value;
			}
		}

		// method to check file and Fill playlist if file isn't empty
		private string[] fillPathsArr(string[] paths)
		{
			string[] isEmpty = File.ReadAllLines(musicPath);
			if (isEmpty != null)
			{
				paths = mplayer.OpenFile(musicPath, paths);
				playWindow.ClearListBox();
				playWindow.LoadPath(paths);
				playWindow.FillListBox();
				openPlaylist.IsEnabled = true;
			}

			return paths;
		}

		//********************************************************************
		//
		//OpenFileDialog, check if Paths isn't empty
		//Fill paths with filenames
		//Clear listBox, enable/disable next&prev buttons
		//Load Paths intj Playlist List, Fill listBox
		//Enable OpenPlayList button , Start Play First Song (and instant stop)
		//
		//********************************************************************
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.Multiselect = true;

			if (openFileDialog1.ShowDialog() == true)
			{
				if(paths != null)
					Array.Clear(paths, 0, paths.Length);

				paths = openFileDialog1.FileNames;

				playWindow.ClearListBox();

				if (paths.Length == 1)
				{
					nextBut.IsEnabled = false;
					prevBut.IsEnabled = false;
				}

				else
				{
					nextBut.IsEnabled = true;
					prevBut.IsEnabled = true;
				}
				
				playWindow.LoadPath(paths);
				playWindow.FillListBox();

				openPlaylist.IsEnabled = true;
				mplayer.Save(musicPath, paths);

				string songnameF = paths[0];
				songName.Text = StringFix(songnameF);
				Chose.URL = songnameF;
				Chose.controls.stop();
			}
		}

		// start playing song
		
		private void playButt_Click(object sender, RoutedEventArgs e)
		{
			Chose.controls.play();
		}

		// pause song
		private void pauseBut_Click_1(object sender, RoutedEventArgs e)
		{
			Chose.controls.pause();
		}

		// go to next song, if song isn't last in queue
		private void nextBut_Click_1(object sender, RoutedEventArgs e)
		{
			if (MPlayer.Iter < paths.Length)
			{
				Chose.URL = paths[MPlayer.Iter];
				songName.Text = StringFix(paths[MPlayer.Iter]);
				MPlayer.Iter++;
			}
		}

		// go to prev song, if song is not first in queue
		private void prevBut_Click(object sender, RoutedEventArgs e)
		{
			if (MPlayer.Iter != 0)
			{
				MPlayer.Iter--;
				Chose.URL = paths[MPlayer.Iter];
				songName.Text = StringFix(paths[MPlayer.Iter]);
			}
		}

		// full stop playing song
		private void stopBut_Click(object sender, RoutedEventArgs e)
		{
			Chose.controls.stop();
		}

		// Normalize string name to show in textbox PUBLIC METHOD
		protected string StringFix(string strToFix)
		{
			string[] words = strToFix.Split(new char[] { '\\' });
			string newSongName = words[2].Replace(".mp3", "");

			return newSongName;
		}

		// voice button, change button image
		private void voiceBut_Click(object sender, RoutedEventArgs e)
		{
			ControlTemplate ct = voiceBut.Template;
			Image changedImg = (Image)ct.FindName("volumeImage", voiceBut);

			if (isVoiceOn)
			{ 
				changedImg.Source = new BitmapImage(new Uri("/Icons/Mute.png", UriKind.RelativeOrAbsolute));
				Chose.settings.volume = 0;
				isVoiceOn = false;
			}

			else
			{
				changedImg.Source = new BitmapImage(new Uri("/Icons/Sound.png", UriKind.RelativeOrAbsolute));
				Chose.settings.volume = (int)setVolume.Value;
				isVoiceOn = true;
			}
		}

		// change volume
		private void setVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Chose.settings.volume = (int)setVolume.Value;
		}

		// Open playlist
		private void openPlaylist_Click(object sender, RoutedEventArgs e)
		{
			playWindow.Show();
		}

		// when load main window - Main is owner of playlist window
		private void mainLoad(object sender, RoutedEventArgs e)
		{
			playWindow.Owner = this;
		}

		// timer tick to change song automaticaly
		void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			Chose.controls.play();
		}

		// if newstate == 8 -> end of song -> go to next song by Click next_But 
		void Chose_PlayStateChange(int NewState)
		{
			if (NewState == 8 && MPlayer.Iter < paths.Length)
			{
				nextBut_Click_1(null, null);
				timer.Start();
			}
		}
	}
}