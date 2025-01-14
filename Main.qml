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

			AudioPreview {
				id: audioPreview
				Layout.fillWidth: true
				height: 200
				color: root.darkTheme ? "white" : "black";
			}
		}
	}
}
