using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// A generic popup screen supporting CustomScreenOption elements
	/// to be able to build Menus and Settings Dialogs easily.
	/// 
	/// The easiest way to do it is deriving a class from this. It is also possible to construct ConfigurableScreens in code.
	/// </summary>
	public class ConfigurableScreen : CustomScreen<ConfigurableScreen>
	{
		Grid content;

		public ConfigurableScreen(object uniqueScope, bool scrolling)
			: base(uniqueScope, scrolling)
		{
			content = new Grid();
			content.Margin = new Thickness(12, 0, 12, 0);
			content.HorizontalAlignment = HorizontalAlignment.Stretch;
			content.VerticalAlignment = VerticalAlignment.Top;
			content.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.15, GridUnitType.Star) });
			content.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.2, GridUnitType.Star) });
			content.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.15, GridUnitType.Star) });
			content.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });

			Child = content;
		}

		/// <summary>
		/// add a row to the grid.
		/// </summary>
		void addRow(int pixelHeight)
		{
			content.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(pixelHeight) });
		}

		List<ConfigurableScreenElement> elements = new List<ConfigurableScreenElement>();
		/// <summary>
		/// the ConfigurableScreenElements that make up this screen, top-down order.
		/// </summary>
		public ConfigurableScreenElement[] Elements
		{
			get { return elements.ToArray(); }
			set
			{
				foreach (ConfigurableScreenElement e in elements)
					e.RemoveFromScreen();
				elements.Clear();
				content.Children.Clear();
				content.RowDefinitions.Clear();
				if (value == null)
					return;
				elements.AddRange(value);
				int curRow = 0;
				foreach (ConfigurableScreenElement o in elements)
				{
					addRow(o.Height);
					o.AddTo(content, curRow++);
				}
			}
		}
	}
}
