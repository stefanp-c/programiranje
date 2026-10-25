--------------------------------------------------------------------------------------Kosarkas.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2zadatak
{
    public class Kosarkas
    {
        public string Prezime {  get; set; }
        public string Klub { get; set; }
        public double Visina { get; set; }
        public string Pozicija { get; set; }

        public override string ToString()
        {
            return Prezime + " | " + Klub + " | " + Visina + " cm | " + Pozicija;
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

namespace _2zadatak
{
    public partial class Form1 : Form
    {
        Queue<Kosarkas> red = new Queue<Kosarkas>();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormUnos f = new FormUnos();

            if(f.ShowDialog() == DialogResult.OK)
            {
                red.Enqueue(f.NoviKosarkas);
                PrikaziKosarkase();
            }
        }
        private void PrikaziKosarkase()
        {
            richTextBox1.Clear();
            foreach (Kosarkas k in red)
            {
                richTextBox1.AppendText(k.ToString() + Environment.NewLine);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (red.Count > 0)
            {
                red.Dequeue();
                PrikaziKosarkase();
            }
            else
            {
                MessageBox.Show("Red je prazan.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (red.Count > 0)
            {
                red.Clear();
                PrikaziKosarkase();
            }
            else
            {
                MessageBox.Show("Red je prazan.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (red.Count > 0)
            {
                double prosek = red.Average(k => k.Visina);
                label1.Text = prosek.ToString("0.00");
            }
            else
            {
                label1.Text = "------";
            }
        }
    }
}

--------------------------------------------------------------------------------------FormUnos.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2zadatak
{
    public partial class FormUnos : Form
    {
        public Kosarkas NoviKosarkas {  get; private set; }
        public FormUnos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double visina;
            if(
                textBox1.Text.Trim() == "" ||
                textBox2.Text.Trim() == "" ||
                textBox4.Text.Trim() == "" ||
                !double.TryParse(textBox3.Text, out visina) ||
                visina <= 0)
            {
                MessageBox.Show("Unesite ispravne podatke.");
                return;
            }
            NoviKosarkas = new Kosarkas()
            {
                Prezime = textBox1.Text,
                Klub = textBox2.Text,
                Visina = visina,
                Pozicija = textBox4.Text
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
