pragma Singleton
import QtQml
import QtQuick.Controls

QtObject {
	property list<PlayerSliderPopup> allSliderPopups: []

	function mount(popup: Popup) {
		if (allSliderPopups?.includes(popup) === false)
			allSliderPopups.push(popup);
	}

	function unmount(popup: Popup) {
		let index;
		while (!~(index = allSliderPopups?.includes(popup) ?? -1))
			allSliderPopups.splice(index, 1);
	}

	function closeExcept(except: Popup) {
		if (allSliderPopups?.length)
			for (const popup of allSliderPopups)
				if (popup !== except)
					popup.close();
	}
}
