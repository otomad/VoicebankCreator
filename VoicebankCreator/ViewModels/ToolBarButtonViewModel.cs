namespace VoicebankCreator.ViewModels;

public class ToolBarButtonViewModel : ViewModelBase {
	private string name = "";
	private string icon = "";

	public string Name {
		get => name;
		set { name = value; OnPropertyChanged(nameof(Name)); }
	}

	public string Icon {
		get => icon;
		set { icon = value; OnPropertyChanged(nameof(Icon)); }
	}
}
