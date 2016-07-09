using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;
using FileSystem=System.IO;

namespace Batch_Priority
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path;
        string filePath;
        string filename;
        string name;
        string saveLoc;
        string prio;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            name = txtName.Text;

            FileSystem.Directory.CreateDirectory(path + "\\BatchPriority");
            if (Environment.Is64BitOperatingSystem)
            {
                FileSystem.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "elevate 64\\elevate.exe", path + "\\BatchPriority\\elevate.exe");
            }else
            {
                FileSystem.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "elevate 32\\elevate.exe", path + "\\BatchPriority\\elevate.exe");
            }

            FileSystem.StreamWriter sw = new System.IO.StreamWriter(path + "\\BatchPriority\\start.bat");
            sw.WriteLine("Start \"" + name + "\" \"" + filePath + "\"");
            if(prio == "Real Time")
            {
                sw.WriteLine("wmic process where name=\"" + filename + "\" CALL setpriority 256");
            }
            else if (prio == "High")
            {
                sw.WriteLine("wmic process where name=\"" + filename + "\" CALL setpriority 128");
            }
            else if (prio == "Above Normal")
            {
                sw.WriteLine("wmic process where name=\"" + filename + "\" CALL setpriority 32768");
            }
            else if (prio == "Normal")
            {
                sw.WriteLine("wmic process where name=\"" + filename + "\" CALL setpriority 32");
            }
            else if (prio == "Below Normal")
            {
                sw.WriteLine("wmic process where name=\"" + filename + "\" CALL setpriority 16384");
            }
            else if (prio == "Idle")
            {
                sw.WriteLine("wmic process where name=\"" + filename + "\" CALL setpriority 64");
            }
            sw.Close();

            FileSystem.StreamWriter sw2 = new System.IO.StreamWriter(path + "\\BatchPriority\\launch.bat");
            sw2.WriteLine("\"" + path + "BatchPriority\\elevate.exe\" \"" + path + "\\BatchPriority\\start.bat\"");
            sw2.Close();

            WshShell wsh = new WshShell();
            IWshShortcut shortcut = wsh.CreateShortcut(saveLoc + "\\" + name + ".lnk");
            shortcut.TargetPath = path + "\\BatchPriority\\launch.bat";
            shortcut.IconLocation = filePath;
            shortcut.Save();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "Select path";
            Forms.DialogResult result = ofd.ShowDialog();
            if(result == Forms.DialogResult.OK)
            {
                if (FileSystem.File.Exists(ofd.FileName))
                {
                    path = ofd.FileName.Remove(ofd.FileName.Length - ofd.SafeFileName.Length);
                    filename = ofd.SafeFileName;
                    filePath = ofd.FileName;
                    txtPath.Text = path;
                }else
                {
                    MessageBox.Show("Can't find the selected file!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnBrowseWhere_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            Forms.DialogResult result = fbd.ShowDialog();
            if(result == Forms.DialogResult.OK)
            {
                if (FileSystem.Directory.Exists(fbd.SelectedPath))
                {
                    saveLoc = fbd.SelectedPath;
                    txtWhere.Text = saveLoc;
                }
                else
                {
                    MessageBox.Show("Can't find the selected folder!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void cmbPriority_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbPriority.Text == "Real Time")
            {
                MessageBox.Show("WARNING: Running processes at Real Time may cause an instabel system!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            prio = cmbPriority.Text;
        }
    }
}
