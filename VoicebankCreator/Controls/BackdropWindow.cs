using static VoicebankCreator.Interop.PInvoke.ParameterTypes;
using static VoicebankCreator.Interop.PInvoke.Methods;

namespace VoicebankCreator.Controls;

/// <summary>
/// BackdropWindow.xaml 的交互逻辑
/// </summary>
public partial class BackdropWindow : Window {
	private IntPtr Handle => new WindowInteropHelper(this).Handle;

	public BackdropWindow() {
		Background = Brushes.Transparent;
		Loaded += Window_Loaded;
	}

	private void Window_Loaded(object sender, RoutedEventArgs e) {
		RefreshFrame();
		RefreshDarkMode();
		SystemBackdropType = DWM_SYSTEMBACKDROP_TYPE.DWMSBT_TRANSIENTWINDOW;
	}

	private void RefreshFrame() {
		IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
		HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
		mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

		MARGINS margins = new() {
			cxLeftWidth = -1,
			cxRightWidth = -1,
			cyTopHeight = -1,
			cyBottomHeight = -1
		};

		ExtendFrame(mainWindowSrc.Handle, margins);
	}

	//[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#132")] // Windows 1903 之后用不了。
	private static bool ShouldAppsUseDarkMode() {
		using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
		object? value = key?.GetValue("AppsUseLightTheme");
		return value is int i && i == 0;
	}

	protected override void OnSourceInitialized(EventArgs e) {
		base.OnSourceInitialized(e);

		// Detect when the theme changed
		HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
		source.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => {
			const int WM_SETTINGCHANGE = 0x001A;
			if (msg == WM_SETTINGCHANGE)
				if (wParam == IntPtr.Zero && Marshal.PtrToStringUni(lParam) == "ImmersiveColorSet")
					RefreshDarkMode();
			return IntPtr.Zero;
		});
	}

	private void RefreshDarkMode() {
		bool isDarkTheme = ShouldAppsUseDarkMode();
		int flag = isDarkTheme ? 1 : 0;
		SetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, flag);
		Application.Current.Resources["ForegroundColor"] = isDarkTheme ? Colors.White : Colors.Black;
		Application.Current.Resources["ForegroundBrush"] = isDarkTheme ? Brushes.White : Brushes.Black;
	}

	public DWM_SYSTEMBACKDROP_TYPE SystemBackdropType {
		set => SetSystemBackdropType(value);
	}

	private void SetSystemBackdropType(DWM_SYSTEMBACKDROP_TYPE systemBackdropType) {
		SetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, (int)systemBackdropType);
	}
}
