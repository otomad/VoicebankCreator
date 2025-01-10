#include "includes.h"
#include <QQuickWindow>
#include <QStyleHints>
#ifdef Q_OS_WIN
#include <windows.h>
#include <dwmapi.h>
#endif
#include "mediaformatprovider.h"

using namespace VoicebankCreator;

static void applyBackdrop(QQmlApplicationEngine* engine);
static bool supportsBackdrop();

int main(int argc, char *argv[]) {
	QGuiApplication app(argc, argv);
	QString appName = app.applicationName();
	QString locale = QLocale::system().name();

	QTranslator translator;
	if (translator.load(appName + "_" + locale + ".qm"))
		QCoreApplication::installTranslator(&translator);

	QQmlApplicationEngine engine;
	QQuickWindow::setGraphicsApi(QSGRendererInterface::OpenGL);
	QObject::connect(
		&engine,
		&QQmlApplicationEngine::objectCreationFailed,
		&app,
		[]() { QCoreApplication::exit(-1); },
		Qt::QueuedConnection
	);

	bool paneVisible = !supportsBackdrop();

	const QStringList nameFilters = getNameFilters();
	QVariantMap initialProperties{
		{ "nameFilters", nameFilters },
		{ "paneVisible", paneVisible },
	};
	engine.setInitialProperties(initialProperties);

	engine.loadFromModule(appName, "Main");

	applyBackdrop(&engine);

	return app.exec();
}

static void applyBackdrop(QQmlApplicationEngine* engine) {
#ifdef Q_OS_WIN
	QQuickWindow::setDefaultAlphaBuffer(true);
	QObject *root = engine->rootObjects()[0];
	QQuickWindow *window = qobject_cast<QQuickWindow *>(root);
	if (!window) return;
	HWND hwnd = (HWND)window->winId();

	// qDebug() << window->flags();
	// window->setFlags(Qt::Window);

	// window->setFlags((Qt::WindowFlags)Qt::WA_TranslucentBackground | Qt::FramelessWindowHint);

	// long exStyle;// GetWindowLongPtr(hwnd, GWL_EXSTYLE);
	// // exStyle &= ~WS_EX_TOOLWINDOW;
	// // exStyle |= WS_EX_APPWINDOW;
	// exStyle = WS_EX_WINDOWEDGE; // WS_EX_WINDOWEDGE;
	// SetWindowLongPtr(hwnd, GWL_EXSTYLE, exStyle);

	// qDebug() << QString::number(exStyle, 16);
	// exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
	// qDebug() << QString::number(exStyle, 16);

	qDebug() << window->format();

	Qt::ColorScheme colorScheme = QGuiApplication::styleHints()->colorScheme();
	unsigned int flag = colorScheme == Qt::ColorScheme::Dark;
	const unsigned long DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
	DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, &flag, sizeof(flag));

	const unsigned long DWMWA_SYSTEMBACKDROP_TYPE = 38;
	unsigned int systemBackdropType = 2;
	DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, &systemBackdropType, sizeof(systemBackdropType));
#endif
}

static bool supportsBackdrop() {
	if (QSysInfo::kernelType() != "winnt")
		return true; // I don't know if other OS supports it.
	QString ntVersionString = QSysInfo::kernelVersion();
	QVersionNumber ntVersion = QVersionNumber::fromString(ntVersionString);
	bool supports = ntVersion >= QVersionNumber(10, 0, 22621);
	// Set system backdrop only available starting with Windows 11 Build 22621.
	return supports;
}
