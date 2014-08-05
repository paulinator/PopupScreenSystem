using System.Windows;
using System.Windows.Controls;

namespace PThomann.Utilities.PopupScreenSystem
{
	public class StringInputScreen : ConfigurableScreen
	{
		TextBoxElement text;
		string input = "";
		public string Text
		{
			get { return input; }
			set {
				if (value == null)
					value = "";
				input = value;
				if (text != null)
					text.Text = value; 
			}
		}

		public StringInputScreen( )
			: base(typeof(StringInputScreen), false)
		{
			Title = "Enter Text";
			Child.VerticalAlignment = VerticalAlignment.Top;
			Child.Margin = new Thickness(0, 20, 0, 0);
			ButtonsProperties(VerticalAlignment.Top, new Thickness(0, 0, 0, 0));
			//((Grid)Child).ShowGridLines = true;
			text = new TextBoxElement()
				{
					Text = input,
					ExtendLeft = true,
					ExtendRight = true,
					ShowLabel = false,
					TextChangedAction = () => { input = text.Text; }
				};
			Elements = new ConfigurableScreenElement[] { text, new DistanceElement{ Height=350, IsEnabled=true } };
			Child.VerticalAlignment = VerticalAlignment.Top;
			Button[] arr = MakeDialogButtons(CustomScreenDialogButtons.OkCancel).ToArray();
			// arr[1].Content = "Submit";
			Buttons = arr;
		}
	}
}
