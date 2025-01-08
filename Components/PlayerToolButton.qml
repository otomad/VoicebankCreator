import QtQuick
import QtQuick.Controls
import ".."

ToolButton {
	property string iconName

	icon.source: Images.iconSource(iconName)
	anchors.verticalCenter: parent.verticalCenter
	font.family: Constants.fontFamily
}
