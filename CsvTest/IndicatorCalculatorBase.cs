using System;
using System.Collections.Generic;
using LumenWorks.Framework.IO.Csv;
using System.Text;
using System.IO;
using System.Globalization;

namespace AiTrader
{
    public abstract class IndicatorCalculatorBase<T>
    {
        protected abstract List<Ohlc> OhlcList { get; set; }

        public virtual void LoadOhlcList(List<BarRecord> barRecords)
        {
            OhlcList = new List<Ohlc>();
            foreach (BarRecord bar in barRecords)
            {
                Ohlc ohlc = new Ohlc();
                ohlc.Open = Convert.ToDouble(bar.OPEN_PRICE);
                ohlc.Close = Convert.ToDouble(bar.CLOSE_PRICE);
                ohlc.Low = Convert.ToDouble(bar.LOW_PRICE);
                ohlc.High = Convert.ToDouble(bar.HIGH_PRICE);
                ohlc.Volume = Convert.ToDouble(bar.TOTAL_VOLUME);

                OhlcList.Add(ohlc);
            }
        }

        public virtual void Load(string path)
        {
            using (CsvReader csv = new CsvReader(new StreamReader(path), true))
            {
                int fieldCount = csv.FieldCount;
                string[] headers = csv.GetFieldHeaders();
                OhlcList = new List<Ohlc>();
                while (csv.ReadNextRecord())
                {
                    Ohlc ohlc = new Ohlc();
                    for (int i = 0; i < fieldCount; i++)
                    {
                        switch (headers[i])
                        {
                            case "START_TIME":
                                break;
                            case "END_TIME":
                                break;
                            case "OPEN_PRICE":
                                ohlc.Open = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "CLOSE_PRICE":
                                ohlc.Close = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "HIGH_PRICE":
                                ohlc.High = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "LOW_PRICE":
                                ohlc.Low = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "TOTAL_VOLUME":
                                ohlc.Volume = int.Parse(csv[i]);
                                break;
                            case "Date":
                                ohlc.Date = new DateTime(Int32.Parse(csv[i].Substring(0, 4)), Int32.Parse(csv[i].Substring(5, 2)), Int32.Parse(csv[i].Substring(8, 2)));
                                break;
                            case "Open":
                                ohlc.Open = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "High":
                                ohlc.High = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "Low":
                                ohlc.Low = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "Close":
                                ohlc.Close = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            case "Volume":
                                ohlc.Volume = int.Parse(csv[i]);
                                break;
                            case "Adj Close":
                                ohlc.AdjClose = double.Parse(csv[i], CultureInfo.InvariantCulture);
                                break;
                            default:
                                break;
                        }
                    }

                    OhlcList.Add(ohlc);
                }
            }
        }

        public virtual void Load(List<Ohlc> ohlcList)
        {
            this.OhlcList = ohlcList;
        }

        public abstract T Calculate();
    }
}
