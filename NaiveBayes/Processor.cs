using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    /// <summary>
    /// Processor will only process the facts at object's property level and not the property of composite property
    /// e.g. Employee.Salary will be processed however Employee.Health.Age will not be processed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TClass">Type of class property (at this moment, I not sure if this needs to be defined)</typeparam>
    public class Processor<T, TClass> where T : new()
    {
        /*
         * Bugs: Class Var property holds the value Yes and YES, it might give different results
         */

        private const string Avg = "Avg";
        private const string Std = "Std";
        private bool isPrepared = false;
        private string classVar = null;
        private IEnumerable<string> facts = null;
        public void Prepare()
        {
            T t = new T();
            var props = t.GetType().GetProperties();

            // check only one class attribute (I am not sure if this is necessary, I might have to remove it later)
            var classProps = props.Where(prop => Attribute.IsDefined(prop, typeof(ClassAttribute)));
            var classCount = classProps.Count();
            if (classCount == 0)
                throw new Exception(string.Format("No Class property found. To fix it, please apply {0} to a property", typeof(ClassAttribute).FullName));
            else if (classCount > 1)
                throw new Exception(string.Format("More than one Class property found. To fix it, please remove {0} from a property", typeof(ClassAttribute).FullName));

            var classProp = classProps.First();
            if (Attribute.IsDefined(classProp, typeof(FactAttribute)))
                throw new Exception("Class property cannot be a Fact");
            else if (!classProp.PropertyType.IsPrimitive)
                throw new Exception("Class property should be primitive");

            var factProps = props.Where(prop => Attribute.IsDefined(prop, typeof(FactAttribute)));
            if (factProps.Count() == 0)
                throw new Exception(string.Format("No Fact property found. To fix it, please apply {0} to a property", typeof(FactAttribute).FullName));

            var nonNumericFacts = string.Join(",", factProps.Where(prop => !prop.PropertyType.IsNumeric()).Select(prop => prop.Name));
            if (!string.IsNullOrWhiteSpace(nonNumericFacts))
                throw new Exception(string.Format("All the facts should be numeric. Facts which are not numeric: {0}", nonNumericFacts));

            classVar = classProp.Name;
            facts = factProps.Select(f => f.Name);
            isPrepared = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="splitRatio">this will split the data to create the mode and to test data to validate the model created by first segment</param>
        public double Process(IList<T> data, double splitRatio = .67)
        {
            if (!isPrepared)
                throw new Exception("Processor is not prepared, please call Prepare method before process anything");

            if (data == null || data.Count == 0)
                throw new ArgumentNullException("no data to process");

            // convert data dictionary of data
            var convertedData = data.Select(d => d.ToDictionary()).ToList();

            IList<IDictionary<string, object>> modelData, testData;
            convertedData.SplitRandomly(out modelData, out testData, splitRatio);

            // Get distinct values of Class (we will collect these values from model data itself)
            var distincts = convertedData.Select(o => o[classVar] == null ? null : o[classVar].ToString()).Distinct();
            // if for any unique value, there are no records, this will lead to imperfect model
            foreach (var distinct in distincts)
                if (modelData.Count(o => o[classVar].ToString() == distinct) == 0)
                    throw new Exception("Created model is imperfact, please try again. This may be becase of sparsely distributed data");

            // prepare model
            var model = PrepareModel(modelData, distincts);
            // calculate the probability of every attribute of every element of test data for all unique values of Class
            // Compare the values and predict the result
            var correct = 0;
            foreach (var item in testData)
            {
                var output = new Dictionary<string, double>();
                foreach (var distinct in distincts)
                {
                    double pItem = 1;
                    foreach (var fact in facts)
                    {
                        var val = Convert.ToDouble(item[fact]);
                        var p = CalculateProbability(val, model[distinct][fact][Avg], model[distinct][fact][Std]);
                        pItem *= p;
                    }
                    output.Add(distinct, pItem);
                }

                var prediction = output.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                if (item[classVar].ToString() == prediction)
                    correct += 1;
            }

            // publish result and analytics
            var correctness = (correct * 1.0) / testData.Count;
            return correctness;
        }

        /// <summary>
        /// This creates the model or 3D array for distinct values of ClassVar, Fact and Avg/Std value
        /// </summary>
        /// <param name="modelData"></param>
        /// <param name="distinctClassValues"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, Dictionary<string, double>>>
            PrepareModel(IEnumerable<IDictionary<string, object>> modelData, IEnumerable<string> distinctClassValues)
        {
            var model = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            // for every distinct value of Class, get the mean and avarage for every distinct value of class
            foreach (var distinct in distinctClassValues)
            {
                model.Add(distinct, new Dictionary<string, Dictionary<string, double>>());
                foreach (var fact in facts)
                {
                    var factData = modelData.Where(o => o[classVar].ToString() == distinct).Select(o => Convert.ToDouble(o[fact]));
                    var mean = factData.Average();
                    var std = factData.StdDev(o => o, mean);

                    model[distinct].Add(fact, new Dictionary<string, double>());
                    model[distinct][fact] = new Dictionary<string, double>();
                    model[distinct][fact].Add(Avg, mean);
                    model[distinct][fact].Add(Std, std);
                }
            }
            return model;
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
}