﻿using ROFLPlayer.Models;
using ROFLPlayer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROFLPlayer
{
    public partial class ExecAddForm : Form
    {

        public LeagueExecutable NewLeagueExec { get; }

        public ExecAddForm()
        {
            InitializeComponent();
            InitForm();
            NewLeagueExec = new LeagueExecutable();
        }

        private void InitForm()
        {
            this.ExecNameTextBox.AutoSize = false;
            this.ExecNameTextBox.Size = new Size(245, 23);

            this.ExecTargetTextBox.AutoSize = false;
            this.ExecTargetTextBox.Size = new Size(245, 23);

            this.ExecStartTextBox.AutoSize = false;
            this.ExecStartTextBox.Size = new Size(245, 23);
        }

        private void ExecBrowseButton_Click(object sender, EventArgs e)
        {
            
            var dialog = new OpenFileDialog
            {
                // Filter out only exes
                Filter = "LeagueClient.exe or League of Legends.exe (*.exe)|*.exe",
                // Show only files starting with "League"
                FileName = "League*",
                // Only allow one file to be selected
                Multiselect = false,
                // Set title of dialog
                Title = "Select League of Legends client"
            };
            // Wait for user to press ok
            while (dialog.ShowDialog() == DialogResult.OK)
            {
                var filepath = dialog.FileName;
                // They didn't select anything, do nothing
                if (string.IsNullOrEmpty(filepath) || filepath.Equals("League*"))
                {
                    return;
                }

                string gamePath = "";
                // Now did they select leagueclient or league of legends?
                switch (Path.GetFileName(filepath))
                {
                    case "LeagueClient.exe":
                        {
                            // Enable update checkbox if ever disabled
                            ExecUpdateCheckbox.Enabled = true;
                            try
                            {
                                // Find the league of legends.exe using game locator
                                gamePath = GameLocator.FindLeagueExecutable(Path.GetDirectoryName(filepath));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Could not find League of Legends executable, please try again\n\n" + ex.Message, "Error finding game executable", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            break;
                        }
                    case "League of Legends.exe":
                        {
                            // Disable update checkbox, could cause big problems
                            ExecUpdateCheckbox.Checked = false;
                            ExecUpdateCheckbox.Enabled = false;
                            gamePath = filepath;
                            break;
                        }
                }

                this.ExecTargetTextBox.Text = gamePath;
                this.ExecStartTextBox.Text = Path.GetDirectoryName(filepath);
                NewLeagueExec.TargetPath = gamePath;
                NewLeagueExec.StartFolder = Path.GetDirectoryName(filepath);

                var fileInfo = FileVersionInfo.GetVersionInfo(gamePath);

                this.GBoxExecNameTextBox.Text = Path.GetFileName(gamePath);
                this.GBoxPatchVersTextBox.Text = fileInfo.FileVersion;
                this.GBoxFileDescTextBox.Text = fileInfo.FileDescription;

                NewLeagueExec.ModifiedDate = File.GetLastWriteTime(gamePath);
                this.GBoxLastModifTextBox.Text = NewLeagueExec.ModifiedDate.ToString("yyyy/dd/MM");

                return;
            }
        }

        private void ExecCancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ExecUpdateCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            NewLeagueExec.EnableUpdates = this.ExecUpdateCheckbox.Checked;
        }

        private void ExecUpdateCheckbox_ToolTip(object sender, EventArgs e)
        {
            CheckBox updateBox = (CheckBox)sender;

            var visTime = 3000;

            var toolTip = new ToolTip();
            toolTip.Show("ROFLPlayer can automatically update target path when League of Legends updates", updateBox, 0, 20, visTime);
        }

        private void ExecSaveButton_Click(object sender, EventArgs e)
        {
            NewLeagueExec.Name = this.ExecNameTextBox.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
