using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace theshow
{
    public partial class Form1 : Form
    {
        List<play> players;
        List<play2> watchlist;
        List<play2> searchlist;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://theshownation.com/marketplace/orders");
            comboBox1.SelectedIndex = -1;
            players = new List<play>();
            watchlist = new List<play2>();
            searchlist = new List<play2>();
            getPlayers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String ur = "http://theshownation.com/marketplace/listing?item_ref_id=";
            if (comboBox1.SelectedIndex >= 0)
            {
                String url = ur + players.ElementAt(comboBox1.SelectedIndex).I;
                webBrowser1.Navigate(url);
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
            label10.Text = "";
            if (webBrowser1.Url.ToString().Contains("http://theshownation.com/marketplace/listing?item_ref_id="))
            {
                String site = webBrowser1.DocumentText;
                int index = 0;
                int index2 = 0;
                String temp="<INPUT name=\"price\" type=\"hidden\" value=\"";
                if(site.Contains(temp))
                {
                    index = site.IndexOf(temp);
                }
                String tempstring = site.Substring(index, 50);
                String buynow = tempstring.Substring(41,((tempstring.IndexOf(">")-1)-41));
                label2.Text = buynow;
                String newstring = site.Substring((index + temp.Length+buynow.Length+620), 150);
                if (newstring.Contains(temp))
                {
                    index2 = newstring.IndexOf(temp);
                }
                String tempstring2 = newstring.Substring(index2, 50);
                String sellprice = tempstring2.Substring(41, ((tempstring2.IndexOf(">") - 1) - 41));
                label4.Text = sellprice;
                double profit = double.Parse(buynow) - double.Parse(sellprice) - (double.Parse(buynow) * .1);
                profit = Math.Round(profit, 0);
                label6.Text = profit.ToString();
                if(profit>0)
                {
                    label6.ForeColor = Color.Green;
                }
                else
                {
                    label6.ForeColor = Color.Red;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (webBrowser1.Url.ToString().Contains("http://theshownation.com/marketplace/listing?item_ref_id="))
            {                              
                String id = webBrowser1.Url.ToString().Substring((webBrowser1.Url.ToString().Length - 5), 5);
                id = id.Replace(" ", String.Empty);             
                String site = webBrowser1.DocumentText;
                StringReader read = new StringReader(site);
                String line;
                while (true)
                {
                    line = read.ReadLine();
                    if(line.Contains("<H2>#"))
                    {
                        break;
                    }
                }
                String name=line.Substring(8, (line.IndexOf(" </H") - 8));
                if (name.ElementAt(0).Equals(' '))
                {
                    name = name.Substring(1, name.Length - 1);
                }
                int index = players.FindIndex(f => f.I == id);
                if (index < 0)
                {
                    using (StreamWriter w = File.AppendText("players.txt"))
                    {
                        w.WriteLine(name + ", " + id);
                        label10.Text = name + " has been added";
                        label10.ForeColor = Color.Black;
                    }
                    comboBox1.Items.Clear();
                    getPlayers();
                }
                else
                {
                    label10.Text = name + " already in list";
                    label10.ForeColor = Color.Red;
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
            String url = "http://theshownation.com/marketplace/search?utf8=✓&main_filter=MLB+Cards&display_name=" + textBox1.Text;
            webBrowser1.Navigate(url);
        }

        async private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://theshownation.com/marketplace/watchlist");
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (webBrowser1.Url.ToString().Contains("http://theshownation.com/marketplace/watchlist"))
            {
                String site = webBrowser1.DocumentText;
                /*using (StreamWriter f = File.AppendText("code.txt"))
                {
                    f.WriteLine(site);                
                }*/
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
                        //Console.WriteLine(line);
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
                        //Console.WriteLine(name);
                        count++;
                    }
                    else if (count == 2)
                    {
                        id = line.Substring((line.Length - 12), (line.IndexOf("\">") - (line.Length - 12)));
                        //Console.WriteLine(id);
                        count++;
                    }
                    else if (count == 3)
                    {
                        buynow = line.Substring(41, ((line.IndexOf(">") - 1) - 41));
                        //Console.WriteLine(buynow);
                        count++;
                    }
                    else if (count == 4)
                    {
                        sellnow = line.Substring(41, ((line.IndexOf(">") - 1) - 41));
                        //Console.WriteLine(sellnow);
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
                List<play2> templist = watchlist.OrderByDescending(o => o.Profit).ToList();
                dataGridView1.DataSource = templist;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns.Remove("ID");
                dataGridView1.AutoResizeColumns();
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
            if (webBrowser2.Url.ToString().Contains("http://theshownation.com/marketplace/watchlist"))
            {
                String site = webBrowser2.DocumentText;
                /*using (StreamWriter f = File.AppendText("code.txt"))
                {
                    f.WriteLine(site);                
                }*/
                String temp = "<H2>";
                String temp1 = "listing?item_ref_id=";
                String temp2 = "<INPUT name=\"price\" type=\"hidden\" value=";
                StringReader read = new StringReader(site);
                StringBuilder what = new StringBuilder();
                String line;
                while((line=read.ReadLine())!=null)
                {
                    if(line.Contains(temp)|| line.Contains(temp1)||line.Contains(temp2))
                    {
                        what.AppendLine(line);
                        //Console.WriteLine(line);
                    }
                }
                StringReader read2 = new StringReader(what.ToString());
                String name="";
                String id = "";
                String buynow="";
                String sellnow="";
                int count = 1;
                while((line=read2.ReadLine())!=null)
                {
                    if(count==1)
                    {
                        name = line.Substring(4, (line.IndexOf(" <") - 4));
                        //Console.WriteLine(name);
                        count++;
                    }
                    else if (count == 2)
                    {
                        id = line.Substring((line.Length-12), (line.IndexOf("\">") - (line.Length-12)));
                        //Console.WriteLine(id);
                        count++;
                    }
                    else if(count == 3)
                    {
                        buynow=line.Substring(41, ((line.IndexOf(">") - 1) - 41));
                        //Console.WriteLine(buynow);
                        count++;
                    }
                    else if(count == 4)
                    {
                        sellnow = line.Substring(41, ((line.IndexOf(">") - 1) - 41));
                        //Console.WriteLine(sellnow);
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
                List<play2> templist = watchlist.OrderByDescending(o=>o.Profit).ToList();
                dataGridView1.DataSource = templist;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns.Remove("ID");
                dataGridView1.AutoResizeColumns();
            }
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
                String url = "http://theshownation.com/marketplace/search?utf8=✓&main_filter=MLB+Cards&display_name=&min_price=" + min + "&max_price=" + max;
                webBrowser1.Navigate(url);
                await Task.Delay(TimeSpan.FromSeconds(3));
                String site = webBrowser1.DocumentText;
                using (StreamWriter f = File.AppendText("code.txt"))
                {
                    f.WriteLine(site);
                }
                String pages = "?page=";
                String line;
                StringReader read = new StringReader(site);
                StringBuilder what = new StringBuilder();
                int numberofpages = 0;
                while ((line = read.ReadLine()) != null)
                {
                    if (line.Contains(pages) && !line.Contains("current"))
                    {
                        what.AppendLine(line);
                        //Console.WriteLine(line);
                    }
                }
                if (what.Length > 0)
                {
                    StringReader read2 = new StringReader(what.ToString());
                    String liner;
                    while ((liner = read2.ReadLine()) != null)
                    {
                        String num = liner.Substring(12, (liner.IndexOf("&") - 12));
                        //Console.WriteLine("Number of LInes: "+num);
                        int temp = 0;
                        temp = Int32.Parse(num);
                        if (temp > numberofpages)
                        {
                            numberofpages = temp;
                        }
                    }
                }
                //Console.WriteLine("Number of pages" + numberofpages);
                if (numberofpages > 0)
                {
                    if (numberofpages > 21)
                    {
                        numberofpages = 21;
                    }

                    for (int i = 1; i <= numberofpages; i++)
                    {
                        String urler = "http://theshownation.com/marketplace/search?page=" + i + "&main_filter=MLB Cards&min_price=" + min + "&max_price=" + max;
                        webBrowser2.Navigate(urler);
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        updatesearchlist(webBrowser2.DocumentText);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                else
                {
                    String urler = "http://theshownation.com/marketplace/search?page=1" + "&main_filter=MLB Cards&min_price=" + min + "&max_price=" + max;
                    webBrowser2.Navigate(urler);
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    updatesearchlist(webBrowser2.DocumentText);
                }              
            }
            else
            {
                label10.Text = "Enter Min or Max Price";
                label10.ForeColor = Color.Red;
            }
        }

        async private void updatesearchlist(String X)
        {
             String temp = "Click Buy Now";
                String temp1 = "listing?item_ref_id=";
                String temp2 = "<INPUT name=\"price\" type=\"hidden\" value=";
                StringReader read = new StringReader(X);
                StringBuilder what = new StringBuilder();
                String line;
                while((line=read.ReadLine())!=null)
                {
                    if(line.Contains(temp)|| (line.Contains(temp1)&& !line.Contains("<B>"))||line.Contains(temp2))
                    {
                        what.AppendLine(line);
                    }
                }
                StringReader read2 = new StringReader(what.ToString());
                String name="";
                String id = "";
                String buynow="";
                String sellnow="";
                int count = 1;             
                while((line=read2.ReadLine())!=null)
                {
                    if(count==3)
                    {
                        name = line.Substring(line.IndexOf("w', '")+5, (line.IndexOf("');") - (line.IndexOf("w', '")+5)));
                        count++;
                    }
                    else if (count == 1)
                    {
                        id = line.Substring((line.IndexOf("d=")+2), (line.IndexOf("\">") - (line.IndexOf("d=")+2)));
                        count++;
                    }
                    else if(count == 2)
                    {
                        buynow=line.Substring(41, (line.IndexOf("\">") - 41));
                        count++;
                    }
                    else if(count == 4)
                    {
                        sellnow = line.Substring(41, (line.IndexOf("\">") - 41));
                        play2 play = new play2();
                        play.Name = name;
                        play.ID = id;
                        play.BuyNow = buynow;
                        play.SellNow = sellnow;
                        double profit = double.Parse(buynow) - double.Parse(sellnow) - (double.Parse(buynow) * .1);
                        profit = Math.Round(profit, 0);
                        play.Profit = profit;
                        int index = searchlist.FindIndex(f => f.ID == id);
                        if (index < 0)
                        {
                            searchlist.Add(play);
                        }
                        count = 1;
                    }
                }
                List<play2> templist = searchlist.OrderByDescending(o=>o.Profit).ToList();
                dataGridView2.DataSource = templist;
                dataGridView2.RowHeadersVisible = false;
                dataGridView2.Columns.Remove("ID");
                dataGridView2.AutoResizeColumns();
                await Task.Delay(TimeSpan.FromSeconds(1));
                label10.Text = "Search Completed";
                label10.ForeColor = Color.Black;
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
    }
}
