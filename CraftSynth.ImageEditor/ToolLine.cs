using System.Drawing;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Line tool
	/// </summary>
	internal class ToolLine : ToolObject
	{
		public ToolLine()
		{
			Cursor = new Cursor(GetType(), "Line.cur");
		}

		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			AddNewObject(drawArea, new DrawLine(p.X, p.Y, p.X + 1, p.Y + 1, drawArea.LineColor, drawArea.LineWidth, drawArea.PenType, drawArea.EndCap));
		}

		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			drawArea.Cursor = Cursor;

			if (e.Button ==
			    MouseButtons.Left)
			{
				Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
				int al = drawArea.TheLayers.ActiveLayerIndex;
				drawArea.TheLayers[al].Graphics[0].MoveHandleTo(point, 2);
				drawArea.Refresh();
			}
		}
	}
}