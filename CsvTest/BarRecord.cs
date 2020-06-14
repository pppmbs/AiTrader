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
        public string ATR_TrueHigh { get; set; }
        public string ATR_TrueLow { get; set; }
        public string Momemtum { get; set; }
        public string ADX_DIPositive { get; set; }
        public string ADX_DINegative { get; set; }
        public string NEXT_OPEN_BAR1 { get; set; }
        public string NEXT_CLOSE_BAR1 { get; set; }
        public string NEXT_OPEN_BAR2 { get; set; }
        public string NEXT_CLOSE_BAR2 { get; set; }
        public string NEXT_OPEN_BAR3 { get; set; }
        public string NEXT_CLOSE_BAR3 { get; set; }
        public string NEXT_OPEN_BAR4 { get; set; }
        public string NEXT_CLOSE_BAR4 { get; set; }
        public string NEXT_OPEN_BAR5 { get; set; }
        public string NEXT_CLOSE_BAR5 { get; set; }
    }
}
