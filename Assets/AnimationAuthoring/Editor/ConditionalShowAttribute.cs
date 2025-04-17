using System;
using UnityEngine;

namespace com.animationauthoring
{
    /// <summary>
    /// Class that is used to compare objects to either show these object in the inspector or not
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class ConditionalShowAttribute : PropertyAttribute
    {
        public string conditionalFieldName;
        public object expectedValue;

        public ConditionalShowAttribute(string conditionalFieldName, object expectedValue)
        {
            this.conditionalFieldName = conditionalFieldName;
            this.expectedValue = expectedValue;
        }
    }
}
