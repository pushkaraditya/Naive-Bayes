using System;

namespace NaiveBayes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FactAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Property)]
    public class ClassAttribute : Attribute { }
}