import QtQuick
import QtQuick.Controls
import QtQuick.Dialogs
import ".."

Item {
	id: root

	signal fileOpened(path: url)

	required property list<string> nameFilters

	ToolBar {
		anchors.fill: parent

		PlayerToolButton {
			text: qsTr("Open")
			iconName: "open"
			onClicked: fileDialog.open()
		}
	}

	FileDialog {
		id: fileDialog
		title: qsTr("Select a file")
		nameFilters: root.nameFilters
		onAccepted: root.fileOpened(fileDialog.selectedFile)
	}
}
