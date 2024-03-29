﻿using FundMonitor.helper;
using FutureMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace FundMonitor
{
    public partial class Form1 : Form
    {
        private List<Position> positions = new List<Position>();
        private double preIncrease = 0, preMinIncrease = 10000;
        private double preClose = 0, cost = 0;
        private int accuracy = 0;
        private string name;
        private int isLong = 1;


        private void setValue(string key, Object value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");
            XmlElement root = (XmlElement)xmlDoc.SelectSingleNode("app");//查找<bookstore>
            if (root == null)
                return;
            root.SetAttribute(key, value.ToString());
            xmlDoc.Save("data.xml");
        }

        private string getValue(string key)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("data.xml");
            XmlElement root = (XmlElement)xmlDoc.SelectSingleNode("app");//查找<bookstore>
            if (root == null)
                return null;
            if (root.HasAttribute(key))
                return root.GetAttribute(key);
            else
                return null;
        }
        public Form1()
        {
            InitializeComponent();

            var left = this.getValue("left");
            if (left != null)
            {
                this.Left = Convert.ToInt32(left);
            }
            var top = this.getValue("top");
            if (top != null)
            {
                this.Top = Convert.ToInt32(top);
            }
            var accuracy = this.getValue("accuracy");
            if (accuracy != null)
            {
                this.accuracy = Convert.ToInt32(accuracy);
            }
            var name = this.getValue("name");
            if (name != null)
            {
                this.name = name;
            }
            var cost = this.getValue("cost");
            if (cost != null)
            {
                this.cost = Convert.ToDouble(cost);
            }
            var isLong = this.getValue("isLong");
            if (isLong != null)
            {
                this.isLong = Convert.ToInt32(isLong);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            string url = string.Concat("http://hq.sinajs.cn/list=", name);
            string body = HttpHelper.GetHttp(url);

            try
            {
                //string[] result = body.Substring(body.IndexOf("\"")).Replace("\"", "").Split(',');

                var futuresInfo = body.Split(',');

                string name = futuresInfo[0];
                name = name.Substring(name.IndexOf("\"") + 1);
                double close = Convert.ToDouble(futuresInfo[8]);
                double priceSale = Convert.ToDouble(futuresInfo[7]);
                double volumeSale = Convert.ToDouble(futuresInfo[12]);
                double priceBuy = Convert.ToDouble(futuresInfo[6]);
                double volumeBuy = Convert.ToDouble(futuresInfo[11]);
                double yestodayClose = Convert.ToDouble(futuresInfo[5]);
                //double diff = preClose != 0 ? close - preClose : 0;
                preClose = close;
                double open = Convert.ToDouble(futuresInfo[2]);
                double increase = close - yestodayClose;
                string fm = "F" + this.accuracy;
                double profit = isLong * (close - cost);

                //if (Math.Abs(profit) > 2000)
                //{
                //    labelFundIncrease.ForeColor = Color.DarkGray;
                //    labelPrice.ForeColor = Color.DarkGray;
                //    labelTime.ForeColor = Color.DarkGray;
                //    return;
                //}
                //labelTime.ForeColor = Color.White;

                this.labelPrice.Text = string.Format("{0:" + fm + "}", close);
                //this.labelPriceIncrease.Text = string.Format("{0:" + fm + "}", increase);
                this.labelTime.Text = futuresInfo[1].Substring(0, 2) + ":" + futuresInfo[1].Substring(2, 2) + ":" + futuresInfo[1].Substring(4, 2);
                this.labelFundIncrease.Text = string.Format("{0:" + fm + "}", profit);

                if (profit > 0)
                {
                    labelFundIncrease.ForeColor = Color.Red;
                }
                else if (profit == 0)
                {
                    labelFundIncrease.ForeColor = Color.White;
                }
                else
                {
                    labelFundIncrease.ForeColor = Color.Cyan;
                }


                if (increase > 0)
                {
                    labelPrice.ForeColor = Color.Red;
                }
                else if (increase == 0)
                {
                    labelPrice.ForeColor = Color.White;
                }
                else
                {
                    labelPrice.ForeColor = Color.Cyan;
                }
                this.TopMost = true;
            }
            catch (Exception)
            {
                labelTime.Text = "-";
                labelPrice.Text = "-";
                this.labelFundIncrease.Text = "-";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer1.Stop();

            this.setValue("left", this.Left);
            this.setValue("top", this.Top);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.move)
            {
                this.Left += System.Windows.Forms.Control.MousePosition.X - this.MouseX;
                this.Top += System.Windows.Forms.Control.MousePosition.Y - this.MouseY;
            }
            this.MouseX = System.Windows.Forms.Control.MousePosition.X;
            this.MouseY = System.Windows.Forms.Control.MousePosition.Y;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.move = false;
        }

        private bool move = false;
        private int MouseX, MouseY;


        private void labelFunIncrease_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Cursor = Cursors.SizeAll;
                this.move = true;
            }
        }

        private void labelFunIncrease_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.move)
            {
                this.Left += System.Windows.Forms.Control.MousePosition.X - this.MouseX;
                this.Top += System.Windows.Forms.Control.MousePosition.Y - this.MouseY;
            }
            this.MouseX = System.Windows.Forms.Control.MousePosition.X;
            this.MouseY = System.Windows.Forms.Control.MousePosition.Y;
        }

        private void labelFunIncrease_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
            this.move = false;
        }

        private void labelFunIncrease_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.Close();
            }
        }

        private void labelTime_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void labelPrice_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Form2 form = new Form2();
            var result = form.ShowDialog();
            if(result == DialogResult.OK)
            {
                name = form.name;
                cost = form.cost;
                accuracy = form.accuracy;
                isLong = form.isLong;
            }
            timer1.Start();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Cursor = Cursors.SizeAll;
                this.move = true;
            }
        }

        private void speake(string msg)
        {
            SpeechSynthesizer speech = new SpeechSynthesizer();
            speech.Rate = 0;
            speech.Volume = 100;
            speech.SpeakAsync(msg);
        }
        private void speake(string msg, int rate)
        {
            SpeechSynthesizer speech = new SpeechSynthesizer();
            speech.Rate = rate;
            speech.Volume = 100;
            speech.SpeakAsync(msg);
        }
    }
    class Position
    {
        public string code;
        public string name;
        public double cost;
        public int amount;
        public string exchange;


        public double increase;

        public Position(string code, string name, double cost, int amount)
        {
            this.code = code;
            this.name = name;
            this.cost = cost;
            this.amount = amount;
            exchange = code.Substring(0, 1).Equals("6") ? "sh" : "sz";
        }
    }
}
