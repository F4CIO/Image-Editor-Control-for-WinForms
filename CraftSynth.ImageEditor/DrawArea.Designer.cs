namespace CraftSynth.ImageEditor
{
    partial class DrawArea
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

       #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.SuspendLayout();
			// 
			// DrawArea
			// 
			this.BackColor = System.Drawing.Color.White;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Name = "DrawArea";
			this.Size = new System.Drawing.Size(100, 100);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawArea_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseUp);
			this.ResumeLayout(false);

        }

        #endregion


	}
}
