using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor.Gui
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.imageEditor1.ParentForm = this;
		}

		private void tsmiImport_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK == this.openFileDialog1.ShowDialog(this))
			{
				this.imageEditor1.ReplaceInitialImage(Image.FromFile(this.openFileDialog1.FileName), false, true);
			}
		}

		private void tsmiExport_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK == this.saveFileDialog1.ShowDialog(this))
			{
				string ext = Path.GetExtension(this.saveFileDialog1.FileName).ToLower().TrimStart('.');
				ImageFormat format = ImageFormat.Bmp;
				switch (ext)
				{
					case "jpg": format = ImageFormat.Jpeg; break;
					case "jpeg": format = ImageFormat.Jpeg; break;
					case "png": format = ImageFormat.Png; break;
					case "gif": format = ImageFormat.Gif; break;
				}
				this.imageEditor1.ExportToFile(this.saveFileDialog1.FileName, format);
			}
		}

		private void tsmiExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void tsmiAbout_Click(object sender, EventArgs e)
		{
			Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = "http://www.f4cio.com/Image-Editor-Control-For-WinForms";
			process.Start();
		}
	}
}
