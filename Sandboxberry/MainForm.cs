// SandboxBerry - tool for copying test data into Salesforce sandboxes
// Copyright (C) 2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

using SandboxberryLib;
using SandboxberryLib.InstructionsModel;
using Sandboxberry.Properties;

namespace Sandboxberry
{
    public partial class MainForm : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MainForm));
        private static readonly int CONST_MaxVisibleLogSize = 20000;

        public MainForm()
        {
            InitializeComponent();
            uxOpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            uxSourceUrl.SelectedIndex = 0;
            uxTargetUrl.SelectedIndex = 0;
            uxWaitPicture.Visible = false;
            RetrieveUserSettings();
        }

        // after serious error, disable everything
        public void DisableForm()
        {
            uxWaitPicture.Visible = false;
            foreach (Control c in this.Controls)
            {
                c.Enabled = false;
            }

            
        }

        public void SaveUserSettings()
        {
            
            Settings.Default["SetSourceUrl"] = uxSourceUrl.SelectedIndex;
            Settings.Default["SetSourceUsername"] = uxSourceUsername.Text;
            Settings.Default["SetTargetUrl"] = uxTargetUrl.SelectedIndex;
            Settings.Default["SetTargetUsername"] = uxTargetUsername.Text;
            Settings.Default["SetInstructionsFile"] = uxInstructionsFilename.Text;
            Settings.Default.Save();
        }

        public void RetrieveUserSettings()
        {
            try
            {
                if (Settings.Default["SetSourceUsername"] != null)
                {
                    uxSourceUrl.SelectedIndex = (int)(Settings.Default["SetSourceUrl"]);
                    uxSourceUsername.Text = Settings.Default["SetSourceUsername"].ToString();
                    uxTargetUrl.SelectedIndex = (int)(Settings.Default["SetTargetUrl"]);
                    uxTargetUsername.Text = Settings.Default["SetTargetUsername"].ToString();
                    uxInstructionsFilename.Text = Settings.Default["SetInstructionsFile"].ToString();
                }

            }
            catch (Exception e)
            {
                logger.Error("Error while retrieving user settings", e);
            }

        }

        private void uxLaunchFileOpen_Click(object sender, EventArgs e)
        {
            // set initial directory if value
            string lastFile = uxInstructionsFilename.Text;
            if (!String.IsNullOrEmpty(lastFile))
            {
                if (System.IO.Directory.Exists(lastFile) || System.IO.File.Exists(lastFile))
                    uxOpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(lastFile);
            }
            if (uxOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                uxInstructionsFilename.Text = uxOpenFileDialog.FileName;
            }
        }

        private async void uxStartButton_Click(object sender, EventArgs e)
        {
            ShowUiProcessStarted();
            SaveUserSettings();

            
            try
            {
                var instructions = LoadInstructions();
                var progress = new Progress<string>(t => ShowMessage(t));
                await Task.Factory.StartNew(() => new PopulateSandbox(instructions).Start(progress),
                                    TaskCreationOptions.LongRunning);

                ShowMessage(string.Format("Finished."));
                ShowUiProcessEnded();
            }
            catch (Exception ex)
            {
                logger.Error("StartButton_Click caught an error", ex);
                ShowMessage("******************");
                ShowMessage(string.Format("Unexpected error: {0} \r\n\r\n See Log.txt file for details",ex.Message));
                ShowUiProcessEnded();
                MessageBox.Show(string.Format("Unexpected error: {0} \r\n\r\n See Log.txt file for details", ex.Message), "Unexpected Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            
        }

        private void ShowUiProcessStarted()
        {
            uxStartButton.Enabled = false;
            uxWaitPicture.Visible = true;
            clearTargetDataToolStripMenuItem.Enabled = false;

        }

        private void ShowUiProcessEnded()
        {
            uxStartButton.Enabled = true;
            uxWaitPicture.Visible = false;
            clearTargetDataToolStripMenuItem.Enabled = true;
        }

        private SbbInstructionSet LoadInstructions()
        {
            ShowMessage(string.Format("Loading instructions from {0} ...", uxInstructionsFilename.Text));
            var instructions = SbbInstructionSet.LoadFromFile(uxInstructionsFilename.Text);
            ShowMessage(string.Format("Instructions loaded."));

            // add credentials to instructions
            instructions.SourceCredentials = GetSourceCredentials();
            instructions.TargetCredentuals = GetTargetCredentials();
            return instructions;
        }

        private SbbCredentials GetSourceCredentials()
        {
            var sourceCred = new SbbCredentials();
            sourceCred.SalesforceUrl = "https://test.salesforce.com/services/Soap/u/32.0";
            if (uxSourceUrl.SelectedItem != null && uxSourceUrl.SelectedItem.ToString().StartsWith("Live"))
                sourceCred.SalesforceUrl = "https://login.salesforce.com/services/Soap/u/32.0";
            sourceCred.SalesforceLogin = uxSourceUsername.Text;
            sourceCred.SalesforcePassword = uxSourcePassword.Text;
            return sourceCred;
        }

        private SbbCredentials GetTargetCredentials()
        {
            var targetCred = new SbbCredentials();
            targetCred.SalesforceUrl = "https://test.salesforce.com/services/Soap/u/32.0";
            targetCred.SalesforceLogin = uxTargetUsername.Text;
            targetCred.SalesforcePassword = uxTargetPassword.Text;
            return targetCred;
        }

        public void ShowMessage(string mess)
        {
            logger.DebugFormat("Progress message: {0}", mess);
            uxProgressBox.Text += string.Format("{0}{1}",
                mess, Environment.NewLine);
            if (uxProgressBox.Text.Length > CONST_MaxVisibleLogSize)
                uxProgressBox.Text = "..." + uxProgressBox.Text.Substring(uxProgressBox.Text.Length - (CONST_MaxVisibleLogSize-4));
            uxProgressBox.SelectionStart = uxProgressBox.Text.Length;
            uxProgressBox.ScrollToCaret();
        }

        private async void clearTargetDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logger.DebugFormat("Asking user to confirm data clear");
            var msResult = MessageBox.Show(string.Format("This will clear all data from Target sandbox for objects listed in Instructions file \r\n {0} \r\n \r\n Are you sure you want to do this?", uxInstructionsFilename.Text),
                "Clear Target Data?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (msResult == System.Windows.Forms.DialogResult.Yes)
            {
                logger.DebugFormat("User confirmed clear of data in {0}", uxInstructionsFilename.Text);
                //ToDo make this an option in the UI
                Boolean ignoreFilters = false;

                ShowUiProcessStarted();
                ShowMessage(string.Format("Starting clear of target data ..."));
                if (ignoreFilters)
                    ShowMessage(string.Format("    (ignoreFilters flag set to true)"));
                try
                {
                    var instructions = LoadInstructions();
                    var progress = new Progress<string>(t => ShowMessage(t));
                    await Task.Factory.StartNew(() => new PopulateSandbox(instructions).DeleteTargetData(progress, ignoreFilters),
                                        TaskCreationOptions.LongRunning);

                    ShowMessage(string.Format("Finished."));

                    ShowUiProcessEnded();
                }
                catch (Exception ex)
                {
                    logger.Error("clearTargetDataToolStripMenuItem_Click caught an error", ex);
                    ShowMessage("******************");
                    ShowMessage(string.Format("Unexpected error: {0} \r\n\r\n See Log.txt file for details", ex.Message));
                    ShowUiProcessEnded();
                    MessageBox.Show(string.Format("Unexpected error: {0} \r\n\r\n See Log.txt file for details", ex.Message), "Unexpected Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                }


            }
        }

        private void uxTestLoginSource_Click(object sender, EventArgs e)
        {
            var sourceCred = GetSourceCredentials();
            var sesh = new SalesforceSession(sourceCred);
            uxTestLoginSourceLabel.Text = "Testing login ...";
            uxTestLoginSourceLabel.Visible = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                sesh.TestLogin();
                uxTestLoginSourceLabel.Text = "Login OK";
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                uxTestLoginSourceLabel.Text = "Login failed";
                Cursor.Current = Cursors.Default;
                MessageBox.Show(string.Format("Test login failed for Source \r\n\r\n {0}", ex.Message), "Test login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void uxTestLoginTarget_Click(object sender, EventArgs e)
        {
            var targetCred = GetTargetCredentials();
            var sesh = new SalesforceSession(targetCred);
            uxTestLoginTargetLabel.Text = "Testing login ...";
            uxTestLoginTargetLabel.Visible = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                sesh.TestLogin();
                uxTestLoginTargetLabel.Text = "Login OK";
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                uxTestLoginTargetLabel.Text = "Login failed";
                Cursor.Current = Cursors.Default;
                MessageBox.Show(string.Format("Test login failed for Target \r\n\r\n {0}", ex.Message), "Test login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
            {
                about.ShowDialog();
            }
        }

       

      
    }
}
