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

namespace PhotoReorder
{
    public partial class Form1 : Form
    {
        // képek forrása
        private string _pathFrom;
        // képek cél könyvtára
        private string _pathTo;

        /// <summary>
        /// Létrehozás
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            btnReorder.Enabled = false;

            if (string.IsNullOrEmpty(_pathFrom))
                return;
            if (string.IsNullOrEmpty(_pathTo))
                return;

            btnReorder.Enabled = true;
        }

        /// <summary>
        /// Forráskönyvtár megadása
        /// </summary>
        private void btnFrom_Click(object sender, EventArgs e)
        {
            // forráskönyvtár megadás
            SelectPath(out _pathFrom);

            tbFrom.Text = _pathFrom;

            UpdateUI();
        }

        /// <summary>
        /// Célkönyvtár
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTo_Click(object sender, EventArgs e)
        {
            // célkönyvtár megadás
            SelectPath(out _pathTo);

            tbTo.Text = _pathTo;

            UpdateUI();
        }

        /// <summary>
        /// Könyvtár kiválasztása
        /// </summary>
        /// <param name="path">a teljes elérési út</param>
        /// <returns>sikeres út választás</returns>
        private bool SelectPath(out string path)
        {
            // eredmény
            var result = false;
            // inicializálás
            path = "";

            // Dialógus nyitása és kezelése
            var od = new FolderBrowserDialog();
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = od.SelectedPath;
                result = true;
            }

            return result;
        }

        private void btnReorder_Click(object sender, EventArgs e)
        {
            DoProcess();
        }

        private void DoProcess()
        {
            // könyvtár ellenőrzés
            var di = new DirectoryInfo(_pathFrom);
            if (di == null || di.Exists == false)
                return;

            tbResult.Text = "";
            // végig minden fájlon
            foreach (var item in di.EnumerateFiles("*.jp*", SearchOption.AllDirectories))
            {
                var created = GetCreated(item.FullName);
                if (created.Length > 0)
                    tbResult.Text += item.FullName + " _ " + created + Environment.NewLine;
            }
        }

        /// <summary>
        /// A fájl létrehozásának ideje
        /// </summary>
        /// <param name="filePath">a féjl elérési útja</param>
        /// <returns>létrehozásának ideje</returns>
        private string GetCreated(string filePath)
        {
            var ei = new ExifInformer(filePath);

            return ei.GetCreatedString();
        }

        //
        private void DoProcess2()
        {
            var fo = new OpenFileDialog();
            string filePath = "";
            if (fo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                filePath = fo.FileName;

            var ei = new ExifInformer(filePath);

            tbResult.Text = ei.GetCreatedString();
        }
    }
}
