/*
 * DEMONSTRATION CODE
 * The following code was provided for demonstration/educational purposes only. 
 * No promises are provided regarding the stability of this code as it is sample
 * code only.  This code may be freely used and distributed, however if the code
 * is passed on without modifications it is requested that this disclaimer be   
 * kept intact.
 * 
 * Created By:
 * Mitchel Sellers (msellers@iowacomputergurus.com)
 * Director of Development
 * IowaComputerGurus L.L.P.
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace airettController
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AirettControllerContext oContext = new AirettControllerContext();
            Application.Run(oContext);
        }
    }
}