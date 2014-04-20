using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoReorder
{
    class MyImage
    {
        // könyvtár adatok
        public string PathSource { get; set; }
        public string PathDest { get; set; }
        public string PathDestRoot { get; set; }

        // fotó adatok
        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public string Machine { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedTime { get; set; }
        public long Size { get; set; }
    }
}
