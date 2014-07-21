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

        /// <summary>
        /// Folyamatjelző
        /// </summary>
        ProgressBar _pgBar;

        #endregion adattagok

        /// <summary>
        /// Létrehozás
        /// </summary>
        /// <param name="tbLog">Naplózás számára ablak</param>
        /// <param name="pgBar">Folyamat jelző</param>
        public ThreadLogger(ref TextBox tbLog, ref ProgressBar pgBar)
        {
            _tbLog = tbLog;
            _pgBar = pgBar;
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

        #region folyamatjelző

        /// <summary>
        /// Folyamatjelző alap állapotba
        /// </summary>
        public void PgbReset()
        {
            _pgBar.BeginInvoke(new Action(() =>
            {
                _pgBar.Value = 0;
            }));
        }

        /// <summary>
        /// A maximális érték beállítása
        /// </summary>
        /// <param name="maxValue">Max érték</param>
        public void PgbInit(int maxValue)
        {
            _pgBar.BeginInvoke(new Action(() =>
            {
                _pgBar.Maximum = maxValue;
            }));
        }

        /// <summary>
        /// Az állapotjelző léptetése
        /// </summary>
        public void PgbStep()
        {
            _pgBar.BeginInvoke(new Action(() =>
            {
                _pgBar.PerformStep();
            }));
        }

        #endregion folyamatjelző
    }
}
