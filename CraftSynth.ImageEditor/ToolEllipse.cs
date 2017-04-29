using System.Drawing;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Ellipse tool
	/// </summary>
	internal class ToolEllipse : ToolRectangle
	{
		public ToolEllipse()
		{
			Cursor = new Cursor(GetType(), "Ellipse.cur");
		}

		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			AddNewObject(drawArea, new DrawEllipse(p.X, p.Y, 1, 1, drawArea.LineColor, drawArea.FillColor, drawArea.DrawFilled, drawArea.LineWidth, drawArea.PenType, drawArea.EndCap));
			
		}
	}
}