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
		property string displayPlaybackRate: root.getDisplayPlaybackRate(mediaPlayer.playbackRate)
		property string displayVolume: root.getDisplayVolume(audio.volume)
	}

	ColumnLayout {
		anchors.fill: parent
		spacing: 0

		MediaPlayer {
			id: mediaPlayer
			videoOutput: videoOutput
			audioOutput: AudioOutput {
				id: audio
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
			Layout.fillWidth: true
			spacing: 0

			readonly property double playbackButtonMargin: 4;

			Row {
				Layout.rightMargin: playbackControl.playbackButtonMargin

				PlayerPlaybackButton {
					id: playBtn
					tooltip: mediaPlayer.playing ? qsTr("Pause") : qsTr("Play")
					iconName: mediaPlayer.playing ? "pause" : "play"
					enabled: internal.isVideoLoaded
					onClicked: root.togglePlaying()
				}
			}

			Label {
				id: currentTime
				text: root.getDisplayTime(mediaPlayer.position)
				enabled: internal.isVideoLoaded
				font.features: { "tnum": 1 }
			}

			Slider {
				id: seeker
				enabled: internal.isVideoLoaded
				to: 1.0
				value: mediaPlayer.position / mediaPlayer.duration
				Layout.fillWidth: true
				onMoved: mediaPlayer.position = value * mediaPlayer.duration
			}

			Label {
				id: durationTime
				text: root.getDisplayTime(mediaPlayer.duration)
				enabled: internal.isVideoLoaded
				font.features: currentTime.font.features
			}

			Row {
				Layout.leftMargin: playbackControl.playbackButtonMargin

				PlayerPlaybackButton {
					id: rateBtn
					smaller: true
					tooltip: qsTr("Playback Rate: ") + internal.displayPlaybackRate
					iconName:
						mediaPlayer.playbackRate === 1 ? "speed_medium" :
						mediaPlayer.playbackRate < 1 ? "speed_low" : "speed_high"
					enabled: internal.isVideoLoaded
					popupItem: ratePopup
					onClicked: mediaPlayer.playbackRate = 1
				}

				PlayerPlaybackButton {
					id: volumeBtn
					smaller: true
					tooltip: qsTr("Volume: ") + (audio.muted ? qsTr("Muted") : internal.displayVolume)
					iconName:
						audio.muted ? "mute" :
						audio.volume === 0 ? "volume0" :
						audio.volume <= 1 / 3 ? "volume1" :
						audio.volume <= 2 / 3 ? "volume2" : "volume3"
					enabled: internal.isVideoLoaded
					popupItem: volumePopup
					onClicked: audio.muted = !audio.muted
				}
			}
		}
	}

	ErrorPopup {
		id: errorPopup
	}

	PlayerSliderPopup {
		id: volumePopup
		targetItem: volumeBtn
		displayValue: internal.displayVolume
		value: audio.volume
		strikeoutValue: audio.muted
		defaultValue: 1
		onMoved: {
			audio.muted = false;
			audio.volume = value;
		}
	}

	PlayerSliderPopup {
		id: ratePopup
		targetItem: rateBtn
		displayValue: internal.displayPlaybackRate
		from: -2
		to: 2
		value: Math.log2(mediaPlayer.playbackRate)
		defaultValue: 0
		onMoved: mediaPlayer.playbackRate = 2 ** value
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

	function getDisplayTime(time: int): string {
		const h = Math.floor(time / 3600000).toString();
		const m = Math.floor(time / 60000).toString();
		const s = Math.floor(time / 1000 - m * 60).toString();
		return `${h.padStart(2, "0")}:${m.padStart(2, "0")}:${s.padStart(2, "0")}`;
	}

	function getDisplayPlaybackRate(value: double): string {
		return value.toFixed(2).replace(/\.?0+$/, "") + "×";
	}

	function getDisplayVolume(value: double): string {
		return (value * 100).toFixed(0) + "%";
	}
}
