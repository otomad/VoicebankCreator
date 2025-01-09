import QtQuick
import QtQuick.Controls
import ".."

ToolButton {
	property string iconName
	property bool smaller: false
	property string tooltip
	property PlayerSliderPopup popupItem

	signal mouseEntered
	signal mouseExited

	id: root
	icon.source: Images.iconSource(iconName)
	height: 48
	spacing: 0
	width: smaller ? height * 0.85 : height
	icon.height: height
	icon.width: height
	font.family: Constants.fontFamily
	hoverEnabled: true
	ToolTip.visible: enabled && hovered && !popupItem?.hovered
	ToolTip.text: tooltip
	ToolTip.toolTip.z: 2

	onHoveredChanged: {
		if (!enabled) return;
		if (hovered) root.mouseEntered();
		else root.mouseExited();
		if (popupItem)
			if (hovered) popupItem.open_();
			else popupItem.close_();
	}
}
