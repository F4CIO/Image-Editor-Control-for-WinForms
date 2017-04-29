using System.Collections.Generic;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Delete command
	/// </summary>
	internal class CommandDelete : Command
	{
		private List<DrawObject> cloneList; // contains selected items which are deleted

		// Create this command BEFORE applying Delete All function.
		public CommandDelete(Layers list)
		{
			cloneList = new List<DrawObject>();

			// Make clone of the list selection.

			foreach (DrawObject o in list[list.ActiveLayerIndex].Graphics.Selection)
			{
				cloneList.Add(o.Clone());
			}
		}

		public override void Undo(Layers list)
		{
			list[list.ActiveLayerIndex].Graphics.UnselectAll();

			// Add all objects from cloneList to list.
			foreach (DrawObject o in cloneList)
			{
				list[list.ActiveLayerIndex].Graphics.Add(o);
			}
		}

		public override void Redo(Layers list)
		{
			// Delete from list all objects kept in cloneList

			int n = list[list.ActiveLayerIndex].Graphics.Count;

			for (int i = n - 1; i >= 0; i--)
			{
				bool toDelete = false;
				DrawObject objectToDelete = list[list.ActiveLayerIndex].Graphics[i];

				foreach (DrawObject o in cloneList)
				{
					if (objectToDelete.ID ==
					    o.ID)
					{
						toDelete = true;
						break;
					}
				}

				if (toDelete)
				{
					list[list.ActiveLayerIndex].Graphics.RemoveAt(i);
				}
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
					if (this.cloneList != null)
					{
						foreach (DrawObject drawObject in cloneList)
						{
							if (drawObject != null)
							{
								drawObject.Dispose();
							}
						}
					}
				}

				// Free any unmanaged objects here. 
				//

				this._disposed = true;
			}
		}

		~CommandDelete()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}