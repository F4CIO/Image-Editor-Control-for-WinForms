using System;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Base class for all drawing tools
	/// </summary>
	internal abstract class Tool:IDisposable
	{
		/// <summary>
		/// Left nous button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public virtual void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
		}


		/// <summary>
		/// Mouse is moved, left mouse button is pressed or none button is pressed
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public virtual void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
		}


		/// <summary>
		/// Left mouse button is released
		/// </summary>
		/// <param name="drawArea"></param>
		/// <param name="e"></param>
		public virtual void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
		}

		#region Destruction
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);       
		}

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
		}

		~Tool()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}