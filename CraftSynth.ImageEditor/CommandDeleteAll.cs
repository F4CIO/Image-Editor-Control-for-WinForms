using System.Collections.Generic;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Delete All command
	/// </summary>
	internal class CommandDeleteAll : Command
	{
		private List<DrawObject> cloneList;

		// Create this command BEFORE applying Delete All function.
		public CommandDeleteAll(Layers list)
		{
			cloneList = new List<DrawObject>();

			// Make clone of the whole list.
			// Add objects in reverse order because GraphicsList.Add
			// insert every object to the beginning.
			int n = list[list.ActiveLayerIndex].Graphics.Count;

			for (int i = n - 1; i >= 0; i--)
			{
				cloneList.Add(list[list.ActiveLayerIndex].Graphics[i].Clone());
			}
		}

		public override void Undo(Layers list)
		{
			// Add all objects from clone list to list -
			// opposite to DeleteAll
			foreach (DrawObject o in cloneList)
			{
				list[list.ActiveLayerIndex].Graphics.Add(o);
			}
		}

		public override void Redo(Layers list)
		{
			// Clear list - make DeleteAll again
			list[list.ActiveLayerIndex].Graphics.Clear();
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

		~CommandDeleteAll()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}