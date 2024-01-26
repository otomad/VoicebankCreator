using VoicebankCreator.Controls;
using VoicebankCreator.Helpers;
using NAudio.WaveFormRenderer;
using NAudio.Wave;
using DrawingColor = System.Drawing.Color;
using DrawingPen = System.Drawing.Pen;
using NAudio.Wave.SampleProviders;
using System.Windows.Threading;
using VoicebankCreator.Media;
using System.Numerics;
using Microsoft.VisualBasic.Devices;

namespace VoicebankCreator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BackdropWindow {
	private readonly WaveFormRenderer renderer = new();
	//private WaveOutEvent? outputDevice;
	private string? FilePath;
	private readonly DispatcherTimer timer;
	private readonly AudioPlayer audioPlayer = new();
	//private AudioFileReader? audio;

	public MainWindow() {
		InitializeComponent();
		timer = new() {
			Interval = TimeSpan.FromMilliseconds(100)
		};
		timer.Tick += Timer_Tick;
		timer.Start();
	}

	private readonly static IPeakProvider? peakProvider = new MaxPeakProvider();

	private readonly static StandardWaveFormRendererSettings? myRendererSettings = new() {
		Width = 640,
		TopHeight = 32,
		BottomHeight = 32,
		BackgroundColor = DrawingColor.Transparent,
		TopPeakPen = new DrawingPen(DrawingColor.Green),
		BottomPeakPen = new DrawingPen(DrawingColor.Green),
	};

	private void Timer_Tick(object? sender, EventArgs e) {
		double current = Player.Position.TotalSeconds;
		CurrentTimeLbl.Text = GetTimecode(current);

		AudioFileReader? audio = audioPlayer.audioForWaveform;
		if (audio != null) {
			audioPlayer.audioForWaveform?.PrepareToReplay();
			OffsetSampleProvider trimmed = new(audio) {
				SkipOver = TimeSpan.FromSeconds(current),
				Take = TimeSpan.FromSeconds(current + 5),
			};
			float[] floatBuffer = new float[5 * trimmed.WaveFormat.AverageBytesPerSecond];
			trimmed.Read(floatBuffer, 0, floatBuffer.Length);
			byte[] buffer = floatBuffer.SelectMany(f => BitConverter.GetBytes(f)).ToArray();
			using RawSourceWaveStream newAudio = new(new MemoryStream(buffer), trimmed.WaveFormat);
			ImageSource image = renderer.Render(newAudio, peakProvider, myRendererSettings).ToImageSource();
			WaveformImage.Source = image;
		}
	}
	
	private static string GetTimecode(double seconds) {
		long time = (long)Math.Floor(seconds);
		string s = (time % 60).ToString("D2");
		time /= 60;
		string m = (time % 60).ToString("D2");
		time /= 60;
		string h = time.ToString("D2");
		return $"{h}:{m}:{s}";
	}

	private void OpenBtn_Click(object sender, RoutedEventArgs e) {
		OpenFileDialog? dialog = new() {
			Filter = "All supported files|*.yml;*.yaml;*.mp4;*.mov;*.m4v;*.wmv;*.asf;*.avi;*.3g2;*.3gp;*.3gp2;*.3gpp;*.wav;*.aiff;*.mp3;*.wma;*.aac;*.m4a;*.adts;*.ogg;*.flac|" +
				"YAML files|*.yml;*.yaml|" +
				"Video files|*.mp4;*.mov;*.m4v;*.wmv;*.asf;*.avi;*.3g2;*.3gp;*.3gp2;*.3gpp|" +
				"Audio files|*.wav;*.aiff;*.mp3;*.wma;*.aac;*.m4a;*.adts;*.ogg;*.flac|" +
				"All files|*.*",
		};
		if (dialog.ShowDialog() != true) return;
		FilePath = dialog.FileName;

		Player.Source = new Uri(FilePath);
		Player_ShowFrame();
		audioPlayer.FilePath = FilePath;
	}

	private void Player_ShowFrame() {
		Player.Play();
		Player.Pause();
	}

	private void ToolBar_Loaded(object sender, RoutedEventArgs e) {
		ToolBar toolBar = (ToolBar)sender;
		if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
			overflowGrid.Visibility = Visibility.Collapsed;
		if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
			mainPanelBorder.Margin = new Thickness();
	}

	private void PlayBtn_Click(object sender, RoutedEventArgs e) {
		//audioPlayer.Play(1);
		Player.Play();
	}

	private void PlaySlowBtn_Click(object sender, RoutedEventArgs e) {
		audioPlayer.Play(0.5);
	}

	private void PauseBtn_Click(object sender, RoutedEventArgs e) {
		Player.Pause();
	}

	private void StopBtn_Click(object sender, RoutedEventArgs e) {
		audioPlayer.Stop();
		Player.Stop();
		Player_ShowFrame();
	}
}
