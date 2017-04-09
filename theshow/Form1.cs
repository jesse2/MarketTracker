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

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigate("theshownation.com/sessions/login");
            comboBox1.SelectedIndex = -1;
            players = new List<play>();
            getPlayers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String ur = "http://theshownation.com/marketplace/listing?item_ref_id=";

            String url = ur+players.ElementAt(comboBox1.SelectedIndex).I;
            webBrowser1.Navigate(url);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.Url.ToString().Contains("http://theshownation.com/marketplace/listing?item_ref_id="))
            {
                String site = webBrowser1.DocumentText;
                using (StreamWriter f = File.AppendText("code.txt"))
                {
                    f.WriteLine(site);
                }
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
                label6.Text = profit.ToString();
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
                String name=line.Substring(9, (line.IndexOf(" </H") - 9));
                int index = players.FindIndex(f => f.I == id);
                if (index < 0)
                {
                    using (StreamWriter w = File.AppendText("players.txt"))
                    {
                        w.WriteLine(name + ", " + id);
                    }
                    comboBox1.Items.Clear();
                    getPlayers();
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
    }
}
