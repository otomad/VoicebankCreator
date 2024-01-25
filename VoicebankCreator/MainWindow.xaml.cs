using VoicebankCreator.Controls;
using VoicebankCreator.Helpers;
using NAudio.WaveFormRenderer;
using NAudio.Wave;
using DrawingColor = System.Drawing.Color;
using DrawingPen = System.Drawing.Pen;
using NAudio.Wave.SampleProviders;
using System.Windows.Threading;

namespace VoicebankCreator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BackdropWindow {
	private readonly WaveFormRenderer renderer = new();
	private WaveOutEvent? outputDevice;
	private string? FilePath;
	private readonly DispatcherTimer timer;
	private AudioFileReader? audio;

	public MainWindow() {
		InitializeComponent();
		timer = new() {
			Interval = TimeSpan.FromMilliseconds(100)
		};
		timer.Tick += Timer_Tick;
		timer.Start();
	}

	private void Timer_Tick(object? sender, EventArgs e) {
		if (audio == null) return;
		double current = Player.Position.TotalSeconds;
		OffsetSampleProvider trimmed = new(audio) {
			SkipOver = TimeSpan.FromSeconds(current),
			Take = TimeSpan.FromSeconds(current + 5),
		};
		ImageSource image = renderer.Render(trimmed, peakProvider, myRendererSettings).ToImageSource();
		WaveformImage.Source = image;
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

		audio?.Dispose();
		audio = new(FilePath);
		outputDevice?.Stop();
		outputDevice?.Dispose();
	}

	private void ToolBar_Loaded(object sender, RoutedEventArgs e) {
		ToolBar toolBar = (ToolBar)sender;
		if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
			overflowGrid.Visibility = Visibility.Collapsed;
		if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
			mainPanelBorder.Margin = new Thickness();
	}

	private void PlayBtn_Click(object sender, RoutedEventArgs e) {
		//Play(1);
		Player.Play();

	}

	private void PlaySlowBtn_Click(object sender, RoutedEventArgs e) {
		Play(0.5);
	}

	private void PauseBtn_Click(object sender, RoutedEventArgs e) {
		Player.Pause();
	}

	private void StopBtn_Click(object sender, RoutedEventArgs e) {
		outputDevice?.Stop();
		Player.Stop();
	}

	private void Play(double playbackRate) {
		if (audio == null) return;
		new Thread(() => {
			outputDevice?.Stop();
			outputDevice = new();
			IWaveProvider resultAudio;
			using AudioFileReader audio = new(FilePath);
			resultAudio = audio;
			if (playbackRate != 1) {
				SampleToWaveProvider16 wave16 = new(audio);
				WaveFormat format = new((int)(wave16.WaveFormat.SampleRate * playbackRate), wave16.WaveFormat.BitsPerSample, wave16.WaveFormat.Channels);
				byte[] buffer = new byte[audio.Length];
				wave16.Read(buffer, 0, buffer.Length);
				using RawSourceWaveStream newAudio = new(new MemoryStream(buffer), format);
				resultAudio = newAudio;
			}

			outputDevice.Init(resultAudio);
			outputDevice.Play();
			outputDevice.PlaybackStopped += (sender, e) => {
				if (sender is not WaveOutEvent outputDevice) return;
				outputDevice.Dispose();
			};
			while (outputDevice?.PlaybackState == PlaybackState.Playing) {
				Thread.Sleep(500);
			}
		}).Start();
	}
}
