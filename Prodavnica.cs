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
        private double ukupnaCenaProizvoda;

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
            get { return ukupnaCenaProizvoda; }
        }

        public Artikal(string naziv, int broj, double cena)
        {
            nazivProizvoda = naziv;
            brojProizvoda = broj;
            cenaJednogProizvoda = cena;

            ukupnaCenaProizvoda = broj * cena;

            // Popust od 20% ako je kolicina veca od 4.
            if (broj > 4)
            {
                ukupnaCenaProizvoda = ukupnaCenaProizvoda * 0.8;
            }

            ukupnaCenaKorpe += ukupnaCenaProizvoda;
        }

        public override string ToString()
        {
            string popust = "ne";

            if (brojProizvoda > 4)
            {
                popust = "da";
            }

            return "Naziv proizvoda: " + nazivProizvoda +
                   ", broj proizvoda: " + brojProizvoda +
                   ", cena jednog proizvoda: " + cenaJednogProizvoda +
                   ", popust: " + popust +
                   ", ukupna cena: " + ukupnaCenaProizvoda;
        }
    }
}


---------------------------------------------------------------
2. Form1.cs – GLAVNA FORMA
---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace Prodavnica
{
    public partial class Form1 : Form
    {
        Stack<Artikal> korpa = new Stack<Artikal>();

        public Form1()
        {
            InitializeComponent();
            ucitajKorpu();
        }

        // Ucitavanje artikala iz fajla Korpa.txt.
        private void ucitajKorpu()
        {
            string putanja = Application.StartupPath + "\\Korpa.txt";

            if (!File.Exists(putanja))
            {
                MessageBox.Show("Korpa je prazna!");
                prikaziKorpu();
                return;
            }

            Artikal.ukupnaCenaKorpe = 0;
            korpa.Clear();

            string[] redovi = File.ReadAllLines(putanja);

            foreach (string red in redovi)
            {
                string[] podaci = red.Split('|');

                if (podaci.Length == 3)
                {
                    string naziv = podaci[0];
                    int broj;
                    double cena;

                    if (int.TryParse(podaci[1], out broj) &&
                        double.TryParse(podaci[2],
                            NumberStyles.Number,
                            CultureInfo.CurrentCulture, out cena) &&
                        broj > 0 && cena > 0)
                    {
                        Artikal a = new Artikal(naziv, broj, cena);
                        korpa.Push(a);
                    }
                }
            }

            prikaziKorpu();
        }

        // Prikaz svih artikala i ukupne cene korpe.
        private void prikaziKorpu()
        {
            richTextBox1.Clear();

            foreach (Artikal a in korpa)
            {
                richTextBox1.AppendText(a.ToString() + "\n\n");
            }

            richTextBox1.AppendText(
                "UKUPNA CENA KORPE: " +
                Artikal.ukupnaCenaKorpe.ToString("0.00"));
        }

        // Dugme UBACI U KORPU.
        private void btnUbaci_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(korpa);
            f.ShowDialog();

            prikaziKorpu();
        }

        // Dugme STORNIRAJ POSLEDNJI.
        private void btnStorniraj_Click(object sender, EventArgs e)
        {
            if (korpa.Count > 0)
            {
                Artikal a = korpa.Pop();

                Artikal.ukupnaCenaKorpe -= a.UkupnaCenaProizvoda;

                prikaziKorpu();
            }
            else
            {
                MessageBox.Show("Korpa je prazna!");
            }
        }

        // Dugme ISPRAZNI KORPU.
        private void btnIsprazni_Click(object sender, EventArgs e)
        {
            DialogResult odgovor = MessageBox.Show(
                "Da li ste sigurni da zelite da ispraznite korpu?",
                "Potvrda",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (odgovor == DialogResult.OK)
            {
                korpa.Clear();
                Artikal.ukupnaCenaKorpe = 0;

                prikaziKorpu();
            }
        }

        // Dugme SACUVAJ U FAJL.
        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            string putanja = Application.StartupPath + "\\Korpa.txt";

            using (StreamWriter sw = new StreamWriter(putanja))
            {
                foreach (Artikal a in korpa)
                {
                    sw.WriteLine(
                        a.NazivProizvoda + "|" +
                        a.BrojProizvoda + "|" +
                        a.CenaJednogProizvoda.ToString(
                            CultureInfo.CurrentCulture));
                }
            }

            MessageBox.Show("Korpa je uspesno sacuvana!");
        }

        // Dugme ZAVRSI KUPOVINU.
        private void btnZavrsi_Click(object sender, EventArgs e)
        {
            DialogResult odgovor = MessageBox.Show(
                "Ukupna vrednost korpe je: " +
                Artikal.ukupnaCenaKorpe.ToString("0.00") +
                "\nDa li ste sigurni da zelite da zavrsite kupovinu?",
                "Zavrsetak kupovine",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (odgovor == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        // Dugme STATISTIKA.
        private void btnStatistika_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3(korpa);
            f.ShowDialog();
        }
    }
}


---------------------------------------------------------------
3. Form2.cs – DODAVANJE ARTIKLA
---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace Prodavnica
{
    public partial class Form2 : Form
    {
        Stack<Artikal> korpa;

        public Form2(Stack<Artikal> s)
        {
            InitializeComponent();
            korpa = s;

            numUpDown1.Minimum = 1;
            numUpDown1.Value = 1;
        }

        // Dugme STAVI U KORPU.
        private void btn1_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            bool greska = false;

            if (tb1.Text.Trim().Length < 3)
            {
                errorProvider1.SetError(
                    tb1, "Naziv mora imati bar 3 karaktera");
                greska = true;
            }

            double cena;

            if (!double.TryParse(
                    tb2.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out cena) || cena <= 0)
            {
                errorProvider1.SetError(
                    tb2, "Cena mora biti pozitivan realan broj");
                greska = true;
            }

            if (numUpDown1.Value <= 1)
            {
                errorProvider1.SetError(
                    numUpDown1, "Kolicina mora biti veca od 1");
                greska = true;
            }

            // Provera da li artikal sa istim nazivom vec postoji.
            foreach (Artikal a in korpa)
            {
                if (a.NazivProizvoda.ToLower() ==
                    tb1.Text.Trim().ToLower())
                {
                    MessageBox.Show("Proizvod vec postoji u korpi!");
                    greska = true;
                    break;
                }
            }

            if (greska)
            {
                return;
            }

            Artikal novi = new Artikal(
                tb1.Text.Trim(),
                (int)numUpDown1.Value,
                cena);

            korpa.Push(novi);

            MessageBox.Show("Artikal je dodat u korpu!");

            this.Close();
        }

        // Dugme RESETUJ.
        private void btn2_Click(object sender, EventArgs e)
        {
            tb1.Clear();
            tb2.Clear();
            numUpDown1.Value = 1;

            errorProvider1.Clear();
        }

        // Dugme ODUSTANI.
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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Prodavnica
{
    public partial class Form3 : Form
    {
        Stack<Artikal> korpa;

        public Form3(Stack<Artikal> s)
        {
            InitializeComponent();

            korpa = s;

            // U CheckedListBox ubacujemo nazive proizvoda.
            foreach (Artikal a in korpa)
            {
                if (!checkedListBox1.Items.Contains(a.NazivProizvoda))
                {
                    checkedListBox1.Items.Add(a.NazivProizvoda);
                }
            }
        }

        private void prikaziStatistiku()
        {
            chart1.Series.Clear();

            // Statistika cene jednog proizvoda.
            if (checkBox1.Checked)
            {
                Series cena = new Series("Cena");
                cena.ChartType = SeriesChartType.Column;

                foreach (string naziv in checkedListBox1.CheckedItems)
                {
                    foreach (Artikal a in korpa)
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

            // Statistika kolicine kupljenog proizvoda.
            if (checkBox2.Checked)
            {
                Series kolicina = new Series("Kolicina");
                kolicina.ChartType = SeriesChartType.Column;

                foreach (string naziv in checkedListBox1.CheckedItems)
                {
                    foreach (Artikal a in korpa)
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

            // Y osa pocinje od 0, a oznake su na 0, 20, 40, 60...
            // Maksimum se automatski prilagodjava podacima grafikona.
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Interval = 20;
            chart1.ChartAreas[0].AxisY.Maximum = double.NaN;
        }

        // Dogadjaj ItemCheck za CheckedListBox.
        private void checkedListBox1_ItemCheck(
            object sender, ItemCheckEventArgs e)
        {
            // BeginInvoke izvrsava prikaz nakon promene cekiranja.
            this.BeginInvoke(new Action(prikaziStatistiku));
        }

        // Dogadjaj CheckedChanged za checkbox CENA.
        private void checkBox1_CheckedChanged(
            object sender, EventArgs e)
        {
            prikaziStatistiku();
        }

        // Dogadjaj CheckedChanged za checkbox KOLICINA.
        private void checkBox2_CheckedChanged(
            object sender, EventArgs e)
        {
            prikaziStatistiku();
        }

        // Dugme ZATVORI.
        private void btnZatvori_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

