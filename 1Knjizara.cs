====================================================================
 1. KLASA: Knjiga.cs
====================================================================

using System;
using System.Collections.Generic;

namespace Knjizara
{
    public class Knjiga
    {
        private string nazivKnjige;
        private string izdavackaKuca;
        private int brojStranica;
        private double ukupnaCena;

        public static Dictionary<string, double> CenaStranice =
            new Dictionary<string, double>()
            {
                { "Nolit", 2.45 },
                { "Prometej", 1.47 },
                { "Laguna", 2.08 },
                { "Danilo Kis", 1.88 }
            };

        public static double UkupnaCenaKnjizare = 0;

        public string NazivKnjige
        {
            get { return nazivKnjige; }
            set { nazivKnjige = value; }
        }

        public string IzdavackaKuca
        {
            get { return izdavackaKuca; }
            set { izdavackaKuca = value; }
        }

        public int BrojStranica
        {
            get { return brojStranica; }
            set { brojStranica = value; }
        }

        public double UkupnaCena
        {
            get { return ukupnaCena; }
        }

        public Knjiga(string naziv, string izdavac, int broj)
        {
            nazivKnjige = naziv;
            izdavackaKuca = izdavac;
            brojStranica = broj;

            ukupnaCena = brojStranica * CenaStranice[izdavackaKuca];

            UkupnaCenaKnjizare += ukupnaCena;
        }

        public override string ToString()
        {
            return "Naziv knjige: " + NazivKnjige +
                   ", izdavačka kuća: " + IzdavackaKuca +
                   ", broj stranica: " + BrojStranica +
                   ", ukupna cena: " + UkupnaCena;
        }
    }
}


====================================================================
 2. GLAVNI PROZOR: Form1.cs
====================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Knjizara
{
    public partial class Form1 : Form
    {
        public static Queue<Knjiga> red = new Queue<Knjiga>();

        public Form1()
        {
            InitializeComponent();
            UcitajIzFajla();
        }

        private void UcitajIzFajla()
        {
            if (!File.Exists("Knjiga.txt"))
            {
                richTextBox1.Text = "Knjižara je prazna.";
                return;
            }

            foreach (string linija in File.ReadAllLines("Knjiga.txt"))
            {
                string[] podaci = linija.Split('|');

                if (podaci.Length == 3)
                {
                    Knjiga k = new Knjiga(
                        podaci[0],
                        podaci[1],
                        int.Parse(podaci[2]));

                    red.Enqueue(k);
                }
            }

            PrikaziKnjige();
        }

        public void PrikaziKnjige()
        {
            richTextBox1.Clear();

            if (red.Count == 0)
            {
                richTextBox1.Text = "Knjižara je prazna.";
            }
            else
            {
                foreach (Knjiga k in red)
                {
                    richTextBox1.AppendText(k.ToString() + "\n");
                }
            }

            richTextBox1.AppendText(
                "\nUkupna cena korpe: " +
                Knjiga.UkupnaCenaKnjizare.ToString("0.00"));
        }

        private void buttonDodaj_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog();
            PrikaziKnjige();
        }

        private void buttonIzbaci_Click(object sender, EventArgs e)
        {
            if (red.Count > 0)
            {
                Knjiga k = red.Dequeue();
                Knjiga.UkupnaCenaKnjizare -= k.UkupnaCena;
                PrikaziKnjige();
            }
            else
            {
                MessageBox.Show("Knjižara je prazna!");
            }
        }

        private void buttonIzbrisiSve_Click(object sender, EventArgs e)
        {
            DialogResult odgovor = MessageBox.Show(
                "Da li ste sigurni da želite da ispraznite knjižaru?",
                "Potvrda",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (odgovor == DialogResult.OK)
            {
                red.Clear();
                Knjiga.UkupnaCenaKnjizare = 0;
                PrikaziKnjige();
            }
        }

        private void buttonSacuvaj_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("Knjiga.txt"))
            {
                foreach (Knjiga k in red)
                {
                    sw.WriteLine(
                        k.NazivKnjige + "|" +
                        k.IzdavackaKuca + "|" +
                        k.BrojStranica);
                }
            }

            MessageBox.Show("Podaci su uspešno sačuvani!");
        }

        private void buttonKraj_Click(object sender, EventArgs e)
        {
            DialogResult odgovor = MessageBox.Show(
                "Ukupna vrednost knjižare je: " +
                Knjiga.UkupnaCenaKnjizare.ToString("0.00") +
                "\nDa li ste sigurni da želite da zatvorite program?",
                "Kraj programa",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (odgovor == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void buttonStatistika_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.ShowDialog();
        }
    }
}

====================================================================
 3. PROZOR ZA UNOS KNJIGE: Form2.cs
====================================================================

using System;
using System.Linq;
using System.Windows.Forms;

namespace Knjizara
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(
                Knjiga.CenaStranice.Keys.ToArray());

            comboBox1.SelectedIndex = 0;
        }

        private void buttonUnesi_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            bool greska = false;

            string naziv = textBox1.Text.Trim();
            string izdavac = comboBox1.Text;
            int broj;

            if (naziv.Length < 3)
            {
                errorProvider1.SetError(
                    textBox1,
                    "Naziv mora imati bar 3 karaktera!");

                greska = true;
            }

            if (!int.TryParse(textBox2.Text, out broj) || broj <= 1)
            {
                errorProvider1.SetError(
                    textBox2,
                    "Broj stranica mora biti ceo broj veći od 1!");

                greska = true;
            }

            if (Form1.red.Any(k =>
                k.NazivKnjige.Equals(
                    naziv,
                    StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show(
                    "Knjiga sa ovim nazivom već postoji!");

                greska = true;
            }

            if (greska)
                return;

            Knjiga k = new Knjiga(naziv, izdavac, broj);

            Form1.red.Enqueue(k);

            MessageBox.Show("Knjiga je uspešno dodata!");

            this.Close();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            comboBox1.SelectedIndex = 0;

            errorProvider1.Clear();

            textBox1.Focus();
        }

        private void buttonOdustani_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


====================================================================
 4. PROZOR ZA STATISTIKU I GRAFIK: Form3.cs
====================================================================

using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Knjizara
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            foreach (Knjiga k in Form1.red)
            {
                checkedListBox1.Items.Add(k.NazivKnjige);
            }
        }

        private void PrikaziGrafik()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            if (!radioButton1.Checked && !radioButton2.Checked)
                return;

            chart1.ChartAreas.Add(new ChartArea());

            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Interval = 500;

            Series s = new Series();
            s.ChartType = SeriesChartType.Line;

            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                string naziv = checkedListBox1.CheckedItems[i].ToString();

                foreach (Knjiga k in Form1.red)
                {
                    if (k.NazivKnjige == naziv)
                    {
                        if (radioButton1.Checked)
                            s.Points.AddXY(naziv, k.BrojStranica);

                        if (radioButton2.Checked)
                            s.Points.AddXY(naziv, k.UkupnaCena);
                    }
                }
            }

            chart1.Series.Add(s);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PrikaziGrafik();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            PrikaziGrafik();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            PrikaziGrafik();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

