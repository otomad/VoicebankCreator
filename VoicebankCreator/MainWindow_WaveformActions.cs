using VoicebankCreator.Controls;

namespace VoicebankCreator;

public partial class MainWindow {
	private void WaveformOutCanvas_MouseWheel(object? sender, MouseWheelEventArgs e) {
		bool isZoomIn = e.Delta > 0;
		Duration *= Math.Pow(0.75, isZoomIn ? 1 : -1);
		DrawWaveform();
	}

	/// <summary>
	/// False - ignore mouse movements and don't scroll
	/// </summary>
	private bool isWaveformMoving = false;
	private Point? waveformMoveStartPosition = null;
	private bool isDrawingRangeZone = false;
	/// <summary>
	/// True - Mouse down -> Mouse up without moving -> Move;<br />
	/// False - Mouse down -> Move
	/// </summary>
	private bool isDeferredMovingStarted = false;

	private void WaveformOutCanvas_MiddleMouseDown(object sender, MouseButtonEventArgs e) {
		if (isWaveformMoving == true) // Moving with a released wheel and pressing a button
			CancelWaveformMoving();
		if (e.ButtonState == MouseButtonState.Pressed && CursorLine.Visibility == Visibility.Visible) {
			if (e.ChangedButton == MouseButton.Middle && !isWaveformMoving) { // Pressing a wheel the first time
				((UIElement)e.Source).CaptureMouse();
				isWaveformMoving = true;
				waveformMoveStartPosition = e.GetPosition(sender as IInputElement);
				Mouse.OverrideCursor = Cursors.SizeWE;
				CurrentTimeSlider_MouseLeftButtonDown(null, null);
			}
		}
	}

	private void WaveformOutCanvas_LeftMouseDown(object sender, MouseButtonEventArgs e) {
		if (e.ButtonState == MouseButtonState.Pressed && CursorLine.Visibility == Visibility.Visible) {
			if (e.ChangedButton == MouseButton.Left && !isDrawingRangeZone) {
				((UIElement)e.Source).CaptureMouse();
				isDrawingRangeZone = true;
				waveformMoveStartPosition = e.GetPosition(sender as IInputElement);
				isDeferredMovingStarted = true; // the default value is true until the opposite value is set
				Mouse.OverrideCursor = Cursors.IBeam;
				Pause();
			}
		}
	}

