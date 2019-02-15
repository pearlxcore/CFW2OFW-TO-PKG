using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;


namespace CFW2OFW_TO_PKG
{

    public partial class Form1 : Form
    {

        private static void extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            
                using (BinaryReader r = new BinaryReader(s))
                
                    using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
                    
                        using (BinaryWriter w = new BinaryWriter(fs))
                        
                            w.Write(r.ReadBytes((int)s.Length));
                        
        }

        private void txtGame_TextChanged(object sender, EventArgs e)
        {

        }




        public Form1()
        {
            InitializeComponent();

            //this.txtFolderPath.Location = new System.Drawing.Point(20, 30);
            //this.txtFolderPath.Size = new System.Drawing.Size(200, 20);
            //this.Controls.Add(this.txtFolderPath);

            this.txtFolderPath.AllowDrop = true;
            this.txtFolderPath.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.txtFolderPath.DragDrop += new DragEventHandler(Form1_DragDrop);
        }


        

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
            txtFolderPath.Enabled = false;
            

            //System.IO.File.Copy(path + "\\Temp", Properties.Resources.zip);

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (Directory.Exists(files[0]))
                {
                    string path = Environment.CurrentDirectory;
                    extract("CFW2OFW_TO_PKG", path, "resources", "cygz.dll");
                    extract("CFW2OFW_TO_PKG", path, "resources", "libeay32.dll");
                    extract("CFW2OFW_TO_PKG", path, "resources", "MSCOMCTL.OCX");
                    extract("CFW2OFW_TO_PKG", path, "resources", "mswinsck.ocx");
                    extract("CFW2OFW_TO_PKG", path, "resources", "psn_package_npdrm.exe");
                    extract("CFW2OFW_TO_PKG", path, "resources", "zlib1.dll");

                    this.txtGame.Text = files[0];
                    string dir = this.txtGame.Text;
                    DirectoryInfo dir_info = new DirectoryInfo(dir);
                    string directory = dir_info.Name;  // System32
                    MessageBox.Show(directory, "GAME ID");

                    string bat = "@echo off\npsn_package_npdrm.exe -n " + directory + ".CONF " + directory;
                    System.IO.File.WriteAllText("go.bat", bat);

                    string conf = "ContentID        = UP0001-" + directory + "_00-1111222233334444\nKlicensee        = 0x00000000000000000000000000000000\nDRMType          = Free\nContentType      = GameExec\nPackageVersion   = 01.00";
                    System.IO.File.WriteAllText(directory + ".CONF", conf);

                    //System.Diagnostics.Process.Start("go.bat");
                    var process = System.Diagnostics.Process.Start("go.bat");
                    process.WaitForExit();
                    System.IO.File.Delete("go.bat");
                    System.IO.File.Delete(directory + ".CONF");
                    System.IO.File.Delete("cygz.dll");
                    System.IO.File.Delete("libeay32.dll");
                    System.IO.File.Delete("MSCOMCTL.OCX");
                    System.IO.File.Delete("mswinsck.ocx");
                    System.IO.File.Delete("psn_package_npdrm.exe");
                    System.IO.File.Delete("zlib1.dll");

                    MessageBox.Show("Done!", "Info");
                    txtGame.Clear();
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Set Game folder (to be converted) and this program in the same directory.\n\nDrag and Drop the game folder anywhere into this program.", "CFW2OFW TO PKG");
        }
    }
}
