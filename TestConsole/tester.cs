using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TestConsole;


namespace SW.Test.Console
{
    public class Tester
    {
        public static string GetFullPath;

        public class TesterTypeDescription
        {
            public string project_mask;
            public string ext_in;
            public string ext_out;
            public string femx;
            public string semx;
            public string AppName;
            public Comparers.IComparer Comparer;
        }

        private readonly Dictionary<string, TesterTypeDescription> _typeDescriptions = new Dictionary<string, TesterTypeDescription>()
        {
            { "axstream", new TesterTypeDescription()  { project_mask = "*.axx", ext_in = ".txt", ext_out = ".txt", Comparer = new Comparers.AxSTREAM(), AppName="AxSTREAM.exe" } },
            { "axnet", new TesterTypeDescription()  { project_mask = "*.axnet", ext_in = ".txt", ext_out = ".txt", Comparer = new Comparers.AxNET(), AppName="AxSTREAMNET.exe" } },
            { "axcycled", new TesterTypeDescription()  {project_mask = "*.txt", ext_in = ".axcout", ext_out = ".axcout", Comparer = new Comparers.AxCycleD(), AppName="cslv_batch.exe" } },
            { "bearing", new TesterTypeDescription()  { project_mask = "*.bdt", ext_in = ".bdt", ext_out = ".bdt", Comparer = new Comparers.Bearing(), AppName="Bearing.exe" } },
            { "rotordynamics", new TesterTypeDescription()  { project_mask = "*.rdt", ext_in = ".rdt", ext_out = ".rdt", Comparer = new Comparers.RotorDynamics(), AppName="RotorDynamics.exe" } },
            { "axcfd", new TesterTypeDescription()  { project_mask = "*.axx",femx = ".femx", ext_in = ".xml", ext_out = ".xml", Comparer = new Comparers.AxCFD(), AppName="Axstream.exe" }},
            { "axstress", new TesterTypeDescription()  { project_mask = "*.axx", semx = ".semx", ext_in = ".xml", ext_out = ".xml", Comparer = new Comparers.AxSTRESS(), AppName="Axstream.exe" }},
            { "axion", new TesterTypeDescription()  { project_mask = "*.axion", ext_in = ".txt", ext_out = ".txt", Comparer = new Comparers.Axion(), AppName="IONConsole.exe" }},
            { "axceafluiddesigner", new TesterTypeDescription()  { project_mask = "*.cea", ext_in = ".flg", ext_out = ".flg", Comparer = new Comparers.AxCeaFluidDesigner(), AppName="AxCeaFluidDesigner.exe" }},

        };
        #region private methods and fields

        public Tester()
        {
        }
        //method for working with extensions
        private string TaskName(string fileName)
        {
            string shortName = Path.GetFileNameWithoutExtension(fileName);
            string[] subs = shortName.Split(new char[] { '.' });
            string task = subs[subs.Length - 1];

            switch (task)
            {
                case "SA1": return "sa";
                case "SS1": return "ss";
                case "SS2": return "ss";
                case "SS1.SS1": return "ss";
                case "SS1.SA1": return "ss";
                case "BM1.BM1": return "bm";
                case "BM1": return "bm";
                case "ST1": return "st";
                case "T": return "ta";
                case "SG1": return "sg";
                case "ST2": return "st";
                case "AM1": return "am";
                case "CS1": return "cs";
                case "CS2": return "cs";
                case "CSM1": return "csm";
                case "LT1": return "lt";
                case "LT2": return "lt";
                case "LH1": return "lh";
                case "TD": return "td";
                case "TH1": return "th";
                case "TF1": return "tf";
                case "TF2": return "tf";
                case "UR1": return "ur";
                case "WD1": return "wd";
                case "TP": return "tp";
                case "TT1": return "tt";
                case "TT2": return "tt";
                default: return "ta";
            }
        }

