using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;
using System.Windows.Forms;

using DocToolkit;

namespace CraftSynth.ImageEditor
{
	public partial class MainForm : UserControl
	{
		#region Members
		private DrawArea drawArea;
		private DocManager docManager;
		private DragDropManager dragDropManager;
		private MruManager mruManager;

		private string argumentFile = ""; // file name from command line

		//private const string registryPath = "Software\\AlexF\\DrawTools";

		private bool _controlKey = false;
		private bool _panMode = false;
		#endregion

		#region Properties

		public Form ParentForm { get; set; }
		/// <summary>
		/// File name from the command line
		/// </summary>
		public string ArgumentFile
		{
			get { return argumentFile; }
			set { argumentFile = value; }
		}

		/// <summary>
		/// Get reference to Edit menu item.
		/// Used to show context menu in DrawArea class.
		/// </summary>
		/// <value></value>
		public ToolStripMenuItem ContextParent
		{
			get { return editToolStripMenuItem; }
		}

		public Image InitialImage { get; set; }
		public string InitialImageAsFilePath{get; set;}
		public byte[] InitialImageAsPngBytes{get; set;}

		public Image Image
		{
			get
			{
				Bitmap b = new Bitmap(drawArea.Width, drawArea.Height);
				using (Graphics g = Graphics.FromImage(b))
				{
					g.Clear(Color.White);
					drawArea.TheLayers.Draw(g);
				}
				//b.Save(@"c:\test.bmp", ImageFormat.Bmp);
				return b;
			}
		}

		private bool _zoomOnMouseWheel = false;
		public bool ZoomOnMouseWheel
		{
			get
			{
				return _zoomOnMouseWheel;
			}
			set
			{
				_zoomOnMouseWheel = value;
			}
		}

		#endregion

		#region Constructor
		public MainForm()
		{
			InitializeComponent();
			MouseWheel += new MouseEventHandler(MainForm_MouseWheel);
			if (this._zoomOnMouseWheel)
			{
				this.pnlDrawArea.MouseWheel += new MouseEventHandler(MainForm_MouseWheel);
			}
		}

		public void Initialize(Form parentForm)
		{
			this.ParentForm = parentForm;
		}

		#endregion

		#region Destructor
		// Flag: Has Dispose already been called? 
		private volatile bool _disposed = false;
		private volatile bool _disposingOrDisposed = false;

		// Protected implementation of Dispose pattern. 
		protected override void Dispose(bool disposing)
		{
			this._disposingOrDisposed = true;
			if (!this._disposed)
			{
				if (disposing)
				{
					// Free any managed objects here. 
					//
					if (this.InitialImage != null)
					{
						this.InitialImage.Dispose();
					}
					if (this.drawArea != null)
					{
						this.drawArea.Dispose();
					}

					if(components != null)
					{
						components.Dispose();
					}
				}

				// Free any unmanaged objects here. 
				//

				this._disposed = true;
			}
			base.Dispose(disposing);
		}

		~MainForm()
		{
			 this.Dispose(false);
		}
		#endregion

		#region Toolbar Event Handlers
		private void toolStripButtonNew_Click(object sender, EventArgs e)
		{
			CommandNew();
		}

		private void toolStripButtonOpen_Click(object sender, EventArgs e)
		{
			CommandOpen();
		}

		private void toolStripButtonSave_Click(object sender, EventArgs e)
		{
			CommandSave();
		}

		private void toolStripButtonPointer_Click(object sender, EventArgs e)
		{
			CommandPointer();
		}

		private void toolStripButtonRectangle_Click(object sender, EventArgs e)
		{
			CommandRectangle();
		}

		private void toolStripButtonEllipse_Click(object sender, EventArgs e)
		{
			CommandEllipse();
		}

		private void toolStripButtonLine_Click(object sender, EventArgs e)
		{
			CommandLine();
		}

		private void toolStripButtonPencil_Click(object sender, EventArgs e)
		{
			CommandPolygon();
		}

		private void toolStripButtonAbout_Click(object sender, EventArgs e)
		{
			CommandAbout();
		}

		private void toolStripButtonUndo_Click(object sender, EventArgs e)
		{
			CommandUndo();
		}

		private void toolStripButtonRedo_Click(object sender, EventArgs e)
		{
			CommandRedo();
		}
		#endregion Toolbar Event Handlers

