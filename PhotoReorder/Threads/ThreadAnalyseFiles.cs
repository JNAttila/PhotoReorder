using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoReorder.Threads
{
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

        /// <summary>
        /// A szülőosztály referenciája
        /// </summary>
        PhotoReorder _super;

        /// <summary>
        /// Fotó album
        /// </summary>
        public Dictionary<string, List<string>> _dict = null;

        /// <summary>
        /// másolt fájlok száma
        /// </summary>
        int fileCopied;

        /// <summary>
        /// kihagyott fájlok kihagyott
        /// </summary>
        int fileCancelled;

        /// <summary>
        /// Részletes logolás legyen-e
        /// </summary>
        bool _debug;

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
        /// <param name="super">a létrehozóra egy referencia</param>
        /// <param name="isDdebug">szükséges-e részletes logolás</param>
        public ThreadAnalyseFiles(string pathFrom, string pathTo, bool byMachines,
            ref List<ExifInformer> exifIL, string pattern, ref TextBox tbLog, PhotoReorder super,
            ref ProgressBar pgBar, bool isDdebug = false)
        {
            _pathFrom = pathFrom;
            _pathTo = pathTo;
            _byMachines = byMachines;
            _exifInfoList = exifIL;
            _searchPattern = pattern;
            _logger = new ThreadLogger(ref tbLog, ref pgBar);
            _super = super;
            _debug = isDdebug;
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
                    if (_debug && fileCnt != lastFileCnt)
                    {
                        _logger.Log("  _  " + (fileCnt - lastFileCnt).ToString() + "db", true, false);
                        lastFileCnt = fileCnt;
                    }

                    lastDir = item.DirectoryName;
                    _logger.Log("Könyvtár: " + lastDir, false);
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

            if (_debug)
                _logger.Log("  _  " + (fileCnt - lastFileCnt).ToString() + "db", true, false);

            _logger.Log("Összes talált fájlok száma: " + fileCnt.ToString());

            // foolyamatjelző alapállapotba
            _logger.PgbReset();
            _logger.PgbInit(fileCnt);

            // minden Exifinformer-nek elemzés
            foreach (var item in _exifInfoList)
            {
                // elemzés
                item.CalcDatas(_byMachines);

                // folyamat állapot kijelzés
                _logger.PgbStep();
            }

            // folyamatjelző újra alapállapotba
            _logger.PgbReset();

            // meglévő képekből fotóalbum építése
            DiscoverDestFolder();

            fileCopied = 0;
            fileCancelled = 0;

            // fájlok másolása az album alapján
            CopyFiles();

            // összesítő eredmény kjelzése
            var result = "Fájlok másolva: " + fileCopied + Environment.NewLine +
                "Fájlok kihagyva: " + fileCancelled;
            _logger.Log(result);

            // indító form módosítása
            _super.UpdateStartBtn(false);
        }

        /// <summary>
        /// Fotó album alapbaállítás
        /// </summary>
        private void InitFotoDict()
        {
            if (_dict == null)
                _dict = new Dictionary<string, List<string>>();

            foreach (var i in _dict)
            {
                i.Value.Clear();
            }
            _dict.Clear();
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
            foreach (var item in di.EnumerateFiles(_searchPattern, SearchOption.AllDirectories))
            {
                var ei = new ExifInformer(new MyImage()
                {
                    FullFileName = item.FullName
                }
                );
                ei.CalcDatas(_byMachines);

                // bejegyzés a fotóalbum megfelelő listájába
                var dictVal = ei._myImage.CreatedTime + "_" + item.Length;

                // fotóalbum elérési út
                var dictKey = _pathTo + ((_byMachines) ? ("\\" + ei._myImage.Machine) : ("")) +
                    "\\" + ei._myImage.CreatedDate;

                if (!_dict.ContainsKey(dictKey))
                {
                    _dict.Add(dictKey, new List<string>());
                }

                if (!_dict[dictKey].Contains(dictVal))
                {
                    _dict[dictKey].Add(dictVal);
                }
            }
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

            // folyamatjelzőalap állapotba
            _logger.PgbInit(_exifInfoList.Count);

            // az eredmények alapján
            foreach (var item in _exifInfoList)
            {
                // mozgatás
                try
                {
                    // folyamat kijelzés
                    _logger.PgbStep();

                    // bejegyzés a fotóalbum megfelelő listájába
                    var dictVal = item._myImage.CreatedTime + "_" + item._myImage.Size;

                    // fotóalbum elérési út
                    var dictKey = _pathTo + ((_byMachines) ? ("\\" + item._myImage.Machine) : ("")) +
                        "\\" + item._myImage.CreatedDate;
                    if (!_dict.ContainsKey(dictKey))
                    {
                        _dict.Add(dictKey, new List<string>());
                    }
                    if (_dict[dictKey].Contains(dictVal))
                    {
                        // egyel több kihagyott fájl 
                        ++fileCancelled;
                        // melyik volt az a fájl
                        _logger.Log("Kihagyva: " + item._myImage.PathSource);

                        // már van ilyen fájl mehetünk tovább
                        continue;
                    }

                    _dict[dictKey].Add(dictVal);

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
                    _logger.Log(">>>" + ex.GetType().ToString() + Environment.NewLine + ex.Message);
                }
            }

            // folyamatjelző újra alap állapotba
            _logger.PgbReset();
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
    }
}
