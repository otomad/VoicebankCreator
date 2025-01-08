import QtQuick
import QtQuick.Controls
import QtMultimedia

Item {
	id: root

	property string source

	MediaPlayer {
		id: mediaPlayer
		// playbackRate: playbackControl.playbackRate
		videoOutput: videoOutput
		audioOutput: AudioOutput {
			id: audio
			// volume: playbackControl.volume
		}
		source: root.source

		// onErrorOccurred: {
		// 	errorPopup.errorMsg = mediaPlayer.errorString
		// 	errorPopup.open()
		// }
	}

	VideoOutput {
		id: videoOutput
		visible: mediaPlayer.hasVideo
		anchors.left: parent.left
		anchors.right: parent.right
		anchors.top: parent.top
		anchors.bottom: playbackControl.top

		TapHandler {
			onTapped: root.togglePlaying()
		}
	}

	Item { // TODO: ColumnLayout
		id: playbackControl
		anchors.left: parent.left
		anchors.right: parent.right
		anchors.bottom: parent.bottom
		anchors.margins: 4
		height: childrenRect.height

		PlayerPlaybackButton {
			id: playBtn
			// icon.source: mediaPlayer.playing ? Images.iconSource("pause") : Images.iconSource("play")
			iconName: mediaPlayer.playing ? "pause" : "play"
			anchors.left: parent.left
			enabled: mediaPlayer.seekable
			onClicked: root.togglePlaying()
		}

		Label {
			id: currentTime
			text: root.getTime(mediaPlayer.position)
			anchors.left: playBtn.right
			anchors.margins: 10
			anchors.verticalCenter: parent.verticalCenter
		}

		Slider {
			id: seeker
			anchors.left: currentTime.right
			anchors.right: durationTime.left
			anchors.verticalCenter: parent.verticalCenter
			enabled: mediaPlayer.seekable
			to: 1.0
			value: mediaPlayer.position / mediaPlayer.duration
			onMoved: mediaPlayer.setPosition(value * mediaPlayer.duration)
		}

		Label {
			id: durationTime
			text: root.getTime(mediaPlayer.duration)
			anchors.right: parent.right
			anchors.margins: currentTime.anchors.margins
			anchors.verticalCenter: parent.verticalCenter
		}
	}

	function play() {
		mediaPlayer.play();
	}

	function pause() {
		mediaPlayer.pause();
	}

	function stop() {
		mediaPlayer.stop();
	}

	function togglePlaying() {
		if (mediaPlayer.playing) mediaPlayer.pause();
		else mediaPlayer.play();
	}

	function getTime(time: int): string {
		const h = Math.floor(time / 3600000).toString();
		const m = Math.floor(time / 60000).toString();
		const s = Math.floor(time / 1000 - m * 60).toString();
		return `${h.padStart(2, "0")}:${m.padStart(2, "0")}:${s.padStart(2, "0")}`;
	}
}
