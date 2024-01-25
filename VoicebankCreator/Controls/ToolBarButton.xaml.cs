namespace VoicebankCreator.Controls;

/// <summary>
/// ToolBarButton.xaml 的交互逻辑
/// </summary>
public partial class ToolBarButton : UserControl {
	public ToolBarButton() {
		InitializeComponent();
		Button.Click += (sender, e) => RaiseEvent(new RoutedEventArgs(ClickRoutedEvent, this));
	}

	private static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon" , typeof(string), typeof(ToolBarButton), new PropertyMetadata(""));

	public string Icon {
		get => (string)GetValue(IconProperty);
		set => SetValue(IconProperty, value);
	}

	private static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ToolBarButton), new PropertyMetadata(""));

	public string Text {
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	private static readonly RoutedEvent ClickRoutedEvent =
		EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(ToolBarButton));

	public event RoutedEventHandler Click {
		add => AddHandler(ClickRoutedEvent, value);
		remove => RemoveHandler(ClickRoutedEvent, value);
	}
}
