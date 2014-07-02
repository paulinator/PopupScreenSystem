using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PThomann.Utilities.PopupScreenSystem
{
	/// <summary>
	/// Base class for all screens in the PopupScreenSystem.
	/// 
	/// Provides a basic full-screen popup with
	/// - 90% opaque black background, white foreground
	/// - a title on the top 
	/// - a bar of buttons on the bottom
	/// - a child element that is centered in the center screen space.
	/// </summary>
	abstract public class CustomScreenBase
	{
		public static Brush link = new SolidColorBrush(Color.FromArgb(255, 201, 226, 255));

		#region private

		readonly bool scrolling;
		readonly Grid grid;
		readonly Rectangle bg;
		protected readonly TextBlock title;
		protected readonly StackPanel buttons;
		protected readonly ScrollViewer scroller;
		readonly System.Windows.Controls.Primitives.Popup popup;
		Microsoft.Phone.Controls.PageOrientation pageOrientation;

		double left = 15, right = 15, top = 15, bottom = 15;
		/// <summary>
		/// Here the Click-Actions are stored by Button.
		/// </summary>
		Dictionary<object, Action> buttonActions = new Dictionary<object, Action>();

		CustomScreenDialogResult result;
		CustomScreenBase lastScreen;

		private void makeOrientation(Orientation orient)
		{
			grid.ColumnDefinitions.Clear();
			grid.RowDefinitions.Clear();

			if (orient == System.Windows.Controls.Orientation.Horizontal)
			{
				grid.Width = Application.Current.Host.Content.ActualHeight;
				grid.Height = Application.Current.Host.Content.ActualWidth;

				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(160) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(90) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(90) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });

				Grid.SetColumnSpan(bg, 5);
				Grid.SetRowSpan(bg, 7);
				Grid.SetColumn(title, 1);
				Grid.SetColumnSpan(title, 3);
				Grid.SetRow(title, 1);
				buttons.HorizontalAlignment = HorizontalAlignment.Stretch;
				Grid.SetColumn(buttons, 3);
				Grid.SetRow(buttons, 2);
				Grid.SetRowSpan(buttons, 3);
				if (scrolling)
				{
					scroller.VerticalAlignment = VerticalAlignment.Top;
					Grid.SetColumn(scroller, 1);
					Grid.SetRow(scroller, 3);
					Grid.SetRowSpan(scroller, 3);
				}
				else if (child != null)
				{
					child.VerticalAlignment = VerticalAlignment.Top;
					Grid.SetColumn(child, 1);
					Grid.SetRow(child, 3);
					Grid.SetRowSpan(child, 3);
				}
			}
			else// vertical
			{
				grid.Height = Application.Current.Host.Content.ActualHeight;
				grid.Width = Application.Current.Host.Content.ActualWidth;

				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(90) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(90) });
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15) });

				Grid.SetColumnSpan(bg, 3);
				Grid.SetRowSpan(bg, 7);
				Grid.SetColumn(title, 1);
				Grid.SetRow(title, 1);
				buttons.HorizontalAlignment = HorizontalAlignment.Center;
				Grid.SetColumn(buttons, 1);
				Grid.SetRow(buttons, 5);
				Grid.SetRowSpan(buttons, 1);
				if (scrolling)
				{
					scroller.VerticalAlignment = VerticalAlignment.Center;
					Grid.SetColumn(scroller, 1);
					Grid.SetRow(scroller, 3);
					Grid.SetRowSpan(scroller, 1);
				}
				else if (child != null)
				{
					child.VerticalAlignment = VerticalAlignment.Center;
					Grid.SetColumn(child, 1);
					Grid.SetRow(child, 3);
					Grid.SetRowSpan(child, 1);
				}
			}
		}

		FrameworkElement child;
		/// <summary>
		/// finalize Hide()
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void anim_Completed(object sender, EventArgs e)
		{
 //           MessageBox.Show("Completed");
			popup.IsOpen = false;
		}
		/// <summary>
		/// handles click events on all buttons made using MakeButton(...).
		/// Executes stored Actions first, then issues a ButtonClick event.
		/// </summary>
		/// <param name="sender">where the event originated</param>
		/// <param name="e">the RoutedEventArgs</param>
		void button_Click(object sender, RoutedEventArgs e)
		{
			if (buttonActions.ContainsKey(sender))
				buttonActions[sender]();
			EventHandler eh = ButtonClick;
			if (eh != null)
				eh(sender, e);
		}

		#endregion private

		#region PopupSystem internals

		/// <summary>
		/// This is intended to be used as a key to denote a scope 
		/// within which this should be the only instance.
		/// It is currently only used in the Popups.Show&lt;T&gt; 
		/// for CustomScreen-derived classes that have a corresponding
		/// CustomScreenParameters-derived class. It is the Type T in these cases.
		/// </summary>
		public readonly object UniqueScope;
		
		/// <summary>
		/// contains the last screen on the same layer,
		/// which is where pressing the back button once will lead if not null.
		/// The PopupScreenSystem assigns this only when ShowMode.Next is used.
		/// </summary>
		public CustomScreenBase LastScreen
		{
			get { return lastScreen; }
			internal set { lastScreen = value; }
		}

		/// <summary>
		/// This is called every time the screen is shown.
		/// </summary>
		virtual protected void OnShow()
		{ 
		}

		/// <summary>
		/// Bring this popup into vision, fading in quickly.
		/// </summary>
		internal void Show(bool returning)
		{
			if (!returning)
			{
				result = CustomScreenDialogResult.Cancel;
				OnShow();
			}

			if (scrolling)
			{
				scroller.ScrollToVerticalOffset(0);
			}
			PageOrientation = Popups.PageOrientation;
			DoubleAnimation anim = new DoubleAnimation();
			anim.From = 0;
			anim.To = 1;
			anim.Duration = new Duration(TimeSpan.FromSeconds(0.3));
			anim.FillBehavior = FillBehavior.HoldEnd;
			Storyboard sb = new Storyboard() { FillBehavior = FillBehavior.HoldEnd };
			Storyboard.SetTarget(anim, grid);
			Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
			sb.Children.Add(anim);

			sb.Begin();
			grid.Opacity = 1;
			popup.IsOpen = true;
		}

		/// <summary>
		/// Bring this popup out of vision, fading out quickly.
		/// </summary>
		internal void Hide()
		{
			DoubleAnimation anim = new DoubleAnimation();
			anim.From = 1;
			anim.To = 0;
			anim.Duration = new Duration(TimeSpan.FromSeconds(0.3));
			anim.FillBehavior = FillBehavior.HoldEnd;
			Storyboard sb = new Storyboard() { FillBehavior = FillBehavior.HoldEnd };
            sb.Duration = anim.Duration;
			Storyboard.SetTarget(anim, grid);
			Storyboard.SetTargetProperty(anim, new PropertyPath("Opacity"));
			sb.Children.Add(anim);
            sb.Completed += anim_Completed;
//			anim.Completed += new EventHandler(anim_Completed);
			sb.Begin();
		}

		#endregion PopupSystem internals

		protected void ButtonsProperties(VerticalAlignment align, Thickness margin)
		{
			buttons.VerticalAlignment = align;
			buttons.Margin = margin;
		}

		public CustomScreenBase(object scope, bool scroll)
		{
			UniqueScope = scope; // TODO: use this like the T in the Popups.Show<T>
			scrolling = scroll;
			if (scope != null)
				Popups.SetPopupUniqueScope(this, scope, false);

			if (0 != (Popups.PageOrientation & Microsoft.Phone.Controls.PageOrientation.Landscape))
				Orientation = System.Windows.Controls.Orientation.Horizontal;
			else
				Orientation = System.Windows.Controls.Orientation.Vertical;

			grid = new Grid();
			buttons = new StackPanel()
			{
				Orientation = Orientation.Vertical,
				VerticalAlignment = VerticalAlignment.Center
			};
			if (scrolling)
			{
				scroller = new ScrollViewer()
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
					HorizontalContentAlignment = HorizontalAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Center,
					VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
					HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
				};
			}
			bg = new Rectangle()
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Fill = new SolidColorBrush(Colors.Black)
			};
			title = new TextBlock()
			{
				Text = "Title",
				FontSize = 32,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 225, 110))
			};
			popup = new System.Windows.Controls.Primitives.Popup()
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			makeOrientation(Orientation);

			grid.Children.Add(bg);
			grid.Children.Add(title);
			grid.Children.Add(buttons);
			if (scrolling)
			{
				grid.Children.Add(scroller);
			}
			popup.Child = grid;

			PageOrientation = Popups.PageOrientation;
		}
		public CustomScreenBase()
			: this(null, true)
		{
		}

		#region UI
		
		/// <summary>
		/// Indicates whether layout is currently horizontal or vertical.
		/// </summary>
		public Orientation Orientation
		{
			get;
			private set;
		}

		/// <summary>
		/// Indicates or sets whether to display horizontal scrollbars (defaults to disabled).
		/// </summary>
		public ScrollBarVisibility HorizontalScrollBarVisibility
		{
			get {
				if (scrolling)
					return scroller.HorizontalScrollBarVisibility;
				else
					return ScrollBarVisibility.Disabled;
			}
			set { if (scrolling) scroller.HorizontalScrollBarVisibility = value; }
		}
		/// <summary>
		/// INdicates or sets whether a vertical scrollbar is shown. defaults to Auto.
		/// </summary>
		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get {
				if (scrolling)
					return scroller.VerticalScrollBarVisibility;
				else
					return ScrollBarVisibility.Disabled;
			}
			set { if (scrolling) scroller.VerticalScrollBarVisibility = value; }
		}

		/// <summary>
		/// The Opacity of the Screen-filling Background Rectangle.
		/// </summary>
		public double BackgroundOpacity
		{
			get { return bg != null ? bg.Opacity : -1; }
			set
			{
				if (value >= 0 && value <= 1 && bg != null)
				{
					bg.Opacity = value;
				}
			}
		}
		/// <summary>
		/// The Opacity of the Child.
		/// </summary>
		public double ForegroundOpacity
		{
			get { return child != null ? child.Opacity : -1; }
			set
			{
				if (value >= 0 && value <= 1 && child != null)
				{
					child.Opacity = value;
				}
			}
		}

		/// <summary>
		/// The Background color of this screen.
		/// </summary>
		public Brush Background
		{
			get { return bg.Fill; }
			set
			{
				if (value != null)
				{
					bg.Fill = value;
				}
			}
		}
		/// <summary>
		/// The dimensions of the grid rows/columns at the edges, which always remain empty.
		/// </summary>
		public Thickness Padding
		{
			get
			{
				return new Thickness(left, top, right, bottom);
			}
			set
			{
				if (value == null)
					return;
				left = value.Left;
				right = value.Right;
				top = value.Top;
				bottom = value.Bottom;
				grid.ColumnDefinitions[0].Width = new GridLength(left);
				grid.ColumnDefinitions[2].Width = new GridLength(right);
				grid.RowDefinitions[0].Height = new GridLength(top);
				grid.RowDefinitions[2].Height = new GridLength(bottom);
			}
		}

		#endregion UI

		#region Buttons

		/// <summary>
		/// the buttons on the bottom of the popup.
		/// Use MakeButton(...) to make Buttons taking advantage 
		/// of the builtin Action support.
		/// </summary>
		public Button[] Buttons
		{
			get
			{
				Button[] res = new Button[buttons.Children.Count];
				for (int i = 0; i < res.Length; i++)
					res[i] = (Button)buttons.Children[i];
				return res;
			}
			set
			{
				buttons.Children.Clear();
				if (value == null)
					return;
				if (Orientation == System.Windows.Controls.Orientation.Vertical)
				{
					foreach (Button b in value)
						buttons.Children.Add(b);
				}
				else
				{
					for (int i = value.Length - 1; i >= 0; i--)
						buttons.Children.Add(value[i]);
				}
			}
		}

		/// <summary>
		/// Creates a Button, assigns its text Content
		/// and an Action to execute when it's clicked.
		/// </summary>
		/// <param name="text">the text content</param>
		/// <param name="action">the clixk action</param>
		/// <returns>the finished button, ready to include in the Buttons array.</returns>
		public Button MakeButton(string text, Action action)
		{
			Button b = new Button()
			{
				Content = text
			};
			//if (Orientation == System.Windows.Controls.Orientation.Horizontal)
			//{
			//	b.Width = 180;
			//}
			b.Click += new RoutedEventHandler(button_Click);
			buttonActions[b] = action;
			return b;
		}

		/// <summary>
		/// Create a List of standard dialog Buttons, 
		/// initialized with text and the logic to set the Result and close the popup.
		/// These can be used to compose the Buttons[].
		/// </summary>
		/// <param name="buttons">Indicates what combination of buttons is returned.</param>
		/// <returns>the wished list of buttons</returns>
		public List<Button> MakeDialogButtons(CustomScreenDialogButtons buttons)
		{
			List<Button> list = new List<Button>();
			if ((((int)buttons) & 1) != 0)  // provides a Cancel button
				list.Add(MakeButton("Cancel", () =>
				{
					Result = CustomScreenDialogResult.Cancel;
					Popups.Back();
				}));
			if ((((int)buttons) & 2) != 0)  // provides a Cancel button
				list.Add(MakeButton("OK", () =>
				{
					Result = CustomScreenDialogResult.Accept;
					Popups.Back();
				}));
			if ((((int)buttons) & 4) != 0)  // provides a Cancel button
			{
				list.Add(MakeButton("No", () =>
				{
					Result = CustomScreenDialogResult.Deny;
					Popups.Back();
				}));
				list.Add(MakeButton("Yes", () =>
				{
					Result = CustomScreenDialogResult.Accept;
					Popups.Back();
				}));
			}
			return list;
		}

		/// <summary>
		/// Occurs when any of the buttons constructed using MakeButton(...) is clicked.
		/// </summary>
		public event EventHandler ButtonClick;

		#endregion Buttons

		#region Content

		/// <summary>
		/// The Title displayed at the top of the screen
		/// </summary>
		public string Title
		{
			get { return title.Text; }
			set
			{
				if (value != null)
					title.Text = value;
			}
		}

		/// <summary>
		/// The child that fills the center space.
		/// </summary>
		protected FrameworkElement Child
		{
			get { return child; }
			set
			{
				if (child != null)
				{
					if (scrolling)
						scroller.Content = null;
					else
						grid.Children.Remove(child);
				}

				child = value;

				if (value == null)
					return;

				if (scrolling) {
					scroller.Content = child;
				}
				else
				{
					//  child.VerticalAlignment = VerticalAlignment.Top; 
					// when working without scroller, users must know for themselves how they want child to be aligned.
					Grid.SetColumn(child, 1);
					Grid.SetRow(child, 3);
					if (Orientation == System.Windows.Controls.Orientation.Horizontal)
						Grid.SetRowSpan(child, 3);
					else
						Grid.SetRowSpan(child, 1);
					grid.Children.Add(child);
				}
			}
		}

		#endregion Content

		#region Dialog

		/// <summary>
		/// Contains the users reaction to the message. 
		/// Evaluate it in the CloseAction.
		/// </summary>
		public CustomScreenDialogResult Result
		{
			get { return result; }
			private set
			{
				if (Enum.IsDefined(typeof(CustomScreenDialogResult), value))
					result = value;
			}
		}

		/// <summary>
		/// Fires when the MessageScreen is closed, be it by back button or a button press on the popup.
		/// Can be used tu evaluate the MessageScreenResult.
		/// </summary>
		public Action CloseAction;

		#endregion Dialog

		#region Input events

		/// <summary>
		/// Pass a BackPress event to this popup
		/// </summary>
		/// <param name="e">Cancellable</param>
		internal void DoBackpress(System.ComponentModel.CancelEventArgs e)
		{
			OnBackPress(e);
			if (e.Cancel)
				return;
			if (CloseAction != null)
				CloseAction();
		}
		/// <summary>
		/// Subclasses can override this to react to BackPress events and even cancel them.
		/// </summary>
		/// <param name="e">CAncellable</param>
		virtual protected void OnBackPress(System.ComponentModel.CancelEventArgs e)
		{ }
		/// <summary>
		/// 
		/// </summary>
		public Microsoft.Phone.Controls.PageOrientation PageOrientation {
			get { return pageOrientation; }
			internal set
			{
				if (pageOrientation == value)
					return;		// no change

				if (Orientation == System.Windows.Controls.Orientation.Horizontal)
				{
					if (value == Microsoft.Phone.Controls.PageOrientation.LandscapeRight)
					{
						TransformGroup tg = new TransformGroup();
						RotateTransform rt = new RotateTransform() { Angle = -90, CenterX = 0, CenterY = 0 };
						tg.Children.Add(rt);
						TranslateTransform tt = new TranslateTransform() { X = 0, Y = grid.Width };
						tg.Children.Add(tt);
						grid.RenderTransform = tg;
					}
					else if (value == Microsoft.Phone.Controls.PageOrientation.LandscapeLeft) // default to LandScapeLeft 
					{
						TransformGroup tg = new TransformGroup();
						TranslateTransform tt = new TranslateTransform() { X = grid.Height, Y = 0 };
						tg.Children.Add(tt);
						RotateTransform rt = new RotateTransform() { Angle = 90, CenterX = grid.Height, CenterY = 0 };
						tg.Children.Add(rt);
						grid.RenderTransform = tg;
					}
					else // it is vertical
					{
						// ??? unreachable code I hope
						makeOrientation(System.Windows.Controls.Orientation.Vertical);
						PageOrientation = value;
						return;
					}
				}
				else if (Orientation == System.Windows.Controls.Orientation.Vertical)
				{
					if (value == Microsoft.Phone.Controls.PageOrientation.PortraitDown)
					{
						RotateTransform rt = new RotateTransform()
						{
							Angle = 180,
							CenterX = grid.Width / 2,
							CenterY = grid.Height / 2
						};
						grid.RenderTransform = rt;
					}
					else if (value == Microsoft.Phone.Controls.PageOrientation.PortraitUp) // default to PortraitUp
					{
						if (pageOrientation == Microsoft.Phone.Controls.PageOrientation.PortraitUp)
							return;	 // already Up, nothing to do.
						grid.RenderTransform = null;
					}
					else	// it is horizontal
					{
						// ??? unreachable code I hope
						makeOrientation(System.Windows.Controls.Orientation.Horizontal);
						PageOrientation = value;
						return;
					}
				}
				pageOrientation = value;
			}
		}

		#endregion Input events
	}

	/// <summary>
	/// Generic CustomScreens with strongly typed c# generics.
	/// That architecture supports matching CustomScreenParameters classes, 
	/// which can help in popup reuse. Sadly, this has no effects as of now.
	/// </summary>
	/// <typeparam name="T">the Type of the derived class</typeparam>
	abstract public class CustomScreen<T> : CustomScreenBase where T : CustomScreen<T> 
	{
		/// <summary>
		/// Create a CustomScreen and use the generic type T as uniqueScope.
		/// </summary>
		/// <summary>
		/// Create a CustomScreen that is stored by the Popup class as the only instance for uniqueScope.
		/// </summary>
		/// <param name="uniqueScope">key to identify this Popup</param>
		public CustomScreen(object uniqueScope, bool scrolling)
			: base(uniqueScope, scrolling)
		{ }
		public CustomScreen()
			: this(null, true)
		{ }
		/// <summary>
		/// Show this popup after applying the parameters.
		/// </summary>
		/// <param name="parameters">the parameters</param>
		internal void Show(CustomScreenParameters<T> parameters)
		{
			ApplyParameters(parameters);
			base.Show(false); // fading effect
		}
		/// <summary>
		/// Override this in subclasses to apply parameters before the popup is shown.
		/// </summary>
		/// <param name="parameters">the parameters</param>
		virtual protected void ApplyParameters(CustomScreenParameters<T> parameters)
		{
		}
	}

	/// <summary>
	/// Parameters for the generic CustomScreen classes, strongly typed.
	/// </summary>
	/// <typeparam name="T">the CustomScreen-derived class these parameters are for.</typeparam>
	abstract public class CustomScreenParameters<T> where T : CustomScreen<T> 
	{
		public readonly object UniqueScope;

		public CustomScreenParameters(object uniqueScope)
		{
			UniqueScope = uniqueScope;
		}

		/// <summary>
		/// Supply constructor parameters here if using Popups.Show(CustomScreenParameters)
		/// </summary>
		virtual public object[] ConstructorParameters { 
			get; 
			protected set; 
		}
	}
}
