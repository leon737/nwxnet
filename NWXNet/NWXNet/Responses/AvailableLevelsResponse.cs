using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class AvailableLevelsResponse : IResponseData
    {
        private List<int> _levels = new List<int>();
        public List<int> Levels
        {
            get { return _levels; }
            set { _levels = value; }
        }

        public int this[int i]
        {
            get { return _levels[i]; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Available Levels:");
            sb.AppendLine();
            foreach (var level in _levels)
            {
                sb.AppendLine(level.ToString());
            }
            return sb.ToString();
        }
    }
}
