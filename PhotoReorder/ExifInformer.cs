using System;
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

            // adatok kinyerése
            try
            {
                _myImage.Size = fi.Length;
                var image = new Bitmap(filePath);
                _myImage._properties = image.PropertyItems;

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
        }

        /// <summary>
        /// Adatok kinyerése
        /// </summary>
        public void CalcDatas(bool sortMachine)
        {
            string _model;
            string _tmpDateTime;
            GetDatas(out _model, out _tmpDateTime);
            _myImage.Machine = _model;

            string _createdDate;
            string _createTime;
            GetDstParams(_tmpDateTime, out _createdDate, out _createTime);

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
        /// A fotó létrehozásának ideje
        /// </summary>
        /// <returns>dátum + idő</returns>
        private bool GetDatas(out string model, out string created)
        {
            // http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/EXIF.html
            // 0x0110  Model             string  IFD0 	 
            // 0x9003  DateTimeOriginal  string  ExifIFD  (date/time when original image was taken)
            model = "";
            created = "";

            var result = false;

            // végig a propertyken
            foreach (var item in _myImage._properties)
            {
                // ez a "DateTimeOriginal"
                if (item.Id == 0x9004)
                {
                    // kódolás
                    created = _encoder.GetString(item.Value, 0, item.Value.Length - 1);
                }

                // a fényképező típusa
                if (item.Id == 0x0110)
                {
                    // kódolás
                    model = _encoder.GetString(item.Value, 0, item.Value.Length - 1);
                }

                // mind a két adat megvan => megállás
                if (created.Length > 0 && model.Length > 0)
                {
                    result = true;
                    break;
                }
            }

            return result;
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
