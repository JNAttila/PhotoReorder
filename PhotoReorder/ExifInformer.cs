﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace PhotoReorder
{
    // http://msdn.microsoft.com/en-us/library/xddt0dz7.aspx
    // http://stackoverflow.com/questions/58649/how-to-get-the-exif-data-from-a-file-using-c-sharp

    class ExifInformer
    {
        // byte adatok konvertálásához
        private ASCIIEncoding _encoder = new ASCIIEncoding();

        // fotó adatok gyűjtője
        public MyImage _myImage = new MyImage();

        // nyers adatok az exif-ből
        private string _rawModel = "";
        private string _rawDate = "";

        /// <summary>
        /// Létrehozás
        /// </summary>
        /// <param name="filePath">Feldolgozandó fájl</param>
        public ExifInformer(MyImage myImage)
        {
            _myImage = myImage;
            InitExifInfo(_myImage.FullFileName);
        }

        /// <summary>
        /// Képadatok kinyerése
        /// </summary>
        /// <param name="filePath">Feldolgozandó fájl</param>
        private void InitExifInfo(string filePath)
        {
            // beállítás üresre
            _myImage.Machine = "_machine";
            _myImage.CreatedDate = "_cDate";
            _myImage.CreatedTime = "_cTime";

            // védelem
            if (string.IsNullOrEmpty(filePath))
                return;
            var fi = new FileInfo(filePath);
            if (!fi.Exists)
                return;

            Bitmap image = null;

            // adatok kinyerése
            try
            {
                _myImage.Size = fi.Length;
                image = new Bitmap(filePath);

                // http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/EXIF.html
                // 0x0110  Model             string  IFD0 	 
                // 0x9003  DateTimeOriginal  string  ExifIFD  (date/time when original image was taken)

                // létrehozás dátuma
                var pItemDate = image.GetPropertyItem(0x9004);
                _rawDate = _encoder.GetString(pItemDate.Value, 0, pItemDate.Value.Length - 1);

                // eszköz neve
                var pItemModel = image.GetPropertyItem(0x0110);
                _rawModel = _encoder.GetString(pItemModel.Value, 0, pItemModel.Value.Length - 1);

                var fileNameArr = _myImage.FullFileName.Split('\\');
                if (fileNameArr != null && fileNameArr.Length > 1)
                {
                    _myImage.FileName = fileNameArr[fileNameArr.Length - 1];
                }
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                // memória gazdálkodás
                image.Dispose();
            }
        }

        /// <summary>
        /// Adatok kinyerése
        /// </summary>
        public void CalcDatas(bool sortMachine)
        {
            _myImage.Machine = _rawModel;

            string _createdDate;
            string _createTime;
            GetDstParams(_rawDate, out _createdDate, out _createTime);

            _myImage.CreatedDate = _createdDate;
            _myImage.CreatedTime = _createTime;

            if (!string.IsNullOrEmpty(_myImage.PathDestRoot))
            {
                var di = new DirectoryInfo(_myImage.PathDestRoot);
                if (di != null && di.Exists)
                {
                    // path összeállítás
                    _myImage.PathDest = new StringBuilder(_myImage.PathDestRoot)
                        .Append(((sortMachine) ? ("\\" + _myImage.Machine) : ("")))
                        .Append("\\").Append(_myImage.CreatedDate)
                        .ToString();
                }
            }
        }

        /// <summary>
        /// Létrehozás dátumából PATH és FILENAME paraméterek
        /// </summary>
        /// <param name="pathParam">a PATH paraméter</param>
        /// <param name="nameParam">a FILENAME paraméter</param>
        /// <returns>sikeres-e a művelet</returns>
        private bool GetDstParams(string dtStr, out string pathParam, out string nameParam)
        {
            var result = false;
            pathParam = "";
            nameParam = "";

            var reateDts = dtStr.Split(' ');
            if (reateDts.Length > 1)
            {
                // elérési útba a dátum
                pathParam = reateDts[0].Replace(':', '-');
                //pathParam = dt.Year.ToString("0000") + "-" +
                //    dt.Month.ToString("00") + "-" + dt.Day.ToString("00");


                // fájlnévbe az időpont
                nameParam = reateDts[1].Replace(':', '_');
                //nameParam = dt.Hour.ToString("00") + "_" +
                //    dt.Minute.ToString("00") + "_" + dt.Second.ToString("00");

                result = true;
            }

            return result;
        }
    }
}
