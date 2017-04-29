using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Polygon graphic object
	/// </summary>
	//[Serializable]
	public class DrawPolygon : DrawLine
	{
		private ArrayList pointArray; // list of points
		private Cursor handleCursor;

		private const string entryLength = "Length";
		private const string entryPoint = "Point";

		private bool _disposed;


		public DrawPolygon()
		{
			pointArray = new ArrayList();
			
			LoadCursor();
			Initialize();
		}

		#region Destruction
		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here.
					if (this.handleCursor != null)
					{
						this.handleCursor.Dispose();
					}
					this._disposed = true;
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~DrawPolygon()
		{
			 this.Dispose(false);
		}
		#endregion

		public DrawPolygon(int x1, int y1, int x2, int y2, Color lineColor, int lineWidth, DrawingPens.PenType penType, LineCap endCap)
		{
			pointArray = new ArrayList();
			pointArray.Add(new Point(x1, y1));
			pointArray.Add(new Point(x2, y2));
			Color = lineColor;
			PenWidth = lineWidth;
			PenType  = penType;
			EndCap = endCap;

			LoadCursor();
			Initialize();
		}

		/// <summary>
		/// Clone this instance
		/// </summary>
		public override DrawObject Clone()
		{
			DrawPolygon drawPolygon = new DrawPolygon();

			foreach (Point p in pointArray)
			{
				drawPolygon.pointArray.Add(p);
			}

			FillDrawObjectFields(drawPolygon);
			return drawPolygon;
		}

		public override void Draw(Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Pen pen;

			if (DrawPen == null)
			{
				pen = new Pen(Color, PenWidth);
				DrawingPens.SetCurrentPen(ref pen, PenType, EndCap);
			}
			else
				pen = DrawPen.Clone() as Pen;

			// Convert the array of points to a GraphicsPath object so lines are mitered correctly at the intersections
			// (not to mention the object is drawn faster then drawing individual lines)
			Point[] pts = new Point[pointArray.Count];
			for (int i = 0; i < pointArray.Count; i++)
			{
				Point px = (Point)pointArray[i];
				pts[i] = px;
			}
			byte[] types = new byte[pointArray.Count];
			for (int i = 0; i < pointArray.Count; i++)
				types[i] = (byte)PathPointType.Line;
			GraphicsPath gp = new GraphicsPath(pts, types);

			// Rotate the path about it's center if necessary
			if (Rotation != 0)
			{
				RectangleF pathBounds = gp.GetBounds();
				Matrix m = new Matrix();
				m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
				gp.Transform(m);
			}
			g.DrawPath(pen, gp);
			//g.DrawCurve(pen, pts);
			//
			//// For DrawBeziers() to work, the pts array must have a minimum of 4 points.
			//// The pts array may have more than 4 points, but if so, then after the first 4 points, remaining points must be in sets of 3 for the call to work.
			//// The following code will adjust the pts array to properly fit these requirements.
			//int numPoints = pts.Length;
			//if (numPoints - 4 <= 0)
			//{
			//    // Cannot call DrawBeziers() so return, drawing nothing.
			//    gp.Dispose();
			//    pen.Dispose();
			//    return;
			//}
			//while ((numPoints - 4) % 3 != 0 && numPoints - 4 > 0)
			//{
			//    // Chop off the last point from the pts array
			//    numPoints--;
			//    Array.Resize(ref pts, numPoints);
			//}
			//g.DrawBeziers(pen, pts);
			gp.Dispose();
			if (pen != null)
				pen.Dispose();
		}

		public void AddPoint(Point point)
		{
			pointArray.Add(point);
		}

		public override int HandleCount
		{
			get { return pointArray.Count; }
		}

		/// <summary>
		/// Get handle point by 1-based number
		/// </summary>
		/// <param name="handleNumber"></param>
		/// <returns></returns>
		public override Point GetHandle(int handleNumber)
		{
			if (handleNumber < 1)
				handleNumber = 1;

			if (handleNumber > pointArray.Count)
				handleNumber = pointArray.Count;

			return ((Point)pointArray[handleNumber - 1]);
		}

		public override Cursor GetHandleCursor(int handleNumber)
		{
			return handleCursor;
		}

		public override void MoveHandleTo(Point point, int handleNumber)
		{
			if (handleNumber < 1)
				handleNumber = 1;

			if (handleNumber > pointArray.Count)
				handleNumber = pointArray.Count;

			pointArray[handleNumber - 1] = point;
			Dirty = true;
			Invalidate();
		}

		public override void Move(int deltaX, int deltaY)
		{
			int n = pointArray.Count;

			for (int i = 0; i < n; i++)
			{
				Point point;
				point = new Point(((Point)pointArray[i]).X + deltaX, ((Point)pointArray[i]).Y + deltaY);

				pointArray[i] = point;
			}
			Dirty = true;
			Invalidate();
		}

		public override void SaveToStream(SerializationInfo info, int orderNumber, int objectIndex)
		{
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}-{2}",
				              entryLength, orderNumber, objectIndex),
				pointArray.Count);

			int i = 0;
			foreach (Point p in pointArray)
			{
				info.AddValue(
					String.Format(CultureInfo.InvariantCulture,
					              "{0}{1}-{2}-{3}",
					              new object[] {entryPoint, orderNumber, objectIndex, i++}),
					p);
			}
			base.SaveToStream(info, orderNumber, objectIndex);
		}

		public override void LoadFromStream(SerializationInfo info, int orderNumber, int objectIndex)
		{
			int n = info.GetInt32(
				String.Format(CultureInfo.InvariantCulture,
				              "{0}{1}-{2}",
				              entryLength, orderNumber, objectIndex));

			for (int i = 0; i < n; i++)
			{
				Point point;
				point = (Point)info.GetValue(
				               	String.Format(CultureInfo.InvariantCulture,
				               	              "{0}{1}-{2}-{3}",
				               	              new object[] {entryPoint, orderNumber, objectIndex, i}),
				               	typeof (Point));

				pointArray.Add(point);
			}
			base.LoadFromStream(info, orderNumber, objectIndex);
		}

		/// <summary>
		/// Create graphic object used for hit test
		/// </summary>
		protected override void CreateObjects()
		{
			if (AreaPath != null)
				return;

			// Create closed path which contains all polygon vertexes
			AreaPath = new GraphicsPath();

			int x1 = 0, y1 = 0; // previous point

			IEnumerator enumerator = pointArray.GetEnumerator();

			if (enumerator.MoveNext())
			{
				x1 = ((Point)enumerator.Current).X;
				y1 = ((Point)enumerator.Current).Y;
			}

			while (enumerator.MoveNext())
			{
				int x2, y2; // current point
				x2 = ((Point)enumerator.Current).X;
				y2 = ((Point)enumerator.Current).Y;

				AreaPath.AddLine(x1, y1, x2, y2);

				x1 = x2;
				y1 = y2;
			}

			AreaPath.CloseFigure();

			// Create region from the path
			AreaRegion = new Region(AreaPath);
		}

		private void LoadCursor()
		{
			handleCursor = new Cursor(GetType(), "PolyHandle.cur");
		}
	}
}