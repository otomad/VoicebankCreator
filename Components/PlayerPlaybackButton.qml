import QtQuick
import QtQuick.Controls
import ".."

ToolButton {
	property string iconName
	property bool smaller: false

	signal mouseEntered
	signal mouseExited

	id: root
	icon.source: Images.iconSource(iconName)
	height: 48
	width: smaller ? height * 0.85 : height
	padding: 0
	icon.height: height
	icon.width: height
	font.family: Constants.fontFamily
	hoverEnabled: true

	onHoveredChanged: {
		if (!enabled) return;
		if (hovered) root.mouseEntered();
		else root.mouseExited();
	}
}
