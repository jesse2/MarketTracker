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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://theshownation.com/marketplace/orders");
            comboBox1.SelectedIndex = -1;
            players = new List<play>();
            watchlist = new List<play2>();
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

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("http://theshownation.com/marketplace/watchlist");            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            watchlist.Clear();
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
                dataGridView1.DataSource = watchlist;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.Columns.Remove("ID");
                dataGridView1.AutoResizeColumns();
            }
        }
    }
}
