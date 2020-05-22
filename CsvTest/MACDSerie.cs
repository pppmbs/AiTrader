using System;
using System.Collections.Generic;
using System.Text;

namespace AiTrader
{
    public class MACDSerie : IIndicatorSerie
    {
        public List<double?> MACDLine { get; set; }
        public List<double?> MACDHistogram { get; set; }
        public List<double?> Signal { get; set; }

        public MACDSerie()
        {
            this.MACDLine = new List<double?>();
            this.MACDHistogram = new List<double?>();
            this.Signal = new List<double?>();
        }
    }
}
