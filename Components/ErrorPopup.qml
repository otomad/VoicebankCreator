import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

Popup {
	id: errorPopup
	anchors.centerIn: Overlay.overlay
	// padding: 30
	modal: true
	focus: true
	closePolicy: Popup.CloseOnEscape | Popup.CloseOnPressOutside

	property alias errorMsg: label.text

	function showError(msg: string) {
		errorMsg = msg;
		open();
	}

	ColumnLayout {
		anchors.fill: parent
		spacing: 15

		RowLayout {
			Layout.fillWidth: true
			spacing: 15

			// Image {
			// 	source: ControlImages.iconSource("Error", false)
			// 	anchors.horizontalCenter: parent.horizontalCenter
			// }

			Label {
				text: qsTr("Error")
				font.pixelSize: 20
				font.weight: Font.DemiBold
			}
		}

		Label {
			id: label
		}

		Button {
			id: okBtn
			text: qsTr("OK")
			Layout.fillWidth: true
			focus: true
			onClicked: errorPopup.close()
		}
	}
}
