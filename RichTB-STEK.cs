----------------------------------------------------------------------------Knjiga.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1zadatak
{
    public class Knjiga
    {
        public string Naziv { get; set; }
        public string Izdavac { get; set; }
        public string Autor { get; set; }
        public double Cena { get; set; }

        public Knjiga(string naziv, string izdavac, string autor, double cena)
        {
            Naziv = naziv;
            Izdavac = izdavac;
            Autor = autor;
            Cena = cena;
        }

        public override string ToString()
        {
            return
                "Naziv: " + Naziv +
                "\nIzdavac: " + Izdavac +
                "\nAutor: " + Autor +
                "\nCena: " + Cena.ToString("0.00") + " din" +
                "\n--------------------------------------\n";

        }
    }
}
----------------------------------------------------------------------------FORM1
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1zadatak
{
    public partial class Form1 : Form
    {
        Stack<Knjiga> stek = new Stack<Knjiga>();
        public Form1()
        {
            InitializeComponent();
        }
        private void PrikaziKnjige()
        {
            richTextBox1.Clear();
            foreach (Knjiga k in stek)
            {
                richTextBox1.AppendText(k.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            if (f.ShowDialog() == DialogResult.OK)
            {
                stek.Push(f.NovaKnjiga);
                PrikaziKnjige();
                UkupnaCena();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(stek.Count > 0)
            {
                stek.Pop();
                PrikaziKnjige();
            }
            else
            {
                MessageBox.Show("Stek je prazan");
            }
            UkupnaCena();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            stek.Clear();
            PrikaziKnjige();
            UkupnaCena();
        }

        private void UkupnaCena()
        {
            double ukupno = 0;

            foreach (Knjiga k in stek)
            {
                ukupno += k.Cena;
            }

            label1.Text = ukupno.ToString();
        }
    }
}
----------------------------------------------------------------------------FORM2
using _1zadatak;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1zadatak
{
    public partial class Form2 : Form
    {
        public Knjiga NovaKnjiga { get; private set; }
        public Form2()
        {
            InitializeComponent();
        }  
        private void button1_Click(object sender, EventArgs e)
        {
            string naziv = textBox1.Text;
            string izdavac = textBox2.Text;
            string autor = textBox3.Text;

            double cena;

            if(naziv == "" || izdavac == "" || autor == "")
            {
                MessageBox.Show("Popuni sva polja!");
                return;
            }
            if(!double.TryParse(textBox4.Text, out cena) || cena < 0)
            {
                MessageBox.Show("Unesi ispravnu cenu!");
                return;
            }

            NovaKnjiga = new Knjiga(naziv, izdavac, autor, cena);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}


