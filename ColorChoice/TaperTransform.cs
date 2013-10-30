// TaperTransform.cs (c) Charles Petzold, 2009

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Petzold.Media3D
{
	public enum TaperSide { Left, Right, Top, Bottom };
	public enum TaperCorner { LeftOrTop, RightOrBottom, Both };

	public class TaperTransform : DependencyObject
	{
		public static readonly DependencyProperty TargetWidthProperty =
			DependencyProperty.Register("TargetWidth",
				typeof(double),
				typeof(TaperTransform),
				new PropertyMetadata(OnTaperPropertyChanged));

		public static readonly DependencyProperty TargetHeightProperty =
			DependencyProperty.Register("TargetHeight",
				typeof(double),
				typeof(TaperTransform),
				new PropertyMetadata(OnTaperPropertyChanged));

		public static readonly DependencyProperty TaperFractionProperty =
			DependencyProperty.Register("TaperFraction",
				typeof(double),
				typeof(TaperTransform),
				new PropertyMetadata(1.0, OnTaperPropertyChanged));

		public static readonly DependencyProperty TaperSideProperty =
			DependencyProperty.Register("TaperSide",
				typeof(TaperSide),
				typeof(TaperTransform),
				new PropertyMetadata(TaperSide.Right, OnTaperPropertyChanged));

		public static readonly DependencyProperty TaperCornerProperty =
			DependencyProperty.Register("TaperCorner",
				typeof(TaperCorner),
				typeof(TaperTransform),
				new PropertyMetadata(TaperCorner.RightOrBottom, OnTaperPropertyChanged));

		public static readonly DependencyProperty Matrix3DProperty =
			DependencyProperty.Register("Matrix3D",
				typeof(Matrix3D),
				typeof(TaperTransform),
				new PropertyMetadata(Matrix3D.Identity));
		public event EventHandler Matrix3DChanged;

		public double TargetWidth
		{
			set { SetValue(TargetWidthProperty, value); }
			get { return (double)GetValue(TargetWidthProperty); }
		}

		public double TargetHeight
		{
			set { SetValue(TargetHeightProperty, value); }
			get { return (double)GetValue(TargetHeightProperty); }
		}

		public double TaperFraction
		{
			set { SetValue(TaperFractionProperty, value); }
			get { return (double)GetValue(TaperFractionProperty); }
		}

		public TaperSide TaperSide
		{
			set { SetValue(TaperSideProperty, value); }
			get { return (TaperSide)GetValue(TaperSideProperty); }
		}

		public TaperCorner TaperCorner
		{
			set { SetValue(TaperCornerProperty, value); }
			get { return (TaperCorner)GetValue(TaperCornerProperty); }
		}

		public Matrix3D Matrix3D
		{
			protected set { SetValue(Matrix3DProperty, value); }
			get { return (Matrix3D)GetValue(Matrix3DProperty); }
		}

		static void OnTaperPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as TaperTransform).OnTaperPropertyChanged(args);
		}

		void OnTaperPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			Matrix3D matx = new Matrix3D();
			switch (TaperSide)
			{
				case TaperSide.Left:
					matx.M11 = TaperFraction;
					matx.M22 = TaperFraction;
					matx.M14 = (TaperFraction - 1) / TargetWidth;

					switch (TaperCorner)
					{
						case TaperCorner.RightOrBottom:
							break;

						case TaperCorner.LeftOrTop:
							matx.M12 = TargetHeight * matx.M14;
							matx.OffsetY = TargetHeight * (1 - TaperFraction);
							break;

						case TaperCorner.Both:
							matx.M12 = (TargetHeight / 2) * matx.M14;
							matx.OffsetY = TargetHeight * (1 - TaperFraction) / 2;
							break;
					}

					break;

				case TaperSide.Top:
					matx.M11 = TaperFraction;
					matx.M22 = TaperFraction;
					matx.M24 = (TaperFraction - 1) / TargetHeight;

					switch (TaperCorner)
					{
						case TaperCorner.RightOrBottom:
							break;

						case TaperCorner.LeftOrTop:
							matx.M21 = TargetWidth * matx.M24;
							matx.OffsetX = TargetWidth * (1 - TaperFraction);
							break;

						case TaperCorner.Both:
							matx.M21 = (TargetWidth / 2) * matx.M24;
							matx.OffsetX = TargetWidth * (1 - TaperFraction) / 2;
							break;
					}
					break;

				case TaperSide.Right:
					matx.M11 = 1 / TaperFraction;
					matx.M14 = (1 - TaperFraction) / (TargetWidth * TaperFraction);

					switch (TaperCorner)
					{
						case TaperCorner.RightOrBottom:
							break;

						case TaperCorner.LeftOrTop:
							matx.M12 = TargetHeight * matx.M14;
							break;

						case TaperCorner.Both:
							matx.M12 = (TargetHeight / 2) * matx.M14;
							break;
					}

					break;

				case TaperSide.Bottom:
					matx.M22 = 1 / TaperFraction;
					matx.M24 = (1 - TaperFraction) / (TargetHeight * TaperFraction);

					switch (TaperCorner)
					{
						case TaperCorner.RightOrBottom:
							break;

						case TaperCorner.LeftOrTop:
							matx.M21 = TargetWidth * matx.M24;
							break;

						case TaperCorner.Both:
							matx.M21 = (TargetWidth / 2) * matx.M24;
							break;
					}
					break;
			}
			
			Matrix3D = matx;

			if (Matrix3DChanged != null)
				Matrix3DChanged(this, EventArgs.Empty);
		}
	}
}
