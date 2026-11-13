============================================================
Gost.cs
============================================================

using System;
using System.Collections.Generic;

namespace Hotel
{
    public class Gost
    {
        private string prezime;
        private int brojNocenja;
        private int brojClanova;
        private string aranzman;
        private double cena;

        public static double ukupnaCena = 0;

        public static Dictionary<string, double> Ponuda =
            new Dictionary<string, double>()
            {
                { "Nocenje sa doruckom", 50.45 },
                { "Polupansion", 70.45 },
                { "Pun pansion", 99.81 },
                { "All inclusive", 130.81 }
            };

        public Gost(string prezime, int brojNocenja,
                    int brojClanova, string aranzman)
        {
            this.prezime = prezime;
            this.brojNocenja = brojNocenja;
            this.brojClanova = brojClanova;
            this.aranzman = aranzman;

            cena = IzracunajCenu();
            ukupnaCena += cena;
        }

        public string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }

        public int BrojNocenja
        {
            get { return brojNocenja; }
            set { brojNocenja = value; }
        }

        public int BrojClanova
        {
            get { return brojClanova; }
            set { brojClanova = value; }
        }

        public string Aranzman
        {
            get { return aranzman; }
            set { aranzman = value; }
        }

        public double Cena
        {
            get { return cena; }
        }

        public static double UkupnaCena
        {
            get { return ukupnaCena; }
        }

        private double IzracunajCenu()
        {
            double iznos = brojClanova * brojNocenja * Ponuda[aranzman];

            if (brojNocenja > 5)
                iznos *= 0.8;

            return iznos;
        }

        public override string ToString()
        {
            return "Porodica: " + Prezime +
                   ", broj clanova: " + BrojClanova +
                   ", broj nocenja: " + BrojNocenja +
                   ", aranzman: " + Aranzman +
                   ", cena: " + Cena.ToString("0.00");
        }
    }
}


============================================================
Form1.cs
============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Hotel
{
    public partial class Form1 : Form
    {
        public static Dictionary<int, Gost> rezervacije =
            new Dictionary<int, Gost>();

        private int sledeciBroj = 1111;

        public Form1()
        {
            InitializeComponent();

            UcitajRezervacije();
            OsveziListu();
        }

        private void UcitajRezervacije()
        {
            string fajl = "Gosti.txt";

            if (!File.Exists(fajl))
            {
                MessageBox.Show("Spisak rezervacija je prazan.");
                textBox1.Text = "0.00";
                return;
            }

            try
            {
                // Pri svakom ucitavanju pocinjemo od praznog recnika
                // kako se podaci ne bi duplirali.
                rezervacije.Clear();
                Gost.ukupnaCena = 0;
                sledeciBroj = 1111;

                string[] redovi = File.ReadAllLines(fajl);

                foreach (string red in redovi)
                {
                    if (string.IsNullOrWhiteSpace(red))
                        continue;

                    string[] podaci = red.Split('|');

                    if (podaci.Length != 4)
                        continue;

                    string prezime = podaci[0].Trim();
                    int nocenja;
                    int clanovi;

                    if (!int.TryParse(podaci[1], out nocenja) ||
                        !int.TryParse(podaci[2], out clanovi) ||
                        nocenja <= 0 || clanovi <= 0)
                        continue;

                    string aranzman = podaci[3].Trim();

                    if (!Gost.Ponuda.ContainsKey(aranzman))
                        continue;

                    Gost g = new Gost(prezime, nocenja, clanovi, aranzman);

                    rezervacije.Add(sledeciBroj, g);
                    sledeciBroj++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri ucitavanju fajla: " + ex.Message);
            }
        }

        private void OsveziListu()
        {
            listBox1.Items.Clear();

            foreach (KeyValuePair<int, Gost> r in rezervacije)
            {
                listBox1.Items.Add(r.Key + " = " + r.Value);
            }

            textBox1.Text = Gost.UkupnaCena.ToString("0.00");
        }

        private void btnUnos_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();

            if (f2.ShowDialog() == DialogResult.OK)
            {
                rezervacije.Add(sledeciBroj, f2.NoviGost);
                sledeciBroj++;

                OsveziListu();
            }
        }

        private void btnObrisi_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Izaberite gosta!");
                return;
            }

            int broj = int.Parse(
                listBox1.SelectedItem.ToString().Split('=')[0].Trim());

            Gost.ukupnaCena -= rezervacije[broj].Cena;
            rezervacije.Remove(broj);

            OsveziListu();
        }

        private void btnObrisiListu_Click(object sender, EventArgs e)
        {
            rezervacije.Clear();
            Gost.ukupnaCena = 0;

            OsveziListu();
        }

        private void btnSacuvaj_Click(object sender, EventArgs e)
        {
            try
            {
                // Isti fajl se koristi i za ucitavanje i za cuvanje.
                using (StreamWriter sw = new StreamWriter("Gosti.txt"))
                {
                    foreach (KeyValuePair<int, Gost> r in rezervacije)
                    {
                        Gost g = r.Value;

                        sw.WriteLine(
                            g.Prezime + "|" +
                            g.BrojNocenja + "|" +
                            g.BrojClanova + "|" +
                            g.Aranzman);
                    }
                }

                MessageBox.Show("Podaci su sacuvani u Gosti.txt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri cuvanju fajla: " + ex.Message);
            }
        }

        private void btnStatistika_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3();
            f3.ShowDialog();
        }

        private void btnKraj_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}


