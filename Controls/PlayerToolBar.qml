import QtQuick
import QtQuick.Controls
import QtQuick.Controls.FluentWinUI3
import QtQuick.Dialogs

Item {
	id: root

	signal fileOpened(path: url)

	ToolBar {
		anchors.fill: parent

		ToolButton {
			text: qsTr("打开")
			onClicked: fileDialog.open()
		}
	}

	FileDialog {
		id: fileDialog
		title: qsTr("选择文件")
		nameFilters: []
		onAccepted: root.fileOpened(fileDialog.selectedFile)
	}
}