	private void WaveformOutCanvas_MiddleMouseUp(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Released) {
			((UIElement)e.Source).ReleaseMouseCapture();
			CancelWaveformMoving();
		}
	}

	private void WaveformOutCanvas_LeftMouseUp(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Released) {
			((UIElement)e.Source).ReleaseMouseCapture();
			bool isNotCreateRangeZone = isDeferredMovingStarted;
			CancelWaveformMoving();
			if (!isNotCreateRangeZone) SubmitCreateRangeZone();
		}
	}

	private void CancelWaveformMoving() {
		isWaveformMoving = false;
		isDrawingRangeZone = false;
		waveformMoveStartPosition = null;
		isDeferredMovingStarted = false;
		Mouse.OverrideCursor = null;
		CurrentTimeSlider_MouseLeftButtonUp(null, null);
	}

	private void SubmitCreateRangeZone() {
		if (ActiveRangeZone == null) return;
		if (ActiveRangeZone.ActualWidth < 10) {
			RangeZonesCanvas.Children.Remove(ActiveRangeZone);
			ActiveRangeZone = null;
			return;
		}
		ActiveRangeZone.IsHitTestVisible = true;
		ActiveRangeZone.Resize += RangeZone_Resize;
		ActiveRangeZone.MouseDown += RangeZone_MouseDown;
		UpdateRangeZoneSeconds(ActiveRangeZone);
		rangeZones.Add(ActiveRangeZone);
		RefreshRangeZonesCanvas();
	}

	private void UpdateRangeZoneSeconds(RangeZone rangeZone) {
		double startPixel = Canvas.GetLeft(rangeZone), lengthPixel = rangeZone.ActualWidth;
		double widthPixel = RangeZonesCanvas.ActualWidth;
		rangeZone.StartSeconds = Player.Position.TotalSeconds - Duration + startPixel / widthPixel * Duration * 2;
		rangeZone.LengthSeconds = lengthPixel / widthPixel * Duration * 2;
	}

	private void RangeZone_Resize(object? sender, RangeZone.RangeZoneResizeEventArgs e) {
		if (sender is not RangeZone rangeZone) return;
		double dpi = Dpi.Get;
		double left = RangeZonesCanvas.PointToScreen(new Point(0, 0)).X / dpi;
		if (e.Side == RangeZone.Side.End) {
			double offset = e.CurrentPosition - e.StartPosition;
			rangeZone.Width = Math.Abs(offset);
			Canvas.SetLeft(rangeZone, e.StartPosition - left + (offset < 0 ? offset : 0));
		} else {
			double offset = e.CurrentPosition - e.EndPosition;
			rangeZone.Width = Math.Abs(offset);
			Canvas.SetLeft(rangeZone, e.EndPosition - left + (offset < 0 ? offset : 0));
		}
		UpdateRangeZoneSeconds(rangeZone);
	}

	private void RefreshRangeZonesCanvas() {
		double widthPixel = RangeZonesCanvas.ActualWidth;
		double startSeconds = Player.Position.TotalSeconds - Duration, endSeconds = Player.Position.TotalSeconds + Duration;
		foreach (RangeZone rangeZone in rangeZones) {
			//Debug.WriteLine($"{rangeZone.StartSeconds} {rangeZone.LengthSeconds}");
			if (rangeZone.StartSeconds + rangeZone.LengthSeconds <= startSeconds || rangeZone.StartSeconds >= endSeconds) {
				if (RangeZonesCanvas.Children.Contains(rangeZone))
					RangeZonesCanvas.Children.Remove(rangeZone);
				continue;
			}
			if (!RangeZonesCanvas.Children.Contains(rangeZone))
				try {
					RangeZonesCanvas.Children.Add(rangeZone);
				} catch (Exception) {
					continue;
				}
			rangeZone.Width = rangeZone.LengthSeconds / (Duration * 2) * widthPixel;
			Canvas.SetLeft(rangeZone, (rangeZone.StartSeconds - Player.Position.TotalSeconds + Duration) / (Duration * 2) * widthPixel);
		}
		//Debug.WriteLine("");
	}

	private void ClearAllRangeZones() {
		for (int i = rangeZones.Count - 1; i >= 0; i--) {
			RangeZone rangeZone = rangeZones[i];
			rangeZones.RemoveAt(i);
			RangeZonesCanvas.Children.Remove(rangeZone);
		}
	}

	private readonly List<RangeZone> rangeZones = new();

	private RangeZone? activeRangeZone;
	private RangeZone? ActiveRangeZone {
		get => activeRangeZone;
		set {
			if (activeRangeZone != null)
				activeRangeZone.IsActive = false;
			activeRangeZone = value;
			if (activeRangeZone != null)
				activeRangeZone.IsActive = true;
			OnPropertyChanged(nameof(HasRangeZoneSelected));
		}
	}

	public bool HasRangeZoneSelected {
		get => ActiveRangeZone != null;
		set => OnPropertyChanged(nameof(HasRangeZoneSelected));
	}

	private void RangeZone_MouseDown(object sender, MouseButtonEventArgs e) {
		if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
			ActiveRangeZone = sender as RangeZone;
	}

	private void WaveformOutCanvas_MouseMove(object sender, MouseEventArgs e) {
		if (CursorLine.Visibility == Visibility.Visible && waveformMoveStartPosition.HasValue) {
			Point currentPosition = e.GetPosition(WaveformImage);
			Vector offset = currentPosition - waveformMoveStartPosition.Value;
			if (isWaveformMoving) {
				CurrentTimeSlider.Value -= offset.X / WaveformOutCanvas.ActualWidth * Duration * 2;
				waveformMoveStartPosition = currentPosition;
			} else if (isDrawingRangeZone) {
				bool previousDeferredMovingStarted = isDeferredMovingStarted;
				if (Math.Abs(offset.X) > 10) isDeferredMovingStarted = false;
				if (isDeferredMovingStarted) return;
				if (previousDeferredMovingStarted && !isDeferredMovingStarted) {
					ActiveRangeZone = new() {
						Width = Math.Abs(offset.X),
						Height = RangeZonesCanvas.ActualHeight,
						IsHitTestVisible = false,
					};
					RangeZonesCanvas.Children.Add(ActiveRangeZone);
					Canvas.SetTop(ActiveRangeZone, 0);
				} else if (ActiveRangeZone != null) {
					ActiveRangeZone.Width = Math.Abs(offset.X);
				}
				Canvas.SetLeft(ActiveRangeZone, waveformMoveStartPosition.Value.X + (offset.X < 0 ? offset.X : 0));
			}
		}
	}
}
