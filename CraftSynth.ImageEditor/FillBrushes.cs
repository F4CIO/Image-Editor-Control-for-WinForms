using System.Drawing;
using System.Drawing.Drawing2D;

namespace CraftSynth.ImageEditor
{
	public class FillBrushes
	{
		#region Enumerations
		public enum BrushType
		{
			Brown,
			Aqua,
			GrayDivot,
			RedDiag,
			ConfettiGreen,
			NoBrush,
			NumberOfBrushes
		} ;
		#endregion Enumerations

		public static Brush SetCurrentBrush(BrushType _bType)
		{
			Brush b = null;
			switch (_bType)
			{
				case BrushType.Aqua:
					b = AquaBrush();
					break;
				case BrushType.Brown:
					b = BrownBrush();
					break;
				case BrushType.ConfettiGreen:
					b = ConfettiBrush();
					break;
				case BrushType.GrayDivot:
					b = GrayDivotBrush();
					break;
				case BrushType.RedDiag:
					b = RedDiagBrush();
					break;
				default:
					break;
			}
			return b;
		}

		private static Brush BrownBrush()
		{
			return new SolidBrush(Color.Brown);
		}

		private static Brush AquaBrush()
		{
			return new SolidBrush(Color.Aqua);
		}

		private static Brush GrayDivotBrush()
		{
			return new HatchBrush(HatchStyle.Divot, Color.Gray, Color.Gainsboro);
		}

		private static Brush RedDiagBrush()
		{
			return new HatchBrush(HatchStyle.ForwardDiagonal, Color.Red, Color.Yellow);
		}

		private static Brush ConfettiBrush()
		{
			return new HatchBrush(HatchStyle.LargeConfetti, Color.Green, Color.White);
		}
	}
}