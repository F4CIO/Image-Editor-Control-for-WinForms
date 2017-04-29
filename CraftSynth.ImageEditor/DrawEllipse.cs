using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Ellipse graphic object
	/// </summary>
	[Serializable]
	public class DrawEllipse : DrawRectangle
	{
		public DrawEllipse()
		{
			SetRectangle(0, 0, 1, 1);
			Initialize();
		}

		/// <summary>
		/// Clone this instance
		/// </summary>
		public override DrawObject Clone()
		{
			DrawEllipse drawEllipse = new DrawEllipse();
			drawEllipse.Rectangle = Rectangle;

			FillDrawObjectFields(drawEllipse);
			return drawEllipse;
		}

		public DrawEllipse(int x, int y, int width, int height, Color lineColor, Color fillColor, bool filled, int lineWidth, DrawingPens.PenType penType, LineCap endCap)
		{
			Rectangle = new Rectangle(x, y, width, height);
			Center = new Point(x + (width / 2), y + (height / 2));
			TipText = String.Format("Ellipse Center @ {0}, {1}", Center.X, Center.Y);
			Color = lineColor;
			FillColor = fillColor;
			Filled = filled;
			PenWidth = lineWidth;
			PenType = penType;
			EndCap = endCap;
			Initialize();
		}

		public override void Draw(Graphics g)
		{
			Pen pen;
			Brush b = new SolidBrush(FillColor);

			if (DrawPen == null)
			{
				pen = new Pen(Color, PenWidth);
				DrawingPens.SetCurrentPen(ref pen, PenType, EndCap);
			}
			else
				pen = (Pen) DrawPen.Clone();
			GraphicsPath gp = new GraphicsPath();
			gp.AddEllipse(GetNormalizedRectangle(Rectangle));
			// Rotate the path about it's center if necessary
			if (Rotation != 0)
			{
				RectangleF pathBounds = gp.GetBounds();
				Matrix m = new Matrix();
				m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
				gp.Transform(m);
			}

			if (Filled) g.FillPath(b, gp);
			g.DrawPath(pen, gp);

			gp.Dispose();
			pen.Dispose();
			b.Dispose();
		}
	}
}