using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayesAlgo
{
    public class Processor
    {
        public List<Person> HandleData(string filepath)
        {
            var lines = File.ReadAllLines(filepath);
            return lines.ToList().ConvertAll(line =>
            {
                var data = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                return new Person
                {
                    PregnancyCount = double.Parse(data[0]),
                    PGC = double.Parse(data[1]),
                    BP = double.Parse(data[2]),
                    Thickness = double.Parse(data[3]),
                    Insulin = double.Parse(data[4]),
                    BMI = double.Parse(data[5]),
                    DiabetesP = double.Parse(data[6]),
                    Age = double.Parse(data[7]),
                    ClassVar = double.Parse(data[8])
                };
            });
        }

        public void Process(List<Person> data)
        {
            // prepare model data and test data
            List<Person> modelData, testData;
            data.SplitRandomly(out modelData, out testData, .67);

            // Get summary based on class value
            var diabetesPatients = modelData.FindAll(p => p.ClassVar == 1);
            var diabetesPatientSummary = new
            {
                PregnancyCountAvg = diabetesPatients.Average(p => p.PregnancyCount),
                PregnancyCountStd = diabetesPatients.StdDev(p => p.PregnancyCount),
                PGCAvg = diabetesPatients.Average(p => p.PGC),
                PGCStd = diabetesPatients.StdDev(p => p.PGC),
                BPAvg = diabetesPatients.Average(p => p.BP),
                BPStd = diabetesPatients.StdDev(p => p.BP),
                ThicknessAvg = diabetesPatients.Average(p => p.Thickness),
                ThicknessStd = diabetesPatients.StdDev(p => p.Thickness),
                InsulinAvg = diabetesPatients.Average(p => p.Insulin),
                InsulinStd = diabetesPatients.StdDev(p => p.Insulin),
                BMIAvg = diabetesPatients.Average(p => p.BMI),
                BMIStd = diabetesPatients.StdDev(p => p.BMI),
                DiabetesPAvg = diabetesPatients.Average(p => p.DiabetesP),
                DiabetesPStd = diabetesPatients.StdDev(p => p.DiabetesP),
                AgeAvg = diabetesPatients.Average(p => p.Age),
                AgeStd = diabetesPatients.StdDev(p => p.Age)
            };
            var healthyPersons = modelData.FindAll(p => p.ClassVar == 0);
            var healthyPersonSummary = new
            {
                PregnancyCountAvg = healthyPersons.Average(p => p.PregnancyCount),
                PregnancyCountStd = healthyPersons.StdDev(p => p.PregnancyCount),
                PGCAvg = healthyPersons.Average(p => p.PGC),
                PGCStd = healthyPersons.StdDev(p => p.PGC),
                BPAvg = healthyPersons.Average(p => p.BP),
                BPStd = healthyPersons.StdDev(p => p.BP),
                ThicknessAvg = healthyPersons.Average(p => p.Thickness),
                ThicknessStd = healthyPersons.StdDev(p => p.Thickness),
                InsulinAvg = healthyPersons.Average(p => p.Insulin),
                InsulinStd = healthyPersons.StdDev(p => p.Insulin),
                BMIAvg = healthyPersons.Average(p => p.BMI),
                BMIStd = healthyPersons.StdDev(p => p.BMI),
                DiabetesPAvg = healthyPersons.Average(p => p.DiabetesP),
                DiabetesPStd = healthyPersons.StdDev(p => p.DiabetesP),
                AgeAvg = healthyPersons.Average(p => p.Age),
                AgeStd = healthyPersons.StdDev(p => p.Age)
            };
            
            // calculate probability
            testData.ForEach(p =>
            {
                var diabetes = CalculateProbability(p.PregnancyCount, diabetesPatientSummary.PregnancyCountAvg, diabetesPatientSummary.PregnancyCountStd)
                    * CalculateProbability(p.PGC, diabetesPatientSummary.PGCAvg, diabetesPatientSummary.PGCStd)
                    * CalculateProbability(p.BP, diabetesPatientSummary.BPAvg, diabetesPatientSummary.BPStd)
                    * CalculateProbability(p.Thickness, diabetesPatientSummary.ThicknessAvg, diabetesPatientSummary.ThicknessStd)
                    * CalculateProbability(p.Insulin, diabetesPatientSummary.InsulinAvg, diabetesPatientSummary.InsulinStd)
                    * CalculateProbability(p.BMI, diabetesPatientSummary.BMIAvg, diabetesPatientSummary.BMIStd)
                    * CalculateProbability(p.DiabetesP, diabetesPatientSummary.DiabetesPAvg, diabetesPatientSummary.DiabetesPStd)
                    * CalculateProbability(p.Age, diabetesPatientSummary.AgeAvg, diabetesPatientSummary.AgeStd)
                    ;
            });

            // predict

            // check prediction

            var summary = new
            {

            };
        }

        /// <summary>
        /// Gaussian Probability Density Function
        /// </summary>
        /// <param name="x"></param>
        /// <param name="mean"></param>
        /// <param name="stdev"></param>
        /// <returns></returns>
        private double CalculateProbability(double x, double mean, double stdev)
        {
            var exponent = Math.Exp(-(Math.Pow(x - mean, 2) / (2 * Math.Pow(stdev, 2))));
            return (1 / (Math.Sqrt(2 * Math.PI) * stdev)) * exponent;
        }
    }

    public static class ListExtension
    {
        public static double StdDev<T>(this List<T> list, Func<T, double> selector, double? mean = null)
        {
            double avg = 0;
            if (mean.HasValue)
                avg = mean.Value;
            else
                avg = list.Average(selector);
            var variance = list.Sum(x => Math.Pow(selector(x) - avg, 2)) / (list.Count - 1);
            return Math.Sqrt(variance);
        }

        public static void SplitRandomly<T>(this List<T> list, out List<T> split, out List<T> remaining, double splitRatio = .5)
        {
            if (list.Count == 0)
                split = remaining = null;
            split = list.ToList();
            remaining = new List<T>();
            var random = new Random();
            while (split.Count * splitRatio > remaining.Count)
            {
                var index = random.Next(0, split.Count - 1);
                var item = split[index];
                remaining.Add(item);
                split.Remove(item);
            }
        }
    }
}