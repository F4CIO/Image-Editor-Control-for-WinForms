using System.Collections.Generic;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Changing state of existing objects:
	/// move, resize, change properties.
	/// </summary>
	internal class CommandChangeState : Command
	{
		// Selected object(s) before operation
		private List<DrawObject> listBefore;

		// Selected object(s) after operation
		private List<DrawObject> listAfter;

		// Track the active Layer where the change took place
		private int activeLayer;

		// Create this command BEFORE operation.
		public CommandChangeState(Layers layerList)
		{
			// Keep objects state before operation.
			activeLayer = layerList.ActiveLayerIndex;
			FillList(layerList[activeLayer].Graphics, ref listBefore);
		}

		// Call this function AFTER operation.
		public void NewState(Layers layerList)
		{
			// Keep objects state after operation.
			FillList(layerList[activeLayer].Graphics, ref listAfter);
		}

		public override void Undo(Layers list)
		{
			// Replace all objects in the list with objects from listBefore
			ReplaceObjects(list[activeLayer].Graphics, listBefore);
		}

		public override void Redo(Layers list)
		{
			// Replace all objects in the list with objects from listAfter
			ReplaceObjects(list[activeLayer].Graphics, listAfter);
		}

		// Replace objects in graphicsList with objects from list
		private void ReplaceObjects(GraphicsList graphicsList, List<DrawObject> list)
		{
			for (int i = 0; i < graphicsList.Count; i++)
			{
				DrawObject replacement = null;

				foreach (DrawObject o in list)
				{
					if (o.ID ==
					    graphicsList[i].ID)
					{
						replacement = o;
						break;
					}
				}

				if (replacement != null)
				{
					graphicsList.Replace(i, replacement);
				}
			}
		}

		// Fill list from selection
		private void FillList(GraphicsList graphicsList, ref List<DrawObject> listToFill)
		{
			listToFill = new List<DrawObject>();

			foreach (DrawObject o in graphicsList.Selection)
			{
				listToFill.Add(o.Clone());
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
					if (this.listBefore != null)
					{
						foreach (DrawObject drawObject in listBefore)
						{
							if (drawObject != null)
							{
								drawObject.Dispose();
							}
						}
					}

					if (this.listAfter != null)
					{
						foreach (DrawObject drawObject in listAfter)
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

		~CommandChangeState()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}