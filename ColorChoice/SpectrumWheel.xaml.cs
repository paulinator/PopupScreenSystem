using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Petzold.Media3D;

	/// ////////////////////////////////////////////////////////////////////////////////////////////////
	/// It is based on the CircularGradientBrushControl by Charles Petzold.
	/// See http://www.charlespetzold.com/blog/2010/06/A-Circular-Gradient-Brush-for-Silverlight.html
	/// for the original plus some theory behind it.
	/// 
	/// Modifications by Paul Thomann, Feb 2013:
	/// - changed namespace
	/// - in DependencyProperty OnChanged handlers, use args.NewValue instead of re-reading the Property
	/// - removed the Color1 and Color2 Properties entirely from the .cs (commented out).
	/// - replaced the inner clip ellipse by a filled ellipse that is on top of everything, 
	///	 because clipping on WP7 Silverlight works only on the outline. 
	///	 The new property CenterColor is the fill color of that ellipse.
	/// - replaced the two-color gradient by a six-color gradient in the XAML.
	///   (actually seven, red->yellow->green->cyan->blue->magenta->red.)
	/// - changed the angle offset from 145 to 45 so that red points up when Angle == 0.
	////////////////////////////////////////////////////////////////////////////////////////////////////
namespace PThomann.Utilities.PopupScreenSystem.ColorChoice
{
	/// <summary>
	/// This is a circular Spectrum as used in the ColorChooser.
	/// </summary>
	public partial class SpectrumWheel : UserControl
	{
		public static readonly DependencyProperty ThicknessProperty =
			DependencyProperty.Register("Thickness",
				typeof(double),
				typeof(SpectrumWheel),
				new PropertyMetadata(48.0, OnThicknessChanged));

		public static readonly DependencyProperty AngleProperty =
			DependencyProperty.Register("Angle",
				typeof(double),
				typeof(SpectrumWheel),
				new PropertyMetadata(OnAngleChanged));

		public static readonly DependencyProperty CenterColorProperty =
			DependencyProperty.Register("CenterColor",
				typeof(Color),
				typeof(SpectrumWheel),
				new PropertyMetadata(Colors.Black, OnCenterColorChanged));

		TaperTransform taperTransformTop = new TaperTransform();
		TaperTransform taperTransformRight = new TaperTransform();
		TaperTransform taperTransformBottom = new TaperTransform();
		TaperTransform taperTransformLeft = new TaperTransform();

		public SpectrumWheel()
		{
			InitializeComponent();

			SizeChanged += new SizeChangedEventHandler(OnSizeChanged);
		}

		public Color CenterColor
		{
			set { SetValue(CenterColorProperty, value); }
			get { return (Color)GetValue(CenterColorProperty); }
		}

		public double Thickness
		{
			set { SetValue(ThicknessProperty, value); }
			get { return (double)GetValue(ThicknessProperty); }
		}

		public double Angle
		{
			set
			{
				// P.T. introduced value preparation
				value %= 360d;
				if (value < 0)
					value += 360d;
				SetValue(AngleProperty, value); 
			}
			get { return (double)GetValue(AngleProperty); }
		}

