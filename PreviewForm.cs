using System;
using System.IO;
using System.Windows.Forms;

namespace File_Structure_Generator
{
    public partial class PreviewForm : Form
    {
        private readonly string _markdownText;
        private readonly string _htmlText;

        public PreviewForm(string markdownText, string htmlText)
        {
            InitializeComponent();
            _markdownText = markdownText;
            _htmlText = htmlText;
        }

        private void PreviewForm_Load(object sender, EventArgs e)
        {
            // Load Markdown into TextBox
            txtMarkdown.Text = _markdownText;

            // Load HTML into WebBrowser
            if (!string.IsNullOrWhiteSpace(_htmlText))
            {
                webBrowser.DocumentText = _htmlText;
            }
            else
            {
                webBrowser.DocumentText = "<html><body><p>No HTML output was generated.</p></body></html>";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
