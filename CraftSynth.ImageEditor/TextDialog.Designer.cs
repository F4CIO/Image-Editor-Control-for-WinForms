namespace CraftSynth.ImageEditor
{
	partial class TextDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextDialog));
			this.txtTheText = new System.Windows.Forms.TextBox();
			this.dlgFont = new System.Windows.Forms.FontDialog();
			this.btnFont = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtTheText
			// 
			this.txtTheText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtTheText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
			this.txtTheText.Location = new System.Drawing.Point(12, 12);
			this.txtTheText.Margin = new System.Windows.Forms.Padding(3, 3, 130, 3);
			this.txtTheText.Name = "txtTheText";
			this.txtTheText.Size = new System.Drawing.Size(352, 20);
			this.txtTheText.TabIndex = 1;
			this.txtTheText.WordWrap = false;
			// 
			// btnFont
			// 
			this.btnFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFont.Image = ((System.Drawing.Image)(resources.GetObject("btnFont.Image")));
			this.btnFont.Location = new System.Drawing.Point(12, 44);
			this.btnFont.Name = "btnFont";
			this.btnFont.Size = new System.Drawing.Size(113, 25);
			this.btnFont.TabIndex = 2;
			this.btnFont.Text = "Text Font";
			this.btnFont.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnFont.UseVisualStyleBackColor = true;
			this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(131, 46);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(114, 23);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(251, 46);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(113, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// TextDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(376, 81);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnFont);
			this.Controls.Add(this.txtTheText);
			this.MinimumSize = new System.Drawing.Size(392, 120);
			this.Name = "TextDialog";
			this.ShowInTaskbar = false;
			this.Text = "Text";
			this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
			this.Load += new System.EventHandler(this.TextDialog_Load);
			this.ResizeEnd += new System.EventHandler(this.TextDialog_ResizeEnd);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtTheText;
		private System.Windows.Forms.Button btnFont;
		private System.Windows.Forms.FontDialog dlgFont;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}