		static void OnCenterColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as SpectrumWheel).OnCenterColorChanged(args);
		}
		void OnCenterColorChanged(DependencyPropertyChangedEventArgs args)
		{
			ellipseGeoInner.Fill = new SolidColorBrush((Color)args.NewValue);
		}

		static void OnThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as SpectrumWheel).OnThicknessChanged(args);
		}

		void OnThicknessChanged(DependencyPropertyChangedEventArgs args)
		{
			SetInnerGeometry(new Size(ActualWidth, ActualHeight));
		}

		static void OnAngleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as SpectrumWheel).rotateTransform.Angle = 45d + (double)args.NewValue;
		}

		void OnSizeChanged(object sender, SizeChangedEventArgs args)
		{
			taperTransformTop.TaperSide = TaperSide.Bottom;
			taperTransformTop.TargetHeight = args.NewSize.Height / 2;
			taperTransformTop.TargetWidth = args.NewSize.Width;
			taperTransformTop.TaperCorner = TaperCorner.Both;
			taperTransformTop.TaperFraction = 0.01;
			projectionTop.ProjectionMatrix = taperTransformTop.Matrix3D;

			taperTransformRight.TaperSide = TaperSide.Left;
			taperTransformRight.TargetHeight = args.NewSize.Height;
			taperTransformRight.TargetWidth = args.NewSize.Width / 2;
			taperTransformRight.TaperCorner = TaperCorner.Both;
			taperTransformRight.TaperFraction = 0.01;
			projectionRight.ProjectionMatrix = taperTransformRight.Matrix3D;

			taperTransformBottom.TaperSide = TaperSide.Top;
			taperTransformBottom.TargetHeight = args.NewSize.Height / 2;
			taperTransformBottom.TargetWidth = args.NewSize.Width;
			taperTransformBottom.TaperCorner = TaperCorner.Both;
			taperTransformBottom.TaperFraction = 0.01;
			projectionBottom.ProjectionMatrix = taperTransformBottom.Matrix3D;

			taperTransformLeft.TaperSide = TaperSide.Right;
			taperTransformLeft.TargetHeight = args.NewSize.Height;
			taperTransformLeft.TargetWidth = args.NewSize.Width / 2;
			taperTransformLeft.TaperCorner = TaperCorner.Both;
			taperTransformLeft.TaperFraction = 0.01;
			projectionLeft.ProjectionMatrix = taperTransformLeft.Matrix3D;

			ellipseGeoOuter.Center = new Point(args.NewSize.Width / 2, args.NewSize.Height / 2);
			ellipseGeoOuter.RadiusX = args.NewSize.Width / 2;
			ellipseGeoOuter.RadiusY = args.NewSize.Height / 2;

			// P.T: no longer necessary because ellipseGeoInner has both Alignments on Center:
			//ellipseGeoInner.Center = new Point(args.NewSize.Width / 2, args.NewSize.Height / 2);

			SetInnerGeometry(args.NewSize);
		}

		void SetInnerGeometry(Size size)
		{
			ellipseGeoInner.Width = Math.Max(0, size.Width - 2 * Thickness);
			ellipseGeoInner.Height = Math.Max(0, size.Height - 2 * Thickness);

			// originally:
			//ellipseGeoInner.RadiusX = size.Width / 2 - Thickness;
			//ellipseGeoInner.RadiusY = size.Height / 2 - Thickness;
		}

		/////////////////////////////////////////////////////////////
		// Removed by P.T.
		/////////////////////////////////////////////////////////////
		//public static readonly DependencyProperty Color1Property =
		//	DependencyProperty.Register("Color1",
		//		typeof(Color),
		//		typeof(SpectrumWheel),
		//		new PropertyMetadata(Colors.Blue, OnColorChanged));
		//
		//public static readonly DependencyProperty Color2Property =
		//	DependencyProperty.Register("Color2",
		//		typeof(Color),
		//		typeof(SpectrumWheel),
		//		new PropertyMetadata(Colors.Red, OnColorChanged));
		//
		//public Color Color1
		//{
		//	set { SetValue(Color1Property, value); }
		//	get { return (Color)GetValue(Color1Property); }
		//}
		//
		//public Color Color2
		//{
		//	set { SetValue(Color2Property, value); }
		//	get { return (Color)GetValue(Color2Property); }
		//}
		//static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		//{
		//	(obj as SpectrumWheel).OnColorChanged(args);
		//}
		//
		//void OnColorChanged(DependencyPropertyChangedEventArgs args)
		//{
		//	brushTop.GradientStops[0].Color = Color1;
		//	brushRight.GradientStops[0].Color = Color1;
		//	brushBottom.GradientStops[0].Color = Color1;
		//	brushLeft.GradientStops[0].Color = Color1;
		//
		//	brushTop.GradientStops[1].Color = Color2;
		//	brushRight.GradientStops[1].Color = Color2;
		//	brushBottom.GradientStops[1].Color = Color2;
		//	brushLeft.GradientStops[1].Color = Color2;
		//}
		///////////////////////////////////////////////////////////////
	}
}
