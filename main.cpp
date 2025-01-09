#include <QGuiApplication>
#include <QQmlApplicationEngine>
#include "mediaformatprovider.h"
#include <QFontDatabase>

using namespace VoicebankCreator;

int main(int argc, char *argv[]) {
	QGuiApplication app(argc, argv);
	QString appName = app.applicationName();
	QString locale = QLocale::system().name();

	QTranslator translator;
	if (translator.load(appName + "_" + locale + ".qm"))
		QCoreApplication::installTranslator(&translator);

	// QFont font("Microsoft YaHei", 14);
	// app.setFont(font);
	// QFontDatabase::setApplicationFallbackFontFamilies(QChar::Script_Common, QStringList("Microsoft YaHei"));

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
