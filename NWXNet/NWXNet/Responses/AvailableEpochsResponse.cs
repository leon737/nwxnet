using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class AvailableEpochsResponse : IResponseData
    {
        private List<DateTime> _epochs = new List<DateTime>();
        public List<DateTime> Epochs
        {
            get { return _epochs; }
            set { _epochs = value; }
        }

        public DateTime this[int i]
        {
            get { return _epochs[i]; }
        }

        public DateTime Earliest
        {
            get { return _epochs[0]; }
        }

        public DateTime Latest
        {
            get { return _epochs[_epochs.Count - 1]; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Available Epochs:");
            sb.AppendLine();
            foreach (var epoch in _epochs)
            {
                sb.AppendLine(epoch.ToNWXString());
            }
            return sb.ToString();
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToNWXString(this DateTime epoch)
        {
            return epoch.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
