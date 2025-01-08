import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
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

	RowLayout {
		id: playbackControl
		anchors.left: parent.left
		anchors.right: parent.right
		anchors.bottom: parent.bottom
		anchors.margins: 4
		anchors.rightMargin: 12
		spacing: 0
		// height: childrenRect.height

		Row {
			Layout.rightMargin: 4

			PlayerPlaybackButton {
				id: playBtn
				iconName: mediaPlayer.playing ? "pause" : "play"
				enabled: mediaPlayer.seekable
				onClicked: root.togglePlaying()
			}

			PlayerPlaybackButton {
				id: volumeBtn
				smaller: true
				iconName:
					mediaPlayer.audioOutput.muted ? "mute" :
					mediaPlayer.audioOutput.volume === 0 ? "volume0" :
					mediaPlayer.audioOutput.volume <= 1 / 3 ? "volume1" :
					mediaPlayer.audioOutput.volume <= 2 / 3 ? "volume2" : "volume3"
				enabled: mediaPlayer.seekable
			}

			PlayerPlaybackButton {
				id: rateBtn
				smaller: true
				iconName:
					mediaPlayer.playbackRate === 1 ? "speed_medium" :
					mediaPlayer.playbackRate < 1 ? "speed_low" : "speed_high"
				enabled: mediaPlayer.seekable
			}
		}

		Label {
			id: currentTime
			text: root.getTime(mediaPlayer.position)
		}

		Slider {
			id: seeker
			enabled: mediaPlayer.seekable
			to: 1.0
			value: mediaPlayer.position / mediaPlayer.duration
			Layout.fillWidth: true
			onMoved: mediaPlayer.setPosition(value * mediaPlayer.duration)
		}

		Label {
			id: durationTime
			text: root.getTime(mediaPlayer.duration)
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
