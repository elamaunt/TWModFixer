using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace TWModFixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetYour_Click(object sender, RoutedEventArgs e)
        {
            YourFile.Text = GetFileByBrowser();
        }

        private void SetOther_Click(object sender, RoutedEventArgs e)
        {
            OtherFile.Text = GetFileByBrowser();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            var yourFilePath = YourFile.Text;
            var otherFilePath = OtherFile.Text;

            if (!TestFile(yourFilePath))
            {
                MessageBox.Show("Bad path of Etalon file");
                return;
            }
            if (!TestFile(otherFilePath))
            {
                MessageBox.Show("Bad path of Fix file");
                return;
            }

            try
            {
                var yourFile = GetParsedFile(yourFilePath);
                var otherFile = GetParsedFile(otherFilePath);
               
                if (yourFile.IsNullOrEmpty() && otherFile.IsNullOrEmpty())
                    return;
                
                // Для каждого активного мод, должен быть установлен такой же
                foreach (var mod in yourFile.Where(x => x.active))
                {
                    var sameMod = otherFile.FirstOrDefault(x => x.uuid == mod.uuid);

                    if (sameMod == null)
                    {
                        var id = GetModId(mod);

                        if (StringComparer.OrdinalIgnoreCase.Compare(id, "Data") == 0)
                            MessageBox.Show("Mod not installed: " + mod.uuid);
                        else
                            if (MessageBox.Show("Mod not installed: " + mod.uuid + ". Open it in browser?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=" + id);
                        return;
                    }
                }

                // Проставляем активность
                foreach (var mod in otherFile)
                    mod.active = yourFile.Where(x => x.active).Any(x => x.uuid == mod.uuid);

                var uuids = yourFile.Select(m => m.uuid).ToArray();

                // Фиксим порядок
                otherFile = otherFile.OrderBy(x => uuids.IndexSelector(x.uuid)).ToArray();

                File.WriteAllText(otherFilePath, JsonConvert.SerializeObject(otherFile));
                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +" "+ ex.StackTrace);
            }
        }

        private string GetModId(Mod mod)
        {
            return Path.GetFileName(Path.GetDirectoryName(mod.packfile));
        }

        private Mod[] GetParsedFile(string yourFilePath)
        {
            return JsonConvert.DeserializeObject<Mod[]>(File.ReadAllText(yourFilePath));
        }

        private bool TestFile(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        }

        public string GetFileByBrowser(string filter = null)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = filter;

            fileDialog.ShowDialog();

            return fileDialog.FileName;
        }
    }
}
