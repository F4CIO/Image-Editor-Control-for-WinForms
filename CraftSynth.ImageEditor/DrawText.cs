using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Rectangle graphic object
	/// </summary>
	//[Serializable]
	public class DrawText : DrawObject
	{
		private Rectangle rectangle;
		private string _theText;
		private Font _font;
		private bool _disposed;


		protected string TheText
		{
			get { return _theText; }
			set
			{
				_theText = value;
				TipText = value;
			}
		}

		public Font TheFont
		{
			get { return _font; }
			set { _font = value; }
		}

		private const string entryRectangle = "Rect";
		private const string entryText = "Text";
		private const string entryFontName = "FontName";
		private const string entryFontBold = "FontBold";
		private const string entryFontItalic = "FontItalic";
		private const string entryFontSize = "FontSize";
		private const string entryFontStrikeout = "FontStrikeout";
		private const string entryFontUnderline = "FontUnderline";


		protected Rectangle Rectangle
		{
			get { return rectangle; }
			set { rectangle = value; }
		}

		public DrawText()
		{
			//SetRectangle(0, 0, 1,1);
			_theText = "";
			Initialize();
		}

		/// <summary>
		/// Clone this instance
		/// </summary>
		public override DrawObject Clone()
		{
			DrawText drawText = new DrawText();

			drawText._font = _font;
			drawText._theText = _theText;
			drawText.rectangle = rectangle;

			FillDrawObjectFields(drawText);
			return drawText;
		}

		#region Destruction
		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
					if (this._font != null)
					{
						this._font.Dispose();
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~DrawText()
		{
			 this.Dispose(false);
		}
		#endregion

		public DrawText(int x, int y, string textToDraw, Font textFont, Color textColor)
		{
			rectangle.X = x;
			rectangle.Y = y;
			_theText = textToDraw;
			_font = textFont;
			Color = textColor;
			Initialize();
		}


		/// <summary>
		/// Draw rectangle
		/// </summary>
		/// <param name="g"></param>
		public override void Draw(Graphics g)
		{

			using (Font biggerFont = new Font(_font.FontFamily, _font.Size + 7, _font.Style))//bad workaround: we want font size to corespont to one showed in text dialog
			{
				Pen pen = new Pen(Color);
				//Brush b = new SolidBrush(Color);
				//g.DrawString(_theText, _font, b, new PointF(Rectangle.X, Rectangle.Y));
				GraphicsPath gp = new GraphicsPath();
				StringFormat format = StringFormat.GenericDefault;
				gp.AddString(_theText, biggerFont.FontFamily, (int) _font.Style, biggerFont.Size,
					new PointF(Rectangle.X, Rectangle.Y), format);
				// Rotate the path about it's center if necessary
				if (Rotation != 0)
				{
					RectangleF pathBounds = gp.GetBounds();
					Matrix m = new Matrix();
					m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width/2), pathBounds.Top + (pathBounds.Height/2)),
						MatrixOrder.Append);
					gp.Transform(m);
				}
				g.DrawPath(pen, gp);
				g.FillPath(new SolidBrush(pen.Color), gp);

				rectangle.Size = g.MeasureString(_theText, biggerFont).ToSize();
				pen.Dispose();
			}
		}

		/// <summary>
		/// Get number of handles
		/// </summary>
		public override int HandleCount
		{
			get { return 8; }
		}


		/// <summary>
		/// Get handle point by 1-based number
		/// </summary>
		/// <param name="handleNumber"></param>
		/// <returns></returns>
		public override Point GetHandle(int handleNumber)
		{
			int x, y, xCenter, yCenter;

			xCenter = rectangle.X + rectangle.Width / 2;
			yCenter = rectangle.Y + rectangle.Height / 2;
			x = rectangle.X;
			y = rectangle.Y;

			switch (handleNumber)
			{
				case 1:
					x = rectangle.X;
					y = rectangle.Y;
					break;
				case 2:
					x = xCenter;
					y = rectangle.Y;
					break;
				case 3:
					x = rectangle.Right;
					y = rectangle.Y;
					break;
				case 4:
					x = rectangle.Right;
					y = yCenter;
					break;
				case 5:
					x = rectangle.Right;
					y = rectangle.Bottom;
					break;
				case 6:
					x = xCenter;
					y = rectangle.Bottom;
					break;
				case 7:
					x = rectangle.X;
					y = rectangle.Bottom;
					break;
				case 8:
					x = rectangle.X;
					y = yCenter;
					break;
			}

			return new Point(x, y);
		}

		/// <summary>
		/// Hit test.
		/// Return value: -1 - no hit
		///                0 - hit anywhere
		///                > 1 - handle number
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override int HitTest(Point point)
		{
			if (Selected)
			{
				for (int i = 1; i <= HandleCount; i++)
				{
					if (GetHandleRectangle(i).Contains(point))
						return i;
				}
			}

			if (PointInObject(point))
				return 0;

			return -1;
		}


		protected override bool PointInObject(Point point)
		{
			return rectangle.Contains(point);
		}

		public override Rectangle GetBounds(Graphics g)
		{
			return rectangle;
		}


		/// <summary>
		/// Get cursor for the handle
		/// </summary>
		/// <param name="handleNumber"></param>
		/// <returns></returns>
		public override Cursor GetHandleCursor(int handleNumber)
		{
			//switch ( handleNumber )
			//{
			//    case 1:
			//        return Cursors.SizeNWSE;
			//    case 2:
			//        return Cursors.SizeNS;
			//    case 3:
			//        return Cursors.SizeNESW;
			//    case 4:
			//        return Cursors.SizeWE;
			//    case 5:
			//        return Cursors.SizeNWSE;
			//    case 6:
			//        return Cursors.SizeNS;
			//    case 7:
			//        return Cursors.SizeNESW;
			//    case 8:
			//        return Cursors.SizeWE;
			//    default:
			return Cursors.Default;
			//}
		}

		/// <summary>
		/// Move handle to new point (resizing)
		/// </summary>
		/// <param name="point"></param>
		/// <param name="handleNumber"></param>
		public override void MoveHandleTo(Point point, int handleNumber)
		{
			//int left = Rectangle.Left;
			//int top = Rectangle.Top;
			//int right = Rectangle.Right;
			//int bottom = Rectangle.Bottom;

			//switch ( handleNumber )
			//{
			//    case 1:
			//        left = point.X;
			//        top = point.Y;
			//        break;
			//    case 2:
			//        top = point.Y;
			//        break;
			//    case 3:
			//        right = point.X;
			//        top = point.Y;
			//        break;
			//    case 4:
			//        right = point.X;
			//        break;
			//    case 5:
			//        right = point.X;
			//        bottom = point.Y;
			//        break;
			//    case 6:
			//        bottom = point.Y;
			//        break;
			//    case 7:
			//        left = point.X;
			//        bottom = point.Y;
			//        break;
			//    case 8:
			//        left = point.X;
			//        break;
			//}

			//SetRectangle(left, top, right - left, bottom - top);
		}


		public override bool IntersectsWith(Rectangle rectangle)
		{
			return Rectangle.IntersectsWith(rectangle);
		}

		/// <summary>
		/// Move object
		/// </summary>
		/// <param name="deltaX"></param>
		/// <param name="deltaY"></param>
		public override void Move(int deltaX, int deltaY)
		{
			rectangle.X += deltaX;
			rectangle.Y += deltaY;
			Dirty = true;
		}

		public override void Dump()
		{
			//base.Dump ();

			//Trace.WriteLine("rectangle.X = " + rectangle.X.ToString(CultureInfo.InvariantCulture));
			//Trace.WriteLine("rectangle.Y = " + rectangle.Y.ToString(CultureInfo.InvariantCulture));
			//Trace.WriteLine("rectangle.Width = " + rectangle.Width.ToString(CultureInfo.InvariantCulture));
			//Trace.WriteLine("rectangle.Height = " + rectangle.Height.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Normalize rectangle
		/// </summary>
		public override void Normalize()
		{
			//rectangle = DrawRectangle.GetNormalizedRectangle(rectangle);
		}

		/// <summary>
		/// Save objevt to serialization stream
		/// </summary>
		/// <param name="info"></param>
		/// <param name="orderNumber">Index of the Layer being saved</param>
		/// <param name="objectIndex">Index of this object in the Layer</param>
		public override void SaveToStream(SerializationInfo info, int orderNumber, int objectIndex)
		{
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryRectangle, orderNumber, objectIndex),
				rectangle);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryText, orderNumber, objectIndex),
				_theText);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontName, orderNumber, objectIndex),
				_font.Name);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontBold, orderNumber, objectIndex),
				_font.Bold);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontItalic, orderNumber, objectIndex),
				_font.Italic);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontSize, orderNumber, objectIndex),
				_font.Size);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontStrikeout, orderNumber, objectIndex),
				_font.Strikeout);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontUnderline, orderNumber, objectIndex),
				_font.Underline);

			base.SaveToStream(info, orderNumber, objectIndex);
		}

		/// <summary>
		/// LOad object from serialization stream
		/// </summary>
		/// <param name="info"></param>
		/// <param name="orderNumber"></param>
		/// <param name="objectIndex"></param>
		public override void LoadFromStream(SerializationInfo info, int orderNumber, int objectIndex)
		{
			rectangle = (Rectangle)info.GetValue(
									String.Format(CultureInfo.InvariantCulture,
												  "{0}{1}-{2}",
												  entryRectangle, orderNumber, objectIndex),
									typeof(Rectangle));
			_theText = info.GetString(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryText, orderNumber, objectIndex));
			string name = info.GetString(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontName, orderNumber, objectIndex));
			bool bold = info.GetBoolean(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontBold, orderNumber, objectIndex));
			bool italic = info.GetBoolean(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontItalic, orderNumber, objectIndex));
			float size = (float)info.GetValue(
									String.Format(CultureInfo.InvariantCulture,
												  "{0}{1}-{2}",
												  entryFontSize, orderNumber, objectIndex),
									typeof(float));
			bool strikeout = info.GetBoolean(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontStrikeout, orderNumber, objectIndex));
			bool underline = info.GetBoolean(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryFontUnderline, orderNumber, objectIndex));
			FontStyle fs = FontStyle.Regular;
			if (bold)
				fs |= FontStyle.Bold;
			if (italic)
				fs |= FontStyle.Italic;
			if (strikeout)
				fs |= FontStyle.Strikeout;
			if (underline)
				fs |= FontStyle.Underline;
			_font = new Font(name, size, fs);

			base.LoadFromStream(info, orderNumber, objectIndex);
		}

		#region Helper Functions
		//public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
		//{
		//if ( x2 < x1 )
		//{
		//    int tmp = x2;
		//    x2 = x1;
		//    x1 = tmp;
		//}

		//if ( y2 < y1 )
		//{
		//    int tmp = y2;
		//    y2 = y1;
		//    y1 = tmp;
		//}

		//return new Rectangle(x1, y1, x2 - x1, y2 - y1);
		//}

		//public static Rectangle GetNormalizedRectangle(Point p1, Point p2)
		//{
		//return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
		//}

		//public static Rectangle GetNormalizedRectangle(Rectangle r)
		//{
		//return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
		//}
		#endregion
	}
}