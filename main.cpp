#include "includes.h"
#include "mediaformatprovider.h"
#include "backdropwindowhelper.h"
#include <QLibraryInfo>
#include <QFontDatabase>

using namespace VoicebankCreator;

int main(int argc, char *argv[]) {
	QGuiApplication app(argc, argv);
	QString appName = app.applicationName();
	QString locale = QLocale::system().name();

	QTranslator translator;
	if (translator.load(appName + "_" + locale + ".qm"))
		QCoreApplication::installTranslator(&translator);

#if QT_VERSION >= QT_VERSION_CHECK(6, 8, 0)
	QFontDatabase::addApplicationFallbackFontFamily(QChar::Script_Han, "Microsoft YaHei");
	QFontDatabase::addApplicationFallbackFontFamily(QChar::Script_Bopomofo, "Microsoft YaHei");
	QFontDatabase::addApplicationFallbackFontFamily(QChar::Script_Hiragana, "Yu Gothic UI");
	QFontDatabase::addApplicationFallbackFontFamily(QChar::Script_Katakana, "Yu Gothic UI");
	QFontDatabase::addApplicationFallbackFontFamily(QChar::Script_Hangul, "Malgun Gothic");
#endif

	QQmlApplicationEngine engine;
	QQuickWindow::setGraphicsApi(QSGRendererInterface::OpenGL);
	QObject::connect(
		&engine,
		&QQmlApplicationEngine::objectCreationFailed,
		&app,
		[]() { QCoreApplication::exit(-1); },
		Qt::QueuedConnection
	);

	bool paneVisible = !BackdropWindowHelper::supports();

	const QStringList nameFilters = getNameFilters();
	QVariantMap initialProperties{
		{ "nameFilters", nameFilters },
		{ "paneVisible", paneVisible },
	};
	engine.setInitialProperties(initialProperties);

	engine.loadFromModule(appName, "Main");

	BackdropWindowHelper backdrop(&engine);
	backdrop.init();

	return app.exec();
}
