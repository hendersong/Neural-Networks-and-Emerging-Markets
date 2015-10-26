using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlClient;
using System.Data;

namespace Bachlor_Essay
{
    class Program
    {
        static void Main(string[] args)
        {
            //connect to database
            Bachlor_EssayDataSetTableAdapters.Historic_NSE_DateTableAdapter inserter = new Bachlor_EssayDataSetTableAdapters.Historic_NSE_DateTableAdapter();
            DateTime today = DateTime.Today;
            String error;
            //file with all the tickers on the NSE, split out each ticker individually
            string ticker_file = @"C:\Users\Gabe Henderson\Google Drive\Senior Year\First Semester\Bachlors Essay\Neural\Neural-Networks-and-Emerging-Markets\Tickers.txt";
            string text = System.IO.File.ReadAllText(ticker_file);
            string[] tickers = text.Split(',');
            foreach (String ticker in tickers) {
                //build the URL to connect to google and get the data
                GoogleFinanceDownloader.DownloadURIBuilder a = new GoogleFinanceDownloader.DownloadURIBuilder("NSE", ticker);
                String URL = a.getGetPricesUrlToDownloadAllData(today);
                GoogleFinanceDownloader.DataProcessor process = new GoogleFinanceDownloader.DataProcessor();
                WebClient streamUrl = new WebClient();
                streamUrl.BaseAddress = URL;
                String data = process.processStreamMadeOfOneDayLinesToExtractHistoricalData(streamUrl.OpenRead(URL), out error);
                //split out each day
                String[] b = data.Split('\n');

                //for each day, split out the volume for the day, open and close prices, and date
                foreach (String i in b)
                {
                    String[] array = i.Split(',');
                    if (array.Length < 5)
                        continue;
                    double c;
                    bool can = double.TryParse(array[1], out c);
                    if (!can)
                    {
                        continue;
                    }
                    String da = array[0];
                    String tic = ticker;
                    double open = Convert.ToDouble(array[1]);
                    double close = Convert.ToDouble(array[4]);
                    double vol = Convert.ToDouble(array[5]);
                    //insert into the database
                    inserter.Insert(tic,da, open, close, vol);
                }
            }
            //so i know when its done
            Console.WriteLine("Finished");
            Console.ReadLine();




        }
    }
}
