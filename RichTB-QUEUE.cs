--------------------------------------------------------------------------------------stavka.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yadatak5
{
    public class stavka
    {
        public string Prezime { get; set; }
        public string Naziv { get; set; }
        public double Broj { get; set; }
        public double Cena { get; set; }
        public double Ukupnacena { get; set; }

        public stavka(string prezime, string naziv, double broj, double cena) // osim ukupna cena
        {
            Prezime = prezime;
            Naziv = naziv;
            Broj = broj;
            Cena = cena;
            Ukupnacena = cena * broj;
        }

        public override string ToString()
        {
            return $"Prezime: {Prezime}, Cvet: {Naziv}, Broj: {Broj}, Cena: {Cena}, Ukupna Cena: {Ukupnacena}";
        }
    }
}

--------------------------------------------------------------------------------------Form1.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yadatak5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Queue<stavka> red = new Queue<stavka>();

        private void osvezi()
        {
            richTextBox1.Clear();
            foreach (stavka item in red)
            {
                richTextBox1.AppendText(item.ToString());
            }
            double suma = 0;
            foreach (stavka item in red)
            {
                suma += item.Ukupnacena;
            }
            label1.Text = suma.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (Form2 f = new Form2())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    red.Enqueue(f.novastavka);
                    osvezi();
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            red.Dequeue();
            osvezi();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            red.Clear();
            osvezi();
        }
    }
}
--------------------------------------------------------------------------------------Form2.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yadatak5
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public stavka novastavka { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string prezime = textBox1.Text;
            string naziv = textBox2.Text;

            double broj;
            double cena;

            if(prezime == "" || naziv == "" || !double.TryParse(textBox3.Text, out broj) || !double.TryParse(textBox4.Text, out cena))
            {
                MessageBox.Show("neispravan unos");
                return;
            }

            novastavka = new stavka(prezime, naziv, broj, cena);

            DialogResult = DialogResult.OK;
            Close();

        }
    }
}

