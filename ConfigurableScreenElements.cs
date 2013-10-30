using Microsoft.Phone.Controls;
using PThomann.Utilities.PopupScreenSystem.ColorChoice;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// An option for composition of a ConfigurableScreen
	/// It gets 70px Height in a 4-column grid.
	/// </summary>
	public abstract class ConfigurableScreenElement
	{
		public int Height = 70;
		/// <summary>
		/// Should we show a help icon?
		/// </summary>
		public bool ShowHelp;
		/// <summary>
		/// Should we show a Label?
		/// </summary>
		public bool ShowLabel = true;
		bool extl, extr;
		/// <summary>
		/// Should the Child be extended all to the left and take the Labels space?
		/// </summary>
		public bool ExtendLeft
		{
			get { return extl; }
			set
			{
				extl = value;
				if (child != null)
				{
					Grid.SetColumn(child, extl ? 0 : 2);
					Grid.SetColumnSpan(child, 1 + (extl ? 2 : 0) + (extr ? 1 : 0));
				}
			}
		}

		VerticalAlignment valign= VerticalAlignment.Center;
		public VerticalAlignment LabelVerticalAlignment
		{
			get { return valign; }
			set
			{
				valign = value;
				if (label != null)
					label.VerticalAlignment = value;
			}
		}
		/// <summary>
		/// Should the child be extended all to the right and take the space of the help icon?
		/// </summary>
		public bool ExtendRight
		{
			get { return extr; }
			set
			{
				extr = value;
				if (child != null)
				{
					Grid.SetColumnSpan(child, 1 + (extl ? 2 : 0) + (extr ? 1 : 0));
				}
			}
		}

		/// <summary>
		/// Will fire when the help icon is clicked
		/// </summary>
		public Action HelpAction;

		FrameworkElement child;
		/// <summary>
		/// The child control that each derived class will have, 
		/// by which they set each other apart.
		/// </summary>
		public FrameworkElement Child
		{
			get { return child; }
			protected set
			{
				if (value != null)
				{
					Grid.SetColumn(value, extl ? 0 : 2);
					Grid.SetColumnSpan(value, 1 + (extl ? 2 : 0) + (extr ? 1 : 0));
				}
				if (child != null)
				{
					if (value != null)
						Grid.SetRow(value, Grid.GetRow(child));

					Grid parent = (Grid)child.Parent;
					if (parent != null)
					{
						parent.Children.Remove(child);
						if (value != null)
							parent.Children.Add(value);
					}
				}
				else if (value != null && label != null)
				{
					Grid.SetRow(value, Grid.GetRow(label));
				}
				else if (value != null && right != null)
				{
					Grid.SetRow(value, Grid.GetRow(right));
				}
				child = value;
			}
		}
		FrameworkElement lchild;
		/// <summary>
		/// The child control that each derived class will have, 
		/// by which they set each other apart.
		/// </summary>
		public FrameworkElement LogicalChild
		{
			get { return lchild; }
			protected set
			{
				lchild = value;
			}
		}

		public bool IsEnabled
		{
			get { 
				if (Child is Control)
					return ((Control)Child).IsEnabled;
				return true;
			}
			set
			{
				if (Child is Control)
					((Control)Child).IsEnabled = value;
				else if (LogicalChild is Control)
					((Control)LogicalChild).IsEnabled = value;
			}
		}

		TextBlock label;

		string labelText = "";
		/// <summary>
		/// The text of the Label.
		/// </summary>
		public string Label
		{
			get { return labelText; }
			set
			{
				labelText = (value == null) ? "" : value;
				if (label != null)
					label.Text = labelText;
			}
		}

		double labelFontSize = 28;
		public double LabelFontSize
		{
			get { return labelFontSize; }
			set
			{
				labelFontSize = value;
				if (label != null)
					label.FontSize = value;
			}
		}

		/// <summary>
		/// Use this to get / set the right control that is displayed when ShowHelp == true.
		/// Can only be changed when this ConfigurableScreenElement is not in a ConfigurableScreen.Elements.
		/// </summary>
		public FrameworkElement HelpControl
		{
			get { return right; }
			set {
				if (!isInScreen)
					right = value;
			}
		}
		FrameworkElement right;

		bool isInScreen;
		internal void RemoveFromScreen()
		{
			if (isInScreen)
				isInScreen = false;
		}

		/// <summary>
		/// Adds all the components of this ConfigurableScreenElement 
		/// to the specified row of the specified grid.
		/// </summary>
		/// <param name="grid">the grid</param>
		/// <param name="row">the row</param>
		internal void AddTo(Grid grid, int row)
		{
			if (isInScreen)
				return;
			isInScreen = true;

			if (ShowLabel)
			{
				label = new TextBlock()
				{
					Text = labelText,
					FontSize = labelFontSize,
					Foreground = new SolidColorBrush(Colors.White),
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = valign
				};
				Grid.SetColumn(label, 0);
				Grid.SetRow(label, row);
				grid.Children.Add(label);
			}
			if (ShowHelp)
			{
				if (right == null)
				{

					right = new Grid() { HorizontalAlignment= HorizontalAlignment.Stretch, VerticalAlignment= VerticalAlignment.Stretch};
					Rectangle r = new Rectangle() { Fill = new SolidColorBrush(Colors.Black), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, MaxHeight=60, MaxWidth=60 };
					QuestionSign qs = new QuestionSign() { Width=30, Height=30, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment= VerticalAlignment.Center};
					//right.RenderTransform = new ScaleTransform() { ScaleX=0.1, ScaleY = 0.1 };
					//right = new Image()
					//{
					//	Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("help.png", UriKind.Relative)),
					//	HorizontalAlignment = HorizontalAlignment.Center,
					//	VerticalAlignment = VerticalAlignment.Center,
					//	Visibility = Visibility.Visible
					//};
					qs.MouseLeftButtonDown += new MouseButtonEventHandler(help_MouseLeftButtonDown);
					r.MouseLeftButtonDown += new MouseButtonEventHandler(help_MouseLeftButtonDown);
					((Grid)right).Children.Add(r);
					((Grid)right).Children.Add(qs);
					Grid.SetColumn(right, 3);
					Grid.SetRow(right, row);
					grid.Children.Add(right);
				}
				else
				{
					Grid.SetColumn(right, 3);
					Grid.SetRow(right, row);
					grid.Children.Add(right);
				}
			}
			if (child != null)
			{
				Grid.SetRow(child, row);
				grid.Children.Add(child);
			}
		}

		void help_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (HelpAction != null)
				HelpAction();
		}
	}

	/// <summary>
	/// An element without children, to create distance between other elements.
	/// </summary>
	public sealed class DistanceElement : ConfigurableScreenElement
	{
		public DistanceElement()
		{
			ShowHelp = false;
			ShowLabel = false;
			Child = null;
		}
	}

	/// <summary>
	/// The Button packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class ButtonElement : ConfigurableScreenElement
	{
		Button button;
		public ButtonElement()
		{
			button = new Button()
			{
				Content = "Button",
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center,
				FontSize = 24,
				Foreground = new SolidColorBrush(Colors.White)
			};
			Child = button;
			button.Click += new RoutedEventHandler(button_Click);
		}

		/// <summary>
		/// Fires when the button is clicked.
		/// </summary>
		public Action ClickAction;
		/// <summary>
		/// Fires when the Button is clicked, after the ClickAction has executed.
		/// </summary>
		public event EventHandler Click;
		
		void button_Click(object sender, RoutedEventArgs e)
		{
			if (ClickAction != null)
				ClickAction();
			EventHandler eh = Click;
			if (eh != null)
				eh(this, e);
		}

		/// <summary>
		/// The button text content.
		/// </summary>
		public string Text
		{
			get { return (string)button.Content; }
			set { 
				if (value != null) 
					button.Content = value; 
			}
		}
	}

	/// <summary>
	/// The HyperlinkButton packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class HyperlinkButtonElement : ConfigurableScreenElement
	{
		HyperlinkButton button;
		/// <summary>
		/// Fires when the HyperlinkButton is clicked.
		/// </summary>
		public Action ClickAction;
		/// <summary>
		/// Fires when the HyperlinkButton is clicked, after the ClickAction has executed.
		/// </summary>
		public event EventHandler Click;

		public HyperlinkButtonElement()
		{
			button = new HyperlinkButton() { 
				FontSize = 22,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center,
				Content = "Hyperlink",
				Foreground = new SolidColorBrush(Colors.White)
			};
			Child = button;
			button.Click += new RoutedEventHandler(button_Click);
		}

		/// <summary>
		/// The link text.
		/// </summary>
		public string Text
		{
			get { return (string)button.Content; }
			set
			{
				button.Content = value == null ? "" : value;
			}
		}

		void button_Click(object sender, RoutedEventArgs e)
		{
			if (ClickAction != null)
				ClickAction();
			EventHandler eh = Click;
			if (eh != null)
				eh(this, e);
		}

		public double FontSize { 
			get { return button.FontSize; }
			set { button.FontSize = value;  }
		}
	}

	/// <summary>
	/// The CheckBox packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class CheckBoxElement : ConfigurableScreenElement
	{
		CheckBox cb;
		/// <summary>
		/// Fires when the CheckBox is checked.
		/// </summary>
		public Action CheckedAction;
		/// <summary>
		/// Fires when the CheckBox is checked, after the CheckedAction has executed.
		/// </summary>
		public event EventHandler Checked;
		/// <summary>
		/// Fires when the CheckBox is unchecked.
		/// </summary>
		public Action UncheckedAction;
		/// <summary>
		/// Fires when the CheckBox is unchecked, after the UncheckedAction has executed.
		/// </summary>
		public event EventHandler Unchecked;

		/// <summary>
		/// get / set the check-state of the CheckBox.
		/// </summary>
		public bool? IsChecked
		{
			get { return cb.IsChecked; }
			set { cb.IsChecked = value; }
		}

		public CheckBoxElement()
		{
			cb = new CheckBox()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center 
			};
			Child = cb;
			cb.Checked += new RoutedEventHandler(cb_Checked);
			cb.Unchecked += new RoutedEventHandler(cb_Unchecked);
		}

		void cb_Unchecked(object sender, RoutedEventArgs e)
		{
			if (UncheckedAction != null)
				UncheckedAction();
			EventHandler eh = Unchecked;
			if (eh != null)
				eh(this, e);
		}
		void cb_Checked(object sender, RoutedEventArgs e)
		{
			if (CheckedAction != null)
				CheckedAction();
			EventHandler eh = Checked;
			if (eh != null)
				eh(this, e);
		}
	}

	/// <summary>
	/// A rectangular colored button that leads to a ColorChoiceScreen, packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class ColorButtonElement : ConfigurableScreenElement
	{
		Rectangle rect;
		public Action ClickAction;
		public event EventHandler Click;

		public ColorButtonElement()
		{
			rect = new Rectangle() { };
			rect.MouseLeftButtonDown += rect_MouseLeftButtonDown;
			rect.MouseLeave += rect_MouseLeave;
			rect.MouseEnter += rect_MouseEnter;
			rect.MouseLeftButtonUp += rect_MouseLeftButtonUp;
			Border bd = new Border()
			{
				BorderThickness = new Thickness(2),
				Margin = new Thickness(10),
				BorderBrush = new SolidColorBrush(Colors.White)
			};
			bd.Child = rect;
			Child = bd;
		}

		void rect_MouseLeave(object sender, MouseEventArgs e)
		{
			if (last == this)
				left = true;
			rect.Fill = new SolidColorBrush(color);
		}
		void rect_MouseEnter(object sender, MouseEventArgs e)
		{
			if (left && last == this)
				rect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(255 - color.R / 2), (byte)(255 - color.G / 2), (byte)(255 - color.B / 2)));
		}
		void rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			last = this;
			left = false;
			rect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)(255 - color.R / 2), (byte)(255 - color.G / 2), (byte)(255 - color.B / 2)));
		}
		void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			left = false;
			last = null;

			if (ClickAction == null)
			{
				ColorChoiceScreen.Show(color, Label);
				ColorChoiceScreen popup = Popups.Current as ColorChoiceScreen;
				popup.CloseAction = () =>
				{
					if (popup.Result == CustomScreenDialogResult.Accept)
					{
						Color = popup.Color;
					}
				};
			}
			else
				ClickAction();
			if (Click != null)
				Click(this, e);
			rect.Fill = new SolidColorBrush(color);
		}

		static bool left;
		static ColorButtonElement last;

		Color color = Colors.Transparent;
		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				rect.Fill = new SolidColorBrush(value);
				OnColorChanged();
			}
		}
		public Action ColorChangedAction;
		void OnColorChanged()
		{
			if (ColorChangedAction != null)
				ColorChangedAction();
		}
	}

	/// <summary>
	/// The HSV ColorChooser as a ConfigurableScreenElement.
	/// </summary>
	public sealed class ColorChoiceElement : ConfigurableScreenElement
	{
		public ColorChoiceElement()
		{
			ExtendLeft = true;
			ExtendRight = true;
			Height = 350;
			Child = new ColorChooser()
			{
				Width = 330,
				Height = 330,
				// Angle = 45,
				SpectrumThickness = 63,
				TriangleOverlap = 7,
				HSV = new ColorHSV(0, 0, 0)
			};
		}

		public Color Color
		{
			get { return ((ColorChooser)Child).Color; }
			set { ((ColorChooser)Child).Color = value; }
		}
	}

	/// <summary>
	/// The TextBox packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class TextBoxElement : ConfigurableScreenElement
	{
		TextBox tb;

		/// <summary>
		/// Fires when the Text changes.
		/// </summary>
		public Action TextChangedAction;
		/// <summary>
		/// Fires when the Text changes, after the TextChangedAction has executed.
		/// </summary>
		public event TextChangedEventHandler TextChanged;

		/// <summary>
		/// The text contained in the TextBox.
		/// </summary>
		public string Text
		{
			get { return tb.Text; }
			set { tb.Text = (value == null) ? "" : value; }
		}

		public TextBoxElement()
		{
			tb = new TextBox()
			{
				FontSize = 24,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			Child = tb;
			tb.TextChanged += new TextChangedEventHandler(tb_TextChanged);
		}

		void tb_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (TextChangedAction != null)
				TextChangedAction();
			TextChangedEventHandler eh = TextChanged;
			if (eh != null)
				eh(this, e);
		}
	}

	/// <summary>
	/// The PThomann.Utilities.PopupScreenSystem.ColorChoice.SimpleSlider packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class SliderElement : ConfigurableScreenElement
	{
		SimpleSlider slider;

		public Action ValueChangedAction;
		public event EventHandler ValueChanged;

		public double Value
		{
			get { return slider.Value; }
			set {
				slider.Value = value;
			}
		}
		public double Minimum
		{
			get { return slider.Minimum; }
			set { slider.Minimum = value; }
		}
		public double Maximum
		{
			get { return slider.Maximum; }
			set { slider.Maximum = value; }
		}
		/// <summary>
		/// This has no effect so far.
		/// </summary>
		public double SmallChange
		{
			get { return slider.SmallChange; }
			set { slider.SmallChange = value; }
		}
		/// <summary>
		/// This has no effect so far.
		/// </summary>
		public double LargeChange
		{
			get { return slider.LargeChange; }
			set { slider.LargeChange = value; }
		}
		public bool LeftEmpty
		{
			get { return slider.LeftEmpty; }
			set { slider.LeftEmpty = value; }
		}
		public bool RightEmpty
		{
			get { return slider.RightEmpty; }
			set { slider.RightEmpty = value; }
		}
		public Color EmptyColor
		{
			get { return slider.EmptyColor; }
			set { slider.EmptyColor = value; }
		}

		/// <summary>
		/// Brush of the little slider itself.
		/// </summary>
		public Brush SliderFill
		{
			get { return slider.SliderFill; }
			set { slider.SliderFill = value; }
		}
		/// <summary>
		/// Brush of the slider path.
		/// </summary>
		public Brush Fill
		{
			get { return slider.Fill; }
			set { slider.Fill = value; }
		}

		TextBlock valueLabel;
		public SliderElement()
		{
			slider = new SimpleSlider()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center,
				Height = 26,
				Margin = new Thickness(10,0,0,0)
			};
			valueLabel = new TextBlock()
			{
				Foreground = Brushes.White,
				FontSize = 26,
				Text = "0",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			Grid grid = new Grid()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Pixel) });
			Grid.SetColumn(valueLabel, 1);
			grid.Children.Add(slider);
			grid.Children.Add(valueLabel);
			Child = grid;
			LogicalChild = slider;
			slider.ValueChanged += slider_ValueChanged;
		}

		void slider_ValueChanged(object sender, EventArgs e) //RoutedPropertyChangedEventArgs<double> e)
		{
			if (ValueChangedAction != null)
				ValueChangedAction();
			valueLabel.Text = slider.Value.ToString("0.##");
			EventHandler eh = ValueChanged;
			if (eh != null)
				eh(this, e);
		}
	}

	/// <summary>
	/// The ListBox packed in a ConfigurableScreenElement.
	/// </summary>
	public sealed class ListBoxElement : ConfigurableScreenElement
	{
		/// <summary>
		/// Creates a sequence of numbers to choose in the ListBox.
		/// </summary>
		/// <param name="start">the first number</param>
		/// <param name="count">the number of numbers</param>
		/// <param name="increment">the difference between consecutive numbers</param>
		public void MakeNumberSelection(int start, int count, int increment)
		{
			box.Items.Clear();
			for (int i = 0; i < count; i++)
			{
				box.Items.Add(start);
				start += increment;
			}
		}

		ListBox box;
		/// <summary>
		/// Create a ListBoxElement.
		/// </summary>
		/// <param name="heightFactor">how many items are visible at once?</param>
		public ListBoxElement(double heightFactor)
		{
			box = new ListBox()
			{
				FontSize = 28,
				Margin = new Thickness(15, 10, 15, 10),
				//Background = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)),	 // gray, looks ugly
				//Foreground = new SolidColorBrush(Colors.White),						   // this was here to make text black in the ComboBox which was white.
																							// However, ComboBox doesn't work anymore at all in WP >= 7.1,
																							// and ListBox has black background and white text by default.
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center,
			};
			Height = (int)(heightFactor * Height);
			Child = box;
			box.SelectionChanged += new SelectionChangedEventHandler(box_SelectionChanged);
		}

		/// <summary>
		/// Will be fired whenever the Selection changes.
		/// </summary>
		public Action SelectionChangedAction;
		void box_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectionChangedAction != null)
				SelectionChangedAction();
			SelectionChangedEventHandler d = SelectionChanged;
			if (d != null)
				d(this, e);
		}

		/// <summary>
		/// the currently selected item.
		/// </summary>
		public object SelectedItem
		{
			get { return box.SelectedItem; }
			set
			{
				if (value != null)
					box.SelectedItem = value;
			}
		}
		/// <summary>
		/// the index of the currently selected item
		/// </summary>
		public int SelectedIndex
		{
			get { return box.SelectedIndex; }
			set
			{
				if (value >= 0 && value < Count)
					box.SelectedIndex = value;
			}
		}
		/// <summary>
		/// The number of items in the ListBox
		/// </summary>
		public int Count
		{
			get { return box.Items.Count; }
		}

		/// <summary>
		/// Add an item to the end of the ListBox
		/// </summary>
		/// <param name="item">the item</param>
		public void Add(object item)
		{
			if (item != null && !box.Items.Contains(item))
				box.Items.Add(item);
		}
		/// <summary>
		/// Add multiple items to the end of the ListBox
		/// </summary>
		/// <param name="item">the item</param>
		public void AddRange(IEnumerable items)
		{
			box.Visibility = Visibility.Collapsed;
			box.IsEnabled = false;
			foreach (object item in items)
			{
				box.Items.Add(item);
			}
			box.IsEnabled = true;
			box.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Insert an item to the ListBox at specified index
		/// </summary>
		/// <param name="index">the index</param>
		/// <param name="item">the item</param>
		public void Insert(int index, object item)
		{
			if (item != null && !box.Items.Contains(item) && index >= 0 && index <= Count)
				box.Items.Insert(index, item);
		}
		/// <summary>
		/// Insert multiple consecutive items to the ListBox starting at specified index.
		/// </summary>
		/// <param name="index">the index</param>
		/// <param name="item">the item</param>
		public void InsertRange(int index, IEnumerable items)
		{
			if (index >= 0 && index <= Count)
			{
				foreach (object item in items)
				{
					Insert(index++, item);
				}
			}
		}
		/// <summary>
		/// Remove an item from the ListBox
		/// </summary>
		/// <param name="item"></param>
		public void Remove(object item)
		{
			if (item != null && box.Items.Contains(item))
				box.Items.Remove(item);
		}
		/// <summary>
		/// Remove the item at specified index from he ListBox.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			if (index >= 0 && index < Count)
				box.Items.RemoveAt(index);
		}

		/// <summary>
		/// Fires whenever the selection in the ListBox changes, after the SelectionChangedAction has executed.
		/// </summary>
		public event SelectionChangedEventHandler SelectionChanged;
	}

}

