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

                if (tickCount == 2000)
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

                    //Write the entire contents of the CSV file into another
                    //Do not use WriteHeader as WriteRecords will have done that already.
                    writer.WriteRecords(barRecords);
                    writer.Flush();
                }
            }
        }
    }
}
