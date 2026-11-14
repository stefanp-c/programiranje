---------------------------------------------------------------
1. Artikal.cs
---------------------------------------------------------------
using System;

namespace Prodavnica
{
    public class Artikal
    {
        private string nazivProizvoda;
        private int brojProizvoda;
        private double cenaJednogProizvoda;

        public static double ukupnaCenaKorpe = 0;

        public string NazivProizvoda
        {
            get { return nazivProizvoda; }
            set { nazivProizvoda = value; }
        }

        public int BrojProizvoda
        {
            get { return brojProizvoda; }
            set { brojProizvoda = value; }
        }

        public double CenaJednogProizvoda
        {
            get { return cenaJednogProizvoda; }
            set { cenaJednogProizvoda = value; }
        }

        public double UkupnaCenaProizvoda
        {
            get
            {
                double cena = brojProizvoda * cenaJednogProizvoda;

                if (brojProizvoda > 4)
                    cena *= 0.8;

                return cena;
            }
        }

        public bool ImaPopust
        {
            get { return brojProizvoda > 4; }
        }

        public Artikal(string naziv, int broj, double cena)
        {
            NazivProizvoda = naziv;
            BrojProizvoda = broj;
            CenaJednogProizvoda = cena;

            ukupnaCenaKorpe += UkupnaCenaProizvoda;
        }

        public override string ToString()
        {
            return "Naziv proizvoda: " + NazivProizvoda +
                ", broj proizvoda: " + BrojProizvoda +
                " cena jednog proizvoda: " + CenaJednogProizvoda +
                " popust: " + (ImaPopust ? "da" : "ne") +
                " ukupna cena: " + UkupnaCenaProizvoda;
        }
    }
}
---------------------------------------------------------------
2. Form1.cs – GLAVNA FORMA
---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Prodavnica
{
    public partial class Form1 : Form
    {
        public static Stack<Artikal> korpa = new Stack<Artikal>();

        public Form1()
        {
            InitializeComponent();
            UcitajIzFajla();
        }

        // Prikaz sadržaja korpe
        public void OsveziPrikaz()
        {
            richTextBox1.Clear();

            Artikal.ukupnaCenaKorpe = 0;

            foreach (Artikal a in korpa)
            {
                richTextBox1.AppendText(a.ToString() + Environment.NewLine);
                Artikal.ukupnaCenaKorpe += a.UkupnaCenaProizvoda;
            }

            richTextBox1.AppendText(
                Environment.NewLine +
                "UKUPNA CENA KORPE: " +
                Artikal.ukupnaCenaKorpe.ToString("0.00"));
        }

        // Ucitavanje iz fajla
        private void UcitajIzFajla()
        {
            if (!File.Exists("Korpa.txt"))
            {
                MessageBox.Show("Korpa je prazna.");
                return;
            }

            korpa.Clear();
            Artikal.ukupnaCenaKorpe = 0;

            string[] redovi = File.ReadAllLines("Korpa.txt");

            foreach (string red in redovi)
            {
                string[] podaci = red.Split('|');

                if (podaci.Length == 3)
                {
                    string naziv = podaci[0];
                    int broj = int.Parse(podaci[1]);

                    double cena = double.Parse(
                        podaci[2],
                        System.Globalization.CultureInfo.GetCultureInfo("sr-Latn-RS"));

                    korpa.Push(new Artikal(naziv, broj, cena));
                }
            }

            OsveziPrikaz();
        }

        // Ubaci u korpu
        private void btnUbaci_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.ShowDialog();

            OsveziPrikaz();
        }

        // Storniranje poslednjeg artikla
        private void btnStorniraj_Click(object sender, EventArgs e)
        {
            if (korpa.Count == 0)
            {
                MessageBox.Show("Korpa je prazna!");
                return;
            }

            Artikal a = korpa.Pop();

            Artikal.ukupnaCenaKorpe -= a.UkupnaCenaProizvoda;

            OsveziPrikaz();
        }

        // Isprazni korpu
        private void btnIsprazni_Click(object sender, EventArgs e)
        {
            DialogResult odgovor = MessageBox.Show(
                "Da li ste sigurni da želite da ispraznite korpu?",
                "Potvrda",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (odgovor == DialogResult.OK)
            {
                korpa.Clear();
                Artikal.ukupnaCenaKorpe = 0;
                OsveziPrikaz();
            }
        }

        // Sacuvaj u fajl
        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("Korpa.txt"))
            {
                // Stack se prolazi od poslednjeg dodatog ka prvom
                foreach (Artikal a in korpa)
                {
                    sw.WriteLine(
                        a.NazivProizvoda + "|" +
                        a.BrojProizvoda + "|" +
                        a.CenaJednogProizvoda);
                }
            }

            MessageBox.Show("Korpa je uspešno sačuvana!");
        }

        // Zavrsi kupovinu
        private void btnZavrsi_Click(object sender, EventArgs e)
        {
            DialogResult odgovor = MessageBox.Show(
                "Ukupna vrednost korpe je: " +
                Artikal.ukupnaCenaKorpe.ToString("0.00") +
                "\nDa li ste sigurni da želite da završite kupovinu?",
                "Završetak kupovine",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (odgovor == DialogResult.OK)
                Application.Exit();
        }

        // Statistika
        private void btnStatistika_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.ShowDialog();
        }
    }
}
---------------------------------------------------------------
3. Form2.cs – DODAVANJE ARTIKLA
---------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Forms;

