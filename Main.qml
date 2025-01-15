import QtQuick
import QtQuick.Window
import QtQuick.Layouts
import QtQuick.Controls.FluentWinUI3
import "Components"
import VoicebankCreator.AudioPreview

Window {
	id: root
	width: 1280
	height: 720
	visible: true
	title: Constants.appDisplayName
	color: "transparent"

	required property list<string> nameFilters
	property bool paneVisible: true
	property bool darkTheme: Application.styleHints.colorScheme === Qt.ColorScheme.Dark

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

			// Label {
			// 	text: "测试Testㄘㄜㄕテストてすと테스트اختبارการทดสอบ"
			// }

			PlayerToolBar {
				id: toolBar
				Layout.fillWidth: true
				Layout.alignment: Qt.AlignLeft | Qt.AlignTop
				Layout.preferredHeight: 48
				nameFilters: root.nameFilters
				onFileOpened: path => {
					playerPreview.source = path;
					playerPreview.play();
					audioPreview.url = path;
				}
			}

			PlayerPreview {
				id: playerPreview
				Layout.fillWidth: true
				Layout.fillHeight: true
			}

			Item {
				id: _item
				Layout.fillWidth: true
				Layout.preferredHeight: 200

				AudioPreview { // AudioPreview PaintedItem
					id: audioPreview
					anchors.fill: parent
					color: root.darkTheme ? "white" : "black";
				}

				ProgressBar {
					id: progressBar
					value: audioPreview.loadingProgress
					width: parent.width * 0.75
					anchors.centerIn: parent
					visible: audioPreview.loadingProgress >= 0 && audioPreview.loadingProgress < 1
				}
			}
		}
	}
}
