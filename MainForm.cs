using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace File_Structure_Generator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        // ============================================================
        // BROWSE BUTTON
        // ============================================================
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtRootPath.Text = dlg.SelectedPath;
            }
        }

        // ============================================================
        // GENERATE BUTTON
        // ============================================================
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtRootPath.Text))
            {
                MessageBox.Show("Invalid root folder.");
                return;
            }

            // Collect file pattern list
            var fileFilters = grpFileTypes.Controls
                .OfType<CheckBox>()
                .Where(cb => cb.Checked)
                .Select(cb => cb.Tag.ToString())
                .ToList();

            // Collect included static folders from the folder-checkbox group
            var staticFolders = grpFolders.Controls
                .OfType<CheckBox>()
                .Where(cb => cb.Checked)
                .Select(cb => cb.Text)
                .ToList();

            var options = new DirectoryScanOptions
            {
                RootPath = txtRootPath.Text,
                UseRelativePaths = chkRelativePaths.Checked,
                FileFilters = fileFilters,
                IncludedFolders = staticFolders  // << CORRECT PROPERTY
            };

            string result = DirectoryScanner.BuildTree(options);

            txtPreview.Text = result;
        }

        // ============================================================
        // COPY BUTTON
        // ============================================================
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPreview.Text))
            {
                Clipboard.SetText(txtPreview.Text);
                //MessageBox.Show("Copied.");
            }
        }

        // ============================================================
        // SAVE BUTTON
        // ============================================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPreview.Text))
            {
                MessageBox.Show("Nothing to save.");
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Text Files|*.txt";
            sfd.FileName = "structure.txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, txtPreview.Text);
                MessageBox.Show("Saved.");
            }
        }

        // ============================================================
        // PREVIEW POPUP (OPTION B)
        // Button exists, only showing popup — NO HTML, NO TEMPLATE
        // ============================================================
        private void btnPreview_Click(object sender, EventArgs e)
        {
            string previewText = txtPreview.Text;
            if (string.IsNullOrWhiteSpace(previewText))
            {
                MessageBox.Show("Generate a preview first.");
                return;
            }

            var popup = new PreviewForm("Preview Output", previewText);
            popup.Show();
        }
    }
}