		#region Menu Event Handlers
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandNew();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandOpen();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandSave();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandSaveAs();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int x = drawArea.TheLayers.ActiveLayerIndex;
			drawArea.TheLayers[x].Graphics.SelectAll();
			drawArea.Refresh();
		}

		private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int x = drawArea.TheLayers.ActiveLayerIndex;
			drawArea.TheLayers[x].Graphics.UnselectAll();
			drawArea.Refresh();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int x = drawArea.TheLayers.ActiveLayerIndex;
			CommandDelete command = new CommandDelete(drawArea.TheLayers);

			if (drawArea.TheLayers[x].Graphics.DeleteSelection())
			{
				drawArea.Refresh();
				drawArea.AddCommandToHistory(command);
			}
		}

		private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			Clear(false);
		}

		private void Clear(bool clearHistory)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			CommandDeleteAll command = new CommandDeleteAll(drawArea.TheLayers);

			if (drawArea.TheLayers[x].Graphics.Clear())
			{
				drawArea.Refresh();
				drawArea.AddCommandToHistory(command);
			}

			if (clearHistory)
			{
				drawArea.ClearHistory();
			}
		}

		private void moveToFrontToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int x = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[x].Graphics.MoveSelectionToFront())
			{
				drawArea.Refresh();
			}
		}

		private void moveToBackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int x = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[x].Graphics.MoveSelectionToBack())
			{
				drawArea.Refresh();
			}
		}

		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//if (drawArea.GraphicsList.ShowPropertiesDialog(drawArea))
			//{
			//    drawArea.SetDirty();
			//    drawArea.Refresh();
			//}
		}

		private void pointerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandPointer();
		}

		private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandRectangle();
		}

		private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandEllipse();
		}

		private void lineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandLine();
		}

		private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandPolygon();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandAbout();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandUndo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandRedo();
		}
		#endregion Menu Event Handlers

		#region DocManager Event Handlers
		/// <summary>
		/// Load document from the stream supplied by DocManager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void docManager_LoadEvent(object sender, SerializationEventArgs e)
		{
			// DocManager asks to load document from supplied stream
			try
			{
				drawArea.TheLayers = (Layers)e.Formatter.Deserialize(e.SerializationStream);
			} catch (ArgumentNullException ex)
			{
				HandleLoadException(ex, e);
			} catch (SerializationException ex)
			{
				HandleLoadException(ex, e);
			} catch (SecurityException ex)
			{
				HandleLoadException(ex, e);
			}
		}


		/// <summary>
		/// Save document to stream supplied by DocManager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void docManager_SaveEvent(object sender, SerializationEventArgs e)
		{
			// DocManager asks to save document to supplied stream
			try
			{
				e.Formatter.Serialize(e.SerializationStream, drawArea.TheLayers);
			} catch (ArgumentNullException ex)
			{
				HandleSaveException(ex, e);
			} catch (SerializationException ex)
			{
				HandleSaveException(ex, e);
			} catch (SecurityException ex)
			{
				HandleSaveException(ex, e);
			}
		}
		#endregion

		#region Event Handlers
		private void MainForm_Load(object sender, EventArgs e)
		{
			// Create draw area
			drawArea = new DrawArea();
			drawArea.MyParent = this;
			drawArea.Location = new Point(0, 0);
			drawArea.Size = new Size(10, 10);
			drawArea.Owner = this;
			drawArea.BorderStyle = BorderStyle.None;
			this.pnlDrawArea.Controls.Add(drawArea);

			// Helper objects (DocManager and others)
			InitializeHelperObjects();

			drawArea.Initialize(this, docManager, InitialImage, InitialImageAsFilePath, InitialImageAsPngBytes);
			ResizeDrawArea();

			LoadSettings();

			// Submit to Idle event to set controls state at idle time
			Application.Idle += delegate
			                    {
				                    if (!this._disposingOrDisposed)
				                    {
					                    this.ResizeDrawArea();
					                    //if (drawArea.PanX != 0 && drawArea.PanY != 0)
					                    //{
					                    //	this.ManualScroll(true, -drawArea.PanX);
					                    //	this.ManualScroll(false, -drawArea.PanY);
					                    //	drawArea.PanX = 0;
					                    //	drawArea.PanY = 0;
					                    //}
					                    SetStateOfControls();
										//Debug.WriteLine("Idle");Debug.Flush();
				                    }
			                    };

			// Open file passed in the command line
			if (ArgumentFile.Length > 0)
				OpenDocument(ArgumentFile);

			// Subscribe to DropDownOpened event for each popup menu
			// (see details in MainForm_DropDownOpened)
			//foreach (ToolStripItem item in menuStrip1.Items)
			//{
			//	if (item.GetType() ==
			//		typeof(ToolStripMenuItem))
			//	{
			//		((ToolStripMenuItem)item).DropDownOpened += MainForm_DropDownOpened;
			//	}
			//}
			
			SetStateOfControls();
		}

		/// <summary>
		/// Resize draw area when form is resized
		/// </summary>
		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (/*WindowState != FormWindowState.Minimized &&*/
				drawArea != null)
			{
				ResizeDrawArea();
			}
		}

		/// <summary>
		/// Form is closing
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason ==
				CloseReason.UserClosing)
			{
				if (!docManager.CloseDocument())
					e.Cancel = true;
			}

			SaveSettings();
		}

		/// <summary>
		/// Popup menu item (File, Edit ...) is opened.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_DropDownOpened(object sender, EventArgs e)
		{
			// Reset active tool to pointer.
			// This prevents bug in rare case when non-pointer tool is active, user opens
			// main main menu and after this clicks in the drawArea. MouseDown event is not
			// raised in this case (why ??), and MouseMove event works incorrectly.
			drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
		}
		#endregion Event Handlers

		#region Other Functions
		/// <summary>
		/// Set state of controls.
		/// Function is called at idle time.
		/// </summary>
		public void SetStateOfControls()
		{
			// Select active tool
			toolStripButtonPointer.Checked = !drawArea.Panning && (drawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
			toolStripButtonRectangle.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
			toolStripButtonEllipse.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
			toolStripButtonArrow.Checked = drawArea.EndCap == LineCap.ArrowAnchor && (drawArea.ActiveTool == DrawArea.DrawToolType.Line);
			toolStripButtonLine.Checked = drawArea.EndCap != LineCap.ArrowAnchor &&(drawArea.ActiveTool == DrawArea.DrawToolType.Line);
			toolStripButtonPencil.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

			pointerToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
			rectangleToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
			ellipseToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
			lineToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Line);
			pencilToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

			switch (drawArea.LineWidth)
			{
				case -1: this.toolStripDropDownButtonLineThickness.Text = "Thinnest"; break;
				case 2: this.toolStripDropDownButtonLineThickness.Text = "Thin"; break;
				case 5: this.toolStripDropDownButtonLineThickness.Text = "Thick"; break;
				case 10: this.toolStripDropDownButtonLineThickness.Text = "Thicker"; break;
				case 15: this.toolStripDropDownButtonLineThickness.Text = "Thickest"; break;
			}

			this.toolStripDropDownButtonPenType.Text = DrawingPens.GetPenTypeAsString(drawArea.PenType);

			tsbLineColor.BackColor = drawArea.LineColor;
			tsbFillColor.BackColor = drawArea.FillColor;

			int x = drawArea.TheLayers.ActiveLayerIndex;
			bool objects = (drawArea.TheLayers[x].Graphics.Count > 0);
			bool selectedObjects = (drawArea.TheLayers[x].Graphics.SelectionCount > 0);
			// File operations
			saveToolStripMenuItem.Enabled = objects;
			//toolStripButtonSave.Enabled = objects;
			saveAsToolStripMenuItem.Enabled = objects;

			// Edit operations
			deleteToolStripMenuItem.Enabled = selectedObjects;
			deleteAllToolStripMenuItem.Enabled = objects;
			selectAllToolStripMenuItem.Enabled = objects;
			unselectAllToolStripMenuItem.Enabled = objects;
			moveToFrontToolStripMenuItem.Enabled = selectedObjects;
			moveToBackToolStripMenuItem.Enabled = selectedObjects;
			propertiesToolStripMenuItem.Enabled = selectedObjects;

			// Undo, Redo
			undoToolStripMenuItem.Enabled = drawArea.CanUndo;
			toolStripButtonUndo.Enabled = drawArea.CanUndo;

			redoToolStripMenuItem.Enabled = drawArea.CanRedo;
			toolStripButtonRedo.Enabled = drawArea.CanRedo;

			// Status Strip
			//tslCurrentLayer.Text = drawArea.TheLayers[x].LayerName;
			//tslNumberOfObjects.Text = drawArea.TheLayers[x].Graphics.Count.ToString();
			//tslPanPosition.Text = drawArea.PanX + ", " + drawArea.PanY;
			//tslRotation.Text = drawArea.Rotation + " deg";
			//tslZoomLevel.Text = (Math.Round(drawArea.Zoom * 100)) + " %";

			// Pan button
			tsbPanMode.Checked = drawArea.Panning;
		}

		/// <summary>
		/// Set draw area to all form client space except toolbar
		/// </summary>
		private void ResizeDrawArea()
		{
			var bounds = drawArea.GetBounds();

			//drawArea.Left = 0;
			//drawArea.Top = 0;
			drawArea.Width = Math.Max(this.pnlDrawArea.ClientRectangle.Width , (int)Math.Round((bounds.Left+ bounds.Width+10)*drawArea.Zoom));
			drawArea.Height = Math.Max(this.pnlDrawArea.ClientRectangle.Height , (int)Math.Round((bounds.Top+bounds.Height+10)*drawArea.Zoom));
			this.pnlDrawArea.Invalidate();
			;
		}

		/// <summary>
		/// Initialize helper objects from the DocToolkit Library.
		/// 
		/// Called from Form1_Load. Initialized all objects except
		/// PersistWindowState wich must be initialized in the
		/// form constructor.
		/// </summary>
		private void InitializeHelperObjects()
		{
			//Excluded by F4CIO:------------------------------------------------------------------
			//// DocManager
			//DocManagerData data = new DocManagerData();
			//data.FormOwner = this.ParentForm;
			//data.UpdateTitle = true;
			//data.FileDialogFilter = "DrawTools files (*.dtl)|*.dtl|All Files (*.*)|*.*";
			//data.NewDocName = "Untitled.dtl";
			//data.RegistryPath = registryPath;

			//docManager = new DocManager(data);
			//docManager.RegisterFileType("dtl", "dtlfile", "DrawTools File");

			//// Subscribe to DocManager events.
			//docManager.SaveEvent += docManager_SaveEvent;
			//docManager.LoadEvent += docManager_LoadEvent;

			//// Make "inline subscription" using anonymous methods.
			//docManager.OpenEvent += delegate(object sender, OpenFileEventArgs e)
			//							{
			//								// Update MRU List
			//								if (e.Succeeded)
			//									mruManager.Add(e.FileName);
			//								else
			//									mruManager.Remove(e.FileName);
			//							};

			//docManager.DocChangedEvent += delegate
			//								{
			//									drawArea.Refresh();
			//									drawArea.ClearHistory();
			//								};

			//docManager.ClearEvent += delegate
			//							{
			//								bool haveObjects = false;
			//								for (int i = 0; i < drawArea.TheLayers.Count; i++)
			//								{
			//									if (drawArea.TheLayers[i].Graphics.Count > 1)
			//									{
			//										haveObjects = true;
			//										break;
			//									}
			//								}
			//								if (haveObjects)
			//								{
			//									drawArea.TheLayers.Clear();
			//									drawArea.ClearHistory();
			//									drawArea.Refresh();
			//								}
			//							};

			//docManager.NewDocument();

			// DragDropManager
			dragDropManager = new DragDropManager(this.ParentForm);
			dragDropManager.FileDroppedEvent += delegate(object sender, FileDroppedEventArgs e) { OpenDocument(e.FileArray.GetValue(0).ToString()); };

			//Excluded by F4CIO:-------------------------------------------------------------------
			// MruManager
			//mruManager = new MruManager();
			//mruManager.Initialize(
			//	this.ParentForm, // owner form
			//	recentFilesToolStripMenuItem, // Recent Files menu item
			//	fileToolStripMenuItem, // parent
			//	registryPath); // Registry path to keep MRU list

			//mruManager.MruOpenEvent += delegate(object sender, MruFileOpenEventArgs e) { OpenDocument(e.FileName); };
		}

		/// <summary>
		/// Handle exception from docManager_LoadEvent function
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="e"></param>
		private void HandleLoadException(Exception ex, SerializationEventArgs e)
		{
			MessageBox.Show(this,
							"Open File operation failed. File name: " + e.FileName + "\n" +
							"Reason: " + ex.Message,
							Application.ProductName);

			e.Error = true;
		}

		/// <summary>
		/// Handle exception from docManager_SaveEvent function
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="e"></param>
		private void HandleSaveException(Exception ex, SerializationEventArgs e)
		{
			MessageBox.Show(this,
							"Save File operation failed. File name: " + e.FileName + "\n" +
							"Reason: " + ex.Message,
							Application.ProductName);

			e.Error = true;
		}

		/// <summary>
		/// Open document.
		/// Used to open file passed in command line or dropped into the window
		/// </summary>
		/// <param name="file"></param>
		public void OpenDocument(string file)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			docManager.OpenDocument(file);
		}

		/// <summary>
		/// Load application settings
		/// </summary>
		private void LoadSettings()
		{
		
		}

		/// <summary>
		/// Save application settings
		/// </summary>
		private void SaveSettings()
		{
		
		}

		/// <summary>
		/// Set Pointer draw tool
		/// </summary>
		private void CommandPointer()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;

			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		/// <summary>
		/// Set Rectangle draw tool
		/// </summary>
		private void CommandRectangle()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
			drawArea.DrawFilled = false;

			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		/// <summary>
		/// Set Ellipse draw tool
		/// </summary>
		private void CommandEllipse()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
			drawArea.DrawFilled = false;

			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		/// <summary>
		/// Set Arrow Line draw tool
		/// </summary>
		private void CommandArrow()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Line;

			drawArea.EndCap = LineCap.ArrowAnchor;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		/// <summary>
		/// Set Line draw tool
		/// </summary>
		private void CommandLine()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Line;

			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		/// <summary>
		/// Set Polygon draw tool
		/// </summary>
		private void CommandPolygon()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Polygon;

			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		/// <summary>
		/// Show About dialog
		/// </summary>
		private void CommandAbout()
		{
			FrmAbout frm = new FrmAbout();
			frm.ShowDialog(this);
		}

		/// <summary>
		/// Open new file
		/// </summary>
		private void CommandNew()
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			docManager.NewDocument();
		}

		/// <summary>
		/// Open file
		/// </summary>
		private void CommandOpen()
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			docManager.OpenDocument("");
		}

		/// <summary>
		/// Save file
		/// </summary>
		private void CommandSave()
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			docManager.SaveDocument(DocManager.SaveType.Save);
		}

		/// <summary>
		/// Save As
		/// </summary>
		private void CommandSaveAs()
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			docManager.SaveDocument(DocManager.SaveType.SaveAs);
		}

		/// <summary>
		/// Undo
		/// </summary>
		private void CommandUndo()
		{
			drawArea.Undo();
		}

		/// <summary>
		/// Redo
		/// </summary>
		private void CommandRedo()
		{
			drawArea.Redo();
		}
		#endregion

		#region Mouse Functions
		private void MainForm_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta != 0)
			{
				if (_zoomOnMouseWheel)
				{
					((HandledMouseEventArgs)e).Handled = true;
				}

				if (_controlKey)
				{
					// We are panning up or down using the wheel
					if (e.Delta > 0)
						this.ManualScroll(false, 10);
					else
						this.ManualScroll(false,-10);
					Invalidate();
				} else
				{
					if (_zoomOnMouseWheel)
					{
						// We are zooming in or out using the wheel
						if (e.Delta > 0)
							AdjustZoom(.1f);
						else
							AdjustZoom(-.1f);
					}
				}
				SetStateOfControls();
				return;
			}
		}
		#endregion Mouse Functions

		#region Keyboard Functions
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			switch (e.KeyCode)
			{
				case Keys.Delete:
					drawArea.TheLayers[al].Graphics.DeleteSelection();
					drawArea.Invalidate();
					break;
				case Keys.Right:
					this.ManualScroll(true,-10);
					drawArea.Invalidate();
					break;
				case Keys.Left:
					this.ManualScroll(true,+10);
					drawArea.Invalidate();
					break;
				case Keys.Up:
					if (e.KeyCode == Keys.Up &&
						e.Shift)
						AdjustZoom(.1f);
					else
						drawArea.PanY += 10;
					drawArea.Invalidate();
					break;
				case Keys.Down:
					if (e.KeyCode == Keys.Down &&
						e.Shift)
						AdjustZoom(-.1f);
					else
						drawArea.PanY -= 10;
					drawArea.Invalidate();
					break;
				case Keys.ControlKey:
					_controlKey = true;
					break;
				default:
					break;
			}
			drawArea.Invalidate();
			SetStateOfControls();
		}

		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			_controlKey = false;
		}
		#endregion Keyboard Functions

		#region Zoom, Pan, Rotation Functions
		/// <summary>
		/// Adjust the zoom by the amount given, within reason
		/// </summary>
		/// <param name="_amount">float value to adjust zoom by - may be positive or negative</param>
		private void AdjustZoom(float _amount)
		{
			drawArea.Zoom += _amount;
			if (drawArea.Zoom < .1f)
				drawArea.Zoom = .1f;
			if (drawArea.Zoom > 10)
				drawArea.Zoom = 10f;

			drawArea.Invalidate();
			SetStateOfControls();
		}

		private void tsbZoomIn_Click(object sender, EventArgs e)
		{
			AdjustZoom(.1f);
		}

		private void tsbZoomOut_Click(object sender, EventArgs e)
		{
			AdjustZoom(-.1f);
		}

		private void tsbZoomReset_Click(object sender, EventArgs e)
		{
			drawArea.Zoom = 1.0f;
			drawArea.Invalidate();
		}

		private void tsbRotateRight_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
				RotateObject(10);
			else
				RotateDrawing(10);
		}

		private void tsbRotateLeft_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
				RotateObject(-10);
			else
				RotateDrawing(-10);
		}

		private void tsbRotateReset_Click(object sender, EventArgs e)
		{
			this._panMode = false;
			drawArea.Panning = this._panMode;

			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
				RotateObject(0);
			else
				RotateDrawing(0);
		}

		/// <summary>
		/// Rotate the selected Object(s)
		/// </summary>
		/// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
		private void RotateObject(int p)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			for (int i = 0; i < drawArea.TheLayers[al].Graphics.Count; i++)
			{
				if (drawArea.TheLayers[al].Graphics[i].Selected)
					if (p == 0)
						drawArea.TheLayers[al].Graphics[i].Rotation = 0;
					else
						drawArea.TheLayers[al].Graphics[i].Rotation += p;
			}
		
			this._panMode = false;
			drawArea.Panning = this._panMode;
	
			drawArea.Invalidate();
			SetStateOfControls();
		}

		/// <summary>
		/// Rotate the entire drawing
		/// </summary>
		/// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
		private void RotateDrawing(int p)
		{
			if (p == 0)
				drawArea.Rotation = 0;
			else
			{
				drawArea.Rotation += p;
				if (p < 0) // Left Rotation
				{
					if (drawArea.Rotation <
						-360)
						drawArea.Rotation = 0;
				} else
				{
					if (drawArea.Rotation > 360)
						drawArea.Rotation = 0;
				}
			}
		
			this._panMode = false;
			drawArea.Panning = this._panMode;
		
			drawArea.Invalidate();
			SetStateOfControls();
		}

		private void tsbPanMode_Click(object sender, EventArgs e)
		{
			_panMode = true;//!_panMode;
			if (_panMode)
			{
				tsbPanMode.Checked = true;
			}
			else
			{
				tsbPanMode.Checked = false;
			}
			drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
			drawArea.Panning = _panMode;
		}

		private void tsbPanReset_Click(object sender, EventArgs e)
		{
			_panMode = false;
			if (tsbPanMode.Checked)
				tsbPanMode.Checked = false;
			drawArea.Panning = false;
			drawArea.PanX = 0;
			drawArea.PanY = drawArea.OriginalPanY;
			drawArea.Invalidate();
		}
		#endregion  Zoom, Pan, Rotation Functions

		private void tslCurrentLayer_Click(object sender, EventArgs e)
		{
			LayerDialog ld = new LayerDialog(drawArea.TheLayers);
			ld.ShowDialog();
			// First add any new layers
			for (int i = 0; i < ld.layerList.Count; i++)
			{
				if (ld.layerList[i].LayerNew)
				{
					Layer layer = new Layer();
					layer.LayerName = ld.layerList[i].LayerName;
					layer.Graphics = new GraphicsList();
					drawArea.TheLayers.Add(layer);
				}
			}
			drawArea.TheLayers.InactivateAllLayers();
			for (int i = 0; i < ld.layerList.Count; i++)
			{
				if (ld.layerList[i].LayerActive)
					drawArea.TheLayers.SetActiveLayer(i);

				if (ld.layerList[i].LayerVisible)
					drawArea.TheLayers.MakeLayerVisible(i);
				else
					drawArea.TheLayers.MakeLayerInvisible(i);

				drawArea.TheLayers[i].LayerName = ld.layerList[i].LayerName;
			}
			// Lastly, remove any deleted layers
			for (int i = 0; i < ld.layerList.Count; i++)
			{
				if (ld.layerList[i].LayerDeleted)
					drawArea.TheLayers.RemoveLayer(i);
			}
			drawArea.Invalidate();
		}

		#region Additional Drawing Tools
		/// <summary>
		/// Draw PolyLine objects - a polyline is a series of straight lines of various lengths connected at their end points.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbPolyLine_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.PolyLine;
			drawArea.DrawFilled = false;

			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbConnector_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Connector;
			drawArea.DrawFilled = false;
	
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}
		/// <summary>
		/// Draw Text objects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbDrawText_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Text;
	
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbFilledRectangle_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
			drawArea.DrawFilled = true;
	
			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbFilledEllipse_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
			drawArea.DrawFilled = true;
	
			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbImage_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Image;
	
			drawArea.EndCap = LineCap.Round;
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbSelectLineColor_Click(object sender, EventArgs e)
		{
			dlgColor.AllowFullOpen = true;
			dlgColor.AnyColor = true;
			if (dlgColor.ShowDialog() ==
				DialogResult.OK)
			{
                drawArea.LineColor = Color.FromArgb(255, dlgColor.Color);
                tsbLineColor.BackColor = Color.FromArgb(255, dlgColor.Color);
			
				this._panMode = false;
				drawArea.Panning = this._panMode;
			}
		}

		private void tsbSelectFillColor_Click(object sender, EventArgs e)
		{
			dlgColor.AllowFullOpen = true;
			dlgColor.AnyColor = true;
            if (dlgColor.ShowDialog() ==
                DialogResult.OK)
            {
                drawArea.FillColor = Color.FromArgb(255, dlgColor.Color);
                tsbFillColor.BackColor = Color.FromArgb(255, dlgColor.Color);
		
				this._panMode = false;
				drawArea.Panning = this._panMode;
            }
		}

		private void tsbLineThinnest_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = -1;
		
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbLineThin_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 2;
		
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbThickLine_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 5;
		
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbThickerLine_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 10;
		
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}

		private void tsbThickestLine_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 15;
	
			this._panMode = false;
			drawArea.Panning = this._panMode;
		}
		#endregion Additional Drawing Tools

		public void ExportToFile(string filePath, ImageFormat imageFormat)
		{
			using (Bitmap b = new Bitmap(drawArea.Width, drawArea.Height))
			{
				using (Graphics g = Graphics.FromImage(b))
				{
					g.Clear(Color.White);
					drawArea.DeselectAll();
					drawArea.TheLayers.Draw(g);
					b.Save(filePath, imageFormat);
				}
			}
		}
		
		public Image ExportToImage()
		{
			Bitmap b = new Bitmap(drawArea.Width, drawArea.Height);
			Graphics g = Graphics.FromImage(b);
			g.Clear(Color.White);
			drawArea.DeselectAll();
			drawArea.TheLayers.Draw(g);
			return b;
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Bitmap b = new Bitmap(drawArea.Width, drawArea.Height);
			Graphics g = Graphics.FromImage(b);
			g.Clear(Color.White);
			drawArea.TheLayers.Draw(g);
			b.Save(@"c:\test.bmp", ImageFormat.Bmp);
			MessageBox.Show("save complete!");
			g.Dispose();
			b.Dispose();
		}

		public void CopyAllToClipboard()
		{
			using (Image image = this.ExportToImage())
			{
				Clipboard.SetDataObject(image, true);
				//Application.DoEvents();
				//Thread.Sleep(500);

				using (RichTextBox tempRtb = new RichTextBox())
				{
					tempRtb.WordWrap = false;

					tempRtb.Paste();
					Application.DoEvents();
					Thread.Sleep(500);

					tempRtb.SelectAll();

					tempRtb.Copy();
					Application.DoEvents();
					Thread.Sleep(500);
				}
			}
		}

    private void cutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      drawArea.CutObject();
    }

	private void solidToolStripMenuItem_Click(object sender, EventArgs e)
	{
		drawArea.PenType = DrawingPens.PenType.Solid;

		this._panMode = false;
		drawArea.Panning = this._panMode;
	}

	private void dottedToolStripMenuItem_Click(object sender, EventArgs e)
	{
		drawArea.PenType = DrawingPens.PenType.Dot;

		this._panMode = false;
		drawArea.Panning = this._panMode;
	}

	private void dashedToolStripMenuItem_Click(object sender, EventArgs e)
	{
		drawArea.PenType = DrawingPens.PenType.Dash;

		this._panMode = false;
		drawArea.Panning = this._panMode;
	}

	private void dotDashedtoolStripMenuItem7_Click(object sender, EventArgs e)
	{
		drawArea.PenType = DrawingPens.PenType.Dash_Dot;

		this._panMode = false;
		drawArea.Panning = this._panMode;
	}

	private void doubleLineToolStripMenuItem8_Click(object sender, EventArgs e)
	{
		drawArea.PenType = DrawingPens.PenType.DoubleLine;

		this._panMode = false;
		drawArea.Panning = this._panMode;
	}

	private void toolStripButtonArrow_Click(object sender, EventArgs e)
	{
		CommandArrow();
	}

	private void ctxtMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
	{
		this.undoToolStripMenuItem2.Enabled = drawArea.CanUndo;
		this.toolStripMenuItem13.Enabled = drawArea.CanRedo;
	}

		/// <summary>
		/// Clears graphics and history and reloads initial image if present.
		/// </summary>
		public void Reset()
		{
			this.Clear(true);
			drawArea.LoadInitialImage(InitialImage, InitialImageAsFilePath, InitialImageAsPngBytes, null);
		}

		private void pnlDrawArea_Scroll(object sender, ScrollEventArgs e)
		{
			//if (this._zoomOnMouseWheel)
			//{
			//	e.NewValue = e.OldValue;
			//}
		}

		internal void ManualScroll(bool isHorizontal, int delta)
		{
			this.ResizeDrawArea();
			int newValue;
			if (isHorizontal)
			{
				newValue = this.pnlDrawArea.HorizontalScroll.Value + delta;
				newValue = Math.Max(this.pnlDrawArea.HorizontalScroll.Minimum, newValue);
				newValue = Math.Min(this.pnlDrawArea.HorizontalScroll.Maximum, newValue);
				this.pnlDrawArea.HorizontalScroll.Value = newValue;
			}
			else
			{
				newValue = this.pnlDrawArea.VerticalScroll.Value + delta;
				newValue = Math.Max(this.pnlDrawArea.VerticalScroll.Minimum, newValue);
				newValue = Math.Min(this.pnlDrawArea.VerticalScroll.Maximum, newValue);
				this.pnlDrawArea.VerticalScroll.Value = newValue;
			}
			this.pnlDrawArea.Invalidate();
		}

		public void ReplaceInitialImage(Image image, bool preserveSize, bool addNewIfNotFound)
		{
			this.InitialImageAsFilePath = null;
			this.InitialImageAsPngBytes = null;
			this.InitialImage = image;
		
			//CommandDelete command = new CommandDelete(drawArea.TheLayers);
			
			var indexAndInitialImage = drawArea.GetInitialImageGraphic();
			if (indexAndInitialImage != null)
			{
				if (!preserveSize)
				{
					indexAndInitialImage.Value.Value.rectangle.Width = image.Width;
					indexAndInitialImage.Value.Value.rectangle.Height = image.Height;
				}
				//TODO: compress theImage here
				indexAndInitialImage.Value.Value.TheImage = (Bitmap) image;
				
				drawArea.Invalidate();
				//drawArea.TheLayers[0].Graphics.RemoveAt(indexAndInitialImage.Value.Key);
				//drawArea.Refresh();
				//drawArea.AddCommandToHistory(command);
			}
			else
			{
				if (addNewIfNotFound)
				{
					drawArea.LoadInitialImage(InitialImage, InitialImageAsFilePath, InitialImageAsPngBytes, null);
				}
			}

		//	drawArea.LoadInitialImage(this.InitialImage, null, null, indexAndInitialImage==null?null:indexAndInitialImage.Value.Value.Clone() as DrawImage);
		}
	}
}