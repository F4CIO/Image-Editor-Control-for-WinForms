using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Image graphic object
	/// </summary>
	//[Serializable]
	public class DrawImage : DrawObject
	{
		public Rectangle rectangle;
		private Bitmap _image;
		// this holds the original image unscaled
		private Bitmap _originalImage;
		public bool IsInitialImage;

		public Bitmap TheImage
		{
			get { return _image; }
			set
			{
				_originalImage = value;
				ResizeImage(rectangle.Width, rectangle.Height);
			}
		}

		private const string entryRectangle = "Rect";
		private const string entryImage = "Image";
		private const string entryImageOriginal = "OriginalImage";

		private bool _disposed = false;

		/// <summary>
		/// Clone this instance
		/// </summary>
		public override DrawObject Clone()
		{
			DrawImage drawImage = new DrawImage();
			drawImage._image = _image;
			drawImage._originalImage = _originalImage;
			drawImage.rectangle = rectangle;
			drawImage.IsInitialImage = IsInitialImage;

			FillDrawObjectFields(drawImage);
			return drawImage;
		}

		protected Rectangle Rectangle
		{
			get { return rectangle; }
			set { rectangle = value; }
		}

		#region Destruction
		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
					if (this._originalImage!=null)
					{
						this._originalImage.Dispose(); 
					}
					if (this._image!=null)
					{
						this._image.Dispose(); 
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~DrawImage()
		{
			 this.Dispose(false);
		}
		#endregion

		public DrawImage()
		{
			SetRectangle(0, 0, 1, 1);
			Initialize();
		}

		public DrawImage(int x, int y, bool isInitialImage)
		{
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = 1;
			rectangle.Height = 1;
			this.IsInitialImage = isInitialImage;
			Initialize();
		}

		public DrawImage(int x, int y, Bitmap image)
		{
			rectangle.X = x;
			rectangle.Y = y;
			_image = (Bitmap)image.Clone();
			SetRectangle(rectangle.X, rectangle.Y, image.Width, image.Height);
			Center = new Point(x + (image.Width / 2), y + (image.Height / 2));
			TipText = String.Format("Image Center @ {0}, {1}", Center.X, Center.Y);
			Initialize();
		}

		/// <summary>
		/// Draw image
		/// </summary>
		/// <param name="g"></param>
		public override void Draw(Graphics g)
		{
			// Get existing World transformation
			Matrix mSave = g.Transform;
			if (Rotation != 0)
			{
				Matrix m = mSave.Clone();
				m.RotateAt(Rotation, new PointF(rectangle.Left + (rectangle.Width / 2), rectangle.Top + (rectangle.Height / 2)), MatrixOrder.Append);
				g.Transform = m;
			}
			if (_image == null)
			{
				Pen p = new Pen(Color.Black, -1f);
				g.DrawRectangle(p, rectangle);
			} else
				g.DrawImage(_image, new Point(rectangle.X, rectangle.Y));
			// Restore World transformation
			g.Transform = mSave;
		}

		protected void SetRectangle(int x, int y, int width, int height)
		{
			rectangle.X = x;
			rectangle.Y = y;
			rectangle.Width = width;
			rectangle.Height = height;
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
			switch (handleNumber)
			{
				case 1:
					return Cursors.SizeNWSE;
				case 2:
					return Cursors.SizeNS;
				case 3:
					return Cursors.SizeNESW;
				case 4:
					return Cursors.SizeWE;
				case 5:
					return Cursors.SizeNWSE;
				case 6:
					return Cursors.SizeNS;
				case 7:
					return Cursors.SizeNESW;
				case 8:
					return Cursors.SizeWE;
				default:
					return Cursors.Default;
			}
		}

		/// <summary>
		/// Move handle to new point (resizing)
		/// </summary>
		/// <param name="point"></param>
		/// <param name="handleNumber"></param>
		public override void MoveHandleTo(Point point, int handleNumber)
		{
			int left = Rectangle.Left;
			int top = Rectangle.Top;
			int right = Rectangle.Right;
			int bottom = Rectangle.Bottom;

			switch (handleNumber)
			{
				case 1:
					left = point.X;
					top = point.Y;
					break;
				case 2:
					top = point.Y;
					break;
				case 3:
					right = point.X;
					top = point.Y;
					break;
				case 4:
					right = point.X;
					break;
				case 5:
					right = point.X;
					bottom = point.Y;
					break;
				case 6:
					bottom = point.Y;
					break;
				case 7:
					left = point.X;
					bottom = point.Y;
					break;
				case 8:
					left = point.X;
					break;
			}
			Dirty = true;
			SetRectangle(left, top, right - left, bottom - top);
			ResizeImage(rectangle.Width, rectangle.Height);
		}

		protected void ResizeImage(int width, int height)
		{
			if (_originalImage != null)
			{
				Bitmap b = new Bitmap(_originalImage, new Size(width, height));
				_image = (Bitmap)b.Clone();
				b.Dispose();
			}
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
			base.Dump();

			Trace.WriteLine("rectangle.X = " + rectangle.X.ToString(CultureInfo.InvariantCulture));
			Trace.WriteLine("rectangle.Y = " + rectangle.Y.ToString(CultureInfo.InvariantCulture));
			Trace.WriteLine("rectangle.Width = " + rectangle.Width.ToString(CultureInfo.InvariantCulture));
			Trace.WriteLine("rectangle.Height = " + rectangle.Height.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Normalize rectangle
		/// </summary>
		public override void Normalize()
		{
			rectangle = DrawRectangle.GetNormalizedRectangle(rectangle);
		}

		/// <summary>
		/// Save object to serialization stream
		/// </summary>
		/// <param name="info"></param>
		/// <param name="orderNumber"></param>
		/// <param name="objectIndex"></param>
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
							  entryImage, orderNumber, objectIndex),
				_image);
			info.AddValue(
				String.Format(CultureInfo.InvariantCulture,
							  "{0}{1}-{2}",
							  entryImageOriginal, orderNumber, objectIndex),
				_originalImage);

			base.SaveToStream(info, orderNumber, objectIndex);
		}

		/// <summary>
		/// Load object from serialization stream
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
			_image = (Bitmap)info.GetValue(
								String.Format(CultureInfo.InvariantCulture,
											  "{0}{1}-{2}",
											  entryImage, orderNumber, objectIndex),
								typeof(Bitmap));
			_originalImage = (Bitmap)info.GetValue(
										String.Format(CultureInfo.InvariantCulture,
													  "{0}{1}-{2}",
													  entryImageOriginal, orderNumber, objectIndex),
										typeof(Bitmap));

			base.LoadFromStream(info, orderNumber, objectIndex);
		}

		#region Helper Functions
		public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
		{
			if (x2 < x1)
			{
				int tmp = x2;
				x2 = x1;
				x1 = tmp;
			}

			if (y2 < y1)
			{
				int tmp = y2;
				y2 = y1;
				y1 = tmp;
			}
			return new Rectangle(x1, y1, x2 - x1, y2 - y1);
		}

		public static Rectangle GetNormalizedRectangle(Point p1, Point p2)
		{
			return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
		}

		public static Rectangle GetNormalizedRectangle(Rectangle r)
		{
			return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
		}
		#endregion Helper Functions
	}
}