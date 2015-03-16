using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace PerfCompare
{
	/// <summary>
	/// Micro-Benchmarking performance comparison tool.
	/// </summary>
    public class PerfCompare : System.Windows.Forms.Form
    {
		const string updateHost = @"http://www.leeholmes.com/projects/perfcompare";
		const string currentVersion = "1.2.200405212";

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage codePage;
        private System.Windows.Forms.TabPage ilDasmPage;
        private System.Windows.Forms.TabPage performancePage;
        private System.Windows.Forms.TextBox codeOptionText;
        private System.Windows.Forms.TextBox ilDasmText;
        private System.Windows.Forms.Label iterationsCount;
        private System.Windows.Forms.TextBox iterationsCountText;
        private System.Windows.Forms.Label resultTicksText;
        private System.Windows.Forms.Label totalMillisecondsText;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Label ticksResult;
        private System.Windows.Forms.Label millisecondResult;
        private System.Windows.Forms.TabPage historyPage;

        private ArrayList runHistory;
        private System.Windows.Forms.DataGrid historyGrid;
        private System.Windows.Forms.Label commentLabel;
        private System.Windows.Forms.TextBox commentText;
        private System.Windows.Forms.Button populateButton;
        private System.Windows.Forms.Button compareButton;
        private System.Windows.Forms.Label parametersLabel;
        private System.Windows.Forms.TextBox parametersText;
        private System.Windows.Forms.CheckBox advancedEditCheck;
        private System.Windows.Forms.Button exportButton;
		private System.Windows.Forms.TabPage updatePage;
		private System.Windows.Forms.RichTextBox updateText;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public PerfCompare()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.Resize += new EventHandler(PerfCompare_Resize);

            runHistory = new ArrayList();
            runHistory.Add(new RunHistory(0, "", DateTime.Now, 0, 0, "Application Loaded", "", false));
            this.Icon = new Icon(this.GetType(), "App.ico");
			InitializeUpdateTab();
            CalculateSizing();
        }

        private void CalculateSizing()
        {
            if((this.Width > 25) && (this.Height > 200))
            {
                SetTabSet();
                SetCodeTab();
                SetIlDasmTab();
                SetStatsTab();
				SetUpdateTab();
            }
        }

        private void SetTabSet()
        {
            tabControl1.Width = this.Width;
            tabControl1.Height = this.Height - 20;
        }

        private void SetCodeTab()
        {
            codePage.Width = this.Width - 25;
            codePage.Height = this.Height - 70;
            codeOptionText.Width = this.Width - 25;
            codeOptionText.Height = this.Height - 95;

            SetCodeBottom();
        }

        private void SetCodeBottom()
        {
            // Compiler label
            this.parametersLabel.Left = 10;
            this.parametersLabel.Top = this.Height - 75;

            // Compiler text
            this.parametersText.Left = this.parametersLabel.Right + 5;
            this.parametersText.Top = this.parametersLabel.Top - 5;

            // Advanced edit flag
            this.advancedEditCheck.Left = this.parametersText.Right + 60;
            this.advancedEditCheck.Top = this.parametersText.Top;
        }

        private void SetIlDasmTab()
        {
            ilDasmPage.Width = this.Width - 25;
            ilDasmPage.Height = this.Height - 70;
            ilDasmText.Width = this.Width - 25;
            ilDasmText.Height = this.Height - 70;
        }

        private void SetStatsTab()
        {
            // History Grid
            historyPage.Width = this.Width - 25;
            historyPage.Height = this.Height - 70;
            historyGrid.Width = this.Width - 25;
            historyGrid.Height = this.Height - 95;

            DataGridTableStyle tableStyle = new DataGridTableStyle();
            historyGrid.TableStyles.Clear();
            tableStyle.MappingName = "ArrayList";
            
            int widthSum = 0;
            widthSum += AddCol("RunTime", "Run Time", 130, tableStyle);
            widthSum += AddCol("Comment", "Comment", 130, tableStyle);
            widthSum += AddCol("Iterations", "Iterations", 70, tableStyle);
            widthSum += AddCol("Ticks", "Ticks", 70, tableStyle);
            widthSum += AddCol("Milliseconds", "Milliseconds", 70, tableStyle);

            int restWidth = 0;
            if(widthSum < (historyGrid.Width - 40))
                restWidth = historyGrid.Width - widthSum - 40; 
            else
                restWidth = 200;

            AddCol("Test", "Code", restWidth, tableStyle);

            historyGrid.TableStyles.Add(tableStyle);
            historyGrid.DataSource = runHistory;
            historyGrid.Refresh();

            SetStatsButtons();
        }
    
        private int AddCol(string mappingName, string headerText, int width, DataGridTableStyle tableStyle)
        {
            // Add "Run Time"
            DataGridTextBoxColumn textCol = new DataGridTextBoxColumn();
            textCol.MappingName = mappingName;
            textCol.HeaderText = headerText;
            textCol.Width = width; 
            tableStyle.GridColumnStyles.Add(textCol);
            historyGrid.TableStyles.Add(tableStyle);
            
            return textCol.Width;
        }

        private void SetStatsButtons()
        {
            // "Populate" button
            this.populateButton.Left = 10;
            this.populateButton.Top = this.Height - this.populateButton.Height - 60;

            // "Compare" button
            this.compareButton.Left = this.populateButton.Right + 10;
            this.compareButton.Top = this.populateButton.Top;

            // "Export" button
            this.exportButton.Left = this.compareButton.Right + 10;
            this.exportButton.Top = this.populateButton.Top;
        }

		private void InitializeUpdateTab()
		{
			// See if we can check for updates
			if(! CanCheckForUpdates())
			{
				this.tabControl1.SelectedIndex = 0;
				return;
			}

			// Check for a more recent version
			PerfCompareUpdate updater = new PerfCompareUpdate(updateHost, currentVersion);
			try
			{
				// No update available, remove tab
				if(! updater.UpdateAvailable)
				{
					this.tabControl1.Controls.Remove(this.updatePage);
					this.tabControl1.SelectedIndex = 0;
					return;
				}

				// Fetch and update the version info if available
				string updateValue = updater.UpdateNotice;
				this.updateText.Rtf = updateValue;
			}
			// Problem accessing host, remove tab
			catch
			{
				this.tabControl1.Controls.Remove(this.updatePage);
				this.tabControl1.SelectedIndex = 0;
				return;
			}
		}

		private void SetUpdateTab()
		{
			updatePage.Width = this.Width - 25;
			updatePage.Height = this.Height - 70;
			updateText.Width = this.Width - 25;
			updateText.Height = this.Height - 70;
		}

		// Can we check for updates?
		// If we don't know, ask the user.
		// If we do know then return
		private bool CanCheckForUpdates()
		{
			if(ConfigUtil.GetConfiguration("CheckForUpdatesOnLoad") == null)
			{
				DialogResult result = MessageBox.Show(
					"Can PerfCompare access the internet to check for updates when it loads?",
					"Checking for Updates",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question);

				ConfigUtil.SetConfiguration("CheckForUpdatesOnLoad", (result == DialogResult.Yes).ToString());
			}

			bool checkForUpdates = false;
			try
			{
				checkForUpdates = Boolean.Parse(ConfigUtil.GetConfiguration("CheckForUpdatesOnLoad"));
			}
			catch{}

			return checkForUpdates;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PerfCompare));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.codePage = new System.Windows.Forms.TabPage();
			this.advancedEditCheck = new System.Windows.Forms.CheckBox();
			this.parametersText = new System.Windows.Forms.TextBox();
			this.parametersLabel = new System.Windows.Forms.Label();
			this.codeOptionText = new System.Windows.Forms.TextBox();
			this.ilDasmPage = new System.Windows.Forms.TabPage();
			this.ilDasmText = new System.Windows.Forms.TextBox();
			this.performancePage = new System.Windows.Forms.TabPage();
			this.commentText = new System.Windows.Forms.TextBox();
			this.commentLabel = new System.Windows.Forms.Label();
			this.millisecondResult = new System.Windows.Forms.Label();
			this.ticksResult = new System.Windows.Forms.Label();
			this.goButton = new System.Windows.Forms.Button();
			this.totalMillisecondsText = new System.Windows.Forms.Label();
			this.resultTicksText = new System.Windows.Forms.Label();
			this.iterationsCountText = new System.Windows.Forms.TextBox();
			this.iterationsCount = new System.Windows.Forms.Label();
			this.historyPage = new System.Windows.Forms.TabPage();
			this.exportButton = new System.Windows.Forms.Button();
			this.compareButton = new System.Windows.Forms.Button();
			this.populateButton = new System.Windows.Forms.Button();
			this.historyGrid = new System.Windows.Forms.DataGrid();
			this.updatePage = new System.Windows.Forms.TabPage();
			this.updateText = new System.Windows.Forms.RichTextBox();
			this.tabControl1.SuspendLayout();
			this.codePage.SuspendLayout();
			this.ilDasmPage.SuspendLayout();
			this.performancePage.SuspendLayout();
			this.historyPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.historyGrid)).BeginInit();
			this.updatePage.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.codePage);
			this.tabControl1.Controls.Add(this.ilDasmPage);
			this.tabControl1.Controls.Add(this.performancePage);
			this.tabControl1.Controls.Add(this.historyPage);
			this.tabControl1.Controls.Add(this.updatePage);
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(568, 440);
			this.tabControl1.TabIndex = 1;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// codePage
			// 
			this.codePage.Controls.Add(this.advancedEditCheck);
			this.codePage.Controls.Add(this.parametersText);
			this.codePage.Controls.Add(this.parametersLabel);
			this.codePage.Controls.Add(this.codeOptionText);
			this.codePage.Location = new System.Drawing.Point(4, 22);
			this.codePage.Name = "codePage";
			this.codePage.Size = new System.Drawing.Size(560, 414);
			this.codePage.TabIndex = 0;
			this.codePage.Text = "Code";
			// 
			// advancedEditCheck
			// 
			this.advancedEditCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.advancedEditCheck.Location = new System.Drawing.Point(400, 272);
			this.advancedEditCheck.Name = "advancedEditCheck";
			this.advancedEditCheck.TabIndex = 5;
			this.advancedEditCheck.Text = "Advanced Edit";
			this.advancedEditCheck.CheckedChanged += new System.EventHandler(this.advancedEditCheck_CheckedChanged);
			// 
			// parametersText
			// 
			this.parametersText.Location = new System.Drawing.Point(176, 269);
			this.parametersText.Name = "parametersText";
			this.parametersText.Size = new System.Drawing.Size(160, 20);
			this.parametersText.TabIndex = 3;
			this.parametersText.Text = "";
			// 
			// parametersLabel
			// 
			this.parametersLabel.Location = new System.Drawing.Point(8, 272);
			this.parametersLabel.Name = "parametersLabel";
			this.parametersLabel.Size = new System.Drawing.Size(176, 23);
			this.parametersLabel.TabIndex = 2;
			this.parametersLabel.Text = "Additional compiler parameters:";
			// 
			// codeOptionText
			// 
			this.codeOptionText.AcceptsReturn = true;
			this.codeOptionText.AcceptsTab = true;
			this.codeOptionText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.codeOptionText.Location = new System.Drawing.Point(8, 8);
			this.codeOptionText.Multiline = true;
			this.codeOptionText.Name = "codeOptionText";
			this.codeOptionText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.codeOptionText.Size = new System.Drawing.Size(552, 256);
			this.codeOptionText.TabIndex = 1;
			this.codeOptionText.Text = "[Enter Some Code]";
			this.codeOptionText.WordWrap = false;
			// 
			// ilDasmPage
			// 
			this.ilDasmPage.Controls.Add(this.ilDasmText);
			this.ilDasmPage.Location = new System.Drawing.Point(4, 22);
			this.ilDasmPage.Name = "ilDasmPage";
			this.ilDasmPage.Size = new System.Drawing.Size(560, 414);
			this.ilDasmPage.TabIndex = 1;
			this.ilDasmPage.Text = "ILDasm Result";
			// 
			// ilDasmText
			// 
			this.ilDasmText.AcceptsReturn = true;
			this.ilDasmText.AcceptsTab = true;
			this.ilDasmText.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.ilDasmText.Location = new System.Drawing.Point(4, 7);
			this.ilDasmText.Multiline = true;
			this.ilDasmText.Name = "ilDasmText";
			this.ilDasmText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.ilDasmText.Size = new System.Drawing.Size(552, 401);
			this.ilDasmText.TabIndex = 2;
			this.ilDasmText.Text = "";
			this.ilDasmText.WordWrap = false;
			// 
			// performancePage
			// 
			this.performancePage.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.performancePage.Controls.Add(this.commentText);
			this.performancePage.Controls.Add(this.commentLabel);
			this.performancePage.Controls.Add(this.millisecondResult);
			this.performancePage.Controls.Add(this.ticksResult);
			this.performancePage.Controls.Add(this.goButton);
			this.performancePage.Controls.Add(this.totalMillisecondsText);
			this.performancePage.Controls.Add(this.resultTicksText);
			this.performancePage.Controls.Add(this.iterationsCountText);
			this.performancePage.Controls.Add(this.iterationsCount);
			this.performancePage.Location = new System.Drawing.Point(4, 22);
			this.performancePage.Name = "performancePage";
			this.performancePage.Size = new System.Drawing.Size(560, 414);
			this.performancePage.TabIndex = 2;
			this.performancePage.Text = "Performance";
			// 
			// commentText
			// 
			this.commentText.Location = new System.Drawing.Point(128, 32);
			this.commentText.Name = "commentText";
			this.commentText.TabIndex = 8;
			this.commentText.Text = "";
			this.commentText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// commentLabel
			// 
			this.commentLabel.Location = new System.Drawing.Point(62, 32);
			this.commentLabel.Name = "commentLabel";
			this.commentLabel.Size = new System.Drawing.Size(56, 23);
			this.commentLabel.TabIndex = 7;
			this.commentLabel.Text = "Comment:";
			this.commentLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// millisecondResult
			// 
			this.millisecondResult.Location = new System.Drawing.Point(128, 104);
			this.millisecondResult.Name = "millisecondResult";
			this.millisecondResult.TabIndex = 6;
			this.millisecondResult.Text = "N/A";
			this.millisecondResult.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ticksResult
			// 
			this.ticksResult.Location = new System.Drawing.Point(128, 72);
			this.ticksResult.Name = "ticksResult";
			this.ticksResult.TabIndex = 5;
			this.ticksResult.Text = "N/A";
			this.ticksResult.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// goButton
			// 
			this.goButton.BackColor = System.Drawing.SystemColors.Control;
			this.goButton.Location = new System.Drawing.Point(176, 144);
			this.goButton.Name = "goButton";
			this.goButton.Size = new System.Drawing.Size(56, 22);
			this.goButton.TabIndex = 4;
			this.goButton.Text = "Go";
			this.goButton.Click += new System.EventHandler(this.goButton_Click);
			// 
			// totalMillisecondsText
			// 
			this.totalMillisecondsText.Location = new System.Drawing.Point(8, 104);
			this.totalMillisecondsText.Name = "totalMillisecondsText";
			this.totalMillisecondsText.TabIndex = 3;
			this.totalMillisecondsText.Text = "Total Milliseconds:";
			// 
			// resultTicksText
			// 
			this.resultTicksText.Location = new System.Drawing.Point(8, 72);
			this.resultTicksText.Name = "resultTicksText";
			this.resultTicksText.TabIndex = 2;
			this.resultTicksText.Text = "Total Ticks:";
			// 
			// iterationsCountText
			// 
			this.iterationsCountText.Location = new System.Drawing.Point(128, 8);
			this.iterationsCountText.Name = "iterationsCountText";
			this.iterationsCountText.TabIndex = 1;
			this.iterationsCountText.Text = "100000";
			this.iterationsCountText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// iterationsCount
			// 
			this.iterationsCount.Location = new System.Drawing.Point(8, 8);
			this.iterationsCount.Name = "iterationsCount";
			this.iterationsCount.Size = new System.Drawing.Size(112, 23);
			this.iterationsCount.TabIndex = 0;
			this.iterationsCount.Text = "Number of Iterations:";
			// 
			// historyPage
			// 
			this.historyPage.Controls.Add(this.exportButton);
			this.historyPage.Controls.Add(this.compareButton);
			this.historyPage.Controls.Add(this.populateButton);
			this.historyPage.Controls.Add(this.historyGrid);
			this.historyPage.Location = new System.Drawing.Point(4, 22);
			this.historyPage.Name = "historyPage";
			this.historyPage.Size = new System.Drawing.Size(560, 414);
			this.historyPage.TabIndex = 3;
			this.historyPage.Text = "History";
			// 
			// exportButton
			// 
			this.exportButton.Location = new System.Drawing.Point(256, 264);
			this.exportButton.Name = "exportButton";
			this.exportButton.Size = new System.Drawing.Size(112, 23);
			this.exportButton.TabIndex = 3;
			this.exportButton.Text = "Export to Clipboard";
			this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
			// 
			// compareButton
			// 
			this.compareButton.Location = new System.Drawing.Point(160, 264);
			this.compareButton.Name = "compareButton";
			this.compareButton.Size = new System.Drawing.Size(72, 23);
			this.compareButton.TabIndex = 2;
			this.compareButton.Text = "Compare";
			this.compareButton.Click += new System.EventHandler(this.compareButton_Click);
			// 
			// populateButton
			// 
			this.populateButton.Location = new System.Drawing.Point(8, 264);
			this.populateButton.Name = "populateButton";
			this.populateButton.Size = new System.Drawing.Size(136, 23);
			this.populateButton.TabIndex = 1;
			this.populateButton.Text = "Restore Values to Form";
			this.populateButton.Click += new System.EventHandler(this.populateButton_Click);
			// 
			// historyGrid
			// 
			this.historyGrid.CaptionVisible = false;
			this.historyGrid.CausesValidation = false;
			this.historyGrid.DataMember = "";
			this.historyGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.historyGrid.Location = new System.Drawing.Point(8, 8);
			this.historyGrid.Name = "historyGrid";
			this.historyGrid.ReadOnly = true;
			this.historyGrid.Size = new System.Drawing.Size(544, 248);
			this.historyGrid.TabIndex = 0;
			// 
			// updatePage
			// 
			this.updatePage.Controls.Add(this.updateText);
			this.updatePage.Location = new System.Drawing.Point(4, 22);
			this.updatePage.Name = "updatePage";
			this.updatePage.Size = new System.Drawing.Size(560, 414);
			this.updatePage.TabIndex = 4;
			this.updatePage.Text = "** Update Available **";
			// 
			// updateText
			// 
			this.updateText.Location = new System.Drawing.Point(8, 8);
			this.updateText.Name = "updateText";
			this.updateText.Size = new System.Drawing.Size(544, 272);
			this.updateText.TabIndex = 0;
			this.updateText.Text = "";
			// 
			// PerfCompare
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(568, 318);
			this.Controls.Add(this.tabControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "PerfCompare";
			this.Text = "PerfCompare";
			this.tabControl1.ResumeLayout(false);
			this.codePage.ResumeLayout(false);
			this.ilDasmPage.ResumeLayout(false);
			this.performancePage.ResumeLayout(false);
			this.historyPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.historyGrid)).EndInit();
			this.updatePage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new PerfCompare());
		}

        private void ilDasmPage_Click()
        {
            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = 1;
            supportClass.CompilerParams = this.parametersText.Text;

            if(this.advancedEditCheck.Checked)
                supportClass.TemplateCode = this.codeOptionText.Text;
            else
                supportClass.HotSpotCode = this.codeOptionText.Text;

            string ilDasmPath = GetIlDasmPath();
            if(ilDasmPath == null)
                return;

            try
            {
                string ilDasmResults = supportClass.GetIlDasm(ilDasmPath);
                this.ilDasmText.Text = ilDasmResults;
            }
            catch(InvalidCodeException codeException)
            {
                this.ilDasmText.Lines = codeException.Message.Split('\n');
            }
        }

        private string GetIlDasmPath()
        {
            string ilDasmPath = ConfigUtil.GetConfiguration("ILDasmPath");

            if((ilDasmPath == null) ||
                (! System.IO.File.Exists(ilDasmPath)))
            {
                OpenFileDialog fileOpen = new OpenFileDialog();
                fileOpen.Title = "Please locate ILDasm.exe";
                fileOpen.InitialDirectory = 
					System.Environment.ExpandEnvironmentVariables("%ProgramFiles%\\Microsoft Visual Studio .NET 2003\\SDK\\v1.1\\bin\\");
                fileOpen.CheckFileExists = true;
                fileOpen.Filter = "Executable Files (*.exe)|*.exe"; 
            
                DialogResult result = fileOpen.ShowDialog();

                if(result == DialogResult.OK)
                {
                    ilDasmPath = fileOpen.FileName;
                    ConfigUtil.SetConfiguration("ILDasmPath", ilDasmPath);
                }
                else
                {
                    return null;
                }
            }

            return ilDasmPath;
        }

        private void goButton_Click(object sender, System.EventArgs e)
        {
            this.goButton.Enabled = false;

            int iterationsCount = 0;

            try
            {
                iterationsCount = Convert.ToInt32(this.iterationsCountText.Text);
            }
            catch
            {
                this.iterationsCountText.Text = "Enter a number.";
            }

            PerfCompareSupport supportClass = new PerfCompareSupport();
            supportClass.ExecutionCount = iterationsCount;
            supportClass.CompilerParams = this.parametersText.Text;

            if(this.advancedEditCheck.Checked)
                supportClass.TemplateCode = this.codeOptionText.Text;
            else
                supportClass.HotSpotCode = this.codeOptionText.Text;

            try
            {
                TimeSpan perfResults = supportClass.TestPerformance();

                this.millisecondResult.Text = perfResults.TotalMilliseconds.ToString();
                this.ticksResult.Text = perfResults.Ticks.ToString();

                runHistory.Add(new RunHistory(iterationsCount,
                    this.codeOptionText.Text,
                    DateTime.Now, perfResults.Ticks, 
                    perfResults.Milliseconds,
                    this.commentText.Text,
                    this.parametersText.Text,
                    this.advancedEditCheck.Checked));
            }
            catch(InvalidCodeException codeException)
            {
                MessageBox.Show(codeException.Message, "Error Compiling Code");
            }

            this.goButton.Enabled = true;
            historyGrid.DataSource = null;
            historyGrid.DataSource = runHistory;
            historyGrid.Refresh();
        }

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if(tabControl1.SelectedIndex == 1)
                ilDasmPage_Click();
        }

        private void PerfCompare_Resize(object sender, EventArgs e)
        {
            CalculateSizing();
        }

        private void populateButton_Click(object sender, System.EventArgs e)
        {
            int selectedIndex = historyGrid.CurrentRowIndex;
            if(selectedIndex < 0)
            {
                MessageBox.Show(
					"Please select a row to re-populate test form with.", 
					"Select a Row");
                return;
            }

            RunHistory currentItem = (RunHistory) runHistory[selectedIndex];
            
            this.codeOptionText.Text = currentItem.Test;
            this.iterationsCountText.Text = currentItem.Iterations.ToString();
            this.commentText.Text = currentItem.Comment;
            this.millisecondResult.Text = currentItem.Milliseconds.ToString();
            this.ticksResult.Text = currentItem.Ticks.ToString();
            this.parametersText.Text = currentItem.CompilerSwitches;
            this.advancedEditCheck.Checked = currentItem.AdvancedEdit;

			StatefulMessageBox message = new StatefulMessageBox(
				"PopulateDialog", 
				"PerfCompare has re-populated the application with values " +
				"from the item you selected.  You may now edit them, " +
				"re-run tests, or whatever you wish.  These changes do not " +
				"affect the original data.", 
				"Form Re-populated");
			message.Show();
			this.tabControl1.SelectedIndex = 0;
        }

        private void compareButton_Click(object sender, System.EventArgs e)
        {
            int selectedCount = 0;
            RunHistory[] selections = new RunHistory[2];

            for(int counter = 0; (counter < runHistory.Count); counter++)
            {
                if(historyGrid.IsSelected(counter))
                {
                    if(selectedCount < 2) 
                        selections[selectedCount] = (RunHistory) runHistory[counter];

                    selectedCount++;
                }
            }

            if(selectedCount != 2)
            {
                MessageBox.Show(
					"Please select exactly two rows to compare.", 
					"Select Exactly 2 Rows");
                return;
            }

            if((selections[0].Ticks == 0) || (selections[1].Ticks == 0))
            {
                MessageBox.Show(
					"One or both of the items did not run long enough to register.  Please try again with an increased iteration count.", 
					"No Data to Compare");
                return;
            }

            int fastest = 0;
            if(selections[0].Ticks > selections[1].Ticks)
                fastest = 1;

            string performanceString = "\"{0}\"\nis\n{1} times faster than\n\"{2}\"";
            MessageBox.Show(
                String.Format(performanceString, 
                selections[fastest].Comment,
                (double) selections[1 - fastest].Ticks / selections[fastest].Ticks,
                selections[1 - fastest].Comment),
				"Performance Comparison");


        }

        private void advancedEditCheck_CheckedChanged(object sender, System.EventArgs e)
        {
            if(advancedEditCheck.Checked)
            {
                PerfCompareSupport supportClass = new PerfCompareSupport();
                this.codeOptionText.Text = supportClass.TemplateCode;
            }
            else
                this.codeOptionText.Text = "[Enter Some Code]";
        }

        private void exportButton_Click(object sender, System.EventArgs e)
        {
            string exportText = "";

            // First try to copy selected rows
            for(int counter = 0; counter < runHistory.Count; counter++)
                if(historyGrid.IsSelected(counter))
                    exportText += ((RunHistory) runHistory[counter]).ToString() + "\r\n";

            // None were selected
            if(exportText == "")
                for(int counter = 0; counter < runHistory.Count; counter++)
                    exportText += ((RunHistory) runHistory[counter]).ToString() + "\r\n";

			exportText = RunHistory.Header + "\r\n" + exportText;
			SetClipboardText(exportText);
        }

		private void SetClipboardText(string exportText)
		{
			StatefulMessageBox message = new StatefulMessageBox(
				"ClipboardDialog", 
				"PerfCompare has placed a tab-delimited version of data " +
				"in the clipboard.  You may now paste it into another " +
				"application, such as Excel, or Notepad.", 
				"Data Exported to Clipboard");
			message.Show();

			try { Clipboard.SetDataObject(exportText, true); }
			catch
			{
				// Try again
				try
				{
					System.Threading.Thread.Sleep(500);
					Clipboard.SetDataObject(exportText, true);
				}
				catch
				{
					MessageBox.Show(
						"Error: Could not export text to clipboard.  Please try again.",
						"Could not export text");
				}
			}
		}
    }
}
