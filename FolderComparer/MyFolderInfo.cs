﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderComparer
{
    /// <summary>
    /// Saját INFO osztály egy könyvtárról
    /// </summary>
    class MyFolderInfo
    {
        #region DataMembers and GetterSetters

        /// <summary>
        /// A könyvtár elérési útja
        /// </summary>
        string _path = "";
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                if (!String.IsNullOrEmpty(_path))
                {
                    _files = new HashSet<string>();
                    // process folder info
                    _validInfo = ProcessFolder();
                }
            }
        }

        /// <summary>
        /// Összes fájlméret
        /// </summary>
        long _sumFileSize = 0;
        public long SumFileSize
        {
            get { return _sumFileSize; }
        }

        /// <summary>
        /// Könyvtárbeli fájlok száma
        /// </summary>
        int _fileNumber = 0;
        public int FileNumber
        {
            get { return _fileNumber; }
        }

        /// <summary>
        /// Feldolgozott adatok vannak-e a példányban
        /// </summary>
        bool _validInfo = false;
        public bool ValidInfo
        {
            get { return _validInfo; }
        }

        /// <summary>
        /// A könyvtár beli fájlok nevei
        /// </summary>
        HashSet<string> _files = null;
        public HashSet<string> Files
        {
            get { return _files; }
        }

        #endregion DataMembers and GetterSetters

        #region Create

        /// <summary>
        /// Létrehozás és könyvtár inicializálás
        /// </summary>
        /// <param name="path">Könyvtár elérési útja</param>
        public MyFolderInfo(string path = null)
        {
            Path = path;
        }

        #endregion Create

        #region Functions

        /// <summary>
        /// Könyvtár adatainak feldolgozása
        /// </summary>
        /// <param name="path">feldolgozandó eléréssi út</param>
        /// <returns>sikeres feldolgozás</returns>
        private bool ProcessFolder()
        {
            // ellenőrzés
            if (string.IsNullOrEmpty(_path))
                return false;

            // könyvtárinfó lekérés
            var di = new DirectoryInfo(_path);
            if (di == null || !di.Exists)
                return false;
            
            // fájlok mérete együtt
            _sumFileSize = 0;

            // fájlok darabszáma
            _fileNumber = 0;

            // a könyvtár összes fájlja
            var xx = di.EnumerateFiles("*.jp*");
            foreach (var x in xx)
            {
                // méret összegzés
                _sumFileSize += x.Length;

                // fájl számlálás
                ++_fileNumber;

                // fájlnév tárolás
                _files.Add(x.Name);
            }

            return true;
        }

        /// <summary>
        /// Két könyvtár megegyezőségének vizsgálata
        /// </summary>
        /// <param name="_mfi">Hasonlítandó</param>
        /// <param name="checkFileNames">fájlnévellenőrzéssel-e</param>
        /// <returns>Azonos könyvtárak-e</returns>
        public bool Equals(MyFolderInfo _mfi = null, bool checkFileNames = true)
        {
            if (_mfi == null) return false;

            if (_mfi.FileNumber != FileNumber) return false;

            if (_mfi.SumFileSize != SumFileSize) return false;

            if (checkFileNames)
            {
                //
            }

            return true;
        }

        #endregion Functions
    }
}
