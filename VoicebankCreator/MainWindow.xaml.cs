using VoicebankCreator.Controls;
using NAudio.Wave;
using System.Windows.Threading;
using VoicebankCreator.Media;
using System.Numerics;
using VoicebankCreator.ViewModels;

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
		IsPlaying = false;
		timer = new() { Interval = TimeSpan.FromMilliseconds(20) };
		timer.Tick += Timer_Tick;
		timer.Start();
		Window_SizeChanged(this, null);
		CurrentTimeSlider.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(CurrentTimeSlider_MouseLeftButtonDown), true);
		CurrentTimeSlider.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(CurrentTimeSlider_MouseLeftButtonUp), true);
	}

	private void Timer_Tick(object? sender, EventArgs e) {
		if (IsCurrentTimeSliderChanging != IsCurrentTimeSliderChangingState.NoChanging) {
			Player.IsMuted = true;
			CurrentTimeLbl.Text = Player.Position.ToString(TIME_SPAN_FORMAT);
			Player.Position = TimeSpan.FromSeconds(CurrentTimeSlider.Value);
			DrawWaveform();
			Player.IsMuted = false;
		} else if (IsPlaying) {
			UpdateTimeSpan();
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

		ClearAllRangeZones();
		Player.Source = new Uri(FilePath);
		Player_ShowFrame();
	}

	private void Player_MediaOpened(object sender, RoutedEventArgs e) {
		CurrentTimeSlider.Maximum = PlayerDuration;
		TotalTimeLbl.Text = Player.NaturalDuration.TimeSpan.ToString(TIME_SPAN_FORMAT);
		Player_ShowFrame();
		audioPlayer.FilePath = FilePath;
		DrawWaveform();
		duration = 2.5;
		AudioLoaded = true;
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

	public ToolBarButtonViewModel PlayingCaption { get; } = new();
	private bool isPlaying = false;
	private bool IsPlaying {
		get => isPlaying;
		set {
			isPlaying = value;
			PlayingCaption.Name = isPlaying ? Properties.Resources.PauseButton : Properties.Resources.PlayButton;
			PlayingCaption.Icon = isPlaying ? "\ue769" : "\ue768";
		}
	}

	private bool audioLoaded = false;
	public bool AudioLoaded {
		get => audioLoaded;
		set { audioLoaded = value; OnPropertyChanged(nameof(AudioLoaded)); }
	}

	public void Play() {
		//audioPlayer.Play(1);
		Player.Play();
		IsPlaying = true;
	}

	public void Pause() {
		Player.Pause();
		IsPlaying = false;
		DrawWaveform();
	}

	private void PlayBtn_Click(object? sender, RoutedEventArgs? e) {
		if (!IsPlaying) Play();
		else Pause();
	}

	private void PlaySelectedBtn_Click(object? sender, RoutedEventArgs? e) {
		double playbackRate = sender == PlaySelectedBtn ? 1 : 0.5;
		if (ActiveRangeZone != null)
			audioPlayer.Play(playbackRate, ActiveRangeZone.StartSeconds, ActiveRangeZone.LengthSeconds);
	}

	private void StopBtn_Click(object? sender, RoutedEventArgs? e) {
		audioPlayer.Stop();
		Player.Stop();
		Player_ShowFrame();
		IsPlaying = false;
		DrawWaveform();
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

	private double PlayerDuration => Player.NaturalDuration.HasTimeSpan ? Player.NaturalDuration.TimeSpan.Seconds : 0;

	private double duration = 2.5;
	public double Duration {
		get => duration;
		set {
			if (PlayerDuration > 0)
				duration = Math.Clamp(value, 0.025, PlayerDuration);
		}
	}

	private void DrawWaveform() {
		TimeSpan timeSpan = TimeSpan.FromSeconds(Duration);
		if (writeableBitmap != null)
			audioPlayer.DrawWaveform(writeableBitmap, Player.Position - timeSpan, Player.Position + timeSpan, SystemParameters.WindowGlassColor);
		RefreshRangeZonesCanvas();
	}

	private const string TIME_SPAN_FORMAT = @"hh\:mm\:ss";

	private enum IsCurrentTimeSliderChangingState {
		NoChanging,
		ChangingAfterPausing,
		ChangingAfterPlaying,
	}

	private IsCurrentTimeSliderChangingState IsCurrentTimeSliderChanging { get; set; } = IsCurrentTimeSliderChangingState.NoChanging;

	private void UpdateTimeSpan() {
		CurrentTimeLbl.Text = Player.Position.ToString(TIME_SPAN_FORMAT);
		CurrentTimeSlider.Value = Player.Position.TotalSeconds;
	}

	private void Player_MediaEnded(object? sender, RoutedEventArgs? e) {
		IsPlaying = false;
	}

	private void Window_ThemeChange(object? sender, RoutedEventArgs? e) {
		DrawWaveform();
	}

	private void CurrentTimeSlider_MouseLeftButtonDown(object? sender, MouseButtonEventArgs? e) {
		IsCurrentTimeSliderChanging = IsPlaying ? IsCurrentTimeSliderChangingState.ChangingAfterPlaying : IsCurrentTimeSliderChangingState.ChangingAfterPausing;
		Pause();
	}

	private void CurrentTimeSlider_MouseLeftButtonUp(object? sender, MouseButtonEventArgs? e) {
		if (IsCurrentTimeSliderChanging == IsCurrentTimeSliderChangingState.ChangingAfterPlaying)
			Play();
		IsCurrentTimeSliderChanging = IsCurrentTimeSliderChangingState.NoChanging;
	}

	private void RemoveSelectedBtn_Click(object sender, RoutedEventArgs e) {
		if (ActiveRangeZone == null) return;
		RangeZonesCanvas.Children.Remove(ActiveRangeZone);
		rangeZones.Remove(ActiveRangeZone);
		ActiveRangeZone = null;
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
		StopBtn_Click(null, null);
		audioPlayer.FilePath = null;
	}
}
