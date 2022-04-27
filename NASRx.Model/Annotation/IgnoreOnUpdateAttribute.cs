using System;

namespace NASRx.Model.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnoreOnUpdateAttribute : Attribute { }
}