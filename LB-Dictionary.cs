---------------------------------------------------------------Artikal.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zadatak3
{
    public class Artikal
    {
        public string Naziv { get; set; }
        public double Broj { get; set; }
        public double Cena { get; set; }
        public double UkupnaCena { get { return Broj * Cena; } }

        public Artikal(string naziv, double broj, double cena)
        {
            Naziv = naziv;
            Broj = broj;
            Cena = cena;
        }
        public override string ToString()
        {
            return
            Naziv +
            " | Količina: " + Broj +
            " | Cena: " + Cena.ToString("0.00") +
            " | Ukupno: " + UkupnaCena.ToString("0.00");
        }
    }
}
---------------------------------------------------------------Form1.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zadatak3
{
    public partial class Form1 : Form
    {
        Dictionary<double, Artikal> artikli = new Dictionary<double, Artikal>();

        double sifra = 1000;
        public Form1()
        {
            InitializeComponent();
        }
        private void Osvezi()
        {
            listBox1.Items.Clear();
            foreach (var item in artikli)
            {
                listBox1.Items.Add(item.Key + " | " + item.Value);
            }

            double ukupno = artikli.Values.Sum(a => a.UkupnaCena);//ako je max onda mora if recnik.count > 0

            label2.Text = ukupno.ToString("0.00");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Form2 f = new Form2())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    Artikal novi = new Artikal
                        (
                        f.Naziv1,
                        f.Broj1,
                        f.Cena1
                        );

                    artikli.Add(sifra, novi);
                    sifra++;

                    Osvezi();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //double a = artikli.Keys.Max();
            //artikli.Remove(a);
            //Osvezi();
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Niste odabrali ni 1 stavku.");
                return;
            }
            double sifra = int.Parse(listBox1.SelectedItem.ToString().Split('|')[0].Trim());
            artikli.Remove(sifra);
            Osvezi();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            artikli.Clear();
            Osvezi();
        }
    }
}

---------------------------------------------------------------Form2.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zadatak3
{
    public partial class Form2 : Form
    {
        public string Naziv1 { get; set; }
        public double Broj1 { get; set; }
        public double Cena1 { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string naziv = textBox1.Text;
            double kolicina;
            double cena;

            if(naziv == "")
            {
                MessageBox.Show("Unesite naziv.");
                return;
            }
            if(!double.TryParse(textBox2.Text, out kolicina) || kolicina <= 0)
            {
                MessageBox.Show("Uneste ispravnu kolicinu.");
                return;
            }
            if (!double.TryParse(textBox3.Text, out cena) || cena < 0)
            {
                MessageBox.Show("Uneste ispravnu cenu.");
                return;
            }

            Naziv1 = naziv;
            Broj1 = kolicina;
            Cena1 = cena;
            
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
