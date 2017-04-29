using System.Drawing;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Connector tool (a Connector is a series of connected straight lines where each line is drawn individually and at least one of the ends is anchored to another object)
	/// </summary>
	internal class ToolConnector : ToolObject
	{
		public ToolConnector()
		{
			Cursor = new Cursor(GetType(), "Pencil.cur");
		}

		private DrawConnector newConnector;
		private bool _drawingInProcess = false; // Set to true when drawing

		/// <summary>
		/// Left mouse button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				_drawingInProcess = false;
				newConnector = null;
			} else
			{
				Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
				int objectID = -1;
				p = TestForConnection(drawArea, p, out objectID);
				
					if (_drawingInProcess == false)
					{
						newConnector = new DrawConnector(p.X, p.Y, p.X + 1, p.Y + 1, drawArea.LineColor, drawArea.LineWidth, drawArea.PenType, drawArea.EndCap);
						newConnector.EndPoint = new Point(p.X + 1, p.Y + 1);
						if (objectID > -1)
						{
							newConnector.StartIsAnchored = true;
							newConnector.StartObjectId = objectID;
						}
						AddNewObject(drawArea, newConnector);
						_drawingInProcess = true;
					} else
					{
						// Drawing is in process, so simply add a new point
						newConnector.AddPoint(p);
						newConnector.EndPoint = p;
						if (objectID > -1)
						{
							newConnector.EndIsAnchored = true;
							newConnector.EndObjectId = objectID;
							_drawingInProcess = false;
						}
					}
			
			}
		}

		private static Point TestForConnection(DrawArea drawArea, Point p, out int objectID)
		{
			// Determine if within 5 pixels of a connection point
			// Step 1: see if a 5 x 5 rectangle centered on the mouse cursor intersects with an object
			// Step 2: If it does, then see if there is a connection point within the rectangle
			// Step 3: If there is, move the point to the connection point, record the object's id in the connector
			//
			objectID = -1;
			Rectangle testRectangle = new Rectangle(p.X - 2, p.Y - 2, 5, 5);
			int al = drawArea.TheLayers.ActiveLayerIndex;
			bool connectionHere = false;
			Point h = new Point(-1, -1);
			GraphicsList gl = drawArea.TheLayers[al].Graphics;
			for (int i = 1; i < gl.Count; i++)
			{
				if (gl[i].IntersectsWith(testRectangle))
				{
					DrawObject obj = (DrawObject)gl[i];
					for (int j = 1; j < obj.HandleCount + 1; j++)
					{
						h = obj.GetHandle(j);
						if (testRectangle.Contains(h))
						{
							connectionHere = true;
							p = h;
							objectID = obj.ID;
				//			obj.DrawConnection(drawArea., j);
							break;
						}
					}
				}
				if (connectionHere)
					break;
			}
			return p;
		}

		/// <summary>
		/// Mouse move - resize new polygon
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			drawArea.Cursor = Cursor;

			if (e.Button !=
				MouseButtons.Left)
				return;

			if (newConnector == null)
				return; // precaution

			Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			int objectID;
			point = TestForConnection(drawArea, point, out objectID);
			// move last point
			newConnector.MoveHandleTo(point, newConnector.HandleCount);
			drawArea.Refresh();
			if (objectID > -1)
			{
				newConnector.EndIsAnchored = true;
				newConnector.EndObjectId = objectID;
				_drawingInProcess = false;
			}
		}

		#region Destruction
		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
					if (this.newConnector != null)
					{
						this.newConnector.Dispose();
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~ToolConnector()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}