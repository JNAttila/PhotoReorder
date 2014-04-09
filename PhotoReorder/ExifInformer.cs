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
        private PropertyItem[] _properties = null;

        /// <summary>
        /// Létrehozás
        /// </summary>
        /// <param name="filePath">Feldolgozandó fájl</param>
        public ExifInformer(string filePath)
        {
            InitExifInfo(filePath);
        }

        /// <summary>
        /// Képadatok kinyerése
        /// </summary>
        /// <param name="filePath">Feldolgozandó fájl</param>
        private void InitExifInfo(string filePath)
        {
            // védelem
            if (string.IsNullOrEmpty(filePath))
                return;
            var fi = new FileInfo(filePath);
            if (!fi.Exists)
                return;

            // adatok kinyerése
            try
            {
                var image = new Bitmap(filePath);
                _properties = image.PropertyItems;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// A fotó létrehozásának ideje
        /// </summary>
        /// <returns>dátum + idő</returns>
        public string GetCreated()
        {
            // http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/EXIF.html
            // 0x9003 	DateTimeOriginal 	string 	ExifIFD 	(date/time when original image was taken)
            var result = "";

            // végig a propertyken
            foreach (var item in _properties)
            {
                // ez a "DateTimeOriginal"
                if (item.Id == 0x9003)
                {
                    // kódolás
                    var encoding = new ASCIIEncoding();
                    var str = encoding.GetString(item.Value, 0, item.Value.Length - 1);

                    // DEBUG
                    //result += "ID:" + item.Id + " Len:" + item.Len +
                    //   " Type:" + item.Type + " Value: " + str + Environment.NewLine;

                    result = str;
                    break;
                }
            }

            return result;
        }

    }
}