        private bool Start(string application, string arguments)
        {
            bool rc = false;
            try
            {
                string d = Environment.CurrentDirectory;
                Process process = Process.Start(application, arguments);
                process.WaitForExit();
                /*rc = 10001  == process.ExitCode; */
                if (rc = 10001 == process.ExitCode)
                {

                }
                else if (rc = 10000 == process.ExitCode)
                {

                }
                else rc = 0 == process.ExitCode;
            }
            catch
            {
                rc = false;
            }
            return rc;
        }
        #endregion

        public Logger Logger { get; set; }
        public int Count { get; set; } = 0;

        private TesterTypeDescription GetTesterDescription(string typename)
        {
            return _typeDescriptions[typename.ToLower()];
        }

        public int Run(string[] args)
        {
            //{ testerType, sourcePath, distDir,  axStreamDir};
            string testerType = args[0].ToLower(); //type
            TesterTypeDescription testerDesctiption = GetTesterDescription(testerType);
            string pathProject = args[1]; //source  // @"";
            string wDirectory = System.IO.Path.GetFullPath(args[2]);
            string applicationPath = Path.Combine(args[3], testerDesctiption.AppName);

            string[] comparer_args = new string[3];
            int err_count = 0;

            if (!Directory.Exists(wDirectory))
            {
                Directory.CreateDirectory(wDirectory);
            }

            if (Logger == null)
            {
                Logger = new Logger(Path.Combine(wDirectory, "log_test.txt"));
            }

            if (!File.Exists(applicationPath))
            {
                Logger.WriteError($"Error : {applicationPath} was not found");
                return -2;
            }

            Logger.WriteLine($"Start test dirrectory {pathProject}");

            string[] projects = Directory.GetFiles(pathProject, testerDesctiption.project_mask);


            Count += projects.Length;
            foreach (string fileName in projects)
            {
                string shortName = Path.GetFileNameWithoutExtension(fileName);
                string ExpectedOutFileName;
                if (testerType == "axcycled") { ExpectedOutFileName = Path.Combine(pathProject, "#" + shortName + ".axcout"); }
                else { ExpectedOutFileName = Path.ChangeExtension(fileName, testerDesctiption.ext_in); }
                string ExpectedOutFileName_semx = Path.ChangeExtension(fileName, testerDesctiption.semx);
                string ExpectedOutFileName1 = Path.Combine(pathProject, shortName + "_input" + testerDesctiption.ext_in);
                string ActualOutFileName;
                if (testerType == "axcycled") { ActualOutFileName = Path.Combine(pathProject, shortName + testerDesctiption.ext_out); }
                else { ActualOutFileName = Path.GetFullPath(Path.Combine(wDirectory, shortName + testerDesctiption.ext_out)); }
                string FemxFileName = Path.ChangeExtension(fileName, testerDesctiption.femx);
                string exactPath = Path.GetFullPath(wDirectory + "\\" + shortName + testerDesctiption.ext_out);
                string FileName_R = Path.Combine(wDirectory, shortName + "_R.dat");
                string FileName_S = Path.Combine(wDirectory, shortName + "_S.dat");
                string FileName_G = Path.Combine(wDirectory, shortName + "_G.dat");
                string FileN_G = Path.Combine(pathProject, shortName + "_G.dat");
                string FileN_R = Path.Combine(pathProject, shortName + "_R.dat");
                string FileN_STR = Path.Combine(pathProject, shortName + ".str");
                string FileN_MRD = Path.Combine(pathProject, shortName + ".mrd");
                string FileN_UPD = Path.Combine(pathProject, shortName + ".upd");
                string FileN_DAT = Path.Combine(pathProject, shortName + ".dat");
                string FileN_INC = Path.Combine(pathProject, shortName + ".inc");
                string FileN_CSV = Path.Combine(pathProject, shortName + ".csv");


                string task = TaskName(fileName);
                string commandArguments;
                string saveWorkingDirrectory = string.Empty;



                switch (testerType)
                {
                    case "axstream":
                        commandArguments = $"\"{fileName}\" -out \"{ActualOutFileName}\" -{task}";
                        break;
                    case "axion":
                        commandArguments = $"\"{fileName}\" -dir:\"{pathProject}\" -debug:\"{ActualOutFileName}\" ";
                        saveWorkingDirrectory = Environment.CurrentDirectory;
                        Environment.CurrentDirectory = args[3];
                        break;
                    case "bearing":
                        commandArguments = $"\"{fileName}\" /{task} \"{ActualOutFileName}\"  ";
                        break;
                    case "bearing2":
                        commandArguments = $"\"{fileName}\" /{task} \"{ActualOutFileName}\"  ";
                        break;
                    case "rotordynamics":
                        commandArguments = $"\"{fileName}\" /{task} \"{ActualOutFileName}\"  ";
                        break;
                    case "axnet":
                        commandArguments = $"\"{fileName}\" -param:\"{ExpectedOutFileName}\" -out:\"{ActualOutFileName}\" -c";
                        break;
                    case "axcfd":
                        commandArguments = $"\"{fileName}\" -w \"{pathProject}\" -fem \"{FemxFileName}\" -femin  \"{ExpectedOutFileName1}\" -save \"{FemxFileName}\" -femout \"{exactPath}\" -t";
                        break;
                    case "axstress":
                        commandArguments = $"\"{fileName}\" -w \"{pathProject}\" -fem \"{ExpectedOutFileName_semx}\" -femout  \"{exactPath}\" -tf ";
                        break;
                    case "axcycled":
                        commandArguments = $"\"{fileName}\"  -out\"{pathProject}\"";
                        break;
                    case "axceafluiddesigner":
                        commandArguments = $"\"{fileName}\"  -c \"{ActualOutFileName}\"";
                        break;


                    default:
                        Logger.WriteError($"Tester type [{testerType}] is not found");
                        return -2;
                }
                Logger.WriteLine($"Start test file {shortName}");

                try
                {

                    if (!File.Exists(ExpectedOutFileName))
                    {
                        throw new Exception($"{shortName} : Expected data {ExpectedOutFileName} was not found");
                    }

                    bool rc = Start(applicationPath, commandArguments);
                    if (!rc)
                    {
                        throw new Exception($"{shortName} : Error execute of {applicationPath} {commandArguments}");
                    }

                    comparer_args[0] = shortName;
                    comparer_args[1] = ActualOutFileName;
                    comparer_args[2] = ExpectedOutFileName;
                    testerDesctiption.Comparer.Compare(comparer_args);

                    Logger.WriteInfo($"Test {shortName} successful");

                    File.Delete(ActualOutFileName);
                    File.Delete(FileName_R);
                    File.Delete(FileName_S);
                    File.Delete(FileName_G);
                    File.Delete(FileN_G);
                    File.Delete(FileN_R);
                    File.Delete(FileN_STR);
                    File.Delete(FileN_MRD);
                    File.Delete(FileN_UPD);
                    File.Delete(FileN_DAT);
                    File.Delete(FileN_INC); 
                    File.Delete(FileN_CSV); 


                }
                catch (Exception ex)
                {
                    Logger.WriteError("Errors:");
                    Logger.WriteError(ex.Message);
                    err_count++;
                    File.Delete(FileN_STR);
                    File.Delete(FileN_MRD);
                    File.Delete(FileN_UPD);
                    File.Delete(FileN_DAT);

                }

                switch (testerType)
                {
                    case "axion":
                        Environment.CurrentDirectory = saveWorkingDirrectory;
                        break;
                }
            }

            string[] directories = Directory.GetDirectories(pathProject);
            foreach (string dirname in directories)
            {
                args[1] = dirname;
                args[2] = Path.Combine(wDirectory, Path.GetFileName(dirname));
                err_count += Run(args);
            }

            if (err_count == 0)
            {
                try
                {
                    Directory.Delete(wDirectory);

                }
                catch
                {

                }
            }
            else
            {
                Logger.WriteLine($"Directory -- {pathProject} has errors in {err_count} of {Count} files");
            }
            return err_count;
        }
    }
}