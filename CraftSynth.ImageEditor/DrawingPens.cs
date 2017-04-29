using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CraftSynth.ImageEditor
{
	public class DrawingPens
	{
		#region Enumerations
		public enum PenType
		{
			Solid,
			Dash,
			Dash_Dot,
			Dot,
			DoubleLine
		} 
		#endregion Enumerations

		public static string GetPenTypeAsString(PenType penType)
		{
			switch (penType)
			{
				case PenType.Solid:
					return "___";
					break;
				case PenType.Dash:
					return "- - -";
					break;
				case PenType.Dash_Dot:
					return "- . -";
					break;
				case PenType.Dot:
					return ". . .";
					break;
				case PenType.DoubleLine:
					return "===";
					break;
				default:
					throw new ArgumentOutOfRangeException("penType");
			}
		}

		/// <summary>
		/// Return a pen based on the type requested
		/// </summary>
		/// <param name="_penType">Type of pen from the PenType enumeration</param>
		/// <returns>Requested pen</returns>
		public static void SetCurrentPen(ref Pen pen, PenType _penType, LineCap endCap)
		{
			switch (_penType)
			{
				case PenType.Solid:
					pen.DashStyle = DashStyle.Solid;
					break;
				case PenType.Dash:
					pen.DashStyle = DashStyle.Dash;
					break;
				case PenType.Dash_Dot:
					pen.DashStyle = DashStyle.DashDot;
					break;
				case PenType.Dot:
					pen.DashStyle = DashStyle.Dot;
					break;
				case PenType.DoubleLine:
					pen.CompoundArray = new float[] {0.0f, 0.1f, 0.2f, 0.3f, 0.7f, 0.8f, 0.9f, 1.0f};
					break;
				default:
					throw new ArgumentOutOfRangeException("_penType");
			}
			pen.LineJoin = LineJoin.Round;
			pen.EndCap = endCap;
			pen.StartCap = LineCap.Round;
		}
	}
}