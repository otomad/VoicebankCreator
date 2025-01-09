import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtMultimedia

Item {
	id: root

	property alias source: mediaPlayer.source

	QtObject {
		id: internal
		property bool isVideoLoaded: mediaPlayer.seekable
	}

	ColumnLayout {
		anchors.fill: parent
		spacing: 0

		MediaPlayer {
			id: mediaPlayer
			// playbackRate: playbackControl.playbackRate
			videoOutput: videoOutput
			audioOutput: AudioOutput {
				id: audio
				// volume: playbackControl.volume
			}

			onErrorOccurred: {
				errorPopup.showError(errorString);
				source = "";
			}
		}

		VideoOutput {
			id: videoOutput
			// visible: mediaPlayer.hasVideo
			Layout.fillWidth: true
			Layout.fillHeight: true

			TapHandler {
				onTapped: root.togglePlaying()
			}
		}

		RowLayout {
			id: playbackControl
			Layout.margins: 4
			Layout.rightMargin: 12
			Layout.fillWidth: true
			spacing: 0
			// height: childrenRect.height

			Row {
				Layout.rightMargin: 4

				PlayerPlaybackButton {
					id: playBtn
					iconName: mediaPlayer.playing ? "pause" : "play"
					enabled: internal.isVideoLoaded
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
					enabled: internal.isVideoLoaded
				}

				PlayerPlaybackButton {
					id: rateBtn
					smaller: true
					iconName:
						mediaPlayer.playbackRate === 1 ? "speed_medium" :
						mediaPlayer.playbackRate < 1 ? "speed_low" : "speed_high"
					enabled: internal.isVideoLoaded
				}
			}

			Label {
				id: currentTime
				text: root.getTime(mediaPlayer.position)
				enabled: internal.isVideoLoaded
			}

			Slider {
				id: seeker
				enabled: internal.isVideoLoaded
				to: 1.0
				value: mediaPlayer.position / mediaPlayer.duration
				Layout.fillWidth: true
				onMoved: mediaPlayer.setPosition(value * mediaPlayer.duration)
			}

			Label {
				id: durationTime
				text: root.getTime(mediaPlayer.duration)
				enabled: internal.isVideoLoaded
			}
		}
	}

	ErrorPopup {
		id: errorPopup
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
