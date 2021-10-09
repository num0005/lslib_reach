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
            var packageReader = new PackageReader(path);
            Package package = packageReader.Read();
            
            AbstractFileInfo abstractFileInfo = package.Files.FirstOrDefault(p => p.Name.ToLowerInvariant() == "globals.lsf");
            if (abstractFileInfo == null)
            {
                MessageBox.Show("The specified package is not a valid savegame (globals.lsf not found)", "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            Resource resource;
            Stream rsrcStream = abstractFileInfo.MakeStream();
            try
            {
                using (var rsrcReader = new LSFReader(rsrcStream))
                {
                    resource = rsrcReader.Read();
                }
            }
            finally
            {
                abstractFileInfo.ReleaseStream();
            }

            return resource;
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
            var conversionParams = ResourceConversionParameters.FromGameVersion(Game);
            var packageReader = new PackageReader(storyFilePath.Text);
            Package package = packageReader.Read();

            AbstractFileInfo globalsLsf = package.Files.FirstOrDefault(p => p.Name.ToLowerInvariant() == "globals.lsf");
            if (globalsLsf == null)
            {
                MessageBox.Show("The specified package is not a valid savegame (globals.lsf not found)", "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load globals.lsf
            Resource resource;
            Stream rsrcStream = globalsLsf.MakeStream();
            try
            {
                using (var rsrcReader = new LSFReader(rsrcStream))
                {
                    resource = rsrcReader.Read();
                }
            }
            finally
            {
                globalsLsf.ReleaseStream();
            }


            // Save globals.lsf
            var rewrittenStream = new MemoryStream();
            var rsrcWriter = new LSFWriter(rewrittenStream);
            rsrcWriter.Version = conversionParams.LSF;
            rsrcWriter.EncodeSiblingData = false;
            rsrcWriter.Write(resource);
            rewrittenStream.Seek(0, SeekOrigin.Begin);

            // Re-package global.lsf
            var rewrittenPackage = new Package();
            StreamFileInfo globalsRepacked = StreamFileInfo.CreateFromStream(rewrittenStream, "globals.lsf");
            rewrittenPackage.Files.Add(globalsRepacked);

            List<AbstractFileInfo> files = package.Files.Where(x => x.Name.ToLowerInvariant() != "globals.lsf").ToList();
            rewrittenPackage.Files.AddRange(files);

            using (var packageWriter = new PackageWriter(rewrittenPackage, $"{storyFilePath.Text}.tmp"))
            {
                packageWriter.Version = conversionParams.PAKVersion;
                packageWriter.Compression = CompressionMethod.Zlib;
                packageWriter.CompressionLevel = CompressionLevel.DefaultCompression;
                packageWriter.Write();
            }

            rewrittenStream.Dispose();
            packageReader.Dispose();

            // Create a backup of the original .lsf
            string backupPath = $"{storyFilePath.Text}.backup";
            if (!File.Exists(backupPath))
            {
                File.Move(storyFilePath.Text, backupPath);
            }
            else
            {
                File.Delete(storyFilePath.Text);
            }

            // Replace original savegame with new one
            File.Move($"{storyFilePath.Text}.tmp", storyFilePath.Text);
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
