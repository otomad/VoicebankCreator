using System.ComponentModel;

namespace VoicebankCreator.ViewModels;

public class ViewModelBase : INotifyPropertyChanged {
	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged(string propertyName) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
