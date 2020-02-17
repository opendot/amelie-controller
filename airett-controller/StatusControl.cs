using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Win32;

//Request library
using System.Net;
using System.IO;
using Tobii.Research;


namespace airettController
{
    class StatusControl
    {
        private string serverStatus;
        private int lastAction;
        private int driverStatus;
        private string localAddress;
        private string programFolder;
        private string gitPath;
        private NotifyIcon mNotifyIcon;

        private ProcessStartInfo psi;
        private ProcessStartInfo trackerPsi;
        private Process process;
        private Process trackerProcess;

        private bool pro = false;
        private int trackerPresent = -1;


        public StatusControl(NotifyIcon ni, string pf)
        {
            const string userRoot = "HKEY_LOCAL_MACHINE";
            const string subkey = "SOFTWARE\\GitForWindows";
            const string keyName = userRoot + "\\" + subkey;
            gitPath = (string)Registry.GetValue(keyName,"InstallPath", "git not found");
            Console.WriteLine(gitPath);
            serverStatus = "";
            lastAction = 0;
            driverStatus = 0;
            mNotifyIcon = ni;
            localAddress = ConfigurationManager.AppSettings["localServerAddress"];
            programFolder = pf;
            checkState(TimeSpan.FromSeconds(1));
            checkTrackerRoutine(TimeSpan.FromSeconds(1));
            configureProcess();
            configureDriverProcess();
            startProcess();
        }

        public async Task checkState(TimeSpan timeout)
        {
            await Task.Delay(timeout).ConfigureAwait(false);

            string res = Get(localAddress);
            updateServerStatus(res);

            checkState(timeout);
        }

        public async Task checkTrackerRoutine(TimeSpan timeout)
        {
            await Task.Delay(timeout).ConfigureAwait(false);
            int check = checkTracker();
            Console.WriteLine(check);
            if (trackerPresent == -1 && check == 0)
            {
                mNotifyIcon.ShowBalloonTip(20000, "Information", "Please connect your eyetracker", ToolTipIcon.Info);
            }
            else if(check == 1 && trackerPresent != 1) {
                startDriverProcess();
            }
            trackerPresent = check;
            Console.WriteLine(trackerPresent);

            checkTrackerRoutine(timeout);
        }

        int checkTracker()
        {
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            if (eyeTrackers.Count == 0)
            {
                return 0;
            }
            IEyeTracker eyeTracker = eyeTrackers[0];
            // Create a calibration object.
            var calibration = new ScreenBasedCalibration(eyeTracker);
            // Enter calibration mode.
            try
            {
                calibration.EnterCalibrationMode();
                calibration.LeaveCalibrationMode();
                pro = true;
                return 1;
            }
            catch (Tobii.Research.InsufficientLicenseException ex)
            {
                Console.WriteLine("no pro license");
                return 1;
            }
            catch (Tobii.Research.DisplayAreaNotValidException ex)
            {
                return 1;
            }

        }


        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            

            HttpStatusCode resp = response.StatusCode;
            return resp.ToString();
            }
            catch (Exception ex)
            {
                return "KO";
            }
        }

        private void configureProcess()
        {
            psi = new ProcessStartInfo();
            psi.FileName = @gitPath+@"\bin\bash.exe";
            psi.WorkingDirectory = @"C:\Program Files\Docker Toolbox";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = false;
            psi.CreateNoWindow = false;
        }

        private void configureDriverProcess()
        {
            trackerPsi = new ProcessStartInfo();
            trackerPsi.WindowStyle = ProcessWindowStyle.Normal;
            trackerPsi.CreateNoWindow = false;
            trackerPsi.RedirectStandardOutput = false;
            trackerPsi.RedirectStandardError = false;
            trackerPsi.UseShellExecute = false;
            trackerPsi.Arguments = pro ? "--pro" : "";
            trackerPsi.FileName = programFolder + "\\airett-driver\\airett_driver.exe";
        }

        public void startProcess()
        {
            process = new Process();
            string backfolder = programFolder.Replace("\\","/");
            Console.WriteLine(backfolder);
            psi.Arguments = @"--login -i ""C:\Program Files\Docker Toolbox\start.sh""  cd "+backfolder+"/airett-rails-server; ./start_staging_local.sh";
            process.StartInfo = psi;

            // start the process
            // then begin asynchronously reading the output
            process.Start();
        }

        public void startDriverProcess()
        {
            trackerProcess = new Process();
            trackerProcess.StartInfo = trackerPsi;
            trackerProcess.EnableRaisingEvents = true;
            Console.WriteLine("starting driver");
            trackerProcess.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            trackerProcess.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            trackerProcess.Exited += new EventHandler(ExitHandler);

            // start the process
            // then begin asynchronously reading the output
            trackerProcess.Start();
            Console.WriteLine("driver started");
            ChildProcessTracker.AddProcess(trackerProcess);
            trackerProcess.BeginOutputReadLine();
            trackerProcess.BeginErrorReadLine();
            trackerProcess.WaitForExit();
        }

        public void stopProcess()
        {
            process = new Process();
            psi.Arguments = @"--login -i ""C:\Program Files\Docker Toolbox\start.sh""  cd " + programFolder + "/airett-rails-server; docker-compose -f docker-compose.staging_local.yml down";
            process.StartInfo = psi;
            // start the process
            // then begin asynchronously reading the output
            process.Start();
        }

        public void updateServerStatus(string res)
        {
            if (res == "OK" && (serverStatus == "KO" || serverStatus == ""))
            {
                mNotifyIcon.ShowBalloonTip(20000, "Information", "Amelie server is ready",
            ToolTipIcon.Info);
                mNotifyIcon.Icon = new System.Drawing.Icon("airett_on.ico");
            }
            else if (res == "KO" && serverStatus == "OK")
            {
                mNotifyIcon.ShowBalloonTip(20000, "Information", "Amelie server is down",
            ToolTipIcon.Info);
                mNotifyIcon.Icon = new System.Drawing.Icon("airett_off.ico");
            }
            serverStatus = res;
        }

        void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            //showTooltip(outLine.Data);
            Console.WriteLine(outLine.Data);
        }

        void ExitHandler(object sender, System.EventArgs e)
        {
            // Console.WriteLine("exited!");
            //process.Close();
            trackerProcess.StartInfo.Arguments = pro ? "--recover --pro" : "--recover";
            trackerProcess.Start();
            ChildProcessTracker.AddProcess(trackerProcess);

        }
    }

    
}
