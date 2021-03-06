﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Supremes;

namespace theshow
{
    public partial class Form1 : Form
    {
        List<play> players;
        List<play2> watchlist;
        List<play2> searchlist;
        List<play3> sellorderlist;
        List<play3> buyorderlist;
        public Form1()
        {
            InitializeComponent();
        }

        struct play
        {
            private String name;
            private String id;
            public String N
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public String I
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
        }

        struct play2
        {
            private String name;
            private String id;
            private String buynow;
            private String sellnow;
            private double profit;
            public String Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public String ID
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
            public String BuyNow
            {
                get
                {
                    return buynow;
                }
                set
                {
                    buynow = value;
                }
            }
            public String SellNow
            {
                get
                {
                    return sellnow;
                }
                set
                {
                    sellnow = value;
                }
            }
            public double Profit
            {
                get
                {
                    return profit;
                }
                set
                {
                    profit = value;
                }
            }
            public void toString()
            {
                Console.WriteLine("Name: " + name + "\nID: " + id + "\nBuyNow: " + buynow + "\nSellNow: " + sellnow + "\nProfit: " + profit);
            }
        }

        struct play3
        {
            private String name;
            private String id;
            private String buynow;
            private String sellnow;
            private String myprice;
            public String Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public String ID
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
            public String BuyNow
            {
                get
                {
                    return buynow;
                }
                set
                {
                    buynow = value;
                }
            }
            public String MyPrice
            {
                get
                {
                    return myprice;
                }
                set
                {
                    myprice = value;
                }
            }
            public String SellNow
            {
                get
                {
                    return sellnow;
                }
                set
                {
                    sellnow = value;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://theshownation.com/marketplace/orders");
            webBrowser3.Navigate("http://theshownation.com/marketplace/completed_orders");
            comboBox1.SelectedIndex = -1;
            players = new List<play>();
            watchlist = new List<play2>();
            searchlist = new List<play2>();
            sellorderlist = new List<play3>();
            buyorderlist = new List<play3>();
            //getPlayers();
        }

        async private void button1_Click(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedIndex >= 0)
            {
                textBox5.Text = "";
                label19.Text = "";
                label21.Text = "";
                String url = "http://theshownation.com/marketplace/completed_orders";
                webBrowser3.Navigate(url);
                await Task.Delay(TimeSpan.FromSeconds(3));
                String myprice = sellorderlist.ElementAt(comboBox1.SelectedIndex).MyPrice;
                textBox4.Text = myprice;
                label10.Text = "";
                label10.ForeColor = Color.Black;
            }
            else
            {
                label10.Text = "Select a player from your list first";
                label10.ForeColor = Color.Red;
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            label2.Text = "";
            label4.Text = "";
            label6.Text = "";
            //label10.Text = "";
            if (webBrowser1.Url.ToString().Contains("http://theshownation.com/marketplace/listing?item_ref_id="))
            {
                int count = 1;
                double buynow = 0;
                double sellnow = 0;
                double profit = 0;
                HtmlElementCollection inputElements = webBrowser1.Document.GetElementsByTagName("INPUT");
                foreach (HtmlElement ie in inputElements)
                {
                    if (ie.OuterHtml.Contains("price") && ie.OuterHtml.Contains("hidden"))
                    {
                        String price = ie.OuterHtml.Substring((ie.OuterHtml.IndexOf("ue=\"") + 4), (ie.OuterHtml.IndexOf("\">") - (ie.OuterHtml.IndexOf("ue=\"") + 4)));
                        if (count == 1)
                        {
                            buynow = double.Parse(price);
                            count++;
                        }
                        else if (count == 2)
                        {
                            sellnow = double.Parse(price);
                            count = 1;
                        }
                    }
                }
                profit = buynow - sellnow - (buynow * .1);
                profit = Math.Round(profit, 0);
                label2.Text = buynow.ToString();
                label4.Text = sellnow.ToString();
                label6.Text = profit.ToString();
                if (profit > 0)
                {
                    label6.ForeColor = Color.Green;
                }
                else
                {
                    label6.ForeColor = Color.Red;
                }
            }
        }

        private void getPlayers()
        {
            players.Clear();
            using (FileStream r = File.Open("players.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (r.Length > 0)
                {
                    using (StreamReader s = new StreamReader(r, true))
                    {
                        String line;
                        while ((line = s.ReadLine()) != null)
                        {
                            String name = line.Substring(0, line.IndexOf(","));
                            String id = line.Substring(name.Length + 2, 5);
                            play temp = new play();
                            temp.N = name;
                            temp.I = id;
                            players.Add(temp);
                        }
                    }
                }
            }
            foreach(play player in players)
            {
                comboBox1.Items.Add(player.N);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0)
            {
                String url = "http://theshownation.com/marketplace/search?utf8=✓&main_filter=MLB+Cards&display_name=" + textBox1.Text;
                webBrowser1.Navigate(url);
                label10.Text = "";
                label10.ForeColor = Color.Black;
            }
            else
            {
                label10.Text = "Enter a name to search";
                label10.ForeColor = Color.Red;
            }
        }

        async private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            watchlist.Clear();            
            webBrowser2.Navigate("http://theshownation.com/marketplace/watchlist");
            await Task.Delay(TimeSpan.FromSeconds(3));

            //new test
            var doc = Dcsoup.Parse(webBrowser2.DocumentText);
            var pages = doc.Select("a[href*=page]");
            int counter = 0;
            if (pages.Count() > 1)
            {
                var page = pages.ElementAt(pages.Count() - 2);
                counter = Int32.Parse(page.Text);
            }
            else
            {
                counter = 1;
            }

            updatewatchlist(counter);
            //updatewatchlist(webBrowser2.DocumentText);
        }

        //previous it had String X, switching to int for now
        async private void updatewatchlist(int X)
        {
            
            for(int i=1;i<=X;i++)
            {
                webBrowser2.Navigate("http://theshownation.com/marketplace/watchlist?page="+i);
                await Task.Delay(3000);
                String site = webBrowser2.DocumentText;
                String temp = "<H2>";
                String temp1 = "listing?item_ref_id=";
                String temp2 = "<INPUT name=\"price\" type=\"hidden\" value=";
                StringReader read = new StringReader(site);
                StringBuilder what = new StringBuilder();
                String line;
                while ((line = read.ReadLine()) != null)
                {
                    if (line.Contains(temp) || line.Contains(temp1) || line.Contains(temp2))
                    {
                        what.AppendLine(line);
                    }
                }
                StringReader read2 = new StringReader(what.ToString());
                String name = "";
                String id = "";
                String buynow = "";
                String sellnow = "";
                int count = 1;
                while ((line = read2.ReadLine()) != null)
                {
                    if (count == 1)
                    {
                        name = line.Substring(4, (line.IndexOf(" <") - 4));
                        count++;
                    }
                    else if (count == 2)
                    {
                        id = line.Substring((line.Length - 12), (line.IndexOf("\">") - (line.Length - 12)));
                        count++;
                    }
                    else if (count == 3)
                    {
                        buynow = line.Substring(41, ((line.IndexOf(">") - 1) - 41));
                        count++;
                    }
                    else if (count == 4)
                    {
                        sellnow = line.Substring(41, ((line.IndexOf(">") - 1) - 41));
                        play2 play = new play2();
                        play.Name = name;
                        play.ID = id;
                        play.BuyNow = buynow;
                        play.SellNow = sellnow;
                        double profit = double.Parse(buynow) - double.Parse(sellnow) - (double.Parse(buynow) * .1);
                        profit = Math.Round(profit, 0);
                        play.Profit = profit;
                        watchlist.Add(play);
                        count = 1;
                    }
                }
            }
            List<play2> templist = watchlist.OrderByDescending(o => o.Profit).ToList();
            dataGridView1.DataSource = templist;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns.Remove("ID");
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRows();
        }

        async private void button6_Click(object sender, EventArgs e)
        {
            label10.Text = "";
            label10.ForeColor = Color.Black;
            String min = textBox2.Text;
            String max = textBox3.Text;
            if (min.Length > 0 || max.Length > 0)
            {
                label10.Text = "Search can take up to 1 minute";
                label10.ForeColor = Color.Red;
                searchlist.Clear();
                dataGridView2.DataSource = null;
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                String serieschoice = comboBox2.Text;
                String series = "";
                String url = "";
                if (serieschoice.Equals("Live"))
                {
                    series = "&series_id=1337";
                }

                url = "http://theshownation.com/marketplace/search?utf8=✓&main_filter=MLB+Cards&display_name=&min_price=" + min + "&max_price=" + max + series;

                webBrowser2.Navigate(url);
                await Task.Delay(TimeSpan.FromSeconds(3));
                var doc = Dcsoup.Parse(webBrowser2.DocumentText);
                var pages = doc.Select("a[href*=page]");
                int counter = 0;
                if (pages.Count() > 1)
                {
                    var page = pages.ElementAt(pages.Count() - 2);
                    counter = Int32.Parse(page.Text);
                    if(counter>20)
                    {
                        counter = 20;
                    }
                }
                else
                {
                    counter = 1;
                }
                updatesearchlist(counter,min,max,series);
            }
        }

         async private void updatesearchlist(int X,String min, String max, String series)
        {
            for (int i = 1; i <= X; i++)
            {
                String urler = "http://theshownation.com/marketplace/search?page=" + i + "&main_filter=MLB Cards&min_price=" + min + "&max_price=" + max + series;
                webBrowser2.Navigate(urler);
                await Task.Delay(3000);
                var pager = Dcsoup.Parse(webBrowser2.DocumentText);
                var tds = pager.Select("td");
                int count = 0;
                foreach (var td in tds)
                {
                    if (count == 3)
                    {
                        var what = td.Select("a[href*=listing]");
                        String whatone = what.ElementAt(0).OuterHtml;
                        int beg = whatone.IndexOf("id=") + 3;
                        String id = whatone.Substring(beg, 5);
                        var yes = td.Select("input[name*=price]");
                        int start = yes.ElementAt(0).OuterHtml.IndexOf("ue=\"") + 4;
                        int end = yes.ElementAt(0).OuterHtml.IndexOf("\">");
                        String buystring = yes.ElementAt(0).OuterHtml.Substring(start, (end - start));
                        int end1 = yes.ElementAt(1).OuterHtml.IndexOf("\">");
                        String sellstring = yes.ElementAt(1).OuterHtml.Substring(start, (end1 - start));
                        String name = what.ElementAt(0).Text;
                        double profit = double.Parse(buystring) - double.Parse(sellstring) - (double.Parse(buystring) * .1);
                        profit = Math.Round(profit, 0);
                        play2 play = new play2();
                        play.Name = name;
                        play.ID = id;
                        play.BuyNow = buystring;
                        play.SellNow = sellstring;
                        play.Profit = profit;                        
                        searchlist.Add(play);
                    }
                    count++;
                    if (count == 4)
                    {
                        count = 0;
                    }
                }
            }
             List<play2> templist = searchlist.OrderByDescending(o => o.Profit).ToList();
             dataGridView2.DataSource = templist;
             dataGridView2.RowHeadersVisible = false;
             dataGridView2.Columns.Remove("ID");
             dataGridView2.AutoResizeColumns();
             dataGridView2.AutoResizeRows();        
            }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                String player = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int index = searchlist.FindIndex(f => f.Name == player);
                string id = searchlist.ElementAt(index).ID;
                string url = "http://theshownation.com/marketplace/listing?item_ref_id=" + id;
                webBrowser1.Navigate(url);
            }
        }

