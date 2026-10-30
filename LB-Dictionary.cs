using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zadatak9
{
    public class Rezervacija
    {
        public string Prezime {  get; set; }
        public string Naziv {  get; set; }
        public double Cena1 { get; set; }
        public int Brkarata { get; set; }
        public double Ukupnacena { get; set; }

        public Rezervacija(string prezime, string naziv, double cena1, int brkarata)
        {
            Prezime = prezime;
            Naziv = naziv;
            Cena1 = cena1;
            Brkarata = brkarata;
            Ukupnacena = brkarata * cena1;
        }

        public override string ToString()
        {
            return $"Prezime: {Prezime} Naziv: {Naziv} Cena1: {Cena1} Brkarata: {Brkarata} Ukupnacena: {Ukupnacena}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zadatak9
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public Rezervacija Nova { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string prezime = textBox1.Text;
            string naziv = textBox2.Text;
            double cena1;
            int brkarata;

            if(prezime == "" || naziv == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("popunite sve");
                return;
            }
            if(!double.TryParse(textBox3.Text, out cena1) || cena1 < 0)
            {
                MessageBox.Show("procerite cenu");
                return;
            }
            if (!int.TryParse(textBox4.Text, out brkarata) || brkarata < 1)
            {
                MessageBox.Show("proverite broj karata");
                return;
            }

            Nova = new Rezervacija(prezime, naziv, cena1, brkarata);

            DialogResult = DialogResult.OK;
            Close();

        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zadatak9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Dictionary<int, Rezervacija> recnik = new Dictionary<int, Rezervacija>();

        int sifra = 1000;
        private void button1_Click(object sender, EventArgs e)
        {
            using (Form2 f2 = new Form2())
            {
                if (f2.ShowDialog() == DialogResult.OK)
                {
                    recnik.Add(sifra, f2.Nova);
                    sifra++;
                    osvezi();
                    najsk();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndices.Count < 1)
            {
                MessageBox.Show("odaberite nesto");
                return;
            }
            int s = int.Parse(listBox1.SelectedItem.ToString().Substring(0,4));
            //int s = int.Parse(listBox1.SelectedItem.ToString().Split(' ')[0]);
            recnik.Remove(s);
            osvezi();
            najsk();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(recnik.Count == 0)
            {
                MessageBox.Show("nema stavki");
                return;
            }
            recnik.Clear();
            osvezi();
            najsk();
        }
        private void osvezi()
        {
            listBox1.Items.Clear();
            foreach (var item in recnik)
            {
                listBox1.Items.Add(item.Key + " " + item.Value);
            }

        }
        private void najsk()
        {
            if (recnik.Count > 0)
            {
                double a = 0;
                foreach (var item in recnik)
                {
                    if (item.Value.Ukupnacena > a)
                        a = item.Value.Ukupnacena;
                }
                label1.Text = a.ToString("0.00");
            }
            else
                label1.Text = "0.00";
            
        }
    }
}
