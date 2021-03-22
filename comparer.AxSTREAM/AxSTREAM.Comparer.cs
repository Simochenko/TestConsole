using SW.Common;
using System;
using System.IO;
using System.Text;

namespace SW.Test.Comparers
{
    public class AxSTREAM : IComparer
    {
        public void Compare(string[] args)
        {
            string shortName = args[0];
            string ActualOutFileName = args[1];
            string ExpectedOutFileName = args[2];

            Parameters Actual = new Parameters(File.ReadAllLines(ActualOutFileName));
            Parameters Expected = new Parameters(File.ReadAllLines(ExpectedOutFileName));

            if (Expected.Count() > Actual.Count())
            {
                throw new Exception($"{shortName} : Actual data has less parameters then Expected data");
            }

            if (Actual == null)
            {
                throw new Exception($"{shortName} : {ActualOutFileName} was not found");
            }

            if (Expected == null)
            {
                throw new Exception($"{shortName} : {ExpectedOutFileName} was not found");
            }

            bool isFaild = false;
            StringBuilder msg = new StringBuilder();

            for (int i = 0; i < Expected.Count(); i++)
            {
                Parameters pA = Actual[i].Parameters;
                Parameters pE = Expected[i].Parameters;

                double a = pA[0].AsDouble();
                double e = pE[0].AsDouble();
                double s = System.Math.Abs(a) + System.Math.Abs(e);

                if (s == 0.0)
                {
                    continue;
                }

                double tolerance = System.Math.Abs(2 * (a - e) / s);

                if (tolerance > 1.3e-3)
                {
                    isFaild = true;
                    string physicalQuantity = Actual[i].Value.Split('\t')[2].Trim();
                    string Units = Actual[i].Value.Split('\t')[1].Trim();
                    msg.AppendLine($"Expected:   {e}  {physicalQuantity}{Units}   Actual:   {a}  {physicalQuantity}{Units}  Tolerance = {100 * tolerance}");
                }
            }
            if (isFaild)
            {
                throw new Exception(msg.ToString());
            }
        }
    }
}
