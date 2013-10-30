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
	public partial class NuanceTriangle : UserControl
	{

		public static readonly DependencyProperty HueProperty =
			DependencyProperty.Register("Hue",
				typeof(Color),
				typeof(NuanceTriangle),
				new PropertyMetadata(Colors.Black, OnHueChanged));
		public Color HueColor
		{
			set { SetValue(HueProperty, value); }
			get { return (Color)GetValue(HueProperty); }
		}

		public NuanceTriangle()
		{
			InitializeComponent();

			SizeChanged += NuanceTriangle_SizeChanged;
		}

		public static void OnHueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as NuanceTriangle).OnHueChanged(args);
		}
		void OnHueChanged(DependencyPropertyChangedEventArgs args)
		{
			if (HueGradient == null)
				return;
			Color hue = (Color)args.NewValue;
			HueGradient.GradientStops[0].Color = hue;
			hue.A = 0xEE;
			HueGradient.GradientStops[1].Color = hue;
			hue.A = 0xDD;
			HueGradient.GradientStops[2].Color = hue;
			hue.A = 0xBB;
			HueGradient.GradientStops[3].Color = hue;
			hue.A = 0x99;
			HueGradient.GradientStops[4].Color = hue;
			hue.A = 0x66;
			HueGradient.GradientStops[5].Color = hue;
			hue.A = 0x44;
			HueGradient.GradientStops[6].Color = hue;
			hue.A = 0x22;
			HueGradient.GradientStops[7].Color = hue;
		}

		void NuanceTriangle_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			double side   = Math.Min(Width, Height),
				   halfR  = side / 4d,
				   triH   = halfR * 3d,
				   triW   = side * 7d / 8d,
				   center = triW / 2d,
				   bottom = halfR + (Height - side) / 2d;

			BackgroundTriangle.Points[0] = new Point(0d, triH);
			HueTriangle.Points[0]		= new Point(0d, triH);
			BlackTriangle.Points[0]	  = new Point(0d, triH);
			WhiteTriangle.Points[0]	  = new Point(0d, triH);

			BackgroundTriangle.Points[1] = new Point(center, 0d);
			HueTriangle.Points[1]		= new Point(center, 0d);
			BlackTriangle.Points[1]	  = new Point(center, 0d);
			WhiteTriangle.Points[1]	  = new Point(center, 0d);

			BackgroundTriangle.Points[2] = new Point(triW, triH);
			HueTriangle.Points[2]		= new Point(triW, triH);
			BlackTriangle.Points[2]	  = new Point(triW, triH);
			WhiteTriangle.Points[2]	  = new Point(triW, triH);

			BackgroundTriangle.Margin	= new Thickness(0, 0, 0, bottom);
			HueTriangle.Margin		   = new Thickness(0, 0, 0, bottom);
			BlackTriangle.Margin		 = new Thickness(0, 0, 0, bottom);
			WhiteTriangle.Margin		 = new Thickness(0, 0, 0, bottom);
		}
	}
}
