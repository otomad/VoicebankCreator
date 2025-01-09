import QtQuick
import QtQuick.Controls
import QtQuick.Layouts

Popup {
	id: root
	focus: true
	width: 60
	height: 200
	x: -65536
	y: -65536
	closePolicy: Popup.CloseOnEscape

	property ToolButton targetItem
	property alias displayValue: label.text
	property alias from: slider.from
	property alias to: slider.to
	property alias value: slider.value
	property double defaultValue
	property alias stepSize: slider.stepSize
	property alias strikeoutValue: label.font.strikeout
	readonly property alias hovered: hoverHandler.hovered
	property list<PlayerSliderPopup> allSliderPopups

	signal moved()

	Item {
		readonly property double padding: 16
		id: wrapper
		anchors.fill: parent
		anchors.margins: -padding
		clip: true

		ColumnLayout {
			id: column
			spacing: 0
			anchors.top: parent.top
			anchors.bottom: parent.bottom
			anchors.horizontalCenter: parent.horizontalCenter
			anchors.margins: wrapper.padding

			Label {
				id: label
				Layout.alignment: Qt.AlignHCenter | Qt.AlignVCenter
			}

			Slider {
				id: slider
				orientation: Qt.Vertical
				Layout.alignment: Qt.AlignHCenter | Qt.AlignVCenter
				Layout.fillHeight: true
				Layout.bottomMargin: -10
				onMoved: root.moved()
				// TODO: Double click slider to reset the value.
				// Cannot implement it now. See: https://forum.qt.io/topic/88414/mousearea-and-slider
			}
		}

		HoverHandler {
			id: hoverHandler
			onHoveredChanged: {
				if (hovered) root.openLite();
				else root.close_();
			}
		}
	}

	function updatePosition() {
		if (!targetItem || !parent) return;
		const { x: left, y: bottom } = targetItem.mapToItem(parent, 0, 0);
		const right = left + targetItem.width;
		y = bottom - height;
		x = (left + right) / 2 - width / 2;
		if (x + width > parent.width) x = parent.width - width;
		if (x < 0) x = 0;
	}

	/**
	 * Open the popup after updating the position.
	 */
	function open_() {
		closingTimer.stop();
		updatePosition();
		closeOtherPopups()
		open();
		updatePosition();
	}

	function openLite() {
		closingTimer.stop();
		closeOtherPopups()
		open();
	}

	/**
	 * Close the popup later.
	 */
	function close_() {
		closingTimer.start();
	}

	function resetDefaultValue() {
		if (defaultValue !== undefined) {
			value = defaultValue;
			moved();
		}
	}

	function closeOtherPopups() {
		PlayerSliderPopupSingletonService.closeExcept(root);
	}

	Timer {
		id: closingTimer
		interval: 500
		onTriggered: {
			if (root.hovered || root.targetItem?.hovered) return;
			root.close();
		}
	}

	Component.onCompleted: PlayerSliderPopupSingletonService.mount(root)

	Component.onDestruction: PlayerSliderPopupSingletonService.unmount(root)
}
