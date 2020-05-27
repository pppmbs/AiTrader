using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AiTrader
{
    static class Constants
    {
        public const int TickCount = 2000; // 2000 ticks per bar
        public const int barsLookAhear = 5; // look ahead 5 bars
    }

    class Strategy
    {
        static public void buildBarRecords(IEnumerable records, List<BarRecord> barRecords)
        {
            int tickCount = 0;
            int volCount = 0;
            BarRecord bar = new BarRecord();
            foreach (DataRecord record in records)
            {
                if (tickCount == 0)
                {
                    bar.START_TIME = String.Copy(record.Time);
                    bar.OPEN_PRICE = String.Copy(record.Last);
                    bar.HIGH_PRICE = String.Copy(record.Last);
                    bar.LOW_PRICE = String.Copy(record.Last);
                }

                if (tickCount == Constants.TickCount)
                {
                    bar.END_TIME = String.Copy(record.Time);
                    bar.CLOSE_PRICE = String.Copy(record.Last);
                    bar.TOTAL_VOLUME = volCount.ToString();
                    tickCount = 0;
                    volCount = 0;

                    barRecords.Add(bar);
                    bar = new BarRecord();
                    continue;
                }
                double last = Convert.ToDouble(record.Last);
                double low = Convert.ToDouble(bar.LOW_PRICE);
                double high = Convert.ToDouble(bar.HIGH_PRICE);
                if (last < low)
                    bar.LOW_PRICE = String.Copy(record.Last);
                if (last > high)
                    bar.HIGH_PRICE = String.Copy(record.Last);

                volCount += Int32.Parse(record.Volume);
                tickCount++;
            }
        }

        static public void buildIndicators(List<BarRecord> barRecords)
        {
            SMA sma9 = new SMA(9);
            sma9.LoadOhlcList(barRecords);
            SingleDoubleSerie sma9Serie = sma9.Calculate();
            double?[] sma9Arry = sma9Serie.Values.ToArray();

            SMA sma20 = new SMA(20);
            sma20.LoadOhlcList(barRecords);
            SingleDoubleSerie sma20Serie = sma20.Calculate();
            double?[] sma20Arry = sma20Serie.Values.ToArray();

            SMA sma50 = new SMA(50);
            sma50.LoadOhlcList(barRecords);
            SingleDoubleSerie sma50Serie = sma50.Calculate();
            double?[] sma50Arry = sma50Serie.Values.ToArray();

            MACD macd = new MACD(true);
            macd.LoadOhlcList(barRecords);
            MACDSerie macdSerie = macd.Calculate();
            double?[] macdHistArry = macdSerie.MACDHistogram.ToArray();

            RSI rsi = new RSI(14);
            rsi.LoadOhlcList(barRecords);
            RSISerie rsiSerie = rsi.Calculate();
            double?[] rsiArry = rsiSerie.RSI.ToArray();

            BollingerBand bollingerBand = new BollingerBand();
            bollingerBand.LoadOhlcList(barRecords);
            BollingerBandSerie bollingerSerie = bollingerBand.Calculate();
            double?[] bollLowArry = bollingerSerie.LowerBand.ToArray();
            double?[] bollUpArry = bollingerSerie.UpperBand.ToArray();

            CCI cci = new CCI();
            cci.LoadOhlcList(barRecords);
            SingleDoubleSerie cciSerie = cci.Calculate();
            double?[] cciArry = cciSerie.Values.ToArray();

            int index = 0;
            foreach (BarRecord bar in barRecords)
            {
                bar.SMA9 = sma9Arry[index].ToString();
                bar.SMA20 = sma20Arry[index].ToString();
                bar.SMA50 = sma50Arry[index].ToString();
                bar.MACD_DIFF = macdHistArry[index].ToString();
                bar.RSI = rsiArry[index].ToString();
                bar.BOLL_LOW = bollLowArry[index].ToString();
                bar.BOLL_HIGH = bollUpArry[index].ToString();
                bar.CCI = cciArry[index].ToString();
                index++;
            }

        }

        // pad the indicator values that are "" with known values
        static public void padIndicators(List<BarRecord> barRecords)
        {
            barRecords.Reverse();
            double lastSMA9 = Convert.ToDouble(barRecords.First().SMA9);
            double lastSM20 = Convert.ToDouble(barRecords.First().SMA20);
            double lastSM50 = Convert.ToDouble(barRecords.First().SMA50);
            double lastMACD = Convert.ToDouble(barRecords.First().MACD_DIFF);
            double lastRSI = Convert.ToDouble(barRecords.First().RSI);
            double lastBollLow = Convert.ToDouble(barRecords.First().BOLL_LOW);
            double lastBollHigh = Convert.ToDouble(barRecords.First().BOLL_HIGH);
            double lastCCI = Convert.ToDouble(barRecords.First().CCI);

            foreach (BarRecord bar in barRecords)
            {
                if (string.IsNullOrEmpty(bar.SMA9))
                    bar.SMA9 = lastSMA9.ToString();
                else
                    lastSMA9 = Convert.ToDouble(bar.SMA9);

                if (string.IsNullOrEmpty(bar.SMA20))
                    bar.SMA20 = lastSM20.ToString();
                else
                    lastSM20 = Convert.ToDouble(bar.SMA20);

                if (string.IsNullOrEmpty(bar.SMA50))
                    bar.SMA50 = lastSM50.ToString();
                else
                    lastSM50 = Convert.ToDouble(bar.SMA50);

                if (string.IsNullOrEmpty(bar.MACD_DIFF))
                    bar.MACD_DIFF = lastMACD.ToString();
                else
                    lastMACD = Convert.ToDouble(bar.MACD_DIFF);

                if (string.IsNullOrEmpty(bar.RSI))
                    bar.RSI = lastRSI.ToString();
                else
                    lastRSI = Convert.ToDouble(bar.RSI);

                if (string.IsNullOrEmpty(bar.BOLL_HIGH))
                    bar.BOLL_HIGH = lastBollHigh.ToString();
                else
                    lastBollHigh = Convert.ToDouble(bar.BOLL_HIGH);

                if (string.IsNullOrEmpty(bar.BOLL_LOW))
                    bar.BOLL_LOW = lastBollLow.ToString();
                else
                    lastBollLow = Convert.ToDouble(bar.BOLL_LOW);

                if (string.IsNullOrEmpty(bar.CCI))
                    bar.CCI = lastCCI.ToString();
                else
                    lastCCI = Convert.ToDouble(bar.CCI);
            }
            barRecords.Reverse();
        }

        static public void buildLookAhead5Bars(List<BarRecord> barRecords)
        {
            BarRecord[] barRecordArry = barRecords.ToArray();
            int index = 0;
            double lastHighPrice = 0.0;
            double lastLowPrice = 0.0;
            foreach (BarRecord bar in barRecords)
            {
                 if ((index + Constants.barsLookAhear) >= barRecordArry.Length)
                {
                    bar.NEXT_HIGH_BAR1 = lastHighPrice.ToString();
                    bar.NEXT_HIGH_BAR2 = lastHighPrice.ToString();
                    bar.NEXT_HIGH_BAR3 = lastHighPrice.ToString();
                    bar.NEXT_HIGH_BAR4 = lastHighPrice.ToString();
                    bar.NEXT_HIGH_BAR5 = lastHighPrice.ToString();
                    bar.NEXT_LOW_BAR1 = lastLowPrice.ToString();
                    bar.NEXT_LOW_BAR2 = lastLowPrice.ToString();
                    bar.NEXT_LOW_BAR3 = lastLowPrice.ToString();
                    bar.NEXT_LOW_BAR4 = lastLowPrice.ToString();
                    bar.NEXT_LOW_BAR5 = lastLowPrice.ToString();

                }
                else
                {
                    bar.NEXT_HIGH_BAR1 = barRecordArry[index + 1].HIGH_PRICE;
                    bar.NEXT_HIGH_BAR2 = barRecordArry[index + 2].HIGH_PRICE;
                    bar.NEXT_HIGH_BAR3 = barRecordArry[index + 3].HIGH_PRICE;
                    bar.NEXT_HIGH_BAR4 = barRecordArry[index + 4].HIGH_PRICE;
                    bar.NEXT_HIGH_BAR5 = barRecordArry[index + 5].HIGH_PRICE;
                    bar.NEXT_LOW_BAR1 = barRecordArry[index + 1].LOW_PRICE;
                    bar.NEXT_LOW_BAR2 = barRecordArry[index + 2].LOW_PRICE;
                    bar.NEXT_LOW_BAR3 = barRecordArry[index + 3].LOW_PRICE;
                    bar.NEXT_LOW_BAR4 = barRecordArry[index + 4].LOW_PRICE;
                    bar.NEXT_LOW_BAR5 = barRecordArry[index + 5].LOW_PRICE;
                    lastHighPrice = Convert.ToDouble(bar.NEXT_HIGH_BAR5);
                    lastLowPrice = Convert.ToDouble(bar.NEXT_LOW_BAR5);

                }
                index++;
            }
        }

        static void Main(string[] args)
        {
            using (var sr = new StreamReader(@"ES.csv"))
            {
                using (var sw = new StreamWriter(@"ES-Bar.csv"))
                {
                    var reader = new CsvReader(sr, CultureInfo.InvariantCulture);
                    var writer = new CsvWriter(sw, CultureInfo.InvariantCulture);

                    //CSVReader will now read the whole file into an enumerable
                    IEnumerable records = reader.GetRecords<DataRecord>().ToList();

                    //Covert ticks into bar records
                    List<BarRecord> barRecords = new List<BarRecord>();
                    buildBarRecords(records, barRecords);

                    //Calculate indicators values
                    buildIndicators(barRecords);

                    //pad the unkown indicators values with known values
                    padIndicators(barRecords);

                    //provide the lookahead bars
                    buildLookAhead5Bars(barRecords);

                    //Write the entire contents of the CSV file into another
                    //Do not use WriteHeader as WriteRecords will have done that already.
                    writer.WriteRecords(barRecords);
                    writer.Flush();
                }
            }
        }
    }
}
