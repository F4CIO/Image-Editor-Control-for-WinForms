using System;
using System.Drawing;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	public partial class TextDialog : Form
	{
		public TextDialog()
		{
			InitializeComponent();
		}

		private string _text;

		public string TheText
		{
			get { return _text; }
			set { _text = value; }
		}

		private Font _font;

		public Font TheFont
		{
			get { return _font; }
			set { _font = value; }
		}

		private Color _color;

		public Color TheColor
		{
			get { return _color; }
			set { _color = value; }
		}

		private float _zoom = 1;

		public float Zoom
		{
			get { return _zoom; }
			set { _zoom = value; }
		}

		private void TextDialog_Load(object sender, EventArgs e)
		{
			this.Height = this.txtTheText.Height + 100;
			this.txtTheText.Font =  new Font(_font.FontFamily, _font.Size*this.Zoom, _font.Style);
			this.txtTheText.ForeColor = _color;
			this.txtTheText.Text = _text;
			this.txtTheText.SelectAll();
			this.Height = this.txtTheText.Height + 100;
		}

		private void btnFont_Click(object sender, EventArgs e)
		{
			dlgFont.Font = _font;
			dlgFont.Color = _color;
			dlgFont.AllowSimulations = true;
			dlgFont.AllowVectorFonts = true;
			dlgFont.AllowVerticalFonts = true;
			dlgFont.MaxSize = 200;
			dlgFont.MinSize = 4;
			dlgFont.ShowApply = false;
			dlgFont.ShowColor = true;
			dlgFont.ShowEffects = true;
			if (dlgFont.ShowDialog() == DialogResult.OK)
			{
				_font = dlgFont.Font;
				_color = dlgFont.Color;
				this.txtTheText.Font =  new Font(_font.FontFamily, _font.Size*this.Zoom, _font.Style);
				txtTheText.ForeColor = _color;
				this.Height = this.txtTheText.Height + 100;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			_text = txtTheText.Text;

		}

		private void TextDialog_ResizeEnd(object sender, EventArgs e)
		{
			this.Height = this.txtTheText.Height + 100;
		}
	}
}