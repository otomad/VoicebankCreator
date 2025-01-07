import QtQuick
import QtQuick.Controls
import QtQuick.Controls.FluentWinUI3
import QtQuick.Dialogs
import QtMultimedia

Window {
	width: 640
	height: 480
	visible: true
	color: "#EAEAEA"
	title: qsTr("Voicebank Creator")

	Column {
		ToolBar {
			anchors.fill: parent

			ToolButton {
				text: qsTr("打开")
				onClicked: fileDialog.open()
			}
		}

		FileDialog {
			id: fileDialog
			title: qsTr("选择文件")
			onAccepted: {
				mediaPlayer.source = fileDialog.selectedFile;
				console.log(fileDialog.selectedFile);
				mediaPlayer.play();
			}
		}

		MediaPlayer {
			id: mediaPlayer

			playbackRate: playbackControl.playbackRate
			videoOutput: videoOutput
			audioOutput: AudioOutput {
				id: audio
				volume: playbackControl.volume
			}
			source: new URL("https://download.qt.io/learning/videos/media-player-example/Qt_LogoMergeEffect.mp4")

			function updateMetadata() {
				root.metadataInfo.clear()
				root.metadataInfo.read(mediaPlayer.metaData)
			}

			onMetaDataChanged: updateMetadata()
			onActiveTracksChanged: updateMetadata()
			onErrorOccurred: {
				errorPopup.errorMsg = mediaPlayer.errorString
				errorPopup.open()
			}
			onTracksChanged: {
				settingsInfo.tracksInfo.selectedAudioTrack = mediaPlayer.activeAudioTrack
				settingsInfo.tracksInfo.selectedVideoTrack = mediaPlayer.activeVideoTrack
				settingsInfo.tracksInfo.selectedSubtitleTrack = mediaPlayer.activeSubtitleTrack
				updateMetadata()
			}

			onMediaStatusChanged: {
				if ((MediaPlayer.EndOfMedia === mediaStatus && mediaPlayer.loops !== MediaPlayer.Infinite) &&
						((root.currentFile < playlistInfo.mediaCount - 1) || playlistInfo.isShuffled)) {
					if (!playlistInfo.isShuffled) {
						++root.currentFile
					}
					root.playMedia()
				} else if (MediaPlayer.EndOfMedia === mediaStatus && root.playlistLooped && playlistInfo.mediaCount) {
					root.currentFile = 0
					root.playMedia()
				}
			}
		}

		VideoOutput {
			id: videoOutput

			anchors.top: fullScreen || Config.isMobileTarget ? parent.top : menuBar.bottom
			anchors.bottom: fullScreen ? parent.bottom : playbackControl.top
			anchors.left: parent.left
			anchors.right: parent.right
			anchors.leftMargin: fullScreen ? 0 : 20
			anchors.rightMargin: fullScreen ? 0 : 20
			visible: mediaPlayer.hasVideo

			property bool fullScreen: false

			TapHandler {
				onDoubleTapped: {
					if (parent.fullScreen) {
						root.showNormal()
					} else {
						root.showFullScreen()
					}
					parent.fullScreen = !parent.fullScreen
				}
				onTapped: {
					root.closeOverlays()
				}
			}
		}
	}
}
