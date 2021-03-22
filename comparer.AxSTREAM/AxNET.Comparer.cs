using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SW.Test.Comparers
{
    public class AxNET : IComparer
    {
        private static IEnumerable<Parameter> ParseTransferParameters(string[] lines)
        {
            Regex reg = new Regex(@"(?<value>[\w\.\,+-]+)(\s+\[(?<unit>.+)\])?(\s+(?<name>[^@]+))?\s+@(?<id>(elements|calculators)[^\r\n\s]+)", RegexOptions.Multiline);
            foreach (string line in lines)
            {
                if (reg.IsMatch(line))
                {
                    Match match = reg.Match(line);
                    yield return new Parameter
                    {
                        Id = match.Result("${id}"),
                        DisplayName = match.Result("${name}")?.TrimEnd(),
                        Unit = match.Result("${unit}"),
                        Value = match.Result("${value}")
                    };
                }
            }
        }

        private class Parameter
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string Value { get; set; }
            public string Unit { get; set; }
        }

        public void Compare(string[] args)
        {
            string projectName = args[0];
            string currentFile = args[1];
            string standartFile = args[2];

            if (!File.Exists(currentFile))
            {
                throw new Exception($"{currentFile} Output file was not created. Project: {projectName}.");
            }

            string[] standardOut = File.ReadAllLines(standartFile);
            string[] currentOut = File.ReadAllLines(currentFile);


            List<Parameter> standardParam = ParseTransferParameters(standardOut).ToList();
            List<Parameter> currentParam = ParseTransferParameters(currentOut).ToList();

            foreach (Parameter param in standardParam)
            {
                Parameter current = currentParam.FirstOrDefault(p => p.Id == param.Id);
                if (current == null)
                {
                    throw new Exception($"No found current output for standard parameter. Project {projectName}");
                }

                if (!double.TryParse(current.Value, out double resValue))
                {
                    throw new Exception($"Output value {current.Value} couldn't be parsed. Project {projectName}");
                }

                if (!double.TryParse(param.Value, out double stndValue))
                {
                    throw new Exception($"Standard value {param.Value} couldn't be parsed. Project {projectName}");
                }

                double delta = 0.01 * Math.Abs(stndValue);
                if (delta == 0)
                {
                    delta = 0.01;
                }

                if (Math.Abs(stndValue - resValue) >= delta)
                {
                    throw new Exception($"Project: {projectName}. Value of parameter {param.DisplayName} not corresponding with standard. Delta: {delta} stndValue: {stndValue}. resValue: {resValue}" );
                }
            }
        }
    }
}
