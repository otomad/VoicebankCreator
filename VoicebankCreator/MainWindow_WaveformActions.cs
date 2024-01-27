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

	private void WaveformOutCanvas_MouseDown(object sender, MouseButtonEventArgs e) {
		if (isWaveformMoving == true) // Moving with a released wheel and pressing a button
			CancelWaveformMoving();
		if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed && CursorLine.Visibility == Visibility.Visible) {
			if (!isWaveformMoving) { // Pressing a wheel the first time
				((UIElement)e.Source).CaptureMouse();
				isWaveformMoving = true;
				waveformMoveStartPosition = e.GetPosition(sender as IInputElement);
				Mouse.OverrideCursor = Cursors.SizeWE;
				CurrentTimeSlider_MouseLeftButtonDown(null, null);
			}
		}
	}

	private void WaveformOutCanvas_MouseUp(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Released) {
			((UIElement)e.Source).ReleaseMouseCapture();
			CancelWaveformMoving();
		}
	}

	private void CancelWaveformMoving() {
		isWaveformMoving = false;
		waveformMoveStartPosition = null;
		Mouse.OverrideCursor = null;
		CurrentTimeSlider_MouseLeftButtonUp(null, null);
	}

	private void WaveformOutCanvas_MouseMove(object sender, MouseEventArgs e) {
		if (isWaveformMoving && waveformMoveStartPosition.HasValue && CursorLine.Visibility == Visibility.Visible) {
			Point currentPosition = e.GetPosition(WaveformImage);
			Vector offset = currentPosition - waveformMoveStartPosition.Value;
			CurrentTimeSlider.Value -= offset.X / WaveformOutCanvas.ActualWidth * Duration * 2;
			waveformMoveStartPosition = currentPosition;
		}
	}
}
