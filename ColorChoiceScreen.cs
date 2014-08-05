using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PThomann.Utilities.PopupScreenSystem.ColorChoice;

namespace PThomann.Utilities.PopupScreenSystem
{
	public class ColorChoiceScreen : ConfigurableScreen
	{
		ColorChoiceElement HSV;
		SliderElement h, s, v, r, g, b;
		bool locked;

		LinearGradientBrush rGrad, gGrad, bGrad, hGrad, sGrad, vGrad;

		public ColorChoiceScreen()
			: base("COLORCHOICE", true)
		{
			SolidColorBrush black = new SolidColorBrush(Colors.Black);
			HSV = new ColorChoiceElement();
			((ColorChooser)HSV.Child).HueChanged += ColorChoiceScreen_HueChanged;
			((ColorChooser)HSV.Child).ColorChanged += ColorChoiceScreen_ColorChanged;
			hGrad = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0.5),
				EndPoint = new Point(1, 0.5)
			};
			hGrad.GradientStops.Add(new GradientStop() { Offset = 0d, Color = Colors.Red });
			hGrad.GradientStops.Add(new GradientStop() { Offset = 1d / 6d, Color = Colors.Yellow });
			hGrad.GradientStops.Add(new GradientStop() { Offset = 1d / 3d, Color = Colors.Green });
			hGrad.GradientStops.Add(new GradientStop() { Offset = 1d / 2d, Color = Colors.Cyan });
			hGrad.GradientStops.Add(new GradientStop() { Offset = 2d / 3d, Color = Colors.Blue });
			hGrad.GradientStops.Add(new GradientStop() { Offset = 5d / 6d, Color = Colors.Magenta });
			hGrad.GradientStops.Add(new GradientStop() { Offset = 1d, Color = Colors.Red });
			h = new SliderElement()
			{
				Label = "Hue",
				ShowHelp = false,
				RightEmpty = false,
				Minimum = 0,
				Maximum = 360,
				LargeChange = 30,
				Fill = hGrad,
				SliderFill = black,
				ValueChangedAction = () =>
				{
					if (!locked)
					{
						locked = true;
						((ColorChooser)HSV.Child).Hue = h.Value;
						locked = false;
					}
				}
			};

