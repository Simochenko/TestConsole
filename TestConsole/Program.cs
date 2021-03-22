using System;
using System.Diagnostics;
using System.IO;


namespace SW.Test.Console
{
    internal class Program
    {
        public static string BuildDir { get; set; }

        private static string StartProccess(string workingDirectory, string programName, string arguments)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = programName;
            p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.Arguments = arguments;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;

        }

        private static string cmd(string command)
        {
            return StartProccess("cmd.exe", $"/C {command}", "");
        }

        private static void Main(string[] args)
        {


            string testerType = args[0]; //type
            string sourcePath = args[1]; //source
            string distPath = args[2]; //results
            string axStreamPath = args[3]; //bin

            string dateTime = DateTime.Now.ToString("dd_MM_yy(hh_mm_ss)");
            string distDir = Path.Combine(distPath, dateTime);
            //            string axStreamDir = Path.Combine(distPath, "Run");
            //            string strCmdText;
            //            Directory.CreateDirectory(distDir);
            //            Directory.CreateDirectory(axStreamDir);
            //            strCmdText = $"/C copy /b {Path.Combine(sourcePath,"*.*")} {distDir}";
            //            Process p = Process.Start("CMD.exe", strCmdText);

            //            string strCmdText1 = $"/C XCOPY \"{Path.Combine(axStreamPath,"*")}\" \"{axStreamDir}\" /y /e /i";
            //            Process p1 = Process.Start("CMD.exe", strCmdText1);
            //            p.WaitForExit();
            //            p1.WaitForExit();

            //   if (!Directory.Exists(distDir))
            //      Directory.CreateDirectory(distDir);
            Directory.SetCurrentDirectory(distPath);

            //Environment.SetEnvironmentVariable("AGENT_BUILDDIRECTORY", Path.Combine(axStreamDir, "AxSTREAM.exe"));

            {
                Tester tester = new Tester();
                string[] arguments = new string[] { testerType, sourcePath, distDir, axStreamPath };
                int err_count = tester.Run(arguments);

            }
            System.Console.WriteLine("Press <ENTER> to exit...");
            System.Console.ReadLine();
        }
    }
}
