using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerfCompare
{
	/// <summary>
	/// Summary description for StatefulMessageBox.
	/// </summary>
	public class StatefulMessageBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.CheckBox hideNextTimeBox;
		private string dialogName;
		private System.Windows.Forms.Label messageText;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StatefulMessageBox(string dialogName, string message, string title)
		{
			this.dialogName = dialogName;

			InitializeComponent();

			this.messageText.Text = message;
			this.Text = title;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(StatefulMessageBox));
			this.okButton = new System.Windows.Forms.Button();
			this.messageText = new System.Windows.Forms.Label();
			this.hideNextTimeBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(107, 128);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "Ok";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// messageText
			// 
			this.messageText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.messageText.Location = new System.Drawing.Point(8, 8);
			this.messageText.Name = "messageText";
			this.messageText.Size = new System.Drawing.Size(280, 80);
			this.messageText.TabIndex = 3;
			this.messageText.Text = "Message";
			this.messageText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// hideNextTimeBox
			// 
			this.hideNextTimeBox.Checked = true;
			this.hideNextTimeBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.hideNextTimeBox.Location = new System.Drawing.Point(8, 96);
			this.hideNextTimeBox.Name = "hideNextTimeBox";
			this.hideNextTimeBox.Size = new System.Drawing.Size(216, 24);
			this.hideNextTimeBox.TabIndex = 2;
			this.hideNextTimeBox.Text = "Do not show me this message again";
			// 
			// StatefulMessageBox
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 160);
			this.Controls.Add(this.hideNextTimeBox);
			this.Controls.Add(this.messageText);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StatefulMessageBox";
			this.Text = "Information";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void okButton_Click(object sender, System.EventArgs e)
		{
			ConfigUtil.SetConfiguration(this.dialogName, (! this.hideNextTimeBox.Checked).ToString());
		}

		public new void Show()
		{
			bool showDialog = true;

			try { showDialog = Boolean.Parse(ConfigUtil.GetConfiguration(this.dialogName)); }
			catch { showDialog = true; }

			if(showDialog)
			{
				ConfigUtil.SetConfiguration(this.dialogName, (! this.hideNextTimeBox.Checked).ToString());
				this.ShowDialog();
			}
			else
				this.Close();
		}
	}
}
