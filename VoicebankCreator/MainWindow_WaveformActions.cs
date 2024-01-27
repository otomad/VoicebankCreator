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

	private void WaveformOutCanvas_MouseDown(object sender, MouseButtonEventArgs e) {
		if (isWaveformMoving == true) // Moving with a released wheel and pressing a button
			CancelWaveformMoving();
		if (e.ButtonState == MouseButtonState.Pressed && CursorLine.Visibility == Visibility.Visible) {
			if (e.ChangedButton == MouseButton.Middle && !isWaveformMoving) { // Pressing a wheel the first time
				((UIElement)e.Source).CaptureMouse();
				isWaveformMoving = true;
				waveformMoveStartPosition = e.GetPosition(sender as IInputElement);
				Mouse.OverrideCursor = Cursors.SizeWE;
				CurrentTimeSlider_MouseLeftButtonDown(null, null);
			} else if (e.ChangedButton == MouseButton.Left && !isDrawingRangeZone) {
				((UIElement)e.Source).CaptureMouse();
				isDrawingRangeZone = true;
				waveformMoveStartPosition = e.GetPosition(sender as IInputElement);
				isDeferredMovingStarted = true; // the default value is true until the opposite value is set
				Mouse.OverrideCursor = Cursors.IBeam;
				Pause();
			}
		}
	}

	private void WaveformOutCanvas_MouseUp(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton is MouseButton.Middle or MouseButton.Left && e.ButtonState == MouseButtonState.Released) {
			((UIElement)e.Source).ReleaseMouseCapture();
			CancelWaveformMoving();
			if (e.ChangedButton == MouseButton.Left) SubmitCreateRangeZone();
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
		ActiveRangeZone.IsHitTestVisible = true;
		ActiveRangeZone.Resize += RangeZone_Resize;
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
			if (offset < 0) // TODO: 鼠标快速移动时另一端会错位。
				Canvas.SetLeft(rangeZone, e.StartPosition - left + offset);
		} else {
			double offset = e.CurrentPosition - e.EndPosition;
			rangeZone.Width = Math.Abs(offset);
			if (offset < 0)
				Canvas.SetLeft(rangeZone, e.EndPosition - left + offset);
		}
		UpdateRangeZoneSeconds(rangeZone);
	}

	private void RefreshRangeZonesCanvas() {
		RangeZonesCanvas.Children.RemoveRange(0, RangeZonesCanvas.Children.Count);
		double widthPixel = RangeZonesCanvas.ActualWidth;
		foreach (RangeZone rangeZone in rangeZones) {
			RangeZonesCanvas.Children.Add(rangeZone);
			rangeZone.Width = rangeZone.LengthSeconds / (Duration * 2) * widthPixel;
			Canvas.SetLeft(rangeZone, (rangeZone.StartSeconds - Player.Position.TotalSeconds + Duration) / (Duration * 2) * widthPixel);
		}
		// TODO: 优化，仅显示画面可见的范围区域，同时可根据情况显示隐藏，不必把所有控件删除后重新添加。
	}

	private List<RangeZone> rangeZones = new();

	private RangeZone? ActiveRangeZone { get; set; }

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
						Width = offset.X,
						Height = RangeZonesCanvas.ActualHeight,
						IsHitTestVisible = false,
					};
					RangeZonesCanvas.Children.Add(ActiveRangeZone);
					Canvas.SetTop(ActiveRangeZone, 0);
					Canvas.SetLeft(ActiveRangeZone, waveformMoveStartPosition.Value.X);
				} else if (ActiveRangeZone != null) {
					ActiveRangeZone.Width = Math.Abs(offset.X);
					Canvas.SetLeft(ActiveRangeZone, waveformMoveStartPosition.Value.X + (offset.X < 0 ? offset.X : 0));
				}
				// TODO: 当区域宽度太窄时则删除。
			}
		}
	}
}
