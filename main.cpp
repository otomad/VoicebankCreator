#include "includes.h"
#include "mediaformatprovider.h"
#include "backdropwindowhelper.h"

using namespace VoicebankCreator;

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
