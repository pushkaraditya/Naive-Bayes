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
        private bool isPrepared = false;
        private PropertyInfo classVar = null;
        private IEnumerable<PropertyInfo> facts = null;
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
                
            classVar = classProps.First();
            if (Attribute.IsDefined(classVar, typeof(FactAttribute)))
                throw new Exception("Class property cannot be a Fact");
            else if(!classVar.PropertyType.IsPrimitive)
                throw new Exception("Class property should be primitive");

            facts = props.Where(prop => Attribute.IsDefined(prop, typeof(FactAttribute)));
            if (facts.Count() == 0)
                throw new Exception(string.Format("No Fact property found. To fix it, please apply {0} to a property", typeof(FactAttribute).FullName));

            var nonNumericFacts = string.Join(",", facts.Where(prop => !prop.PropertyType.IsNumeric()).Select(prop => prop.Name));
            if (!string.IsNullOrWhiteSpace(nonNumericFacts))
                throw new Exception(string.Format("All the facts should be numeric. Facts which are not numeric: {0}", nonNumericFacts));
            // segreagate class variable and facts

            isPrepared = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="splitRatio">this will split the data to create the mode and to test data to validate the model created by first segment</param>
        public void Process(IList<T> data, double splitRatio = .67)
        {
            if (!isPrepared)
                throw new Exception("Processor is not prepared, please call Prepare method before process anything");

            IList<T> modelData, testData;
            data.SplitRandomly(out modelData, out testData);

            // Get distinct values of Class (we will collect these values from model data itself)
            // if for any unique value, there are no records, this will lead to imperfect model

            // prepare model

            // calculate the probability of every attribute of every element of test data for all unique values of Class

            // Compare the values and predict the result

            // publish result and analytics
        }

        public void PrepareModel(IEnumerable<T> modelData)
        {
            // for every distinct value of Class, get the mean and avarage for every distinct value of class

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