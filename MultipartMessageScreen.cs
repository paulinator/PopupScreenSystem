using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// Like MessageScreen, but instead of a single Text property, it contains a 
	/// vertical StackPanel to fill with whatever Controls you like.
	/// 
	/// It also contains Add()ers for text, links, and images.
	/// </summary>
	public class MultipartMessageScreen : CustomScreenBase
	{
		StackPanel panel;

		public MultipartMessageScreen()
			: this(null, true)
		{ }
		public MultipartMessageScreen(object uniqueScope, bool scrolling)
			: base(uniqueScope, scrolling)
		{
			if (Orientation == Orientation.Horizontal)
				Grid.SetColumnSpan(title, 2);
			Buttons = MakeDialogButtons(CustomScreenDialogButtons.OK).ToArray();

			panel = new StackPanel()
			{
				Orientation = Orientation.Vertical
			};
			Child = panel;
		}
		public void AddPart(UIElement control)
		{
			panel.Children.Add(control);
		}

		public Image AddImage(string relativePath)
		{
			return AddImage(new Uri(relativePath, UriKind.Relative));
		}
		public Image AddImage(Uri source)
		{
			Image img = new Image()
			{   
				Source = new BitmapImage(source),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Stretch = Stretch.Uniform
			};
			panel.Children.Add(img);
			return img;
		}
		/// <summary>
		/// Add a Hyperlink to the StackPanel.
		/// </summary>
		/// <param name="text">visible text of the link</param>
		/// <param name="uri">URI to navigate to.</param>
		/// <returns>the added HyperlinkButton, in case you want to modify it.</returns>
		public HyperlinkButton AddHyperlink(string text, Uri uri)
		{
			HyperlinkButton hb = new HyperlinkButton()
			{
				Content = text,
				NavigateUri = uri,
				FontSize = 26,
				Foreground = link,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};
			panel.Children.Add(hb);
			return hb;
		}
		/// <summary>
		/// Add a block of Text to the StackPanel.
		/// </summary>
		/// <param name="text">the text contained.</param>
		/// <returns>the added TextBlock, in case you want to modify it.</returns>
		public TextBlock AddText(string text)
		{
			TextBlock tb = new TextBlock()
			{
				Text = text,
				Foreground = Brushes.White,
				FontSize = 26,
				TextAlignment = TextAlignment.Center,
				TextWrapping = System.Windows.TextWrapping.Wrap,
				Margin = new Thickness(5)
			};
			panel.Children.Add(tb);
			return tb;
		}
		public StackPanel CreateImagePanel()
		{
			ScrollViewer sv = new ScrollViewer()
			{
				HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
				VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
			};
			StackPanel sp = new StackPanel()
			{
				Orientation = System.Windows.Controls.Orientation.Horizontal
			};
			sv.Content = sp;
			AddPart(sv);
			return sp;
		}
		public Image AddImageToPanel(StackPanel panel, Uri path)
		{
			Image img = new Image()
			{
				Margin = new Thickness(3),
				Stretch = Stretch.None,
				Source = new BitmapImage(path),
				VerticalAlignment = VerticalAlignment.Top
			};
			panel.Children.Add(img);
			return img;
		}
		public Image AddImageToPanel(StackPanel panel, string relativePath)
		{
			return AddImageToPanel(panel, new Uri(relativePath, UriKind.Relative));
		}
	}
}