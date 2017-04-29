using System.Drawing;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Pointer tool
	/// </summary>
	internal class ToolPointer : Tool
	{
		private enum SelectionMode
		{
			None,
			NetSelection, // group selection is active
			Move, // object(s) are moves
			Size // object is resized
		}

		private SelectionMode selectMode = SelectionMode.None;

		// Object which is currently resized:
		private DrawObject resizedObject;
		private int resizedObjectHandle;

		// Keep state about last and current point (used to move and resize objects)
		private Point lastPoint = new Point(0, 0);
		private Point startPoint = new Point(0, 0);
		private CommandChangeState commandChangeState;
		private bool wasMove;
		private ToolTip toolTip = new ToolTip();

		/// <summary>
		/// Left mouse button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			commandChangeState = null;
			wasMove = false;

			selectMode = SelectionMode.None;
			Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));

			// Test for resizing (only if control is selected, cursor is on the handle)
			int al = drawArea.TheLayers.ActiveLayerIndex;
			int n = drawArea.TheLayers[al].Graphics.SelectionCount;

			for (int i = 0; i < n; i++)
			{
				DrawObject o = drawArea.TheLayers[al].Graphics.GetSelectedObject(i);
				int handleNumber = o.HitTest(point);

				if (handleNumber > 0)
				{
					selectMode = SelectionMode.Size;
					// keep resized object in class members
					resizedObject = o;
					resizedObjectHandle = handleNumber;
					// Since we want to resize only one object, unselect all other objects
					drawArea.TheLayers[al].Graphics.UnselectAll();
					o.Selected = true;
					commandChangeState = new CommandChangeState(drawArea.TheLayers);
					break;
				}
			}

			// Test for move (cursor is on the object)
			if (selectMode == SelectionMode.None)
			{
				int n1 = drawArea.TheLayers[al].Graphics.Count;
				DrawObject o = null;

				for (int i = 0; i < n1; i++)
				{
					if (drawArea.TheLayers[al].Graphics[i].HitTest(point) == 0)
					{
						o = drawArea.TheLayers[al].Graphics[i];
						break;
					}
				}

				if (o != null)
				{
					selectMode = SelectionMode.Move;

					// Unselect all if Ctrl is not pressed and clicked object is not selected yet
					if ((Control.ModifierKeys & Keys.Control) == 0 &&
						!o.Selected)
						drawArea.TheLayers[al].Graphics.UnselectAll();

					// Select clicked object
					o.Selected = true;
					commandChangeState = new CommandChangeState(drawArea.TheLayers);

					drawArea.Cursor = Cursors.SizeAll;
				}
			}

			// Net selection
			if (selectMode == SelectionMode.None)
			{
				// click on background
				if ((Control.ModifierKeys & Keys.Control) == 0)
					drawArea.TheLayers[al].Graphics.UnselectAll();

				selectMode = SelectionMode.NetSelection;
				drawArea.DrawNetRectangle = true;
			}

			lastPoint.X = point.X;
			lastPoint.Y = point.Y;
			startPoint.X = point.X;
			startPoint.Y = point.Y;

			drawArea.Capture = true;
			drawArea.NetRectangle = DrawRectangle.GetNormalizedRectangle(startPoint, lastPoint);
			drawArea.Refresh();
		}


		/// <summary>
		/// Mouse is moved.
		/// None button is pressed, ot left button is pressed.
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			int al = drawArea.TheLayers.ActiveLayerIndex;
			wasMove = true;
			//toolTip.InitialDelay = 1;

			// set cursor when mouse button is not pressed
			if (e.Button ==
				MouseButtons.None)
			{
				Cursor cursor = null;

				if (drawArea.TheLayers[al].Graphics != null)
				{
					// Hide tooltip in case it was displayed
					//toolTip.Hide(drawArea);
					for (int i = 0; i < drawArea.TheLayers[al].Graphics.Count; i++)
					{
						int n = drawArea.TheLayers[al].Graphics[i].HitTest(point);
						if (n > 0)
						{
							cursor = drawArea.TheLayers[al].Graphics[i].GetHandleCursor(n);
							break;
						}
                        //if (n == 0)
                        //    toolTip.Show(drawArea.TheLayers[al].Graphics[i].TipText, drawArea, point, 250);
					}
				}

				if (cursor == null)
					cursor = Cursors.Default;

				drawArea.Cursor = cursor;
				return;
			}

			if (e.Button !=
				MouseButtons.Left)
				return;

			// Left button is pressed

			// Find difference between previous and current position
			int dx = point.X - lastPoint.X;
			int dy = point.Y - lastPoint.Y;

			lastPoint.X = point.X;
			lastPoint.Y = point.Y;

			// resize
			if (selectMode == SelectionMode.Size)
			{
				if (resizedObject != null)
				{
					resizedObject.MoveHandleTo(point, resizedObjectHandle);
					drawArea.Refresh();
				}
			}

			// move
			if (selectMode == SelectionMode.Move)
			{
				int n = drawArea.TheLayers[al].Graphics.SelectionCount;

				for (int i = 0; i < n; i++)
				{
					drawArea.TheLayers[al].Graphics.GetSelectedObject(i).Move(dx, dy);
				}

				drawArea.Cursor = Cursors.SizeAll;
				drawArea.Refresh();
			}

			if (selectMode == SelectionMode.NetSelection)
			{
				drawArea.NetRectangle = DrawRectangle.GetNormalizedRectangle(startPoint, lastPoint);
				drawArea.Refresh();
				return;
			}
		}

		/// <summary>
		/// Right mouse button is released
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (selectMode == SelectionMode.NetSelection)
			{
				// Group selection
				drawArea.TheLayers[al].Graphics.SelectInRectangle(drawArea.NetRectangle);

				selectMode = SelectionMode.None;
				drawArea.DrawNetRectangle = false;
			}

			if (resizedObject != null)
			{
				// after resizing
				resizedObject.Normalize();
				resizedObject = null;
			}

			drawArea.Capture = false;
			drawArea.Refresh();

			if (commandChangeState != null && wasMove)
			{
				// Keep state after moving/resizing and add command to history
				commandChangeState.NewState(drawArea.TheLayers);
				drawArea.AddCommandToHistory(commandChangeState);
				commandChangeState = null;
			}
			lastPoint = drawArea.BackTrackMouse(e.Location);
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
					if (this.resizedObject != null)
					{
						this.resizedObject.Dispose();
					}
					if (this.commandChangeState != null)
					{
						this.commandChangeState.Dispose();
					}
					if (this.toolTip != null)
					{
						this.toolTip.Dispose();
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~ToolPointer()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}