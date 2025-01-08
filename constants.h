#ifndef CONSTANTS_H
#define CONSTANTS_H

#include <QObject>
#include <QQmlEngine>

namespace VoicebankCreator { Q_NAMESPACE
	class Constants : public QObject {
		Q_OBJECT

		public:
			explicit Constants(QObject *parent = nullptr);

			const QString appDisplayName = "Voicebank Creator";

			QString fontFamily = tr("Segoe UI");
	};
}

#endif // CONSTANTS_H
