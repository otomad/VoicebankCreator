import QtQuick
import QtQuick.Controls
import ".."

ToolButton {
	property string iconName

	icon.source: Images.iconSource(iconName)
	height: 48
	width: height
	padding: 0
	anchors.verticalCenter: parent.verticalCenter
	icon.height: height
	icon.width: height
	font.family: Constants.fontFamily
}
