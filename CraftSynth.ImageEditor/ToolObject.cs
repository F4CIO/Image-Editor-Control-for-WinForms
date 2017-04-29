using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Base class for all tools which create new graphic object
	/// </summary>
	internal abstract class ToolObject : Tool
	{
		private Cursor cursor;

		/// <summary>
		/// Tool cursor.
		/// </summary>
		protected Cursor Cursor
		{
			get { return cursor; }
			set { cursor = value; }
		}


		/// <summary>
		/// Left mouse is released.
		/// New object is created and resized.
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.Count > 0)
				drawArea.TheLayers[al].Graphics[0].Normalize();
			//drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;

			drawArea.Capture = false;
			drawArea.Refresh();
		}

		/// <summary>
		/// Add new object to draw area.
		/// Function is called when user left-clicks draw area,
		/// and one of ToolObject-derived tools is active.
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="o"></param>
		protected void AddNewObject(DrawArea drawArea, DrawObject o)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			drawArea.TheLayers[al].Graphics.UnselectAll();

			o.Selected = true;
			o.Dirty = true;
			int objectID = 0;
			// Set the object id now
			for (int i = 0; i < drawArea.TheLayers.Count; i++)
			{
				objectID = +drawArea.TheLayers[i].Graphics.Count;
			}
			objectID++;
			o.ID = objectID;
			drawArea.TheLayers[al].Graphics.Add(o);

			drawArea.Capture = true;
			drawArea.Refresh();
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
					if (this.cursor != null)
					{
						this.cursor.Dispose();
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~ToolObject()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}