using LSLib.LS;
using LSLib.LS.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ConverterApp
{
    public partial class OsirisPane : UserControl
    {
        public Game Game;

        public OsirisPane(ISettingsDataSource settingsDataSource)
        {
            InitializeComponent();

            storyFilePath.DataBindings.Add("Text", settingsDataSource, "Settings.Story.InputPath", true, DataSourceUpdateMode.OnPropertyChanged);
            goalPath.DataBindings.Add("Text", settingsDataSource, "Settings.Story.OutputPath", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void storyFileBrowseBtn_Click(object sender, EventArgs e)
        {
            if (storyPathDlg.ShowDialog(this) == DialogResult.OK)
            {
                storyFilePath.Text = storyPathDlg.FileName;
            }
        }

        private void goalPathBrowseBtn_Click(object sender, EventArgs e)
        {
            if (goalPathDlg.ShowDialog(this) == DialogResult.OK)
            {
                goalPath.Text = goalPathDlg.SelectedPath;
            }
        }

        private void LoadStory(Stream s)
        {
        }

        public Resource LoadResourceFromSave(string path)
        {
            throw new NotImplementedException();
        }

        private void loadStoryBtn_Click(object sender, EventArgs e)
        {
            string extension = Path.GetExtension(storyFilePath.Text)?.ToLower();

            switch (extension)
            {
                case ".lsv":
                {
                    var resource = LoadResourceFromSave(storyFilePath.Text);
                    if (resource == null) return;

                    LSLib.LS.Node storyNode = resource.Regions["Story"].Children["Story"][0];
                    var storyStream = new MemoryStream(storyNode.Attributes["Story"].Value as byte[] ?? throw new InvalidOperationException("Cannot proceed with null Story node"));

                    LoadStory(storyStream);

                    MessageBox.Show("Save game database loaded successfully.");
                    break;
                }
                case ".osi":
                {
                    using (var file = new FileStream(storyFilePath.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        LoadStory(file);
                    }

                    MessageBox.Show("Story file loaded successfully.");
                    break;
                }
                default:
                {
                    MessageBox.Show($"Unsupported file extension: {extension}", "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }

        private void SaveSavegameDatabase()
        {
            throw new NotImplementedException();
        }

        private void SaveStory()
        {
        }

        private void saveStoryBtn_Click(object sender, EventArgs e)
        {

            string extension = Path.GetExtension(storyFilePath.Text)?.ToLower();

            switch (extension)
            {
                case ".lsv":
                {
                    SaveSavegameDatabase();
                    MessageBox.Show("Save game database save successful.");
                    break;
                }
                case ".osi":
                {
                    SaveStory();
                    MessageBox.Show("Story file save successful.");
                    break;
                }
                default:
                {
                    MessageBox.Show($"Unsupported file extension: {extension}", "Story save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }

        private void decompileStoryBtn_Click(object sender, EventArgs e)
        {
        }

        private void databaseSelectorCb_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnDebugExport_Click(object sender, EventArgs e)
        {
        }
    }
}
