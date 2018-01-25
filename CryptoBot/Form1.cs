using Binance.Net;
using Cryptobot.Interfaces;
using CryptoBot.Indicators;
using CryptoBot.Strategy.TrendReversal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ZedGraph;

namespace CryptoBot
{
    public partial class Form1 : Form
    {
        private List<IStrategy> _pairs = new List<IStrategy>();
        private BinanceClient _client;
        private Thread _thread;
        private bool _threadRunning;

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
               // if (_pairs.Count >= 5) break;
#endif
            }
            this.toolStripProgressBar1.Maximum = _pairs.Count;
            this.toolStripProgressBar1.Minimum = 0;
            this.toolStripProgressBar1.Value = 0;
            Start();
        }

        private void Start()
        {
            if (_threadRunning) return;
            _threadRunning = true;
            _thread = new Thread(new ThreadStart(Process));
            _thread.Start();
        }

        private void Stop()
        {
            _threadRunning = false;
            _thread.Join();
            _thread = null;
        }

        private void Process()
        {
            while (_threadRunning)
            {
                var table = new DataTable();
                var signal = _pairs[0].Process();
                table.Columns.Add("Pair");
                foreach (var indicator in signal.Indicators)
                {
                    table.Columns.Add(indicator.Name);
                }
                table.Columns.Add("Valid");

                var signals = new List<Signal>();
                int index = 0;
                foreach (var pair in _pairs)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        this.toolStripProgressBar1.Value = index;
                        this.toolStripStatusLabel1.Text = $"Updating {pair.Symbol.NiceName}";
                        index++;
                    }));

                    if (!_threadRunning) return;
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
                    if (!_threadRunning) return;
                    var row = (DataRow)table.NewRow();
                    row["Pair"] = sig.Symbol.NiceName;
                    foreach (var indicator in sig.Indicators)
                    {
                        row[indicator.Name] = indicator.IsValid ? "X" : "";
                    }
                    var validIndicators = sig.Indicators.Count(e => e.IsValid);
                    if (validIndicators == sig.Indicators.Count) row["Valid"] = "X";
                    else row["Valid"] = "";
                    table.Rows.Add(row);
                }
                this.Invoke((MethodInvoker)(() =>
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = new DataView(table);
                    this.toolStripStatusLabel1.Text = $"";
                    this.toolStripProgressBar1.Value = 0;
                }));

                for (int i = 60; i >= 0; i--)
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        this.toolStripStatusLabel1.Text = $"Next update in {i} sec..";
                    }));
                    Thread.Sleep(1000);
                    if (!_threadRunning) return;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            var pair = _pairs.FirstOrDefault(e => e.Symbol.NiceName == pairName);
            if (pair != null)
            {
                var candleStickPoints = new StockPointList();
                var candles = pair.Symbol.Candles;
                var lastCandle = candles.Count - 1;

                for (int i = lastCandle; i >= 0; i--)
                {
                    var candle = candles[i];
                    double x = new XDate(candles[lastCandle].OpenTime);

                    StockPt pt = new StockPt(new XDate(candle.OpenTime),
                                            (double)candle.High,
                                            (double)candle.Low,
                                            (double)candle.Open,
                                            (double)candle.Close,
                                            100000);
                    candleStickPoints.Add(pt);
                }

                var masterPane = zedGraphControl1.MasterPane;
               // while (masterPane.PaneList.Count < 2) masterPane.PaneList.Add(new GraphPane());
                var mainPane = masterPane.PaneList[0];
                // var mbfxPane = masterPane.PaneList[1];
                //mbfxPane.

                string timeFrame = "";
                switch (pair.Symbol.TimeFrame)
                {
                    case TimeFrame.Day:
                        timeFrame = "D1";
                        break;
                    case TimeFrame.FourHour:
                        timeFrame = "H4";
                        break;
                    case TimeFrame.Month:
                        timeFrame = "M1";
                        break;
                    case TimeFrame.OneHour:
                        timeFrame = "H1";
                        break;
                }
                mainPane.Title.Text = pair.Symbol.NiceName + " " + timeFrame;
                mainPane.CurveList.Clear();
                var candleSticks = mainPane.AddJapaneseCandleStick("candles", candleStickPoints);
                candleSticks.Stick.FallingFill = new Fill(Color.Red);
                candleSticks.Stick.RisingFill = new Fill(Color.Blue);

                mainPane.XAxis.Type = AxisType.DateAsOrdinal;

                var trendLine = new TrendLine();
                trendLine.Refresh(pair.Symbol);

                var zigZag = new ZigZag();
                zigZag.Refresh(pair.Symbol);

                var ma15Points = new PointPairList();
                var trendPoints = new PointPairList();
                var zigzagBuyPoints = new PointPairList();
                var zigzagSellPoints = new PointPairList();
                for (int i = lastCandle; i >= 0; i--)
                {
                    var candle = candles[i];
                    ma15Points.Add(new XDate(candle.OpenTime),
                                   (double)MovingAverage.Get(pair.Symbol, i, 15, MaMethod.Sma, AppliedPrice.Close));

                    trendPoints.Add(new XDate(candle.OpenTime),
                                    (double)trendLine.GetValue(i)
                                    );

                    var arrow = zigZag.GetArrow(i);
                    if (arrow == ArrowType.Buy)
                        zigzagBuyPoints.Add(new XDate(candle.OpenTime), (double)candle.Low);
                    else
                        zigzagBuyPoints.Add(new XDate(candle.OpenTime), (double)0);

                    if (arrow == ArrowType.Sell)
                        zigzagSellPoints.Add(new XDate(candle.OpenTime), (double)candle.High);
                    else
                        zigzagSellPoints.Add(new XDate(candle.OpenTime), (double)0);
                }
                var ma15Curve = mainPane.AddCurve("MA15", ma15Points, Color.DarkGray, SymbolType.None);
                var trendCurve = mainPane.AddCurve("Trend", trendPoints, Color.Green, SymbolType.None);

                var zigZagCurveBuy = mainPane.AddCurve("", zigzagBuyPoints, Color.Green, SymbolType.Triangle);
                var zigZagCurveSell = mainPane.AddCurve("", zigzagSellPoints, Color.Red, SymbolType.TriangleDown);
                zigZagCurveBuy.Line.StepType = StepType.ForwardSegment;
                zigZagCurveSell.Line.StepType = StepType.ForwardSegment;

                // pretty it up a little
                mainPane.Chart.Fill = new Fill(Color.LightGray, Color.LightGray, 45.0f);
                mainPane.Fill = new Fill(Color.LightGray, Color.LightGray, 45.0f);

                // Tell ZedGraph to calculate the axis ranges
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            var row = (DataRowView)dgv.Rows[e.RowIndex].DataBoundItem;
            var pairName = (string)row.Row["pair"];
            var pair = _pairs.FirstOrDefault(x => x.Symbol.NiceName == pairName);
            if (pair != null)
            {
                if (pair.Signal.Type == SignalType.Sell)
                {
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.BackColor = Color.Red;
                }
                else if (pair.Signal.Type == SignalType.Buy)
                {
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.BackColor = Color.Green;
                }
                else if (pair.Signal.Type == SignalType.None)
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.BackColor = Color.LightGray;
                }
            }
        }
    }
}