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
using System.Diagnostics;

namespace CFW2OFW_TO_PKG
{

    public partial class Form1 : Form
    {
        static string directory,  dir_new, split_dest, replace_string;
        int s = 0;
        int results;
        string[] files;
        private int result;
        string pathSplit;
        string temp = Path.GetTempPath() + "CTP";
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
            CheckForIllegalCrossThreadCalls = false;
            //this.txtFolderPath.Location = new System.Drawing.Point(20, 30);
            //this.txtFolderPath.Size = new System.Drawing.Size(200, 20);
            //this.Controls.Add(this.txtFolderPath);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 1;
            this.txtFolderPath.AllowDrop = true;
            this.txtFolderPath.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.txtFolderPath.DragDrop += new DragEventHandler(Form1_DragDrop);
        }


        

        private void Form1_Load(object sender, EventArgs e)
        {
            Directory.CreateDirectory(temp);
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
                files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (Directory.Exists(files[0]))
                {
                    if (checkBox1.Checked)
                    {

                        this.txtGame.Text = files[0];
                        string dir = this.txtGame.Text;
                        DirectoryInfo dir_info = new DirectoryInfo(dir);
                        pathSplit = dir_info.ToString();
                        directory = dir_info.Name;  // System32
                        replace_string = Path.GetDirectoryName(pathSplit) + @"\";

                        if (Directory.GetFiles(pathSplit, "*.sfo").Length == 0)
                        {
                            MessageBox.Show("Game folder not valid", "CFW2OFW TO PKG", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                        else
                        {
                            if (directory.ToString().Contains("NPEB"))
                            {
                                dir_new = directory.Replace("NPEB", "BLES");
                            }
                            else if (directory.ToString().Contains("NPUB"))
                            {
                                dir_new = directory.Replace("NPUB", "BLUS");

                            }
                           
                            File.WriteAllBytes(temp + "\\cygz.dll", Properties.Resources.cygz);
                            File.WriteAllBytes(temp + "\\psn_package_npdrm.exe", Properties.Resources.psn_package_npdrm);
                            File.WriteAllBytes(temp + "\\dirsplit.exe", Properties.Resources.dirsplit);
                            File.WriteAllBytes(temp + "\\cygwin.dll", Properties.Resources.cygwin1);
                            File.WriteAllBytes(temp + "\\isgsg.dll", Properties.Resources.isgsg);
                            File.WriteAllBytes(temp + "\\nhcolor.exe", Properties.Resources.nhcolor);
                            File.WriteAllBytes(temp + "\\ps3xploit_rifgen_edatresign.exe", Properties.Resources.ps3xploit_rifgen_edatresign);
                            File.WriteAllBytes(temp + "\\sfk.exe", Properties.Resources.sfk);
                            File.WriteAllBytes(temp + "\\sfoprint.exe", Properties.Resources.sfoprint);
                            File.WriteAllBytes(temp + "\\Wprompt.exe", Properties.Resources.Wprompt);



                            richTextBox1.Text += "working path : " + pathSplit + "\n\n";


                            Process p = new Process();
                            p.StartInfo.FileName = temp + "\\dirsplit.exe";
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.Arguments = " \"" + pathSplit + "\" 3685376656";
                            richTextBox1.Text += "dirsplit argument : " + p.StartInfo.FileName + p.StartInfo.Arguments + "\n\n";
                            p.Start();

                            p.WaitForExit();
                            

                            string[] fileEntries = Directory.GetFiles(Environment.CurrentDirectory, "chunk-*.txt");
                            foreach (var file in fileEntries)
                            {
                                s++;
                            }

                            richTextBox2.Text = "[*] Output PKG : " + s.ToString() + "\n";

                            backgroundMoveandCreate.RunWorkerAsync();
                            

                        }


                        
                       
                        
                        /*
                        Process Split = new Process();
                        Split.StartInfo.FileName = temp + "\\make_pkg.bat";
                        Split.StartInfo.CreateNoWindow = true;
                        Split.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

                        Split.Start();
                        Split.WaitForExit();
                        */
                    }
                    else
                    {
                        panel1.Visible = true;
                        panel2.Visible = false;
                        label1.Visible = false;

                        backgroundWorker1.RunWorkerAsync();
                    }
                    

                }
                else
                {
                    MessageBox.Show("Only drag and drop game folder goddamitt!", "CFW2OFW TO PKG", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void createBatandRun()
        {
            File.WriteAllBytes(temp + "\\make_pkg.bat", Properties.Resources.make_pkg);

            string bat = File.ReadAllText(temp + "\\make_pkg.bat");
            string bat_path = Path.GetDirectoryName(temp + "\\make_pkg.bat");
            string text = bat.Replace("tmp=", "tmp=" + bat_path).Replace("src=", "src=" + Environment.CurrentDirectory).Replace("fsz=", "fsz=4190109696").Replace("sfo=", "sfo=" + pathSplit + "\\PARAM.SFO").Replace("fdr=", "fdr=" + pathSplit).Replace("fnm=", "fnm=" + Path.GetDirectoryName(pathSplit)).Replace("dnm=", "dnm=" + directory);
            File.WriteAllText(temp + "\\make_pkg.bat", text);

            
            Process Split = new Process();
            Split.StartInfo.FileName = temp + "\\make_pkg.bat";
            Split.StartInfo.CreateNoWindow = true;
            Split.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            Split.Start();
            Split.WaitForExit();

            result = Split.ExitCode;


        }

        private void backgroundMoveandCreate_DoWork(object sender, DoWorkEventArgs e)
        {
            if (File.Exists("chunk-01.txt"))
            {
                richTextBox1.Text += "chunk-01 exist\n\n";
                string fileName = Environment.CurrentDirectory + "\\chunk-01.txt";
                File.WriteAllText(fileName, File.ReadAllText(fileName).Replace(replace_string, ""));

                System.IO.Directory.CreateDirectory(String.Format(@"{0}/{1}/{2}/{3}", Path.GetDirectoryName(pathSplit), "SPLITTED", "chunk-01", directory));
                split_dest = Path.GetDirectoryName(pathSplit) + "\\SPLITTED\\" + "chunk-01\\" + directory + "\\";
                richTextBox1.Text += "split destination : " + split_dest.ToString() + "\n\n";
                richTextBox1.Text += "replace string destination : " + replace_string.ToString() + "\n\n";


                File.Copy(Environment.CurrentDirectory + "\\chunk-01.txt", Path.GetDirectoryName(pathSplit) + "\\chunk-01.txt", true);
                richTextBox2.Text += "[*] Processing PKG.. Please wait. This may take a few minutes..\n";

                createBatandRun();


                if (result == 0) // success
                {
                    richTextBox2.Text += "[*] Operation completed successfully. Games converted into 2 PKG\n[*] To install on HAN/HEN please enable HAN/HEN first\n";
                }
                else
                {
                    richTextBox2.Text += "[*] Operation failed\n";
                }
            }
            else
            {
                MessageBox.Show("The game has a file of more than 4190109696 byte. This folder can not be divided into parts.", "CFW2OFW TO PKG", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
            MessageBox.Show("Set Game folder (to be converted) and this program in the same directory.\n\nDrag and Drop the game folder anywhere into this program.\n\nIt will produce the PKG version of the game. The PKG can be installed on CFW or HAN PS3.\n\nBefore installing on HAN PS3, please enable debug PKG.", "CFW2OFW TO PKG", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            string path = Environment.CurrentDirectory;
            File.WriteAllBytes("cygz.dll", Properties.Resources.cygz);
            File.WriteAllBytes("libeay32.dll", Properties.Resources.libeay32);
            File.WriteAllBytes("MSCOMCTL.OCX", Properties.Resources.MSCOMCTL);
            File.WriteAllBytes("mswinsck.ocx", Properties.Resources.mswinsck);
            File.WriteAllBytes("psn_package_npdrm.exe", Properties.Resources.psn_package_npdrm);
            File.WriteAllBytes("zlib1.dll", Properties.Resources.zlib1);



            this.txtGame.Text = files[0];
            string dir = this.txtGame.Text;
            DirectoryInfo dir_info = new DirectoryInfo(dir);
            pathSplit = dir_info.ToString();
            directory = dir_info.Name;  // System32

            string bat = "@echo off\npsn_package_npdrm.exe -n " + directory + ".CONF " + directory;
            System.IO.File.WriteAllText("go.bat", bat);

            string conf = "ContentID        = UP0001-" + directory + "_00-1111222233334444\nKlicensee        = 0x00000000000000000000000000000000\nDRMType          = Free\nContentType      = GameExec\nPackageVersion   = 01.00";
            System.IO.File.WriteAllText(directory + ".CONF", conf);

            Process p = new Process();
            p.StartInfo.FileName = "go.bat";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.Start();

            this.Text += " | Converting " + directory.ToString();
            this.Refresh();



            //System.Diagnostics.Process.Start("go.bat");

            p.WaitForExit();
            result = p.ExitCode;
        }

        

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            label1.Visible = true;

            this.Text = "CFW2OFW TO PKG";
            this.Refresh();
            if (result == 0) // success
            {
                delete();

                MessageBox.Show("Operation completed successfully", "CFW2OFW TO PKG", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                delete();

                MessageBox.Show("Operation failed", "CFW2OFW TO PKG", MessageBoxButtons.OK, MessageBoxIcon.Hand);

            }

            txtGame.Clear();
        }

        void delete()
        {
            System.IO.File.Delete("go.bat");
            System.IO.File.Delete(directory + ".CONF");
            System.IO.File.Delete("cygz.dll");
            System.IO.File.Delete("libeay32.dll");
            System.IO.File.Delete("MSCOMCTL.OCX");
            System.IO.File.Delete("mswinsck.ocx");
            System.IO.File.Delete("psn_package_npdrm.exe");
            System.IO.File.Delete("zlib1.dll");
        }


        private static void CopyDirectory()
        {

           

           
        }

    }
}