			sGrad = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0.5),
				EndPoint = new Point(1, 0.5)
			};
			sGrad.GradientStops.Add(new GradientStop() { Offset = 0d, Color = Colors.White });
			sGrad.GradientStops.Add(new GradientStop() { Offset = 1d, Color = Colors.Red });
			s = new SliderElement()
			{
				Label = "Saturation",
				ShowHelp = false,
				RightEmpty = false,
				Minimum = 0,
				Maximum = 1,
				Fill = sGrad,
				ValueChangedAction = () =>
				{
					if (!locked)
					{
						locked = true;
						((ColorChooser)HSV.Child).HSV = new ColorHSV(h.Value, s.Value, v.Value);
						locked = false;
					}
				}
			};

			vGrad = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0.5),
				EndPoint = new Point(1, 0.5)
			};
			vGrad.GradientStops.Add(new GradientStop() { Offset = 0d, Color = Colors.Black });
			vGrad.GradientStops.Add(new GradientStop() { Offset = 1d, Color = Colors.Red });
			v = new SliderElement()
			{
				Label = "Value",
				ShowHelp = false,
				RightEmpty = false,
				Minimum = 0,
				Maximum = 1,
				Fill = vGrad,
				ValueChangedAction = () =>
				{
					if (!locked)
					{
						locked = true;
						((ColorChooser)HSV.Child).HSV = new ColorHSV(h.Value, s.Value, v.Value);
						locked = false;
					}
				}
			};

			rGrad = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0.5),
				EndPoint = new Point(1, 0.5)
			};
			rGrad.GradientStops.Add(new GradientStop() { Offset = 0d, Color = Colors.Black });
			rGrad.GradientStops.Add(new GradientStop() { Offset = 1d, Color = Colors.Red });
			r = new SliderElement()
			{
				Label = "Red",
				ShowHelp = false,
				RightEmpty = false,
				Minimum = 0,
				Maximum = 255,
				Fill = rGrad,
				ValueChangedAction = () =>
				{
					if (!locked)
					{
						locked = true;
						r.Value = Math.Round(r.Value);
						((ColorChooser)HSV.Child).Color = Color.FromArgb(255, (byte)r.Value, (byte)g.Value, (byte)b.Value);
						locked = false;
					}
				}
			};

			gGrad = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0.5),
				EndPoint = new Point(1, 0.5)
			};
			gGrad.GradientStops.Add(new GradientStop() { Offset = 0d, Color = Colors.Red });
			gGrad.GradientStops.Add(new GradientStop() { Offset = 1d, Color = Colors.Yellow });
			g = new SliderElement()
			{
				Label = "Green",
				ShowHelp = false,
				RightEmpty = false,
				Minimum = 0,
				Maximum = 255,
				Fill = gGrad,
				ValueChangedAction = () =>
				{
					if (!locked)
					{
						locked = true;
						g.Value = Math.Round(g.Value);
						((ColorChooser)HSV.Child).Color = Color.FromArgb(255, (byte)r.Value, (byte)g.Value, (byte)b.Value);
						locked = false;
					}
				}
			};

			bGrad = new LinearGradientBrush()
			{
				StartPoint = new Point(0, 0.5),
				EndPoint = new Point(1, 0.5)
			};
			bGrad.GradientStops.Add(new GradientStop() { Offset = 0d, Color = Colors.Red });
			bGrad.GradientStops.Add(new GradientStop() { Offset = 1d, Color = Colors.Magenta });
			b = new SliderElement()
			{
				Label = "Blue",
				ShowHelp = false,
				Minimum = 0,
				Maximum = 255,
				Fill = bGrad,
				RightEmpty = false,
				ValueChangedAction = () =>
				{
					if (!locked)
					{
						locked = true;
						b.Value = Math.Round(b.Value);
						((ColorChooser)HSV.Child).Color = Color.FromArgb(255, (byte)r.Value, (byte)g.Value, (byte)b.Value);
						locked = false;
					}
				}
			};


			Elements = new ConfigurableScreenElement[] 
			{
				HSV ,
				r,g,b,
				h,s,v
			};

			Buttons = MakeDialogButtons(CustomScreenDialogButtons.OkCancel).ToArray();
			Buttons[0].Content = "Choose";
		}

		void ColorChoiceScreen_HueChanged(object sender, EventArgs e)
		{
			if (!locked)
			{
				locked = true;
				h.Value = ((ColorChooser)HSV.Child).Hue;
				locked = false;
			}
			sGrad.GradientStops[0].Color = ColorHelper.ToRGB(h.Value, 0.01d, v.Value);
			sGrad.GradientStops[1].Color = ColorHelper.ToRGB(h.Value, 1d, v.Value);
			vGrad.GradientStops[0].Color = ColorHelper.ToRGB(h.Value, s.Value, 0.01d);
			vGrad.GradientStops[1].Color = ColorHelper.ToRGB(h.Value, s.Value, 1d);
		}

		void ColorChoiceScreen_ColorChanged(object sender, EventArgs e)
		{
			ColorChooser cc = HSV.Child as ColorChooser;
			Color color = cc.Color;
			locked = true;
			h.Value = cc.Hue;
			s.Value = cc.HSV.S;
			v.Value = cc.HSV.V;
			r.Value = color.R;
			g.Value = color.G;
			b.Value = color.B;
			locked = false;
			sGrad.GradientStops[0].Color = ColorHelper.ToRGB(h.Value, 0.01d, v.Value);
			sGrad.GradientStops[1].Color = ColorHelper.ToRGB(h.Value, 1d, v.Value);
			vGrad.GradientStops[0].Color = ColorHelper.ToRGB(h.Value, s.Value, 0.01d);
			vGrad.GradientStops[1].Color = ColorHelper.ToRGB(h.Value, s.Value, 1d);
			rGrad.GradientStops[0].Color = Color.FromArgb(255, 0, color.G, color.B);
			rGrad.GradientStops[1].Color = Color.FromArgb(255, 255, color.G, color.B);
			gGrad.GradientStops[0].Color = Color.FromArgb(255, color.R, 0, color.B);
			gGrad.GradientStops[1].Color = Color.FromArgb(255, color.R, 255, color.B);
			bGrad.GradientStops[0].Color = Color.FromArgb(255, color.R, color.G, 0);
			bGrad.GradientStops[1].Color = Color.FromArgb(255, color.R, color.G, 255);
		}

		public Color Color
		{
			get { return ((ColorChooser)HSV.Child).Color; }
			set { ((ColorChooser)HSV.Child).Color = value; }
		}
		public ColorHSV ColorHSV
		{
			get { return ((ColorChooser)HSV.Child).HSV; }
			set { ((ColorChooser)HSV.Child).HSV = value; }
		}

		public static void Show(System.Windows.Media.Color color, string title)
		{
			if (!Popups.Show(ShowMode.OnTop, "COLORCHOICE"))
				Popups.Show(new ColorChoiceScreen(), ShowMode.OnTop);
			ColorChoiceScreen s = (ColorChoiceScreen)Popups.Current;
			s.Color = color;
			s.Title = title;
		}
	}
}
