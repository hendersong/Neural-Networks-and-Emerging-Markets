using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Data.Entity;

namespace Bachlor_Essay
{
    class Program
    {
        static void Main(string[] args)
        {
            Bachlor_EssayDataSetTableAdapters.Historic_NSE_DateTableAdapter inserter = new Bachlor_EssayDataSetTableAdapters.Historic_NSE_DateTableAdapter();
            Bachlor_EssayEntities db = new Bachlor_EssayEntities();
            string india_GDP = "https://www.quandl.com/api/v3/datasets/ODA/IND_NGDPD.csv?start_date=2000-01-01?auth_token=B8sK7V6UPsviYsy4pNFF";
            string us_yield = "https://www.quandl.com/api/v3/datasets/USTREASURY/YIELD.csv?start_date=2000-01-01?auth_token=B8sK7V6UPsviYsy4pNFF";
            //Download_Macro(india_GDP, db);
            download_us_bond_yields(us_yield, db);
            Console.ReadKey();
            Console.WriteLine("Done");

        }
        static void download_us_bond_yields(string URL, Bachlor_EssayEntities db)
        {
            WebClient streamUrl = new WebClient();
            streamUrl.BaseAddress = URL;
            string URI = URL;
            Stream d = streamUrl.OpenRead(URI);
            StreamReader reader = new StreamReader(d);
            while (reader.Peek() > 0)
            {
                string s = reader.ReadLine();
                Console.WriteLine(s);
                Console.WriteLine();
                string[] data= s.Split(',');
                US_Yield g = new US_Yield();
                double c;
                bool can = double.TryParse(data[1], out c);
                if (!can)
                {
                    continue;
                }
                g.Date_ = data[0];
                if(!String.IsNullOrEmpty(data[1]))
                    g.one_mo = Convert.ToDouble(data[1]);
                if (!String.IsNullOrEmpty(data[2]))
                    g.three_mo = Convert.ToDouble(data[2]);
                if (!String.IsNullOrEmpty(data[3]))
                    g.six_mo = Convert.ToDouble(data[3]);
                if (!String.IsNullOrEmpty(data[4]))
                    g.one_yr = Convert.ToDouble(data[4]);
                if (!String.IsNullOrEmpty(data[5]))
                    g.two_yr = Convert.ToDouble(data[5]);
                if (!String.IsNullOrEmpty(data[6]))
                    g.three_yr = Convert.ToDouble(data[6]);
                if (!String.IsNullOrEmpty(data[7]))
                    g.five_yr = Convert.ToDouble(data[7]);
                if (!String.IsNullOrEmpty(data[8]))
                    g.seven_yr = Convert.ToDouble(data[8]);
                if (!String.IsNullOrEmpty(data[9]))
                    g.ten_yr = Convert.ToDouble(data[9]);
                if (!String.IsNullOrEmpty(data[10]))
                    g.twenty_yr = Convert.ToDouble(data[10]);
                if (!String.IsNullOrEmpty(data[11]))
                    g.thirty_yr = Convert.ToDouble(data[11]);
                db.US_Yield.Add(g);
            }
            db.SaveChanges();
            d.Close();
            reader.Close();
        }
        static void Download_Macro(string URL, Bachlor_EssayEntities db)
        {
            string India_GDP = URL;
            WebClient streamUrl = new WebClient();
            streamUrl.BaseAddress = India_GDP;
            string URI = India_GDP;
            Stream data = streamUrl.OpenRead(URI);
            StreamReader reader = new StreamReader(data);
            while (reader.Peek() > 0)
            {
                string s = reader.ReadLine();
                string[] data_string = s.Split(',');
                India_GDP g = new India_GDP();
                double c;
                bool can = double.TryParse(data_string[1], out c);
                if (!can)
                {
                    continue;
                }
                g.Date_ = data_string[0];
                g.GDP = Convert.ToDouble(data_string[1]);
                db.India_GDP.Add(g);
            }
            db.SaveChanges();
            data.Close();
            reader.Close();
        }
        static void Download_prices(Bachlor_EssayDataSetTableAdapters.Historic_NSE_DateTableAdapter inserter){
                //connect to database
                
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
                        inserter.Insert(tic, da, open, close, vol);
                    }
                }
                //so i know when its done
                Console.WriteLine("Finished");
                Console.ReadLine();
            }




        
    }
}
