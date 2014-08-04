using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoReorder
{
    public partial class PhotoReorder : Form
    {
        #region adattagok

        // képek forrása
        private string _pathFrom;

        // adatoka gyűjtő sora
        public List<ExifInformer> _eiList = new List<ExifInformer>();

        /// <summary>
        /// Naplózást végző objektum
        /// </summary>
        private ThreadLogger _logger;

        /// <summary>
        /// Képelemzés szála
        /// </summary>
        private Thread analyseThread = null;

        #endregion adattagok

        /// <summary>
        /// Létrehozás
        /// </summary>
        public PhotoReorder()
        {
            InitializeComponent();
            UpdateUI();

            _logger = new ThreadLogger(ref tbResult, ref pgBarMain);
        }

        /// <summary>
        /// A folyamatot elinidító gomb felületénem módosítása
        /// </summary>
        /// <param name="isRunning">a beállítandó állapot</param>
        public void UpdateStartBtn(bool isRunning)
        {
            btnReorder.BeginInvoke(new Action(() =>
            {
                //btnReorder.Enabled = !isRunning;
                btnReorder.Text = (isRunning) ? ("Rendezés STOP") : ("Rendez");
            }));
        }

        /// <summary>
        /// Az elérési utak szerint alakul a felület
        /// </summary>
        private void UpdateUI()
        {
            btnReorder.Enabled = false;

            if (string.IsNullOrEmpty(_pathFrom))
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
            od.RootFolder = Environment.SpecialFolder.Desktop;
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = od.SelectedPath;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// A rendezés lényegi része
        /// </summary>
        private void btnReorder_Click(object sender, EventArgs e)
        {
            if (analyseThread == null || !analyseThread.IsAlive)
            {
                _logger.Log("Forrásképek elemzése");

                // indító form módosítása
                UpdateStartBtn(true);

                AnalyseFiles();
            }
            else
            {
                _logger.Log("Elemzés megszakítva!");

                // indító form módosítása
                UpdateStartBtn(false);

                analyseThread.Abort();
            }
        }

        /// <summary>
        /// A rendezés folyamata
        /// </summary>
        private void AnalyseFiles()
        {
            // képelemző szál létrehozása
            analyseThread = new Thread(new Threads.ThreadAnalyseFiles(_pathFrom,
                chbMachine.Checked, ref _eiList, ThreadFileType.IMAGE_JPG, ref tbResult,
                this, ref pgBarMain, chbDebug.Checked, chbDelDuplicated.Checked).AnalyleFiles);

            // képelemzés indítása
            analyseThread.Start();
        }
    }
}
