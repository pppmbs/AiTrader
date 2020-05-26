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
    public class BarRecord
    {
        public string START_TIME { get; set; }
        public string END_TIME { get; set; }
        public string OPEN_PRICE { get; set; }
        public string CLOSE_PRICE { get; set; }
        public string HIGH_PRICE { get; set; }
        public string LOW_PRICE { get; set; }
        public string TOTAL_VOLUME { get; set; }
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

                    List<BarRecord> barRecords = new List<BarRecord>();
                    buildBarRecords(records, barRecords);

                    //Write the entire contents of the CSV file into another
                    //Do not use WriteHeader as WriteRecords will have done that already.
                    writer.WriteRecords(barRecords);
                    writer.Flush();
                }
            }
        }
    }
}
