import QtQuick
import QtQuick.Controls
import QtQuick.Dialogs

Item {
	id: root
	// height: toolBar.childrenRect.height

	signal fileOpened(path: url)

	property alias nameFilters: fileDialog.nameFilters

	ToolBar {
		id: toolBar
		anchors.fill: parent
		anchors.topMargin: -16

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