namespace Prodavnica
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            numericUpDown1.Minimum = 1;
            numericUpDown1.Maximum = 1000;
            numericUpDown1.Value = 2;
        }

        // Stavi u korpu
        private void btn1_Click(object sender, EventArgs e)
        {
            bool ispravno = true;

            errorProvider1.Clear();

            string naziv = tb1.Text.Trim();

            // Provera naziva
            if (naziv.Length < 3)
            {
                errorProvider1.SetError(
                    tb1,
                    "Naziv mora imati najmanje 3 karaktera!");

                ispravno = false;
            }

            // Provera cene
            double cena;

            if (!double.TryParse(
                tb2.Text,
                NumberStyles.Number,
                CultureInfo.GetCultureInfo("sr-Latn-RS"),
                out cena) || cena <= 0)
            {
                errorProvider1.SetError(
                    tb2,
                    "Cena mora biti pozitivan realan broj!");

                ispravno = false;
            }

            // Provera kolicine
            if (numericUpDown1.Value <= 1)
            {
                errorProvider1.SetError(
                    numericUpDown1,
                    "Količina mora biti veća od 1!");

                ispravno = false;
            }

            // Provera da li artikal vec postoji
            foreach (Artikal a in Form1.korpa)
            {
                if (a.NazivProizvoda.Equals(
                    naziv,
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Proizvod sa tim nazivom već postoji!");

                    ispravno = false;
                    break;
                }
            }

            if (!ispravno)
                return;

            // Kreiranje i dodavanje artikla
            Artikal novi = new Artikal(
                naziv,
                (int)numericUpDown1.Value,
                cena);

            Form1.korpa.Push(novi);

            MessageBox.Show("Artikal je uspešno dodat u korpu!");

            this.Close();
        }

        // Resetovanje polja
        private void btn2_Click(object sender, EventArgs e)
        {
            tb1.Clear();
            tb2.Clear();

            numericUpDown1.Value = 2;

            errorProvider1.Clear();

            tb1.Focus();
        }

        // Odustani
        private void btn3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
---------------------------------------------------------------
4. Form3.cs – STATISTIKA
---------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Prodavnica
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            UcitajNazive();
            chart1.Series.Clear();

            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Interval = 20;
        }

        // Ucitavanje naziva proizvoda
        private void UcitajNazive()
        {
            checkedListBox1.Items.Clear();

            foreach (Artikal a in Form1.korpa)
            {
                checkedListBox1.Items.Add(a.NazivProizvoda);
            }
        }

        // Prikaz statistike
        private void PrikaziStatistiku()
        {
            chart1.Series.Clear();

            if (!checkBoxCena.Checked &&
                !checkBoxKolicina.Checked)
            {
                MessageBox.Show(
                    "Izaberite cenu ili količinu!");

                return;
            }

            if (checkBoxCena.Checked)
            {
                Series cena = new Series("Cena");
                cena.ChartType = SeriesChartType.Column;

                foreach (string naziv in checkedListBox1.CheckedItems)
                {
                    foreach (Artikal a in Form1.korpa)
                    {
                        if (a.NazivProizvoda == naziv)
                        {
                            cena.Points.AddXY(
                                naziv,
                                a.CenaJednogProizvoda);
                        }
                    }
                }

                chart1.Series.Add(cena);
            }

            if (checkBoxKolicina.Checked)
            {
                Series kolicina = new Series("Količina");
                kolicina.ChartType = SeriesChartType.Column;

                foreach (string naziv in checkedListBox1.CheckedItems)
                {
                    foreach (Artikal a in Form1.korpa)
                    {
                        if (a.NazivProizvoda == naziv)
                        {
                            kolicina.Points.AddXY(
                                naziv,
                                a.BrojProizvoda);
                        }
                    }
                }

                chart1.Series.Add(kolicina);
            }

            chart1.ChartAreas[0].RecalculateAxesScale();
        }

        // Promena izabranih proizvoda
        private void checkedListBox1_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            PrikaziStatistiku();
        }

        // Promena checkbox-a za cenu
        private void checkBoxCena_CheckedChanged(
            object sender, EventArgs e)
        {
            PrikaziStatistiku();
        }

        // Promena checkbox-a za kolicinu
        private void checkBoxKolicina_CheckedChanged(
            object sender, EventArgs e)
        {
            PrikaziStatistiku();
        }

        // Zatvaranje forme
        private void btn1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

