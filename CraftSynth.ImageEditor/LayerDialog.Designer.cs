namespace CraftSynth.ImageEditor
{
	partial class LayerDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgvLayers = new System.Windows.Forms.DataGridView();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnAddLayer = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgvLayers)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvLayers
			// 
			this.dgvLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvLayers.Location = new System.Drawing.Point(13, 13);
			this.dgvLayers.Name = "dgvLayers";
			this.dgvLayers.Size = new System.Drawing.Size(648, 212);
			this.dgvLayers.TabIndex = 0;
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(586, 231);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnAddLayer
			// 
			this.btnAddLayer.Location = new System.Drawing.Point(13, 231);
			this.btnAddLayer.Name = "btnAddLayer";
			this.btnAddLayer.Size = new System.Drawing.Size(75, 23);
			this.btnAddLayer.TabIndex = 2;
			this.btnAddLayer.Text = "Add Layer";
			this.btnAddLayer.UseVisualStyleBackColor = true;
			this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
			// 
			// LayerDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(668, 266);
			this.Controls.Add(this.btnAddLayer);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.dgvLayers);
			this.Name = "LayerDialog";
			this.Text = "Layers";
			((System.ComponentModel.ISupportInitialize)(this.dgvLayers)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvLayers;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnAddLayer;
	}
}