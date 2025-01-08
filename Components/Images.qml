pragma Singleton
import QtQml

QtObject {
	function iconSource(fileName: string): url {
		return Qt.resolvedUrl(`/qt/qml/${Qt.application.name}/Icons/${fileName}.svg`);
	}
}
