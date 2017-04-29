using System;
using System.Collections.Generic;

/// Undo-Redo code is written using the article:
/// http://www.codeproject.com/cs/design/commandpatterndemo.asp
//  The Command Pattern and MVC Architecture
//  By David Veeneman.
namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Class is responsible for executing Undo - Redo operations
	/// </summary>
	internal class UndoManager:IDisposable
	{
		#region Class Members
		private Layers layers;

		private List<Command> historyList;
		private int nextUndo;
		#endregion  Class Members

		#region Constructor
		public UndoManager(Layers layerList)
		{
			layers = layerList;

			ClearHistory();
		}
		#endregion Constructor

		#region Destruction
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);       
		}

		private bool _disposed = false;

		protected void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
					if (this.historyList != null)
					{
						foreach (Command command in this.historyList)
						{
							if (command != null)
							{
								command.Dispose();
							}
						}
					}
					if (this.layers != null)
					{
						for (int i = 0; i < this.layers.Count; i++)
						{
							if (this.layers[i] != null)
							{
								this.layers[i].Dispose();
							}
						}
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
		}

		~UndoManager()
		{
			 this.Dispose(false);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Return true if Undo operation is available
		/// </summary>
		public bool CanUndo
		{
			get
			{
				// If the NextUndo pointer is -1, no commands to undo
				if (nextUndo < 0 ||
					nextUndo > historyList.Count - 1) // precaution
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Return true if Redo operation is available
		/// </summary>
		public bool CanRedo
		{
			get
			{
				// If the NextUndo pointer points to the last item, no commands to redo
				if (nextUndo == historyList.Count - 1)
				{
					return false;
				}

				return true;
			}
		}
		#endregion Properties

		#region Public Functions
		/// <summary>
		/// Clear History
		/// </summary>
		public void ClearHistory()
		{
			if (this.historyList != null)
			{
				foreach (Command command in historyList)
				{
					if (command != null)
					{
						command.Dispose();
					}
				}
			}
			historyList = new List<Command>();
			nextUndo = -1;
		}

		/// <summary>
		/// Add new command to history.
		/// Called by client after executing some action.
		/// </summary>
		/// <param name="command"></param>
		public void AddCommandToHistory(Command command)
		{
			// Purge history list
			TrimHistoryList();

			// Add command and increment undo counter
			historyList.Add(command);

			nextUndo++;
		}

		/// <summary>
		/// Undo
		/// </summary>
		public void Undo()
		{
			if (!CanUndo)
			{
				return;
			}

			// Get the Command object to be undone
			Command command = historyList[nextUndo];

			// Execute the Command object's undo method
			command.Undo(layers);

			// Move the pointer up one item
			nextUndo--;
		}

		/// <summary>
		/// Redo
		/// </summary>
		public void Redo()
		{
			if (!CanRedo)
			{
				return;
			}

			// Get the Command object to redo
			int itemToRedo = nextUndo + 1;
			Command command = historyList[itemToRedo];

			// Execute the Command object
			command.Redo(layers);

			// Move the undo pointer down one item
			nextUndo++;
		}
		#endregion Public Functions

		#region Private Functions
		private void TrimHistoryList()
		{
			// We can redo any undone command until we execute a new 
			// command. The new command takes us off in a new direction,
			// which means we can no longer redo previously undone actions. 
			// So, we purge all undone commands from the history list.*/

			// Exit if no items in History list
			if (historyList.Count == 0)
			{
				return;
			}

			// Exit if NextUndo points to last item on the list
			if (nextUndo == historyList.Count - 1)
			{
				return;
			}

			// Purge all items below the NextUndo pointer
			for (int i = historyList.Count - 1; i > nextUndo; i--)
			{
				historyList.RemoveAt(i);
			}
		}
		#endregion
	}
}