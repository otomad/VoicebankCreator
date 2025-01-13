#ifndef BACKDROPWINDOWHELPER_H
#define BACKDROPWINDOWHELPER_H

#include "includes.h"

/**
 * @brief Flags for specifying the system-drawn backdrop material of a window, including behind the non-client area.
 * @see https://learn.microsoft.com/windows/win32/api/dwmapi/ne-dwmapi-dwm_systembackdrop_type DWM_SYSTEMBACKDROP_TYPE enumeration (dwmapi.h)
 */
enum class SystemBackdropType : unsigned int {
	/**
	 * The default. Let the Desktop Window Manager (DWM) automatically decide the system-drawn backdrop material for this window.
	 * This applies the backdrop material just behind the default Win32 title bar. This behavior attempts to preserve maximum backwards
	 * compatibility. For this reason, the DWM might also decide to draw no backdrop material at all based on internal heuristics.
	 */
	Auto,
	/**
	 * Don't draw any system backdrop.
	 */
	None,
	/**
	 * Draw the backdrop material effect corresponding to a long-lived window behind the entire window bounds.
	 *
	 * For Windows 11, this corresponds to Mica in its default variant. The material effect might change with future Windows releases.
	 */
	MainWindow,
	/**
	 * Draw the backdrop material effect corresponding to a transient window behind the entire window bounds.
	 *
	 * For Windows 11, this corresponds to Desktop Acrylic, also known as Background Acrylic, in its brightest variant.
	 * The material effect might change with future Windows releases.
	 */
	TransientWindow,
	/**
	 * Draw the backdrop material effect corresponding to a window with a tabbed title bar behind the entire window bounds.
	 *
	 * For Windows 11, this corresponds to Mica in its alternate variant (Mica Alt). The material might change with future releases of Windows.
	 */
	TabbedWindow,
};

/**
 * @brief The ColorScheme enum add "Auto" enumurator to the Qt::ColorScheme.
 */
enum class ColorScheme : const int {
	Unknown,
	Light,
	Dark,
	Auto,
};

class BackdropWindowHelper : QObject {
	Q_OBJECT

	public:
		explicit BackdropWindowHelper(QQmlApplicationEngine *engine);
		explicit BackdropWindowHelper(QQuickWindow *window);
		explicit BackdropWindowHelper(HWND hwnd);
		static bool supports();
		void init(SystemBackdropType backdrop = SystemBackdropType::MainWindow);
		void setBackdrop(SystemBackdropType backdrop);
		void setColorScheme(ColorScheme colorScheme);
		void setColorScheme(Qt::ColorScheme colorScheme);

	public slots:
		void onColorSchemeChanged(Qt::ColorScheme colorScheme);

	private:
		HWND hwnd;
		ColorScheme colorSchemeSelected = ColorScheme::Auto;
};

#endif // BACKDROPWINDOWHELPER_H