============================================================
Form2.cs
============================================================

using System;
using System.Windows.Forms;

namespace Hotel
{
    public partial class Form2 : Form
    {
        public Gost NoviGost { get; private set; }

        public Form2()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(
                new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            comboBox1.SelectedIndex = 0;
        }

        private void btnUnesi_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            bool ispravno = true;

            if (tb1.Text.Trim().Length < 3)
            {
                errorProvider1.SetError(
                    tb1, "Prezime mora imati najmanje 3 karaktera!");

                ispravno = false;
            }

            int nocenja;

            if (!int.TryParse(tb2.Text, out nocenja) || nocenja <= 0)
            {
                errorProvider1.SetError(
                    tb2, "Broj nocenja mora biti pozitivan ceo broj!");

                ispravno = false;
            }

            if (comboBox1.SelectedItem == null)
            {
                errorProvider1.SetError(
                    comboBox1, "Izaberite broj clanova!");

                ispravno = false;
            }

            string aranzman = "";

            if (rbNocenje.Checked)
                aranzman = "Nocenje sa doruckom";
            else if (rbPolupansion.Checked)
                aranzman = "Polupansion";
            else if (rbPunPansion.Checked)
                aranzman = "Pun pansion";
            else if (rbAllInclusive.Checked)
                aranzman = "All inclusive";
            else
            {
                MessageBox.Show("Izaberite aranzman!");
                ispravno = false;
            }

            if (!ispravno)
            {
                MessageBox.Show("Proverite unete podatke!");
                return;
            }

            int clanovi = Convert.ToInt32(comboBox1.SelectedItem);

            NoviGost = new Gost(
                tb1.Text.Trim(),
                nocenja,
                clanovi,
                aranzman);

            MessageBox.Show("Rezervacija je uspesno dodata!");

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tb1.Clear();
            tb2.Clear();

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;

            rbNocenje.Checked = false;
            rbPolupansion.Checked = false;
            rbPunPansion.Checked = false;
            rbAllInclusive.Checked = false;

            errorProvider1.Clear();
            tb1.Focus();
        }

        private void btnOdustani_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}


============================================================
Form3.cs
============================================================

using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Hotel
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();

            UcitajGoste();

            // Podrazumevano prikazujemo statistiku nocenja.
            rbNocenja.Checked = true;

            PrikaziStatistiku();
        }

        private void UcitajGoste()
        {
            checkedListBox1.Items.Clear();

            foreach (var r in Form1.rezervacije)
            {
                checkedListBox1.Items.Add(
                    r.Key + " - " + r.Value.Prezime);
            }
        }

        private void PrikaziStatistiku()
        {
            chart1.Series.Clear();

            Series s = new Series("Statistika");
            s.ChartType = SeriesChartType.Pie;

            foreach (object stavka in checkedListBox1.CheckedItems)
            {
                string izabrani = stavka.ToString();

                int broj = int.Parse(
                    izabrani.Split('-')[0].Trim());

                if (!Form1.rezervacije.ContainsKey(broj))
                    continue;

                Gost g = Form1.rezervacije[broj];

                double vrednost;

                if (rbNocenja.Checked)
                    vrednost = g.BrojNocenja;
                else
                    vrednost = g.Cena;

                s.Points.AddXY(g.Prezime, vrednost);
            }

            chart1.Series.Add(s);
        }

        private void checkedListBox1_ItemCheck(
            object sender, ItemCheckEventArgs e)
        {
            // ItemCheck se poziva pre nego sto se promeni CheckedItems,
            // pa prikaz osvezavamo nakon zavrsetka obrade dogadjaja.
            this.BeginInvoke(new Action(PrikaziStatistiku));
        }

        private void rbNocenja_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNocenja.Checked)
                PrikaziStatistiku();
        }

        private void rbCena_CheckedChanged(object sender, EventArgs e)
        {
            if (rbCena.Checked)
                PrikaziStatistiku();
        }

        private void btnZatvori_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
