using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ARMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        DateTime date1;
        public Thread t;
        public delegate void MyDelegate(TreeNode addNode, TreeNode node, string pathTree);
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                textBoxDirectory.Text = FBD.SelectedPath;
            }
        }
        private void GetDirectories(string path, TreeNode nodeToAddTo, string directory)
        {                        
            string[] dirs = Directory.GetDirectories(path);

            DirectoryInfo rootDir = new DirectoryInfo(path);           
            foreach (string dir in dirs)
            {
                DirectoryInfo di = new DirectoryInfo(dir);
                TreeNode node = new TreeNode(di.Name);
                BeginInvoke(new MyDelegate(AddNode), nodeToAddTo, node, path);
                Thread.Sleep(40);
                try
                {                   
                    foreach (var dire in di.GetDirectories())
                    {
                        TreeNode n = new TreeNode(dire.Name);
                        BeginInvoke(new MyDelegate(AddNode), node, n, path);
                        Thread.Sleep(40);
                        GetDirectories(dire.FullName, n, directory);
                    }    
                    foreach (var file in di.GetFiles(directory))
                    {                       
                        TreeNode n = new TreeNode(file.Name);
                        BeginInvoke(new MyDelegate(AddNode), node, n, path);
                        Thread.Sleep(40);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    
                }                                
            }
            foreach (var file in rootDir.GetFiles(directory))
            {
                
                TreeNode n = new TreeNode(file.Name);
                BeginInvoke(new MyDelegate(AddNode), nodeToAddTo, n, path);
                Thread.Sleep(40);

            }
        }

        

        void AddNode(TreeNode addNode, TreeNode node, string pathTree)
        {
            addNode.Nodes.Add(node);
            LabelCountFile.Text = Convert.ToString(Convert.ToInt32(LabelCountFile.Text) + 1);
            LabelFile.Text = pathTree + "\\" + node.Text;
            
        }
        private void button2_Click(object sender, EventArgs e)
        {          
            treeView1.Nodes.Clear();
            LabelCountFile.Text = "0";
            TreeNode node = new TreeNode(textBoxDirectory.Text);            
            treeView1.Nodes.Add(node);         
            if (t != null)
            {
                t.Abort();
            }
            string fileName = Application.StartupPath.ToString() + "\\1.txt";           
            using (StreamWriter sw = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
            {
                sw.WriteLine(textBoxDirectory.Text + "\n" + textBoxFileName.Text);
            }
                                                     
            date1 = new DateTime(0, 0);
            t = new Thread(delegate () { GetDirectories(textBoxDirectory.Text, node, textBoxFileName.Text); });
            t.IsBackground = true;
            timer1.Enabled = true;
            timer1.Start();
            t.Start();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            t.Abort();
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
            if (t.IsAlive)
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
                }
                catch
                {                    
                }
            }    
            
        }
    }
}
