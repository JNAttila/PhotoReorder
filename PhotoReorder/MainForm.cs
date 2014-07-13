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
        // képek forrása
        private string _pathFrom;
        // képek cél könyvtára
        private string _pathTo;

        // adatoka gyűjtő sora
        public List<ExifInformer> _eiList = new List<ExifInformer>();

        // másolt fájlok
        int fileCopied;
        // kihagyott fájlok
        int fileCancelled;

        /// <summary>
        /// Fotó album
        /// </summary>
        public Dictionary<string, List<string>> dict = null;

        /// <summary>
        /// Naplózást végző objektum
        /// </summary>
        private ThreadLogger _logger;

        /// <summary>
        /// Képelemzés szála
        /// </summary>
        private Thread analyseThread = null;

        /// <summary>
        /// Létrehozás
        /// </summary>
        public PhotoReorder()
        {
            InitializeComponent();
            UpdateUI();

            _logger = new ThreadLogger(ref tbResult);
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
            if (string.IsNullOrEmpty(_pathTo))
                return;

            btnReorder.Enabled = true;
        }

        /// <summary>
        /// Pontos idő
        /// </summary>
        /// <returns>formázott</returns>
        string Now()
        {
            return DateTime.Now.ToLongTimeString() + " - ";
        }

        ///// <summary>
        ///// Naplózás
        ///// </summary>
        ///// <param name="msg">üzenet</param>
        ///// <param name="printNewLine">kell-e új sor</param>
        ///// <param name="printDate">kell-e dátum</param>
        //void Log(string msg, bool printNewLine = true, bool printDate = true)
        //{
        //    tbResult.BeginInvoke(new Action(() =>
        //    {
        //        // bejegyzés dekorálása dátummal és befejés sortöréssel, igény szerint
        //        var newMsg = ((printDate) ? (Now()) : ("")) + msg + ((printNewLine) ? (Environment.NewLine) : (""));

        //        // a kijelzés után az utolsó üzenet látszódjon
        //        tbResult.Text += newMsg;
        //        Update();
        //        tbResult.Select(tbResult.Text.Length, 0);
        //        tbResult.ScrollToCaret();
        //    }));
        //}

        /// <summary>
        /// Fotó album alapbaállítás
        /// </summary>
        private void InitFotoDict()
        {
            if (dict == null)
                dict = new Dictionary<string, List<string>>();

            foreach (var i in dict)
            {
                i.Value.Clear();
            }
            dict.Clear();
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

            //AnalyseFiles();

            //DiscoverDestFolder();

            //if (chbMove.Checked)
            //{
            //    fileCopied = 0;
            //    fileCancelled = 0;

            //    CopyFiles();

            //    var result = "Fájlok másolva: " + fileCopied + Environment.NewLine +
            //        "Fájlok kihagyva: " + fileCancelled;

            //    tbResult.Text += result;
            //    MessageBox.Show(result, "Rendezés eredménye",
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        /// <summary>
        /// A rendezés folyamata
        /// </summary>
        private void AnalyseFiles()
        {
            //new Thread(new MapUsingThread(map, "mfmt_", randSeed).Find
            analyseThread = new Thread(new ThreadAnalyseFiles(_pathFrom, _pathTo,
                chbMachine.Checked, ref _eiList, ThreadFileType.IMAGE_JPG, ref tbResult,
                this).AnalyleFiles);

            analyseThread.Start();
        }

        /// <summary>
        /// Célkönyvtár felderítése, már ottlévő fájlok után kutatva
        /// </summary>
        private void DiscoverDestFolder()
        {
            // könyvtár ellenőrzés
            var di = new DirectoryInfo(_pathTo);
            if (di == null || di.Exists == false)
                return;

            InitFotoDict();

            // végig minden fájlon
            foreach (var item in di.EnumerateFiles("*.jp*", SearchOption.AllDirectories))
            {
                var ei = new ExifInformer(new MyImage()
                {
                    FullFileName = item.FullName
                }
                );
                ei.CalcDatas(chbMachine.Checked);

                // bejegyzés a fotóalbum megfelelő listájába
                var dictVal = ei._myImage.CreatedTime + "_" + item.Length;

                // fotóalbum elérési út
                var dictKey = _pathTo + ((chbMachine.Checked) ? ("\\" + ei._myImage.Machine) : ("")) +
                    "\\" + ei._myImage.CreatedDate;
                if (!dict.ContainsKey(dictKey))
                {
                    dict.Add(dictKey, new List<string>());
                }
                if (!dict[dictKey].Contains(dictVal))
                    dict[dictKey].Add(dictVal);
            }
        }

        /// <summary>
        /// A szám szerint megváltoztatja a fájl nevét
        /// </summary>
        /// <param name="fileName">fájl eredeti neve</param>
        /// <param name="number">sorszám</param>
        /// <returns>módosított fájlnév</returns>
        private string FileNamePlusCounter(string fileName, int number)
        {
            // eredmény
            string result = "";

            // fájlnév darabok
            var fName = fileName.Split('.');
            if (fName == null)
                // nem OK, szóval egyszerűsítés
                return fileName + number;

            // a fájlnév elemei mentén
            for (int i = 0; i < fName.Length - 1; ++i)
            {
                result += fName[i] + ".";
            }
            // szám fájlnévbe építése + kiterjesztés
            result += number.ToString("000") + "." + fName[fName.Length - 1];

            return result;
        }

        /// <summary>
        /// Fájlok másolása paraméterek szerint
        /// </summary>
        private void CopyFiles()
        {
            // könyvtár ellenőrzése
            DirectoryInfo di = null;
            // fájl ellenőrzése
            FileInfo fi = null;

            // fájl számozás
            int fileCnt = 0;
            // teljes fájl név
            string fullNewFileName;

            // az eredmények alapján
            foreach (var item in _eiList)
            {
                // mozgatás
                try
                {
                    // bejegyzés a fotóalbum megfelelő listájába
                    var dictVal = item._myImage.CreatedTime + "_" + item._myImage.Size;

                    // fotóalbum elérési út
                    var dictKey = _pathTo + ((chbMachine.Checked) ? ("\\" + item._myImage.Machine) : ("")) +
                        "\\" + item._myImage.CreatedDate;
                    if (!dict.ContainsKey(dictKey))
                    {
                        dict.Add(dictKey, new List<string>());
                    }
                    if (dict[dictKey].Contains(dictVal))
                    {
                        // egyel több kihagyott fájl 
                        ++fileCancelled;
                        // melyik volt az a fájl
                        _logger.Log("Kihagyva: " + item._myImage.PathSource);

                        this.Refresh();

                        // már van ilyen fájl mehetünk tovább
                        continue;
                    }

                    dict[dictKey].Add(dictVal);

                    // célkönyvtár ellenőrzése, ha kell létrehozása
                    di = new DirectoryInfo(item._myImage.PathDest);
                    if (!di.Exists)
                        di.Create();

                    // új fájl teljes eléréi útja
                    fullNewFileName = item._myImage.PathDest + "\\" + item._myImage.FileName;

                    // egyedi fájlnév keresése
                    fi = new FileInfo(fullNewFileName);
                    if (fi.Exists)
                    {
                        while (fi.Exists)
                        {
                            fullNewFileName = item._myImage.PathDest + "\\"
                                + FileNamePlusCounter(item._myImage.FileName, ++fileCnt);

                            fi = new FileInfo(fullNewFileName);
                        }
                    }

                    File.Copy(item._myImage.FullFileName, fullNewFileName);

                    fileCnt = 0;

                    // egyel több másolt fájl
                    ++fileCopied;
                }
                catch (Exception ex)
                {
                    this.tbResult.Text += ">>>" + ex.GetType().ToString() + Environment.NewLine + ex.Message;
                    this.Refresh();
                }
            }
        }
    }
}
