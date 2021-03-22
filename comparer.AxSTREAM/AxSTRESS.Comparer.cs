using System;
using System.Text;
using System.Xml;

namespace SW.Test.Comparers
{
    public class AxSTRESS : IComparer
    {
        public void Compare(string[] args)
        {
            string shortName = args[0];
            string ActualOutFileName = args[1];
            string ExpectedOutFileName = args[2];

            XmlDocument Actual = new XmlDocument();
            Actual.Load(ActualOutFileName);
            XmlDocument Expected = new XmlDocument();
            Expected.Load(ExpectedOutFileName);
            XmlNodeList actualStress = Actual.ChildNodes[1].ChildNodes;
            XmlNodeList expectedStress = Expected.ChildNodes[1].ChildNodes;

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

            for (
                int i = 0; i < actualStress.Count; i++)
            {
                for (int j = 0; j < actualStress[i].ChildNodes.Count; j++)
                {

                    double a = double.Parse(actualStress[i].ChildNodes[j].InnerText);
                    double e = double.Parse(expectedStress[i].ChildNodes[j].InnerText);

                    double s = System.Math.Abs(a) + System.Math.Abs(e);
                    if (s == 0)
                    {
                        continue;
                    }

                    double tolerance = System.Math.Abs(2 * (a - e) / s);

                    if (tolerance > 0.5)
                    {
                        isFaild = true;
                        a = double.Parse(actualStress[i].ChildNodes[j].InnerText);
                        e = double.Parse(expectedStress[i].ChildNodes[j].InnerText);
                        s = System.Math.Abs(a) + System.Math.Abs(e);
                        tolerance = System.Math.Abs(2 * (a - e) / s) * 100;
                        msg.AppendLine(shortName + ":      Expected:   " + e + "  " + "   Actual:   " + a + "       Tolerance = " + tolerance.ToString());
                    }
                }
            }

            if (isFaild)
            {
                throw new Exception(msg.ToString());
            }
        }
    }
}
