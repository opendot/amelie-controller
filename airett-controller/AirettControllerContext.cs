/*
 * DEMONSTRATION CODE
 * The following code was provided for demonstration/educational purposes only. 
 * No promises are provided regarding the stability of this code as it is sample
 * code only.  This code may be freely used and distributed, however if the code
 * is passed on without modifications it is requested that this disclaimer be   
 * kept intact.
 * 
 * Created By:
 * Daniele Ciminieri
 * Senior Developer
 * Opendot
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Configuration;


namespace airettController
{

    public class AirettControllerContext : ApplicationContext
    {
        #region Private Members
        private System.ComponentModel.IContainer mComponents;   //List of components
        private NotifyIcon mNotifyIcon;
        private ContextMenuStrip mContextMenu;
        private ToolStripMenuItem mDisplayStart;
        private ToolStripMenuItem mDisplayStartBrowser;
        private ToolStripMenuItem mDisplayStop;
        private ToolStripMenuItem mExitApplication;
        private StatusControl status;
        private string programFolder;
        #endregion

        public AirettControllerContext()
        {

            programFolder = ConfigurationManager.AppSettings["programFolder"];
         
            //Instantiate the component Module to hold everything
            mComponents = new System.ComponentModel.Container();
            
            
            //Instantiate the NotifyIcon attaching it to the components container and 
            //provide it an icon, note, you can imbed this resource 
            mNotifyIcon = new NotifyIcon(this.mComponents);
            mNotifyIcon.Icon = new System.Drawing.Icon("airett_off.ico");
            mNotifyIcon.Text = "Amelie controller";
            mNotifyIcon.Visible = true;

            status = new StatusControl(mNotifyIcon, programFolder);

            //Instantiate the context menu and items
            mContextMenu = new ContextMenuStrip();
            mDisplayStart = new ToolStripMenuItem();
            mDisplayStop = new ToolStripMenuItem();
            mDisplayStartBrowser = new ToolStripMenuItem();
            mExitApplication = new ToolStripMenuItem();

            //Attach the menu to the notify icon
            mNotifyIcon.ContextMenuStrip = mContextMenu;

            //Setup the items and add them to the menu strip, adding handlers to be created later
            mDisplayStart.Text = "restart server";
            mDisplayStart.Click += new EventHandler(mDisplayStart_Click);
            mContextMenu.Items.Add(mDisplayStart);

            mDisplayStop.Text = "stop server";
            mDisplayStop.Click += new EventHandler(mDisplayStop_Click);
            mContextMenu.Items.Add(mDisplayStop);

            mDisplayStartBrowser.Text = "start suite";
            mDisplayStartBrowser.Click += new EventHandler(mDisplayStartBrowser_Click);
            mContextMenu.Items.Add(mDisplayStartBrowser);


            mExitApplication.Text = "Exit";
            mExitApplication.Click += new EventHandler(mExitApplication_Click);
            mContextMenu.Items.Add(mExitApplication);

        }

        void mDisplayStart_Click(object sender, EventArgs e)
        {
            status.startProcess();
        }

        void mDisplayStartBrowser_Click(object sender, EventArgs e)
        {

            ProcessStartInfo bprocessInfo;
            Process bprocess;
            
            bprocessInfo = new ProcessStartInfo();
            bprocessInfo.FileName = "cmd.exe";
            bprocessInfo.Arguments = "/c "+programFolder+"\\amelie.bat";
            bprocess = new Process();
            bprocess.StartInfo = bprocessInfo;
            bprocess.Start();
            

        }

        void mDisplayStop_Click(object sender, EventArgs e)
        {
            status.stopProcess();
        }

        void showTooltip(string data)
        {
            if (data != "" && data != null)
            {
                mNotifyIcon.ShowBalloonTip(20000, "Information", data, ToolTipIcon.Info);
            }
        }

        void mExitApplication_Click(object sender, EventArgs e)
        {
            //Call our overridden exit thread core method!
            mNotifyIcon.Visible = false;
            ExitThreadCore();
        }

        protected override void ExitThreadCore()
        {
            //Call the base method to exit the application
            base.ExitThreadCore();
        }
    }
}
