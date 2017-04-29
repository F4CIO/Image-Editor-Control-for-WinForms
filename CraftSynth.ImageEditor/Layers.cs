using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Collection of <see cref="Layer"/>s used to organize the drawing surface
	/// </summary>
	[Serializable]
	public class Layers : ISerializable, IDisposable
	{
		// Contains the list of Layers
		private ArrayList layerList;

		private bool _isDirty;

		/// <summary>
		/// Dirty is True if any graphic element in any Layer is dirty, else False
		/// </summary>
		public bool Dirty
		{
			get
			{
				if (_isDirty == false)
				{
					foreach (Layer l in layerList)
					{
						if (l.Dirty)
						{
							_isDirty = true;
							break;
						}
					}
				}
				return _isDirty;
			}
			//set
			//{
			//    foreach (Layer l in layerList)
			//        l.Dirty = false;
			//    _isDirty = false;
			//}
		}

		private const string entryCount = "LayerCount";
		private const string entryLayer = "LayerType";

		public Layers()
		{
			layerList = new ArrayList();
		}

		/// <summary>
		/// Returns the index of the Active Layer - only one Layer may be active at any one time
		/// </summary>
		public int ActiveLayerIndex
		{
			get
			{
				int i = 0;
				foreach (Layer l in layerList)
				{
					if (l.IsActive)
						break;
					i++;
				}
				return i;
			}
		}

		protected Layers(SerializationInfo info, StreamingContext context)
		{
			layerList = new ArrayList();

			int n = info.GetInt32(entryCount);

			for (int i = 0; i < n; i++)
			{
				string typeName;
				typeName = info.GetString(
					String.Format(CultureInfo.InvariantCulture,
					              "{0}{1}",
					              entryLayer, i));

				object _layer;
				_layer = Assembly.GetExecutingAssembly().CreateInstance(typeName);
				((Layer)_layer).LoadFromStream(info, i);
				layerList.Add(_layer);
			}
		}

		/// <summary>
		/// Save object to serialization stream
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(entryCount, layerList.Count);

			int i = 0;

			foreach (Layer l in layerList)
			{
				info.AddValue(
					String.Format(CultureInfo.InvariantCulture,
					              "{0}{1}",
					              entryLayer, i),
					l.GetType().FullName);

				l.SaveToStream(info, i);
				i++;
			}
		}

		/// <summary>
		/// Draw all objects contained in all visible layers
		/// </summary>
		/// <param name="g">Graphics object to draw on</param>
		public void Draw(Graphics g)
		{
			foreach (Layer l in layerList)
			{
				if (l.IsVisible)
					l.Draw(g);
			}
		}

		/// <summary>
		/// Clear all objects in the list
		/// </summary>
		/// <returns>
		/// true if at least one object is deleted
		/// </returns>
		public bool Clear()
		{
			bool result = (layerList.Count > 0);
			foreach (Layer l in layerList)
				l.Graphics.Clear();

			if (layerList.Count > 0)
			{
				for (int i = layerList.Count - 1; i >= 0; i--)
				{
					if (layerList[i] != null)
					{
						((DrawObject) layerList[i]).Dispose();
					}
					layerList.RemoveAt(i);
				}
			}
			// Create a default Layer since there must be at least one Layer at all times
			CreateNewLayer("Default");

			// Set dirty flag based on result. Result is true only if at least one item was cleared and since the list is empty, there can be nothing dirty.
			if (result)
				_isDirty = false;
			return result;
		}

		/// <summary>
		/// Returns number layers in the collection - useful for for loops
		/// </summary>
		public int Count
		{
			get { return layerList.Count; }
		}

		/// <summary>
		/// Allows iterating through the list of layers using a for loop
		/// </summary>
		/// <param name="index">the index of the layer to return</param>
		/// <returns>the specified layer object</returns>
		public Layer this[int index]
		{
			get
			{
				if (index < 0 ||
				    index >= layerList.Count)
					return null;
				return (Layer)layerList[index];
			}
		}

		/// <summary>
		/// Adds a new layer to the collection
		/// </summary>
		/// <param name="obj">The layer object to add</param>
		public void Add(Layer obj)
		{
		  layerList.Add(obj);
			// insert to the top of z-order
      //layerList.Insert(0, obj);
		}

		/// <summary>
		/// Create a new layer at the head of the layers list and set it to Active and Visible.
		/// </summary>
		/// <param name="theName">The name to assign to the new layer</param>
		public void CreateNewLayer(string theName)
		{
			// Deactivate the currently active Layer
			if (layerList.Count > 0)
				((Layer)layerList[ActiveLayerIndex]).IsActive = false;
			// Create new Layer, set it visible and active
			Layer l = new Layer();
			l.IsVisible = true;
			l.IsActive = true;
			l.LayerName = theName;
			// Initialize empty GraphicsList for future objects
			l.Graphics = new GraphicsList();
			// Add to Layers collection
			Add(l);
		}

		/// <summary>
		/// Inactivate the active <see cref="Layer"/> by setting all layers to inactive.
		/// Brute force approach
		/// </summary>
		public void InactivateAllLayers()
		{
			foreach (Layer l in layerList)
			{
				l.IsActive = false;
				// Make sure nothing is selected on the currently active layer before switching layers.
				if (l.Graphics != null)
					l.Graphics.UnselectAll();
			}
		}

		/// <summary>
		/// Makes the specified <see cref="Layer"/> invisible
		/// </summary>
		/// <param name="p">index of <see cref="Layer"/> to make invisible</param>
		public void MakeLayerInvisible(int p)
		{
			if (p > -1 &&
			    p < layerList.Count)
				((Layer)layerList[p]).IsVisible = false;
		}

		/// <summary>
		/// Makes the specified <see cref="Layer"/> visible
		/// </summary>
		/// <param name="p">index of <see cref="Layer"/> to make visible</param>
		public void MakeLayerVisible(int p)
		{
			if (p > -1 &&
			    p < layerList.Count)
				((Layer)layerList[p]).IsVisible = true;
		}

		/// <summary>
		/// Changes the active <see cref="Layer"/> to the one indicated
		/// </summary>
		/// <param name="p">index of the <see cref="Layer"/> to activate</param>
		public void SetActiveLayer(int p)
		{
      //// If the current layer is the same as the layer we are switching to, then do nothing.
      //if (ActiveLayerIndex == p)
      //  return;

			// Ensure the index is valid
			if (p > -1 &&
			    p < layerList.Count)
			{
				// Make sure nothing is selected on the currently active layer before switching layers.
        //if (((Layer)layerList[ActiveLayerIndex]).Graphics != null)
        //  ((Layer)layerList[ActiveLayerIndex]).Graphics.UnselectAll();
        //((Layer)layerList[ActiveLayerIndex]).IsActive = false;

				((Layer)layerList[p]).IsActive = true;
				((Layer)layerList[p]).IsVisible = true;
			}
		}

		/// <summary>
		/// Removes the specified <see cref="Layer"/> from the collection - deleting all graphic objects the <see cref="Layer"/> contains
		/// </summary>
		/// <param name="p">index of the <see cref="Layer"/> to remove</param>
		public void RemoveLayer(int p)
		{
			if (ActiveLayerIndex == p)
			{
				MessageBox.Show("Cannot Remove the Active Layer");
				return;
			}
			if (layerList.Count == 1)
			{
				MessageBox.Show("There is only one Layer in this drawing! You Cannot Remove the Only Layer!");
				return;
			}
			// Ensure the index is valid
			if (p > -1 &&
			    p < layerList.Count)
			{
				((Layer)layerList[p]).Graphics.Clear();
				layerList.RemoveAt(p);
			}
		}

		// Public implementation of Dispose pattern callable by consumers. 
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);       
		}

		// Flag: Has Dispose already been called? 
		bool _disposed = false;

		// Protected implementation of Dispose pattern. 
		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{

				if (disposing)
				{
					// Free any managed objects here. 
					//
					if (layerList != null)
					{
						foreach (Layer layer in layerList)
						{
							if (layer != null)
							{
								layer.Dispose();
							}
						}
					}
				}

				// Free any unmanaged objects here. 
				//

				this._disposed = true;
			}
		}

		~Layers()
		{
			 this.Dispose(false);
		}
	}
}