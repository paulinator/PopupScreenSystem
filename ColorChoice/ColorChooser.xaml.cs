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
using System.Windows.Input;

namespace PThomann.Utilities.PopupScreenSystem.ColorChoice
{
	/// <summary>
	/// A beautiful control to select a color, HSV style.
	/// (c) Paul Thomann 2013
	/// 
	/// Uses a modified version of Charles Petzold's CircularGradientBrushControl
	/// for the spectrum circle. 
	/// </summary>
	public partial class ColorChooser : UserControl
	{
		#region Properties

		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color",
				typeof(Color),
				typeof(ColorChooser),
				new PropertyMetadata(Colors.Red, OnColorChanged));
		/// <summary>
		/// The currently selected Color, as determined by Hue, Saturation, and Luminosity.
		/// </summary>
		public Color Color
		{
			set { SetValue(ColorProperty, value); }
			get { return (Color)GetValue(ColorProperty); }
		}
		public static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as ColorChooser).OnColorChanged(args);
		}

		public static readonly DependencyProperty HSVProperty =
			DependencyProperty.Register("HSV",
				typeof(ColorHSV),
				typeof(ColorChooser),
				new PropertyMetadata(new ColorHSV(0, 1, 1), OnHSVChanged));
		/// <summary>
		/// The currently selected Color, as determined by Hue, Saturation, and Luminosity.
		/// </summary>
		public ColorHSV HSV
		{
			set { SetValue(HSVProperty, value); }
			get { return (ColorHSV)GetValue(HSVProperty); }
		}
		public static void OnHSVChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as ColorChooser).OnHSVChanged(args);
		}

		public static readonly DependencyProperty HueProperty =
			DependencyProperty.Register("Hue",
				typeof(double),
				typeof(ColorChooser),
				new PropertyMetadata(0d, OnHueChanged));
		/// <summary>
		/// The Hue in degrees (0..360). 
		/// 0/360 is pure Red, 
		/// 300 is Yellow, 
		/// 240 is pure Green, 
		/// 180 is Cyan,
		/// 120 is pure Blue,
		/// 60 is Magenta.
		/// Obviously, changing Hue turns the wheel.
		/// </summary>
		public double Hue
		{
			set
			{
				value %= 360;
				if (value < 0)
					value += 360;
				if (value != Hue)
				{
					SetValue(HueProperty, value);
				}
			}
			get { return (double)GetValue(HueProperty); }
		}
		public static void OnHueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as ColorChooser).OnHueChanged(args);
		}

		public static readonly DependencyProperty SpectrumThicknessProperty =
			DependencyProperty.Register("SpectrumThickness",
				typeof(double),
				typeof(ColorChooser),
				new PropertyMetadata(0d, OnSpectrumThicknessChanged));
		/// <summary>
		/// The Spectrum Thickness in pixels.
		/// </summary>
		public double SpectrumThickness
		{
			set 
			{
				if (value >= 0 && value != SpectrumThickness) 
					SetValue(SpectrumThicknessProperty, value); 
			}
			get { return (double)GetValue(SpectrumThicknessProperty); }
		}
		public static void OnSpectrumThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as ColorChooser).OnSpectrumThicknessChanged(args);
		}

		public static readonly DependencyProperty TriangleOverlapProperty =
			DependencyProperty.Register("TriangleOverlap",
				typeof(double),
				typeof(ColorChooser),
				new PropertyMetadata(0d, OnTriangleOverlapChanged));
		/// <summary>
		/// The NuanceTriangle's overlapping the SpectrumCircle, in pixels.
		/// </summary>
		public double TriangleOverlap
		{
			set { 
				if (value != TriangleOverlap)
					SetValue(TriangleOverlapProperty, value); 
			}
			get { return (double)GetValue(TriangleOverlapProperty); }
		}
		public static void OnTriangleOverlapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as ColorChooser).OnTriangleOverlapChanged(args);
		}

		#endregion Properties

		public event EventHandler ColorChanged;
		public event EventHandler HueChanged;

		public ColorChooser()
		{
			InitializeComponent();

			SizeChanged += ColorChooser_SizeChanged;
		}

		/// <summary>
		/// Returns the complementary Color (obtained by using the same A and inverting R, G, B).
		/// </summary>
		public Color ComplementaryColor(Color c)
		{
			return Color.FromArgb(c.A, (byte)(255 - c.R), (byte)(255 - c.G), (byte)(255 - c.B));
		}

		#region NuanceIndicator circle

		double halfSide, yBottom, sideLength, triaHeight, xCenter, nuanceRadius;

		void updateNuanceIndicatorPosition()
		{
			ColorHSV hsv = HSV;
			Thickness m = new Thickness(
				xCenter - nuanceRadius + ((2 - hsv.S) * hsv.V - 1) * halfSide,
				yBottom - nuanceRadius - hsv.S * hsv.V * triaHeight, 
				0, 
				0);
			NuanceIndicator.Margin = m;
			NuanceIndicator.Stroke = new SolidColorBrush(getBlackOrWhiteForContrast(Color));
		}

		void setNuanceByPosition(Point p)
		{
			double a, b, d;
			a = 0.5773502691896258d * (yBottom - p.Y);   // (yB - p.Y) / sqrt(3)
			b = xCenter + halfSide - p.X - a;
			d = sideLength - b;
			if (d == 0)
				d = 0.00001;
			HSV = new ColorHSV(
				Hue,
				Math.Max(0, Math.Min(1, 2 * a / d)), 
				Math.Max(0, Math.Min(1, d / sideLength))
			);
		}

		Color getBlackOrWhiteForContrast(Color c)
		{
			if (Math.Max(c.R, c.G) > 150)   // we ignore blue here because we want the indicator to be white on blue.
				return Colors.Black;
			else
				return Colors.White;
		}

		#endregion NuanceIndicator circle

		#region Size Changes

		double innerDiameter { 
			get { return SpectrumWheel.Width - 2 * SpectrumWheel.Thickness; } 
		}
		void updateTriangleSize()
		{
			double d = innerDiameter + 2 * TriangleOverlap;
			if (d < 1)
			{
				TriangleOverlap = - (innerDiameter - 1) / 2;
				d = 1;
			}
			if (d > SpectrumWheel.Width)
			{
				TriangleOverlap = (SpectrumWheel.Width - innerDiameter) / 2;
				return;
			}
			NuanceTriangle.Width = d;
			NuanceTriangle.Height = d;

			// pre-calculate values we will need a lot for placing / reading the NuanceIndicator:
			triaHeight = d * 0.75d;
			halfSide = 0.4330127018922193d * d;  // sqrt(3)/4 * 2r = sqrt(3)/2 * r = sqrt(3/4 * r^2) = sqrt(r^2 - (r/2)^2) = half triangle side length
			sideLength = 2 * halfSide;
			yBottom = SpectrumThickness - TriangleOverlap + triaHeight;
			nuanceRadius = NuanceIndicator.Width / 2;
			xCenter = Width / 2;

			// place the NuanceIndicator right now:
			updateNuanceIndicatorPosition();
		}

		void ColorChooser_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double s = Math.Min(Width, Height);
			SpectrumWheel.Width  = s;
			SpectrumWheel.Height = s;
			updateTriangleSize();
		}
		void OnTriangleOverlapChanged(DependencyPropertyChangedEventArgs args)
		{
			updateTriangleSize();
		}
		void OnSpectrumThicknessChanged(DependencyPropertyChangedEventArgs args)
		{
			double st = (double)args.NewValue;
			double halfW = SpectrumWheel.Width / 2;
			if (st < 5)
			{
				SpectrumThickness = 5;
				return;
			}
			else if (st > halfW)
			{
				SpectrumThickness = halfW;
				return;
			}
			SpectrumWheel.Thickness = st;
			HueIndicator.Height = st + 1d;
			updateTriangleSize();
		}

		#endregion Size Changes
		
		#region Color Changes 

		bool color_locked;

		void OnHueChanged(DependencyPropertyChangedEventArgs args)
		{
			if (!color_locked && args.NewValue != args.OldValue)
			{
				double hue = (double)args.NewValue;
				color_locked = true;
				Color = ColorHelper.ToRGB(hue, HSV.S, HSV.V);
				SpectrumWheel.Angle = 360d - hue;
				NuanceTriangle.HueColor = hueToColor(hue);
				EventHandler HueChanged = this.HueChanged;
				if (HueChanged != null)
					HueChanged(this, new EventArgs());
				color_locked = false;
			}
		}

		void OnColorChanged(DependencyPropertyChangedEventArgs args)
		{
			double hue;
			if (!color_locked && args.OldValue != args.NewValue)
			{
				color_locked = true;
				HSV = ColorHelper.ToHSV((Color)args.NewValue);
				hue = HSV.H;
				Hue = hue;
				//Saturation = HSV.S;
				//Luminosity = HSV.V;
				color_locked = false;
			}
			else
				hue = Hue;
			SpectrumWheel.Angle = 360d - hue;   // because we turn the wheel instead of the triangle
			NuanceTriangle.HueColor = hueToColor(hue);
			SpectrumWheel.CenterColor = (Color)args.NewValue;
			EventHandler ColorChanged = this.ColorChanged;
			if (ColorChanged != null)
				ColorChanged(this, new EventArgs());
			updateNuanceIndicatorPosition();
		}

		private void OnHSVChanged(DependencyPropertyChangedEventArgs args)
		{
			if (!color_locked && args.NewValue != args.OldValue)
			{
				color_locked = true;
				Hue = ((ColorHSV)args.NewValue).H;
				Color = ColorHelper.ToRGB((ColorHSV)args.NewValue);
				color_locked = false;
			}
		}

		/// <summary>
		/// Turns an angle (in degrees) into the correct Color on the SpectrumWheel
		/// </summary>
		Color hueToColor(double angle)
		{
			byte r, g, b;
			
			angle %= 360;
			if (angle < 0)
				angle += 360d;

			if (angle >= 300)				   // red -> magenta
			{
				r = 255;
				g = 0;
				b = (byte)((360d - angle) * 255d / 60d);
			}
			else if (angle >= 240)   //magenta ->  blue
			{
				r = (byte)((angle - 240d) * 255d / 60d);
				g = 0;
				b = 255;
			}
			else if (angle >= 180d) // blue -> cyan
			{
				r = 0;
				g = (byte)((240d - angle) * 255d / 60d);
				b = 255;
			}
			else if (angle >= 120)  //  cyan -> green
			{
				r = 0;
				g = 255;
				b = (byte)((angle - 120d) * 255d / 60d);
			}
			else if (angle >= 60) //  green -> yellow
			{
				r = (byte)((120d - angle) * 255d / 60d);
				g = 255;
				b = 0;
			}
			else // yellow -> red
			{
				r = 255;
				g = (byte)((angle) * 255d / 60d);
				b = 0;
			}
			return Color.FromArgb(255, r, g, b); ;
		}

		#endregion Color Changes

		#region Mouse / Touch Activity

		double mouseStartAngle, mouseCurrentAngle, spectrumStartAngle;
		bool mouseIsDownOnSpectrum, mouseIsDownOnNuance;

		private void Spectrum_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			disableScrollViewers(true);
			mouseIsDownOnSpectrum = true;
			mouseIsDownOnNuance = false;
			mouseStartAngle = getSpectrumRelativeAngle(e);
			spectrumStartAngle = SpectrumWheel.Angle;
			Touch.FrameReported += Touch_FrameReported;
		}

		private void Nuance_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			disableScrollViewers(true);
			mouseIsDownOnNuance = true;
			mouseIsDownOnSpectrum = false;
			Touch.FrameReported += Touch_FrameReported;
		}

		void Touch_FrameReported(object sender, TouchFrameEventArgs e)
		{
			foreach (TouchPoint tp in e.GetTouchPoints(Application.Current.RootVisual))
				if (tp.Action != TouchAction.Up)
					return;
			Touch.FrameReported -= Touch_FrameReported;
			disableScrollViewers(false);
			mouseIsDownOnNuance = false;
			mouseIsDownOnSpectrum = false;
		}

		byte event_counter;
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (0 != event_counter++ % 3)   // skip every 3rd event for performance
			{
				if (mouseIsDownOnSpectrum)
				{
					mouseCurrentAngle = getSpectrumRelativeAngle(e);
					Hue = 360 - spectrumStartAngle - mouseCurrentAngle + mouseStartAngle;
				}
				else if (mouseIsDownOnNuance)
				{
					setNuanceByPosition(e.GetPosition(SpectrumWheel));
				}
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			if (mouseIsDownOnNuance || mouseIsDownOnSpectrum)
			{
				if (!scrollersDisabled)
				{
					scrollersDisabled = true;
					foreach (ScrollViewer s in scrollersStates.Keys)
					{
						s.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
						s.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
					}
				}
			}
		}
		
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (mouseIsDownOnNuance || mouseIsDownOnSpectrum)
			{
				if (scrollersDisabled)
				{
					scrollersDisabled = false;
					foreach (ScrollViewer s in scrollersStates.Keys)
					{
						s.HorizontalScrollBarVisibility = scrollersStates[s].Horizontal;
						s.VerticalScrollBarVisibility = scrollersStates[s].Vertical;
					}
				}
			}
		}

		private double getSpectrumRelativeAngle(MouseEventArgs e)
		{
			Point mp = e.GetPosition(CenterPoint);
			return Math.Atan2(mp.Y, mp.X) * 57.29577951308232d; // * 360 / (2 * PI)
		}

		#endregion Mouse / Touch Activity

		#region ScrollViewer disabling

		struct ScrollVisibilities
		{
			public ScrollBarVisibility Horizontal;
			public ScrollBarVisibility Vertical;
		}
		Dictionary<ScrollViewer, ScrollVisibilities> scrollersStates = new Dictionary<ScrollViewer, ScrollVisibilities>();
		bool scrollersDisabled;
		void disableScrollViewers(bool disable)
		{
			if (scrollersDisabled == disable)   // can't disable if disabled or enable if enabled
				return;
			scrollersDisabled = disable;

			if (disable)
			{
				scrollersStates.Clear();	// do this here also to make absolutely sure we have a known, empty state of the dictionary
				DependencyObject dpo = Parent;
				while (dpo is FrameworkElement)
				{
					if (dpo is ScrollViewer)
					{
						ScrollViewer s = dpo as ScrollViewer;
						ScrollVisibilities v = new ScrollVisibilities()
						{
							Horizontal = s.HorizontalScrollBarVisibility,
							Vertical = s.VerticalScrollBarVisibility
						};
						scrollersStates.Add(s, v);
						s.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
						s.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
					}
					dpo = ((FrameworkElement)dpo).Parent;
				}
			}
			else // restore
			{
				foreach (ScrollViewer s in scrollersStates.Keys)
				{
					s.HorizontalScrollBarVisibility = scrollersStates[s].Horizontal;
					s.VerticalScrollBarVisibility = scrollersStates[s].Vertical;
				}
				scrollersStates.Clear();
			}
		}

		#endregion ScrollViewer disabling
	}
}
