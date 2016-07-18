using System;
using QBR.Infrastructure.Models.DataTypes;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class TransientArrivedEventArgs : EventArgs
    {
        private readonly Transient _transient;

        public TransientArrivedEventArgs(Transient transient)
        {
            _transient = transient;
        }

        public Transient Transient
        {
            get { return _transient; }
        }

        public string VialCaseId { get; set; }
    }
}
