namespace File_Structure_Generator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtRootPath.Text = dlg.SelectedPath;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txtRootPath.Text))
            {
                MessageBox.Show("Invalid root folder.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fileFilters = grpFileTypes.Controls
                .OfType<CheckBox>()
                .Where(cb => cb.Checked)
                .Select(cb => cb.Tag.ToString()!)
                .ToList();

            var includeFolders = grpFolders.Controls
                .OfType<CheckBox>()
                .Where(cb => cb.Checked)
                .Select(cb => cb.Text)
                .ToList();

            var options = new DirectoryScanOptions
            {
                UseRelativePaths = chkRelativePaths.Checked,
                FileFilters = fileFilters,
                IncludedFolders = includeFolders,
                RootPath = txtRootPath.Text
            };

            var tree = DirectoryScanner.GenerateTree(options);
            txtPreview.Text = tree;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPreview.Text))
            {
                MessageBox.Show("Nothing to save. Generate first.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|Markdown (*.md)|*.md",
                FileName = "file_structure.txt"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, txtPreview.Text);
                MessageBox.Show("Saved!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPreview.Text))
            {
                MessageBox.Show("Nothing to copy!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Clipboard.SetText(txtPreview.Text);
            MessageBox.Show("Copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
