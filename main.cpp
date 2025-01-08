#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include "mediaformatprovider.h"

using namespace VoicebankCreator;

int main(int argc, char *argv[]) {
	QGuiApplication app(argc, argv);
	QString appName = app.applicationName();
	QString locale = QLocale::system().name();

	QTranslator translator;
	if (translator.load(appName + "_" + locale + ".qm"))
		QCoreApplication::installTranslator(&translator);

	QQmlApplicationEngine engine;
	QObject::connect(
		&engine,
		&QQmlApplicationEngine::objectCreationFailed,
		&app,
		[]() { QCoreApplication::exit(-1); },
		Qt::QueuedConnection
	);

	const QStringList nameFilters = getNameFilters();
	QVariantMap initialProperties{
		{ "nameFilters", nameFilters },
	};
	engine.setInitialProperties(initialProperties);

	engine.loadFromModule(appName, "Main");

	return app.exec();
}
