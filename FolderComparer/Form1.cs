﻿using System;
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
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var _folder = GetFolder();
            //MessageBox.Show(_folder);
            var di = new DirectoryInfo(_folder);

            if (di == null) return;

            textBox1.Text = "";
            var fileColl = di.EnumerateDirectories();
            foreach (var x in fileColl)
            {
                textBox1.Text += ProcessFolder(x.FullName);
            }
        }

        private string ProcessFolder(string path)
        {
            string result = "";

            var x = new MyFolderInfo(path);

            result += x.Path + Environment.NewLine;
            result += "FileNumber = " + x.FileNumber + Environment.NewLine;
            result += "SumFileSize = " + x.SumFileSize + Environment.NewLine;

            return result;
        }

        private string GetFolder()
        {
            var result = "";

            var fod = new FolderBrowserDialog();
            fod.ShowNewFolderButton = false;
            if (fod.ShowDialog() == DialogResult.OK)
            {
                result = fod.SelectedPath;
            }

            return result;
        }
    }
}
