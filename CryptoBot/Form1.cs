using Binance.Net;
using Cryptobot.Interfaces;
using CryptoBot.Strategy.TrendReversal;
using CryptoBotApp.bot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace CryptoBot
{
    public partial class Form1 : Form
    {
        private List<IStrategy> _pairs = new List<IStrategy>();
        private BinanceClient _client;
        public Form1()
        {
            InitializeComponent();
            var apiKey = ConfigurationManager.AppSettings["apikey"];
            var apiSecret = ConfigurationManager.AppSettings["apisecret"];

            _client = new BinanceClient(apiKey, apiSecret);

            var result = _client.GetAllPrices();
            if (!result.Success)
            {
                return;
            }

            var factory = new TrendReversalStrategyFactory();
            var timeFrame = TimeFrame.FourHour;
            var symbols = result.Data.OrderBy(e => e.Symbol);
            foreach (var symbol in symbols)
            {
                _pairs.Add(factory.Create(new CryptoBotApp.bot.Symbol(_client, symbol.Symbol, timeFrame)));
#if DEBUG
                if (_pairs.Count >= 5) break;
#endif
            }

        }

        private void Process()
        {
            var table = new DataTable();
            dataGridView1.Rows.Clear();
            var signal = _pairs[0].Process();
            table.Columns.Add("Pair");
            foreach (var indicator in signal.Indicators)
            {
                table.Columns.Add(indicator.Name);
            }

            var signals = new List<Signal>();
            foreach (var pair in _pairs)
            {
                pair.Symbol.Refresh();
                if (pair.Symbol.Candles == null) continue;
                if (pair.Symbol.Candles.Count == 0) continue;
                signal = pair.Process();
                if (signal.Indicators[0].IsValid)
                {
                    signals.Add(signal);
                }
            }
            signals = signals.OrderByDescending(e => e.Count).ToList();
            foreach (var sig in signals)
            {
                var row = (DataRow)table.NewRow();
                row["Pair"] = sig.Symbol.Name;
                foreach (var indicator in sig.Indicators)
                {
                    row[indicator.Name] = indicator.IsValid ? "X" : "";
                }
                table.Rows.Add(row);
            }
            dataGridView1.DataSource = new DataView(table);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process();

            var myPane = zedGraphControl1.GraphPane;

            myPane.Title.Text = "Trend reversal strategy";
            myPane.XAxis.Title.Text = "Date";
            myPane.YAxis.Title.Text = "Price";

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs xx)
        {
            if (dataGridView1.SelectedRows.Count <= 0) return;
            var row = dataGridView1.SelectedRows[0];
            string pairName = (string)row.Cells[0].Value;
            var pair = _pairs.FirstOrDefault(e => e.Symbol.Name == pairName);
            if (pair != null)
            {
                var spl = new StockPointList();
                var candles = pair.Symbol.Candles;
                var lastCandle = candles.Count - 1;

                for (int i = lastCandle; i >=0; i--)
                {
                    var candle = candles[i];
                    double x = new XDate(candles[lastCandle].OpenTime);

                    StockPt pt = new StockPt(new XDate(candle.OpenTime), 
                                            (double)candle.High,
                                            (double)candle.Low,
                                            (double)candle.Open,
                                            (double)candle.Close, 
                                            100000);
                    spl.Add(pt);
                }

                var myPane = zedGraphControl1.GraphPane;
                myPane.CurveList.Clear();
                var candleSticks = myPane.AddJapaneseCandleStick("trades", spl);
                candleSticks.Stick.IsAutoSize = true;
                candleSticks.Stick.Color = Color.Blue;

                // Use DateAsOrdinal to skip weekend gaps
                myPane.XAxis.Type = AxisType.DateAsOrdinal;

                // pretty it up a little
                myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
                myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);

                // Tell ZedGraph to calculate the axis ranges
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();

            }
        }
    }
}