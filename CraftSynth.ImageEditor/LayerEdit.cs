namespace CraftSynth.ImageEditor
{
	public class LayerEdit
	{
		private string _name;
		private bool _visible;
		private bool _active;
		private bool _new;
		private bool _deleted;

		/// <summary>
		/// Layer Name
		/// </summary>
		public string LayerName
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// IsVisible is True if this Layer is visible, else False
		/// </summary>
		public bool LayerVisible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		/// <summary>
		/// IsActive is True if this is the active Layer, else False
		/// </summary>
		public bool LayerActive
		{
			get { return _active; }
			set { _active = value; }
		}

		/// <summary>
		/// True if Layer was added in the dialog
		/// </summary>
		public bool LayerNew
		{
			get { return _new; }
			set { _new = value; }
		}

		/// <summary>
		/// True if Layer was deleted in the dialog
		/// </summary>
		public bool LayerDeleted
		{
			get { return _deleted; }
			set { _deleted = value; }
		}
	}
}