﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] split;
        DateTime date1;
        public Thread t;
        static ManualResetEventSlim mre;
        public delegate void MyDelegate1(TreeNode addNode, TreeNode node);
        public delegate void MyDelegate2(string path);
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                textBoxDirectory.Text = FBD.SelectedPath;
            }
        }
        
        private void Fill(string path, TreeNode nodeToAddTo, string directory)
        {
            mre.Wait();
            DirectoryInfo rootDir = new DirectoryInfo(path);
            try
            {
                foreach (var dire in rootDir.GetDirectories())
                {
                    TreeNode n = new TreeNode(dire.Name);
                    BeginInvoke(new MyDelegate1(AddNode), nodeToAddTo, n);
                    Thread.Sleep(40);
                    Fill(dire.FullName, n, directory);
                }

                foreach (var file in rootDir.GetFiles(directory))
                {
                    string[] lines = File.ReadAllLines(file.FullName, Encoding.Default);
                    BeginInvoke(new MyDelegate2(CurrentFile), file.FullName);
                    bool text = false;
                    foreach (string line in lines)
                    {
                        if (text == false)
                        {
                            foreach (string sp in split)
                            {                           
                                if (line.Contains(sp))
                                {
                                    TreeNode n = new TreeNode(file.Name);
                                    BeginInvoke(new MyDelegate1(AddNode), nodeToAddTo, n);
                                    Thread.Sleep(10);
                                    text = true;
                                    break;
                                }                            
                            }
                        }
                        else
                        {
                            break;
                        }
                    }                    
                }
            }
            catch (UnauthorizedAccessException)
            {

            }                        
        }

        void CurrentFile(string path)
        {
            LabelFile.Text = path;
            LabelCountFile.Text = Convert.ToString(Convert.ToInt32(LabelCountFile.Text) + 1);
        }

        void AddNode(TreeNode addNode, TreeNode node)
        {
            addNode.Nodes.Add(node);                            
        }
        private void button2_Click(object sender, EventArgs e)
        {          
            treeView1.Nodes.Clear();
            LabelFile.Text = "";
            LabelCountFile.Text = "0";
            TreeNode node = new TreeNode(textBoxDirectory.Text);            
            treeView1.Nodes.Add(node);         
            if (t != null)
            {
                t.Abort();
            }
            string fileName = Application.StartupPath.ToString() + "\\1.txt";
            split = textBoxSymbol.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
            {
                sw.WriteLine(textBoxDirectory.Text + "\n" + textBoxFileName.Text + "\n" + textBoxSymbol.Text);
            }
                                                     
            date1 = new DateTime(0, 0);  
            mre = new ManualResetEventSlim(true);
            t = new Thread(delegate () { Fill(textBoxDirectory.Text, node, textBoxFileName.Text); });            
            t.IsBackground = true;
            timer1.Enabled = true;
            timer1.Start();
            t.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mre.Reset();        
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mre.Set();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {    
            if (t != null)
            {
                t.Abort();
            }
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mre.IsSet && t.IsAlive)
            {
                date1 = date1.AddSeconds(1);
                LabelTime.Text = date1.ToString("mm:ss");
            }
                
                                      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            string fileName = Application.StartupPath.ToString() + "\\1.txt";
            if (File.Exists(fileName) != true)
            {
                using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
                {
                    
                }
            }
            else
            {
                try
                {
                    string[] lines = File.ReadAllLines(fileName);
                    textBoxDirectory.Text = lines[0];
                    textBoxFileName.Text = lines[1];
                    textBoxSymbol.Text = lines[2];
                }
                catch
                {                    
                }
            }    
            
        }

        private void textBoxFileName_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.SetToolTip(textBoxFileName, "Введите имя файла.\n Примеры: 1.txt; *.txt; 1.*; *.*");
        }

        private void textBoxSymbol_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.SetToolTip(textBoxSymbol, "Введите нужные символы чере пробел");
        }
    }
}
