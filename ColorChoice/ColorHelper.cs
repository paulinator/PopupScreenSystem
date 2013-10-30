using System;
using System.Windows.Media;

namespace PThomann.Utilities.PopupScreenSystem.ColorChoice
{
	///<remarks>
	///This is based on Xiaojie Liu's work, see http://www.codeproject.com/Articles/144060/Silverlight-ColorPicker for the original.
	///
	/// See http://www.codeproject.com/info/cpol10.aspx for license information.
	///
	/// I changed things to suit my needs:
	/// - I removed the HSB part completely.
	/// - I coalesced the HSV_H, HSV_S and HSV_V functions into ToHSV, using my struct ColorHSV as return type.
	/// - I renamed HSV2RGB to ToRGB.
	/// - I provided two overloads of each function, one taking a packed Color/ColorHSV, one taking the individual values separately.
	/// - I changed ToRGB so that all values for H are guaranteed to be valid (not for S and V).
	/// - I moved the class into my namespace.
	/// </remarks>
	/// <summary>
	/// Static Helper for RGB <-> HSV conversions.
	/// Note that converting back and forth can introduce significant error, mostly on the Hue.
	/// </summary>
	public static class ColorHelper
	{
		/// <summary>
		/// Convert an HSV color to RGB.
		/// </summary>
		/// <param name="HSV">HSV Color.</param>
		/// <returns>RGB Color</returns>
		public static Color ToRGB(ColorHSV HSV)
		{
			return ToRGB(HSV.H, HSV.S, HSV.V);
		}
		/// <summary>
		/// Convert an HSV color to RGB.
		/// </summary>
		/// <param name="H">Hue 0..360 degrees</param>
		/// <param name="S">Saturation 0..1</param>
		/// <param name="V">Value 0..1</param>
		/// <returns>RGB Color</returns>
		public static Color ToRGB(double H, double S, double V)
		{
			H = (H % 360) / 60;
			if (H < 0)
			{
				H += 6d;
			}
			//S = Math.Max(0, Math.Min(1, S));
			//V = Math.Max(0, Math.Min(1, V));
			double c = V * S;
			double x = c * (1 - Math.Abs(H % 2 - 1));

			double r = 0d;
			double g = 0d;
			double b = 0d;

			if (H < 1d)
			{
				r = c;
				g = x;
				b = 0;
			}
			else if (H < 2d)
			{
				r = x;
				g = c;
				b = 0;
			}
			else if (H < 3d)
			{
				r = 0;
				g = c;
				b = x;
			}
			else if (H < 4d)
			{
				r = 0;
				g = x;
				b = c;
			}
			else if (H < 5d)
			{
				r = x;
				g = 0;
				b = c;
			}
			else // if (h0 < 6d)
			{
				r = c;
				g = 0;
				b = x;
			}

			double m = V - c;

			r = r + m;
			g = g + m;
			b = b + m;

			return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		/// <summary>
		/// Convert an RGB color to HSV.
		/// </summary>
		/// <param name="color">RGB Color</param>
		/// <returns>HSV Color.</returns>
		public static ColorHSV ToHSV(Color color)
		{
			return ToHSV(color.R / 255d, color.G / 255d, color.B / 255d);
		}
		/// <summary>
		/// Convert an RGB color to HSV.
		/// </summary>
		/// <param name="R">Red 0..1</param>
		/// <param name="G">Green 0..1</param>
		/// <param name="B">Blue 0..1</param>
		/// <returns>HSV Color</returns>
		public static ColorHSV ToHSV(double R, double G, double B)
		{
			double h, s, v;

			//R = Math.Max(0, Math.Min(1, R));
			//G = Math.Max(0, Math.Min(1, G));
			//B = Math.Max(0, Math.Min(1, B));

			double max = Math.Max(R, Math.Max(G, B));
			double min = Math.Min(R, Math.Min(G, B));
			double c = max - min;
			
			h = 0d;
			if (!(R == G && G == B))
			{
				if (R == max)
				{
					h = (G - B) / c;
				}
				else if (G == max)
				{
					h = 2d + ((B - R) / c);
				}
				else if (B == max)
				{
					h = 4d + ((R - G) / c);
				}

				h *= 60d;
				if (h < 0d)
				{
					h += 360d;
				}
			}
			if (max <= 0d)  // for some reason, on my Phone double.Epsilon was NOT larger than 0
				s = 0d;
			else
				s = c / max;

			v = max;

			return new ColorHSV(h, s, v);
		}
	}
}
