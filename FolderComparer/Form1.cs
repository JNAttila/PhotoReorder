using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderComparer
{
    public partial class Form1 : Form
    {
        // utoljára megnyitott könyvtár
        string lastFolder = null;

        public Form1()
        {
            InitializeComponent();
        }

        List<MyFolderInfo> myFolderInfoList = new List<MyFolderInfo>();

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            btnSelectFolder.Enabled = false;

            var _folder = GetFolder();
            var di = new DirectoryInfo(_folder);
            if (di == null) return;

            textBox1.Text = "";
            myFolderInfoList.Clear();

            var fileColl = di.EnumerateDirectories("*", SearchOption.AllDirectories);
            foreach (var x in fileColl)
            {
                //textBox1.Text += x.FullName + Environment.NewLine;
                textBox1.Text += ProcessFolder(x.FullName);
            }

            btnSelectFolder.Enabled = true;

            textBox1.Text += "OK" + Environment.NewLine;
        }

        private string ProcessFolder(string path)
        {
            string result = "";

            var x = new MyFolderInfo(path);

            foreach (var item in myFolderInfoList)
            {
                if (item.AreEquals(x))
                {
                    result += item.Path + Environment.NewLine;
                    result += x.Path + Environment.NewLine;
                    //result += "FileNumber = " + x.FileNumber + Environment.NewLine;
                    //result += "SumFileSize = " + x.SumFileSize + Environment.NewLine;
                    result += "------------------------------" + Environment.NewLine;

                    break;
                }
            }

            if (result == "")
            {
                myFolderInfoList.Add(x);
            }

            return result;
        }

        private string GetFolder()
        {
            var result = "";

            var fod = new FolderBrowserDialog();
            fod.ShowNewFolderButton = false;
            fod.SelectedPath = lastFolder;
            if (fod.ShowDialog() == DialogResult.OK)
            {
                result = fod.SelectedPath;
            }

            lastFolder = result;

            return result;
        }
    }
}
