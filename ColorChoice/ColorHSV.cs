using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PThomann.Utilities.PopupScreenSystem.ColorChoice
{
	public struct ColorHSV
	{
		public double H;
		public double S;
		public double V;

		public ColorHSV(double h, double s, double v)
		{
			H = h;
			S = s;
			V = v;
		}
		public override string ToString()
		{
			return String.Format("({0:0.000},{1:0.000},{2:0.000})", H, S, V);
		}

		public override bool Equals(object obj)
		{
			if (obj is ColorHSV)
			{
				return (H == ((ColorHSV)obj).H && S == ((ColorHSV)obj).S && V == ((ColorHSV)obj).V);
			}
			return false;
		}
		public override int GetHashCode()
		{
			return (byte)(255 * H) << 16 | (byte)(255 * S) << 8 | (byte)(255 * V); 
		}
	}
}
