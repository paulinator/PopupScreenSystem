using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace PThomann.Utilities.PopupScreenSystem.ColorChoice
{
	public partial class SimpleSlider : UserControl
	{
		public SimpleSlider()
		{
			InitializeComponent();
			SizeChanged += SimpleSlider_SizeChanged;
			IsEnabledChanged += SimpleSlider_IsEnabledChanged;
		}

		void SimpleSlider_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			gradientRectangle.Fill = (bool)e.NewValue ? (Brush)gradientBrush : (Brush)new SolidColorBrush(Color.FromArgb(255, 72, 72, 72));
		}

		void SimpleSlider_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			adjustSizes();
		}

		private void adjustSizes()
		{
			if (Maximum == Minimum)
				return;
			leftBlackRectangle.Width = ActualWidth * (Value - Minimum) / (Maximum - Minimum);
			rightBlackRectangle.Width = ActualWidth - ActualWidth * (Value - Minimum) / (Maximum - Minimum);
			slider.Margin = new Thickness((ActualWidth - slider.Width) * (Value - Minimum) / (Maximum - Minimum), 0, 0, 0);
		}

		public event EventHandler ValueChanged;

		#region Properties

		public static DependencyProperty LeftEmptyProperty =
			DependencyProperty.Register("LeftEmpty",
			typeof(bool), typeof(SimpleSlider),
			new PropertyMetadata(false, OnLeftEmptyChanged));
		private static void OnLeftEmptyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnLeftEmptyChanged(args);
		}
		void OnLeftEmptyChanged(DependencyPropertyChangedEventArgs args)
		{
			leftBlackRectangle.Visibility = (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
		}
		public bool LeftEmpty
		{
			get { return (bool)GetValue(LeftEmptyProperty); }
			set { SetValue(LeftEmptyProperty, value); }
		}

		public static DependencyProperty RightEmptyProperty =
			DependencyProperty.Register("RightEmpty",
			typeof(bool), typeof(SimpleSlider),
			new PropertyMetadata(true, OnRightEmptyChanged));
		private static void OnRightEmptyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnRightEmptyChanged(args);
		}
		void OnRightEmptyChanged(DependencyPropertyChangedEventArgs args)
		{
			rightBlackRectangle.Visibility = (bool)args.NewValue ? Visibility.Visible : Visibility.Collapsed;
		}
		public bool RightEmpty
		{
			get { return (bool)GetValue(RightEmptyProperty); }
			set { SetValue(RightEmptyProperty, value); }
		}

		public static DependencyProperty EmptyColorProperty =
			DependencyProperty.Register("EmptyColor",
			typeof(Color), typeof(SimpleSlider),
			new PropertyMetadata(Colors.Black, OnEmptyColorChanged));
		static void OnEmptyColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnEmptyColorChanged(args);
		}
		private void OnEmptyColorChanged(DependencyPropertyChangedEventArgs args)
		{
			leftBlackRectangle.Fill = new SolidColorBrush((Color)args.NewValue);
			rightBlackRectangle.Fill = new SolidColorBrush((Color)args.NewValue);
		}
		public Color EmptyColor
		{
			get { return (Color)GetValue(EmptyColorProperty); }
			set { SetValue(EmptyColorProperty, value); }
		}

		public static DependencyProperty ValueProperty =
			DependencyProperty.Register("Value",
			typeof(double), typeof(SimpleSlider),
			new PropertyMetadata(0d, OnValueChanged));
		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnValueChanged(args);
		}
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		private void OnValueChanged(DependencyPropertyChangedEventArgs args)
		{
			double value = (double)args.NewValue;
			if (value < Minimum)
			{
				value = Minimum;
				Value = value;
			}
			if (value > Maximum)
			{
				value = Maximum;
				Value = value;
			}
			adjustSizes();

			if (ValueChanged != null)
				ValueChanged(this, new EventArgs());
		}
	   
		public static DependencyProperty MinimumProperty =
			DependencyProperty.Register("Minimum",
			typeof(double), typeof(SimpleSlider),
			new PropertyMetadata(0d, OnMinimumChanged));
		private static void OnMinimumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnMinimumChanged(args);
		}
		public double Minimum
		{
			get { return (double)GetValue(MinimumProperty); }
			set { SetValue(MinimumProperty, value); }
		}
		private void OnMinimumChanged(DependencyPropertyChangedEventArgs args)
		{
			double minimum = (double)args.NewValue;
			if (minimum > Maximum)
			{
				Maximum = minimum;
				Value = minimum;
			}
			else if (minimum > Value)
				Value = minimum;
			adjustSizes();
		}
		
		public static DependencyProperty MaximumProperty =
			DependencyProperty.Register("Maximum",
			typeof(double), typeof(SimpleSlider),
			new PropertyMetadata(1d, OnMaximumChanged));
		private static void OnMaximumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnMaximumChanged(args);
		}
		public double Maximum
		{
			get { return (double)GetValue(MaximumProperty); }
			set { SetValue(MaximumProperty, value); }
		}
		private void OnMaximumChanged(DependencyPropertyChangedEventArgs args)
		{
			double maximum = (double)args.NewValue;
			if (maximum < Minimum)
			{
				Minimum = maximum;
				Value = maximum;
			}
			else if (maximum < Value)
				Value = maximum;
			adjustSizes();
		}

		public static DependencyProperty SmallChangeProperty =
			DependencyProperty.Register("SmallChange",
			typeof(double), typeof(SimpleSlider),
			new PropertyMetadata(0.05d, OnSmallChangeChanged));
		private static void OnSmallChangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnSmallChangeChanged(args);
		}
		public double SmallChange
		{
			get { return (double)GetValue(SmallChangeProperty); }
			set { SetValue(SmallChangeProperty, value); }
		}
		private void OnSmallChangeChanged(DependencyPropertyChangedEventArgs args)
		{
			//double minimum = (double)args.NewValue;
			//if (minimum > Maximum)
			//{
			//	Maximum = minimum;
			//	Value = minimum;
			//}
			//else if (minimum > Value)
			//	Value = minimum;
			//slider.Margin = new Thickness((ActualWidth - slider.Width) * (Value - minimum) / (Maximum - minimum), 0, 0, 0);
		}

		public static DependencyProperty LargeChangeProperty =
			DependencyProperty.Register("LargeChange",
			typeof(double), typeof(SimpleSlider),
			new PropertyMetadata(0.2d, OnLargeChangeChanged));
		private static void OnLargeChangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnLargeChangeChanged(args);
		}
		public double LargeChange
		{
			get { return (double)GetValue(LargeChangeProperty); }
			set { SetValue(LargeChangeProperty, value); }
		}
		private void OnLargeChangeChanged(DependencyPropertyChangedEventArgs args)
		{
			//double maximum = (double)args.NewValue;
			//if (maximum < Minimum)
			//{
			//	Minimum = maximum;
			//	Value = maximum;
			//}
			//else if (maximum < Value)
			//	Value = maximum;
			//slider.Margin = new Thickness((ActualWidth - slider.Width) * (Value - Minimum) / (maximum - Minimum), 0, 0, 0);
		}

		public static DependencyProperty FillProperty =
			DependencyProperty.Register("Fill",
			typeof(Brush), typeof(SimpleSlider),
			new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255,0x55,0x99,255)), OnFillChanged));
		private static void OnFillChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnFillChanged(args);
		}
		private void OnFillChanged(DependencyPropertyChangedEventArgs args)
		{ 
			gradientRectangle.Fill = (Brush)args.NewValue;
		}

		/// <summary>
		/// The slider path is filled with this brush.
		/// </summary>
		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static DependencyProperty SliderFillProperty =
			DependencyProperty.Register("SliderFill",
			typeof(Brush), typeof(SimpleSlider),
			new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0x33, 0x33, 0x33)), OnSliderFillChanged));
		private static void OnSliderFillChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnSliderFillChanged(args);
		}
		/// <summary>
		/// The little slider itself is filled with this brush.
		/// </summary>
		public Brush SliderFill
		{
			get { return (Brush)GetValue(SliderFillProperty); }
			set { SetValue(SliderFillProperty, value); }
		}
		private void OnSliderFillChanged(DependencyPropertyChangedEventArgs args)
		{
			slider.Fill = (Brush)args.NewValue;
		}

		public static DependencyProperty GradientMarginProperty =
			DependencyProperty.Register("GradientMargin",
			typeof(double), typeof(SimpleSlider),
			new PropertyMetadata(0d, OnGradientMarginChanged));
		private static void OnGradientMarginChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((SimpleSlider)obj).OnGradientMarginChanged(args);
		}
		public double GradientMargin
		{
			get { return (double)GetValue(GradientMarginProperty); }
			set { SetValue(GradientMarginProperty, value); }
		}
		private void OnGradientMarginChanged(DependencyPropertyChangedEventArgs args)
		{
			gradientRectangle.Margin = new Thickness(0, (double)args.NewValue, 0, (double)args.NewValue);
			leftBlackRectangle.Margin = new Thickness(1, (double)args.NewValue - 1, 1, (double)args.NewValue - 1);
			rightBlackRectangle.Margin = new Thickness(1, (double)args.NewValue - 1, 1, (double)args.NewValue - 1);
		}

		#endregion Properties

		#region Touch

		static bool isMouseDown, wasMouseDown;
		static SimpleSlider last;

		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			if (IsEnabled)
			{
				isMouseDown = true;
				last = this;
				OnMouseMove(e);
			}
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			isMouseDown = false;
			wasMouseDown = false;
			last = null;
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
		{
			if (last == this)
				isMouseDown = wasMouseDown;
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
		{
			if (last == this)
				wasMouseDown = isMouseDown;
			isMouseDown = false;
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			if (isMouseDown)
			{
				Value = Minimum + (Maximum - Minimum) * Math.Max(0d, Math.Min(1d, e.GetPosition(touchRectangle).X / (touchRectangle.ActualWidth - slider.ActualWidth)));
			}
		}

		private void Touch_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			OnMouseLeftButtonDown(e);
		}

		#endregion Touch
	}
}
