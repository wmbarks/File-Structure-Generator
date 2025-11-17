using System.Drawing;
using System.Windows.Forms;

namespace File_Structure_Generator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtRootPath = new TextBox();
            btnBrowse = new Button();
            grpFileTypes = new GroupBox();
            grpFolders = new GroupBox();
            chkRelativePaths = new CheckBox();
            btnGenerate = new Button();
            btnCopy = new Button();
            btnSave = new Button();
            txtPreview = new TextBox();

            SuspendLayout();

            // ============================
            // Path textbox
            // ============================
            txtRootPath.Location = new Point(12, 12);
            txtRootPath.Size = new Size(650, 31);

            // ============================
            // Browse Button
            // ============================
            btnBrowse.Location = new Point(670, 12);
            btnBrowse.Size = new Size(90, 31);
            btnBrowse.Text = "Browse";
            btnBrowse.Click += btnBrowse_Click;

            // ============================
            // File Types Group
            // ============================
            grpFileTypes.Text = "File Types to Include";
            grpFileTypes.Location = new Point(12, 70);
            grpFileTypes.Size = new Size(300, 260);

            // ============================
            // Folders Group
            // ============================
            grpFolders.Text = "Folders to Include";
            grpFolders.Location = new Point(330, 70);
            grpFolders.Size = new Size(300, 260);

            Controls.Add(grpFileTypes);
            Controls.Add(grpFolders);

            // ============================
            // ALWAYS add file type checkboxes
            // ============================
            AddFileTypeCheckbox("*.cs");
            AddFileTypeCheckbox("*.csproj");
            AddFileTypeCheckbox("*.py");
            AddFileTypeCheckbox("*.json");
            AddFileTypeCheckbox("*.md");
            AddFileTypeCheckbox("*.txt");
            AddFileTypeCheckbox("*.xml");
            AddFileTypeCheckbox("*.yml");

            // ============================
            // UNIVERSAL STATIC FOLDERS
            // ============================
            // Checked = include
            // Unchecked = skip
            AddFolderCheckbox(".git", true);
            AddFolderCheckbox(".vs", true);
            AddFolderCheckbox("bin", true);
            AddFolderCheckbox("obj", true);
            AddFolderCheckbox("Properties", true);
            AddFolderCheckbox("Resources", true);

            // ============================
            // Relative checkbox
            // ============================
            chkRelativePaths.Location = new Point(660, 70);
            chkRelativePaths.Text = "Relative paths";
            chkRelativePaths.AutoSize = true;

            // ============================
            // Generate Button
            // ============================
            btnGenerate.Location = new Point(660, 120);
            btnGenerate.Size = new Size(100, 32);
            btnGenerate.Text = "Generate";
            btnGenerate.Click += btnGenerate_Click;

            // ============================
            // Copy Button
            // ============================
            btnCopy.Location = new Point(660, 160);
            btnCopy.Size = new Size(100, 32);
            btnCopy.Text = "Copy";
            btnCopy.Click += btnCopy_Click;

            // ============================
            // Save Button
            // ============================
            btnSave.Location = new Point(660, 200);
            btnSave.Size = new Size(100, 32);
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;

            // ============================
            // Preview textbox
            // ============================
            txtPreview.Location = new Point(12, 350);
            txtPreview.Size = new Size(795, 350);
            txtPreview.Multiline = true;
            txtPreview.ScrollBars = ScrollBars.Both;
            txtPreview.Font = new Font("Consolas", 9F);

            // ============================
            // Add remaining controls
            // ============================
            Controls.Add(txtRootPath);
            Controls.Add(btnBrowse);
            Controls.Add(chkRelativePaths);
            Controls.Add(btnGenerate);
            Controls.Add(btnCopy);
            Controls.Add(btnSave);
            Controls.Add(txtPreview);

            ClientSize = new Size(820, 720);
            Text = "File Structure Generator";

            ResumeLayout(false);
            PerformLayout();
        }

        // ===========================================================
        // THESE MUST EXIST IN THIS PARTIAL CLASS
        // ===========================================================

        private void AddFileTypeCheckbox(string pattern)
        {
            var cb = new CheckBox
            {
                Text = pattern,
                Tag = pattern,
                AutoSize = true,
                Checked = false,
                Location = new Point(10, 20 + (grpFileTypes.Controls.Count * 25))
            };

            grpFileTypes.Controls.Add(cb);
        }

        private void AddFolderCheckbox(string folder, bool defaultChecked)
        {
            var cb = new CheckBox
            {
                Text = folder,
                AutoSize = true,
                Checked = defaultChecked,
                Location = new Point(10, 20 + (grpFolders.Controls.Count * 25))
            };

            grpFolders.Controls.Add(cb);
        }

        private TextBox txtRootPath;
        private Button btnBrowse;
        private GroupBox grpFileTypes;
        private GroupBox grpFolders;
        private CheckBox chkRelativePaths;
        private Button btnGenerate;
        private Button btnCopy;
        private Button btnSave;
        private TextBox txtPreview;
    }
}
