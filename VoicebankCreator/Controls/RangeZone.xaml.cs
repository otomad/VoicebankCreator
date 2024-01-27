namespace VoicebankCreator.Controls;

/// <summary>
/// RangeZone.xaml 的交互逻辑
/// </summary>
public partial class RangeZone : UserControl {
	public RangeZone() {
		InitializeComponent();
	}

	public double StartSeconds { get; set; }
	public double LengthSeconds { get; set; }

	public enum Side {
		Start = -1,
		End = 1,
	}

	private Side? isResizing = null;
	private Point? startPosition = null;
	private Point? endPosition = null;

	private void Side_MouseDown(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed && isResizing == null) {
			isResizing = sender == Start ? Side.Start : Side.End;
			startPosition = PointToScreen(new Point(0, 0));
			endPosition = PointToScreen(new Point(ActualWidth, 0));
			((UIElement)e.Source).CaptureMouse();
		}
	}

	private void Side_MouseUp(object sender, MouseButtonEventArgs e) {
		if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Released) {
			((UIElement)e.Source).ReleaseMouseCapture();
			isResizing = null;
			startPosition = null;
		}
	}

	private void Side_MouseMove(object sender, MouseEventArgs e) {
		if (isResizing != null && startPosition.HasValue && endPosition.HasValue) {
			Window window = Application.Current.MainWindow;
			Point currentPosition = window.PointToScreen(Mouse.GetPosition(window));
			Resize(this, new(e, isResizing.Value, startPosition.Value.X, endPosition.Value.X, currentPosition.X));
		}
	}

	public Func<RangeZone, Side, double?>? GetAnotherSide { get; set; }

	public event EventHandler<RangeZoneResizeEventArgs> Resize = null!;

	public class RangeZoneResizeEventArgs : MouseEventArgs {
		public Side Side { get; }
		public double StartPosition { get; }
		public double EndPosition { get; }
		public double CurrentPosition { get; }
		public RangeZoneResizeEventArgs(MouseEventArgs e, Side side, double startPosition, double endPosition, double currentPosition) :
			base(e.MouseDevice, e.Timestamp, e.StylusDevice) {
			double dpi = Dpi.Get;
			Side = side;
			StartPosition = startPosition / dpi;
			EndPosition = endPosition / dpi;
			CurrentPosition = currentPosition / dpi;
		}
	}
}
