using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// Kind of a MessageBox as full screen popup. 
	/// Keep in mind it is a non-modal Show()!
	/// </summary>
	sealed public class MessageScreen : CustomScreen<MessageScreen>
	{
		public MessageScreen(string title, string message, object uniqueScope, bool scrolling)
			: base(uniqueScope, scrolling)
		{
			text = new TextBlock()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				FontSize = 28,
				Foreground = new SolidColorBrush(Colors.White),
				TextAlignment = TextAlignment.Center,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(10)
			};
			Child = text;

			Message = message;
			Title = title;
		}

		public MessageScreen()
			: this("", "", null, true)
		{ }
		public MessageScreen(string title, string message)
			: this(title, message, null, true)
		{ }

		TextBlock text;
		/// <summary>
		/// The message to display.
		/// </summary>
		public string Message {
			get { return text.Text; }
			set {
				text.Text = (value == null ? "" : value);
			}
		}
	}
}
