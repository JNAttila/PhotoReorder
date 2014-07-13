using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoReorder
{
    /// <summary>
    /// Fájl típusonként keresési minta
    /// </summary>
    public static class ThreadFileType
    {
        /// <summary>
        /// JPG és JPEG képek kereséséhez minta
        /// </summary>
        public static string IMAGE_JPG
        {
            get { return "*.jp*"; }
        }
    }

    /// <summary>
    /// Naplózást végző osztály
    /// </summary>
    public class ThreadLogger
    {
        #region adattagok

        /// <summary>
        /// Naplózás számára ablak
        /// </summary>
        TextBox _tbLog;

        #endregion adattagok

        /// <summary>
        /// Létrehozás
        /// </summary>
        /// <param name="tbLog">Naplózás számára ablak</param>
        public ThreadLogger(ref TextBox tbLog)
        {
            _tbLog = tbLog;
        }

        /// <summary>
        /// Pontos idő
        /// </summary>
        /// <returns>formázott</returns>
        string Now()
        {
            return DateTime.Now.ToLongTimeString() + " - ";
        }

        /// <summary>
        /// Naplózás
        /// </summary>
        /// <param name="msg">üzenet</param>
        /// <param name="printNewLine">kell-e új sor</param>
        /// <param name="printDate">kell-e dátum</param>
        public void Log(string msg, bool printNewLine = true, bool printDate = true)
        {
            // bejegyzés dekorálása dátummal és befejés sortöréssel, igény szerint
            var newMsg = ((printDate) ? (Now()) : ("")) + msg +
                ((printNewLine) ? (Environment.NewLine) : (""));

            _tbLog.BeginInvoke(new Action(() =>
            {
                _tbLog.AppendText(newMsg);
            }));
        }
    }

    public class ThreadAnalyseFiles
    {
        #region tagváltozók

        /// <summary>
        /// az adatok begyűjtésének kezdeti könyvtára
        /// </summary>
        string _pathFrom;

        /// <summary>
        /// Képek másolásásnak gyökér könyvtára
        /// </summary>
        string _pathTo;

        /// <summary>
        /// Kell-e gépenként külön könyvtár
        /// </summary>
        bool _byMachines;

        /// <summary>
        /// képi infpormációk gyűjtő helye
        /// </summary>
        List<ExifInformer> _exifInfoList;

        /// <summary>
        /// Fájlok keresési mintája
        /// </summary>
        string _searchPattern;

        /// <summary>
        /// Naplózást végző osztály
        /// </summary>
        ThreadLogger _logger;

        PhotoReorder _super;

        #endregion tagváltozók

        /// <summary>
        /// Létrehozás
        /// </summary>
        /// <param name="pathFrom">fájlok keresésének kiindulási helye</param>
        /// <param name="pathTo">fájlok másolásának cél gyökér könyvtára</param>
        /// <param name="byMachines">kell-e gépenként külön könyvtár</param>
        /// <param name="exifIL">képii információ listája</param>
        /// <param name="pattern">képkeresési minta</param>
        /// <param name="tbLog">log üzenetek ablaka</param>
        public ThreadAnalyseFiles(string pathFrom, string pathTo, bool byMachines,
            ref List<ExifInformer> exifIL, string pattern, ref TextBox tbLog, PhotoReorder super)
        {
            _pathFrom = pathFrom;
            _pathTo = pathTo;
            _byMachines = byMachines;
            _exifInfoList = exifIL;
            _searchPattern = pattern;
            _logger = new ThreadLogger(ref tbLog);
            _super = super;
        }

        /// <summary>
        /// Fájlok elemzése
        /// </summary>
        public void AnalyleFiles()
        {
            // könyvtár ellenőrzés
            var di = new DirectoryInfo(_pathFrom);
            if (di == null || di.Exists == false)
                return;

            _exifInfoList.Clear();

            // könyvtárak számlálója
            int fileCnt = 0;
            // utolsó elemzett könyvtár neve
            string lastDir = "";
            int lastFileCnt = 0;

            // végig minden fájlon
            foreach (var item in di.EnumerateFiles(_searchPattern, SearchOption.AllDirectories))
            {
                if (lastDir != item.DirectoryName)
                {
                    if (fileCnt != lastFileCnt)
                    {
                        _logger.Log("Képek : " + (fileCnt - lastFileCnt).ToString() + "db");
                        lastFileCnt = fileCnt;
                    }

                    lastDir = item.DirectoryName;
                    _logger.Log("Könyvtár: " + lastDir);
                }

                // listában tárolás
                _exifInfoList.Add(new ExifInformer(new MyImage()
                {
                    FullFileName = item.FullName,
                    PathDestRoot = _pathTo
                }
                ));

                ++fileCnt;
            }

            _logger.Log("Képek : " + (fileCnt - lastFileCnt).ToString() + "db");

            _logger.Log("Összes talált fájlok száma: " + fileCnt.ToString());

            // minden Exifinformer-nek elemzés
            foreach (var item in _exifInfoList)
            {
                item.CalcDatas(_byMachines);
            }

            // indító form módosítása
            _super.UpdateStartBtn(false);
        }
    }

    public class ThreadDiscoverDestFolder
    {
    }

    public class ThreadCopyFiles
    {
    }
}
