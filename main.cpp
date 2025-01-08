#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include "mediaformatprovider.h"

using namespace VoicebankCreator;

int main(int argc, char *argv[]) {
	QGuiApplication app(argc, argv);

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

	engine.loadFromModule("VoicebankCreator", "Main");

	return app.exec();
}
