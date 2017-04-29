using System.Drawing;
using System.Windows.Forms;

namespace CraftSynth.ImageEditor
{
	/// <summary>
	/// Rectangle tool
	/// </summary>
	internal class ToolText : ToolObject
	{
		public ToolText()
		{
			Cursor = new Cursor(GetType(), "Rectangle.cur");
		}

		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			TextDialog td = new TextDialog();
			td.TopLevel = true;
			td.TopMost = true;
			td.TheColor = drawArea.LineColor;
			td.TheText = _lastText ?? "";
			td.TheFont = _lastFont ?? new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);
			td.Zoom = drawArea.Zoom;
			td.StartPosition = FormStartPosition.Manual;
			Point pnlLocationOnScreen = drawArea.MyParent.pnlDrawArea.PointToScreen(new Point(0, 0));
			Point pp = e.Location;
			pp = new Point(
				pnlLocationOnScreen.X+pp.X //hit point on screen
				-18-SystemInformation.Border3DSize.Width-SystemInformation.SizingBorderWidth //-text box location
				+drawArea.Left //+scroll amount
				,
				pnlLocationOnScreen.Y+pp.Y //hit point on screen
				-18-SystemInformation.Border3DSize.Height-SystemInformation.SizingBorderWidth -SystemInformation.CaptionHeight //-text box location
				+drawArea.Top //+scroll amount
				);
			td.Location = pp;
			if (td.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(td.Text))
			{
				_lastText = td.TheText;
				_lastFont = td.TheFont;
				string t = td.TheText;
				Color c = td.TheColor;
				Font f = td.TheFont;
				Point p = drawArea.MyParent.PointToClient(td.Location);
				p = new Point(p.X+17-drawArea.Left,p.Y+15-drawArea.Top);
				p = drawArea.BackTrackMouse(p);
				AddNewObject(drawArea, new DrawText(p.X, p.Y, t, f, c));

				int al = drawArea.TheLayers.ActiveLayerIndex;
		        drawArea.AddCommandToHistory(new CommandAdd(drawArea.TheLayers[al].Graphics[0]));

				drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
			}
		}

		private static string _lastText;
		private static Font _lastFont;

		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			drawArea.Cursor = Cursor;
			if (e.Button == MouseButtons.Left)
			{
				Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
				int al = drawArea.TheLayers.ActiveLayerIndex;
				drawArea.TheLayers[al].Graphics[0].MoveHandleTo(point, 5);
				drawArea.Refresh();
			}
		}

		#region Destruction
		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
					if (_lastFont != null)
					{
						_lastFont.Dispose();
						_lastFont = null;
					}
				}

				// Free any unmanaged objects here. 
				
				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~ToolText()
		{
			 this.Dispose(false);
		}
		#endregion
	}
}