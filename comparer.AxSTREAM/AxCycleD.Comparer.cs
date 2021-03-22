using System;
using System.IO;
using System.Text;


namespace SW.Test.Comparers
{
    public class AxCycleD : IComparer
    {
        public static int DifferentFiles;
        public void Compare(string[] args)
        {
            string projectName = args[0];
            string currentFile = args[1];
            string standartFile = args[2];

            string[] Expected = new string[] { };
            string[] Actual = new string[] { };
            CheckLenght(standartFile, currentFile, out Expected, out Actual);
            bool isFaild = false;
            StringBuilder msg = new StringBuilder();

            for (int j = 0; j < Expected.Length; j++)
            {
                string[] _expected = Expected[j].Split('|');
                string[] _actual = Actual[j].Split('|');
                if (_expected.Length == 1)
                {
                    _expected = Expected[j].Split(' ');
                    _actual = Actual[j].Split(' ');
                }

                bool nr = CheckRowLeng(_expected, _actual, j, currentFile);

                if (!nr)
                {
                   msg.ToString(/*$"LENGTH >>{Actual[j]}<< IS DIFFERENT FROM >>{Expected[j]}<<"*/);
                }

                for (int t = 0; t < _actual.Length; t++)
                {
                    double.TryParse(_expected[t], out double ExpNumb);
                    double.TryParse(_actual[t], out double ActNumb);
                    double s = System.Math.Abs(ActNumb) + System.Math.Abs(ExpNumb);
               
                    if (s == 0.0)
                    { continue; }

                    double tolerance = ((ActNumb - ExpNumb) / ((ActNumb + ExpNumb) / 2) ) *100;
                    
                    if (tolerance > 0.5 & ActNumb > 0.0001)                    {
                        isFaild = true;
                        msg.AppendLine(ActNumb + " actual   " + ExpNumb + " expected   " + "Tolerance = " + tolerance);
                      
                    }
                 }
            }
            if (isFaild)
            {
                throw new Exception(msg.ToString());
            }
        }

        public static bool CheckRowLeng(string[] Exp, string[] Act, int numR, string Name)
        {
            return Exp.Length != Act.Length;
        }
        public static void CheckLenght(string Exp, string Act, out string[] Expecte, out string[] Actua)
        {
            Expecte = new string[] { };
            Actua = new string[] { };
            try
            {
                if (!File.Exists(Act))
                {
                    throw new FileNotFoundException($"File {Act} isn't exist");
                }

                Expecte = File.ReadAllLines(Exp);
                Actua = File.ReadAllLines(Act);

                if (Expecte.Length != Actua.Length)
                {
                    throw new Exception($"LENGTH {Act} IS DIFFERENT FROM {Exp}");
                }
            }
            catch (FileNotFoundException)
            {
                throw new Exception($"File {Act} isn't exist");
            }

        }
    }
}

