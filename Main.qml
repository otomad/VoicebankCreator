import QtQuick
import QtQuick.Window
import QtQuick.Layouts
import QtQuick.Controls.FluentWinUI3
import "Components"

Window {
	id: root
	width: 1280
	height: 720
	visible: true
	title: Constants.appDisplayName
	color: "transparent"
	// flags: Qt.FramelessWindowHint

	required property list<string> nameFilters
	property bool paneVisible: true

	Pane {
		anchors.fill: parent
		padding: 0
		visible: root.paneVisible
	}

	Item {
		anchors.fill: parent

		ColumnLayout {
			anchors.fill: parent
			spacing: 0

			PlayerToolBar {
				id: toolBar
				Layout.fillWidth: true
				Layout.alignment: Qt.AlignLeft | Qt.AlignTop
				Layout.preferredHeight: 58
				nameFilters: root.nameFilters
				onFileOpened: path => {
					playerPreview.source = path;
					playerPreview.play();
				}
			}

			PlayerPreview {
				id: playerPreview
				Layout.fillWidth: true
				Layout.fillHeight: true
			}
		}
	}
}