        async private void button7_Click(object sender, EventArgs e)
        {
            dataGridView3.DataSource = null;
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            sellorderlist.Clear();
            dataGridView4.DataSource = null;
            dataGridView4.Rows.Clear();
            dataGridView4.Columns.Clear();
            buyorderlist.Clear();
            String url = "http://theshownation.com/marketplace/orders";
            webBrowser3.Navigate(url);
            await Task.Delay(TimeSpan.FromSeconds(3));
            HtmlElementCollection tables = webBrowser3.Document.GetElementsByTagName("TABLE");
            int count = 1;
            String fa="";
            int index = 1;
            String id = ""; 
            String name="";
            foreach(HtmlElement tbl in tables)
            {
                String temp = tbl.InnerText;              
                HtmlElementCollection ff = tbl.GetElementsByTagName("td");
                foreach(HtmlElement st in ff)
                {
                    fa = st.InnerText;
                    String fsd = st.InnerHtml;
                    if(fa!=null)
                    {
                        if(!fa.ElementAt(1).Equals(' '))
                        {                          
                            fa = fa.Trim();
                            if(index == 1)
                            {                            
                               String tempo = fsd.Substring((fsd.IndexOf("d=") + 2), (fsd.IndexOf("\">") - (fsd.IndexOf("d=") + 2)));
                               name = fa;
                               id = tempo;
                                index++;
                            }
                            else if(index == 2)
                            {
                                if(count==1)
                                {
                                    play3 player = new play3();
                                    player.Name = name;
                                    player.ID = id;
                                    player.MyPrice = fa;
                                    buyorderlist.Add(player); 
                                }
                                if(count==2)
                                {
                                    play3 player = new play3();
                                    player.Name = name;
                                    player.ID = id;
                                    player.MyPrice = fa;
                                    sellorderlist.Add(player);                                 
                                }
                                index++;
                            }
                            else if(index==3)
                            {
                                index = 1;
                            }
                        }
                    }
                }
                count++;
            }

            updatesellorders();          
        }

