using System;
using System.Collections.Generic;
using System.Text;

namespace AiTrader
{
    public class BarRecord
    {
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string OPEN_PRICE { get; set; }
        public string CLOSE_PRICE { get; set; }
        public string HIGH_PRICE { get; set; }
        public string LOW_PRICE { get; set; }
        public string TOTAL_VOLUME { get; set; }
        public string SMA9 { get; set; }
        public string SMA20 { get; set; }
        public string SMA50 { get; set; }
        public string MACD_DIFF { get; set; }
        public string RSI { get; set; }
        public string BOLL_LOW { get; set; }
        public string BOLL_HIGH { get; set; }
        public string CCI { get; set; }

    }
}
