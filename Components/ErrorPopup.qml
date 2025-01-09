import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Controls.impl

Popup {
	id: errorPopup
	anchors.centerIn: Overlay.overlay
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
		spacing: 12

		RowLayout {
			Layout.fillWidth: true
			spacing: 12

			ColorImage {
				readonly property int size: 24

				source: Images.iconSource("error")
				sourceSize.height: size
				sourceSize.width: size
				color: title.color
			}

			Label {
				id: title
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
			Keys.onReturnPressed: click() // Enter key
			Keys.onEnterPressed: click() // Numpad enter key
		}
	}
}