        async private void updatesellorders()
        {
            int count3 = 1;
            for (int i = 0; i < sellorderlist.Count; i++)
            {
                play3 temp = sellorderlist.ElementAt(i);               
                 webBrowser2.Navigate("http://theshownation.com/marketplace/listing?item_ref_id="+temp.ID);
                                    await Task.Delay(TimeSpan.FromSeconds(2));
                                    HtmlElementCollection temas = webBrowser2.Document.GetElementsByTagName("INPUT");
                                    foreach(HtmlElement fff in temas)
                                    {                                        
                                        if (fff.OuterHtml.Contains("price") && fff.OuterHtml.Contains("hidden"))
                                        {
                                            String price = fff.OuterHtml.Substring((fff.OuterHtml.IndexOf("ue=\"") + 4), (fff.OuterHtml.IndexOf("\">") - (fff.OuterHtml.IndexOf("ue=\"") + 4)));
                                            if(count3==1)
                                            {            
                                                temp.BuyNow = price;
                                                count3++;
                                            }
                                            else if(count3 == 2)
                                            {                                              
                                                 temp.SellNow = price;
                                                 count3 = 1;
                                            }
                                        }
                                    }
                                    sellorderlist[i] = temp;
            }
            updatesellgrid();
        }

        private void updatesellgrid()
        {           
            dataGridView3.DataSource = sellorderlist;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.Columns.Remove("ID");
            dataGridView3.AutoResizeColumns();
            dataGridView3.AutoResizeRows();
            updatebuyorders();
        }

