using NaiveBayes;

namespace NaiveBayesAlgo
{
    public class Person
    {
        [Class]
        public double ClassVar { get; set; }

        [Fact]
        public double PregnancyCount { get; set; }
        [Fact]
        public double PGC { get; set; }
        [Fact]
        public double BP { get; set; }
        [Fact]
        public double Thickness { get; set; }
        [Fact]
        public double Insulin { get; set; }
        [Fact]
        public double BMI { get; set; }
        [Fact]
        public double DiabetesP { get; set; }
        [Fact]
        public double Age { get; set; }
    }
}