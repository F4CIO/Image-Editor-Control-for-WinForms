using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	public partial class LayerDialog : Form
	{
		public List<LayerEdit> layerList = new List<LayerEdit>();

		public LayerDialog(Layers _layers)
		{
			InitializeComponent();
			for (int i = 0; i < _layers.Count; i++)
			{
				LayerEdit le = new LayerEdit();
				le.LayerName = _layers[i].LayerName;
				le.LayerVisible = _layers[i].IsVisible;
				le.LayerActive = _layers[i].IsActive;

				layerList.Add(le);
			}
      SetDataGrid();
		}

	  private void SetDataGrid()
	  {
	    dgvLayers.DataSource = layerList;
	    dgvLayers.Columns[0].HeaderText = "Layer Name";
	    dgvLayers.Columns[1].HeaderText = "Visible";
	    dgvLayers.Columns[2].HeaderText = "Active";
	    dgvLayers.Columns[3].HeaderText = "New";
	    dgvLayers.Columns[4].HeaderText = "Deleted";
	  }

	  private void btnAddLayer_Click(object sender, EventArgs e)
		{
			LayerEdit le = new LayerEdit();
			le.LayerName = "New Layer";
			le.LayerNew = true;
			layerList.Add(le);
			dgvLayers.DataSource = null;
      SetDataGrid();
    }

		private void btnClose_Click(object sender, EventArgs e)
		{
			int active = 0;
			for (int i = 0; i < layerList.Count; i++)
				if (layerList[i].LayerActive)
					active++;
			if (active > 1)
				MessageBox.Show("There can be only one Active layer at a time\nCorrect this by only checking the Active box on one layer.");
			else
				Close();
		}
	}
}