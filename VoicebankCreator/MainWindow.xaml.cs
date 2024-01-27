using VoicebankCreator.Controls;
using NAudio.Wave;
using System.Windows.Threading;
using VoicebankCreator.Media;

namespace VoicebankCreator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : BackdropWindow {
	private string? FilePath;
	private readonly DispatcherTimer timer;
	private readonly AudioPlayer audioPlayer = new();
	private WriteableBitmap? writeableBitmap;

	public MainWindow() {
		InitializeComponent();
		timer = new() { Interval = TimeSpan.FromMilliseconds(100) };
		timer.Tick += Timer_Tick;
		timer.Start();
		Window_SizeChanged(this, null);
	}

	private void Timer_Tick(object? sender, EventArgs e) {
		if (!IsPlaying) return;

		UpdateTimeSpan();

		AudioFileReader? audio = audioPlayer.audioForWaveform;
		if (audio != null && writeableBitmap != null) {
			DrawWaveform();
		}
	}

	private void OpenBtn_Click(object? sender, RoutedEventArgs? e) {
		OpenFileDialog? dialog = new() {
			Filter = "All supported files|*.yml;*.yaml;*.mp4;*.mov;*.m4v;*.wmv;*.asf;*.avi;*.3g2;*.3gp;*.3gp2;*.3gpp;*.wav;*.aiff;*.mp3;*.wma;*.aac;*.m4a;*.adts;*.flac|" +
				"YAML files|*.yml;*.yaml|" +
				"Video files|*.mp4;*.mov;*.m4v;*.wmv;*.asf;*.avi;*.3g2;*.3gp;*.3gp2;*.3gpp|" +
				"Audio files|*.wav;*.aiff;*.mp3;*.wma;*.aac;*.m4a;*.adts;*.flac|" +
				"All files|*.*",
		};
		if (dialog.ShowDialog() != true) return;
		FilePath = dialog.FileName;

		Player.Source = new Uri(FilePath);
		Player_ShowFrame();
		audioPlayer.FilePath = FilePath;
		DrawWaveform();
	}

	private void Player_ShowFrame() {
		Player.Play();
		Player.Pause();
		UpdateTimeSpan();
	}

	private void ToolBar_Loaded(object sender, RoutedEventArgs? e) {
		ToolBar toolBar = (ToolBar)sender;
		if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
			overflowGrid.Visibility = Visibility.Collapsed;
		if (toolBar.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
			mainPanelBorder.Margin = new Thickness();
	}

	private bool IsPlaying { get; set; } = false;

	private void PlayBtn_Click(object? sender, RoutedEventArgs? e) {
		//audioPlayer.Play(1);
		Player.Play();
		IsPlaying = true;
	}

	private void PlaySlowBtn_Click(object? sender, RoutedEventArgs? e) {
		audioPlayer.Play(0.5);
	}

	private void PauseBtn_Click(object? sender, RoutedEventArgs? e) {
		Player.Pause();
		IsPlaying = false;
	}

	private void StopBtn_Click(object? sender, RoutedEventArgs? e) {
		audioPlayer.Stop();
		Player.Stop();
		Player_ShowFrame();
		IsPlaying = false;
	}

	private void Window_SizeChanged(object? sender, SizeChangedEventArgs? e) {
		if (e != null && e.PreviousSize.Width == e.NewSize.Width) return;
		int width = (int)WaveformOutCanvas.ActualWidth;
		int height = (int)WaveformOutCanvas.ActualHeight;
		if (width > 0 && height > 0) {
			WaveformImage.Width = width;
			WaveformImage.Height = height;
			const int DPI = 96;
			writeableBitmap = new WriteableBitmap(width, height, DPI, DPI, PixelFormats.Bgra32, null);
			WaveformImage.Source = writeableBitmap;
			DrawWaveform();
		}
	}

	private void DrawWaveform() {
		if (writeableBitmap != null)
			audioPlayer.DrawWaveform(writeableBitmap, SystemParameters.WindowGlassColor);
	}

	private void UpdateTimeSpan() {
		CurrentTimeLbl.Text = Player.Position.ToString(@"hh\:mm\:ss");
	}

	private void Player_MediaEnded(object sender, RoutedEventArgs e) {
		IsPlaying = false;
	}

	private void Window_ThemeChange(object sender, RoutedEventArgs e) {
		DrawWaveform();
	}
}