        async private void updatebuyorders()
        {
            int count3 = 1;
            for (int i = 0; i < buyorderlist.Count; i++)
            {
                play3 temp = buyorderlist.ElementAt(i);
                webBrowser2.Navigate("http://theshownation.com/marketplace/listing?item_ref_id=" + temp.ID);
                await Task.Delay(TimeSpan.FromSeconds(2));
                HtmlElementCollection temas = webBrowser2.Document.GetElementsByTagName("INPUT");
                foreach (HtmlElement fff in temas)
                {
                    if (fff.OuterHtml.Contains("price") && fff.OuterHtml.Contains("hidden"))
                    {
                        String price = fff.OuterHtml.Substring((fff.OuterHtml.IndexOf("ue=\"") + 4), (fff.OuterHtml.IndexOf("\">") - (fff.OuterHtml.IndexOf("ue=\"") + 4)));
                        if (count3 == 1)
                        {
                            temp.BuyNow = price;
                            count3++;
                        }
                        else if (count3 == 2)
                        {
                            temp.SellNow = price;
                            count3 = 1;
                        }
                    }
                }
                buyorderlist[i] = temp;
            }
            updatebuygrid();
        }

        private void updatebuygrid()
        {
            dataGridView4.DataSource = buyorderlist;
            dataGridView4.RowHeadersVisible = false;
            dataGridView4.Columns.Remove("ID");
            dataGridView4.AutoResizeColumns();
            dataGridView4.AutoResizeRows();
            comboBox1.Items.Clear();
            foreach(play3 play in sellorderlist)
            {
                comboBox1.Items.Add(play.Name);
            }
        }

        private void dataGridView3_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                String player = dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int index = sellorderlist.FindIndex(f => f.Name == player);
                string id = sellorderlist.ElementAt(index).ID;
                string url = "http://theshownation.com/marketplace/listing?item_ref_id=" + id;
                webBrowser1.Navigate(url);
            }
        }

        private void dataGridView4_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                String player = dataGridView4.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                try
                {
                    int index = buyorderlist.FindIndex(f => f.Name == player);
                    
                    string id = buyorderlist.ElementAt(index).ID;
                    string url = "http://theshownation.com/marketplace/listing?item_ref_id=" + id;
                    webBrowser1.Navigate(url);
                }
                catch(Exception eff){
                    MessageBox.Show("Issue with Buy List: \nAmount of players in list: "+buyorderlist.Count());
                }
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                String player = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                String buynow = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                int index = watchlist.FindIndex(f => f.Name == player && f.BuyNow==buynow);
                string id = watchlist.ElementAt(index).ID;
                string url = "http://theshownation.com/marketplace/listing?item_ref_id=" + id;
                webBrowser1.Navigate(url);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                double sell = double.Parse(textBox4.Text);
                double buy = double.Parse(textBox5.Text);
                double profit = sell - buy - (sell * .1);
                double min = ((buy+100) / .9);
                min = Math.Ceiling(min);
                profit = Math.Round(profit, 0);
                label19.Text = profit.ToString();
                label21.Text = min.ToString();
                label10.Text = "";
                label10.ForeColor = Color.Black;
            }catch(Exception eff)
            {
                label10.Text = "values messed up";
                label10.ForeColor = Color.Red;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo look = new ProcessStartInfo("https://www.paypal.me/payjesse");
            Process.Start(look);
        }

        private void dataGridView2_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            label10.Text = "Search Completed";
            label10.ForeColor = Color.Black;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            webBrowser3.Navigate("http://theshownation.com/marketplace/completed_orders");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser3.Navigate("http://theshownation.com/marketplace/orders");
        }
    }
}
