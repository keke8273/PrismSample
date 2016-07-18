using System;

namespace QBR.Utilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }
}
