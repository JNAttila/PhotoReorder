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
        /// A forráskönyvtárban itt lesznek megtalálhatóak az átmozgatott fájlok
        /// </summary>
        const string _destPrefix = "__Album__";

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
        /// Duplikált képek legyenek-e törölve
        /// </summary>
        bool _delDuplic;

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

        #region Interface

        /// <summary>
        /// Létrehozás
        /// </summary>
        /// <param name="pathFrom">fájlok keresésének kiindulási helye</param>
        /// <param name="byMachines">kell-e gépenként külön könyvtár</param>
        /// <param name="exifIL">képii információ listája</param>
        /// <param name="pattern">képkeresési minta</param>
        /// <param name="tbLog">log üzenetek ablaka</param>
        /// <param name="super">a létrehozóra egy referencia</param>
        /// <param name="isDdebug">szükséges-e részletes logolás</param>
        public ThreadAnalyseFiles(string pathFrom, bool byMachines,
            ref List<ExifInformer> exifIL, string pattern, ref TextBox tbLog, PhotoReorder super,
            ref ProgressBar pgBar, bool isDdebug = false, bool delDuplic = false)
        {
            _pathFrom = pathFrom;
            _pathTo = pathFrom + "\\" + _destPrefix;

            _byMachines = byMachines;
            _exifInfoList = exifIL;
            _searchPattern = pattern;

            _logger = new ThreadLogger(ref tbLog, ref pgBar);
            _super = super;
            _debug = isDdebug;
            _delDuplic = delDuplic;
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

            DirectoryInfo subDir;
            // végig minden fájlon
            foreach (var item in di.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                // infó az aktuális könyvtárról
                subDir = new DirectoryInfo(item.FullName);

                // a célkönyvtárban ilyenkor ne keressen
                var destFolder = item.FullName.Contains(_pathTo);
                if (destFolder)
                    continue;

                // fájl lista a keresési minta szerint a könyvtárban
                var subFiles = subDir.GetFiles(_searchPattern, SearchOption.TopDirectoryOnly);
                // progressbar iniciaizálás
                _logger.PgbReset();
                _logger.PgbInit(subFiles.Length);

                _logger.Log("Könyvtár: " + item.FullName, false);

                // a könyvtár minden érdekes fájlja
                for (int i = 0; i < subFiles.Length; ++i)
                {
                    // képadat elemzéshez listában tárolás
                    _exifInfoList.Add(new ExifInformer(new MyImage()
                    {
                        FullFileName = subFiles[i].FullName,
                        PathDestRoot = _pathFrom + '\\' + _destPrefix
                    }
                    ));

                    // folyamatjelző
                    _logger.PgbStep();

                    // össz fájl darabszám
                    ++fileCnt;
                }

                _logger.Log((_debug) ? ("  -  " + subFiles.Length.ToString() + "db") : (""), true, false);
            }

            _logger.PgbReset();
            _logger.Log("Összes talált fájlok száma: " + fileCnt.ToString());

            // folyamatjelző készítése az új körre
            _logger.PgbInit(_exifInfoList.Count);

            // minden Exifinformer-nek elemzés
            foreach (var item in _exifInfoList)
            {
                // elemzés
                item.CalcDatas(_byMachines);

                // folyamat állapot kijelzés
                _logger.PgbStep();
            }

            _logger.Log("Adatok előkészítve" + ((_debug) ? (": " + _exifInfoList.Count.ToString() + "db fájl") : ("")));

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

        #endregion Interface

        /// <summary>
        /// Fotó album alapba állítás
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

            // folyamat kijelzés a felületre
            _logger.Log("Meglévő képek elemzése", false);

            // a cél könyvtárban
            foreach (var dir in di.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                // végig minden könyvtáron
                var subDir = new DirectoryInfo(dir.FullName);
                // alkönyvtár beli fájlok
                var subDirFiles = subDir.GetFiles(_searchPattern, SearchOption.TopDirectoryOnly);

                // folyamatjelző beállítása
                _logger.PgbReset();
                _logger.PgbInit(subDirFiles.Length);

                // végig minden alkönyvtár beli fájlon
                for (int i = 0; i < subDirFiles.Length; ++i)
                {
                    var ei = new ExifInformer(new MyImage()
                        {
                            FullFileName = subDirFiles[i].FullName
                        }
                    );
                    ei.CalcDatas(_byMachines);

                    // bejegyzés a fotóalbum megfelelő listájába
                    // a képet azonosítja a létrehozás időpontjával és a fájl méretével
                    var dictVal = ei._myImage.CreatedTime + "_" + subDirFiles[i].Length;

                    // fotóalbum elérési út
                    // a kép besorolását a fényképezőgép típusa és a létrehozás dátuma adja
                    var dictKey = _pathTo + ((_byMachines) ? ("\\" + ei._myImage.Machine) : ("")) +
                        "\\" + ei._myImage.CreatedDate;

                    // "adott géppel ezen a napon" van-e már kép
                    if (!_dict.ContainsKey(dictKey))
                    {
                        _dict.Add(dictKey, new List<string>());
                    }

                    // "a géppel aznap készített egy újabb kép"
                    if (!_dict[dictKey].Contains(dictVal))
                    {
                        _dict[dictKey].Add(dictVal);
                    }

                    // folyamatjelző léptetése
                    _logger.PgbStep();
                }
            }

            // vége a folyamatnak
            _logger.Log("  -  OK", true, false);

            // folyamatjelző alap állapotba
            _logger.PgbReset();
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

                    // EXIF adat nélkül kihagyás
                    if (!item.IsExif)
                        continue;

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

                        if (_debug)
                        {
                            // melyik volt az a fájl
                            _logger.Log("Kihagyva: " + item._myImage.PathSource);
                        }

                        // ha kell, duplikált kép törlése
                        if (_delDuplic)
                        {
                            File.Delete(item._myImage.PathSource);
                        }

                        // már van ilyen fájl mehetünk tovább
                        continue;
                    }

                    _dict[dictKey].Add(dictVal);

                    // célkönyvtár ellenőrzése, ha kell létrehozása
                    di = new DirectoryInfo(item._myImage.PathDest);
                    if (!di.Exists)
                    {
                        di.Create();
                        di.Refresh();
                    }
                    // sikertelen létrehozás
                    if (!di.Exists)
                    {
                        if (_debug)
                            _logger.Log("Célkönyvtár nem hozható létre: " + item._myImage.PathDest);
                        continue;
                    }

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

                    // kép mozgatása cél helyre
                    File.Move(item._myImage.FullFileName, fullNewFileName);

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
