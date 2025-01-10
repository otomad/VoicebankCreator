#include "backdropwindowhelper.h"
#include <QStyleHints>
#ifdef Q_OS_WIN
#include <windows.h>
#include <dwmapi.h>
#endif

BackdropWindowHelper::BackdropWindowHelper(QQmlApplicationEngine *engine) {
	QObject *root = engine->rootObjects()[0];
	QQuickWindow *window = qobject_cast<QQuickWindow *>(root);
	if (!window) return;
	hwnd = (HWND)window->winId();
}

BackdropWindowHelper::BackdropWindowHelper(QQuickWindow *window) {
	if (!window) return;
	hwnd = (HWND)window->winId();
}

BackdropWindowHelper::BackdropWindowHelper(HWND hwnd) {
	this->hwnd = hwnd;
}

bool BackdropWindowHelper::supports() {
	if (QSysInfo::kernelType() != "winnt")
		return true; // I don't know if other OS supports it.
	QString ntVersionString = QSysInfo::kernelVersion();
	QVersionNumber ntVersion = QVersionNumber::fromString(ntVersionString);
	bool supports = ntVersion >= QVersionNumber(10, 0, 22621);
	// Set system backdrop only available starting with Windows 11 Build 22621.
	return supports;
}

static const unsigned long DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
static const unsigned long DWMWA_SYSTEMBACKDROP_TYPE = 38;

#define SUPPORTS_COLOR_SCHEME_CHANGED QT_VERSION >= QT_VERSION_CHECK(6, 5, 0)

void BackdropWindowHelper::init(SystemBackdropType backdrop) {
#ifdef Q_OS_WIN
	setColorScheme(ColorScheme::Auto);
	setBackdrop(backdrop);

	#if SUPPORTS_COLOR_SCHEME_CHANGED
	connect(QGuiApplication::styleHints(), SIGNAL(colorSchemeChanged(Qt::ColorScheme)), this, SLOT(&onColorSchemeChanged));
	#endif
#endif
}

void BackdropWindowHelper::setBackdrop(SystemBackdropType backdrop) {
#ifdef Q_OS_WIN
	DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, &backdrop, sizeof(backdrop));
#endif
}

void BackdropWindowHelper::setColorScheme(ColorScheme colorScheme) {
	colorSchemeSelected = colorScheme;
#ifdef Q_OS_WIN
	Qt::ColorScheme actual = colorScheme == ColorScheme::Auto ?
		QGuiApplication::styleHints()->colorScheme() :
		Qt::ColorScheme(colorScheme);
	setColorScheme(actual);
#endif
}

void BackdropWindowHelper::setColorScheme(Qt::ColorScheme colorScheme) {
#ifdef Q_OS_WIN
	unsigned int flag = colorScheme == Qt::ColorScheme::Dark;
	DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, &flag, sizeof(flag));
#endif
}

void BackdropWindowHelper::onColorSchemeChanged(Qt::ColorScheme colorScheme) {
#ifdef Q_OS_WIN
	if (colorSchemeSelected == ColorScheme::Auto)
		setColorScheme(colorScheme);
#endif
}
