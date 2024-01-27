namespace VoicebankCreator.Helpers;

public struct Dpi {
	public double X { get; set; }

	public double Y { get; set; }

	public Dpi(double x, double y) {
		X = x;
		Y = y;
	}

	public static Dpi GetDpiFromVisual(Visual visual) {
		PresentationSource? source = PresentationSource.FromVisual(visual);

		double dpiX = 1, dpiY = 1;

		if (source?.CompositionTarget != null) {
			dpiX = source.CompositionTarget.TransformToDevice.M11;
			dpiY = source.CompositionTarget.TransformToDevice.M22;
		}

		return new Dpi(dpiX, dpiY);
	}

	public static double Get => Dpi.GetDpiFromVisual(Application.Current.MainWindow).X;
}