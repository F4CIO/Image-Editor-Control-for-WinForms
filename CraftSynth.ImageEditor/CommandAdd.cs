using System;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Add new object command
	/// </summary>
	internal class CommandAdd : Command, IDisposable
	{
		private DrawObject drawObject;

		// Create this command with DrawObject instance added to the list
		public CommandAdd(DrawObject drawObject) : base()
		{
			// Keep copy of added object
			this.drawObject = drawObject.Clone();
		}

		/// <summary>
		/// Undo last Add command
		/// </summary>
		/// <param name="list">Layers collection</param>
		public override void Undo(Layers list)
		{
			list[list.ActiveLayerIndex].Graphics.DeleteLastAddedObject();
		}

		/// <summary>
		/// Redo last Add command
		/// </summary>
		/// <param name="list">Layers collection</param>
		public override void Redo(Layers list)
		{
			list[list.ActiveLayerIndex].Graphics.UnselectAll();
			list[list.ActiveLayerIndex].Graphics.Add(drawObject);
			
			if (drawObject is DrawImage && (drawObject as DrawImage).IsInitialImage)
			{
				drawObject.Selected = true;
				list[list.ActiveLayerIndex].Graphics.MoveSelectionToBack();
				list[list.ActiveLayerIndex].Graphics.UnselectAll();
			}
		}

		#region Destruction
		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		public override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{

				if (disposing)
				{
					// Free any managed objects here. 
					//
					if (this.drawObject != null)
					{
						this.drawObject.Dispose();
					}
				}

				// Free any unmanaged objects here. 
				//

				this._disposed = true;
			}
		}

		~CommandAdd()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}