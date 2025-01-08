import QtQuick
import QtQuick.Controls.FluentWinUI3
import "Components"

Window {
	id: root
	width: 1280
	height: 720
	visible: true
	title: Constants.appDisplayName

	required property list<string> nameFilters

	Pane {
		anchors.fill: parent
		padding: 0

		PlayerToolBar {
			id: toolBar

			width: parent.width
			height: 50

			nameFilters: root.nameFilters
			onFileOpened: path => {
				playerPreview.source = path;
				playerPreview.play();
			}
		}

		PlayerPreview {
			id: playerPreview

			anchors.top: toolBar.bottom
			anchors.bottom: parent.bottom
			anchors.left: parent.left
			anchors.right: parent.right
		}
	}

}
