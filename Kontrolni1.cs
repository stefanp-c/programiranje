====================================================================
 1. KLASA: Knjiga.cs
====================================================================

using System;
using System.Collections.Generic;

public class Knjiga
{
    // Rečnik predefinisanih cena stranica po izdavačkim kućama
    public static Dictionary<string, double> CeneStranica = new Dictionary<string, double>()
    {
        { "Nolit", 2.45 },
        { "Prometej", 1.47 },
        { "Laguna", 2.08 },
        { "Danilo Kis", 1.88 }
    };

    private string nazivKnjige;
    private string izdavackaKuca;
    private int brojStranica;
    private double ukupnaCenaProizvoda;
    
    // Statični podatak - ukupna cena svih knjiga u knjižari
    public static double ukupnaCenaSvihKnjiga = 0;

    // Javna svojstva (Properties)
    public string NazivKnjige
    {
        get => nazivKnjige;
        set => nazivKnjige = value;
    }

    public string IzdavackaKuca
    {
        get => izdavackaKuca;
        set => izdavackaKuca = value;
    }

    public int BrojStranica
    {
        get => brojStranica;
        set
        {
            brojStranica = value;
            IzracunajCenu();
        }
    }

    public double UkupnaCenaProizvoda => ukupnaCenaProizvoda;

    // Konstruktor
    public Knjiga(string naziv, string izdavac, int brStranica)
    {
        nazivKnjige = naziv;
        izdavackaKuca = izdavac;
        brojStranica = brStranica;
        IzracunajCenu();
    }

    private void IzracunajCenu()
    {
        if (CeneStranica.ContainsKey(izdavackaKuca))
        {
            ukupnaCenaProizvoda = brojStranica * CeneStranica[izdavackaKuca];
        }
        else
        {
            ukupnaCenaProizvoda = 0;
        }
    }

    // Redefinisanje ToString funkcije prema zahtevu
    public override string ToString()
    {
        return $"Naziv knjige: {nazivKnjige}, izdavačka kuća:{izdavackaKuca}, broj stranica: {brojStranica}, ukupna cena: {ukupnaCenaProizvoda:F2}";
    }
}


====================================================================
 2. GLAVNI PROZOR: Form1.cs
====================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public partial class Form1 : Form
{
    // Red (Queue) kao kolekcija podataka
    public static Queue<Knjiga> knjizareRed = new Queue<Knjiga>();

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        UcitajIzFajla();
    }

    public void UcitajIzFajla()
    {
        knjizareRed.Clear();
        if (File.Exists("Knjiga.txt"))
        {
            string[] linije = File.ReadAllLines("Knjiga.txt");
            foreach (var linija in linije)
            {
                if (string.IsNullOrWhiteSpace(linija)) continue;
                string[] delovi = linija.Split('|');
                if (delovi.Length == 3)
                {
                    string naziv = delovi[0];
                    string izdavac = delovi[1];
                    if (int.TryParse(delovi[2], out int brStrana))
                    {
                        Knjiga k = new Knjiga(naziv, izdavac, brStrana);
                        knjizareRed.Enqueue(k);
                    }
                }
            }
            OsveziPrikaz();
        }
        else
        {
            richTextBox1.Text = "Knjižara je prazna.";
        }
    }

    public void OsveziPrikaz()
    {
        richTextBox1.Clear();
        Knjiga.ukupnaCenaSvihKnjiga = 0;

        foreach (var k in knjizareRed)
        {
            richTextBox1.AppendText(k.ToString() + "\n");
            Knjiga.ukupnaCenaSvihKnjiga += k.UkupnaCenaProizvoda;
        }

        richTextBox1.AppendText($"\nUkupna cena korpe / knjižare: {Knjiga.ukupnaCenaSvihKnjiga:F2} RSD");
    }

    private void btnDodajKnjigu_Click(object sender, EventArgs e)
    {
        Form2 f2 = new Form2();
        f2.ShowDialog();
        OsveziPrikaz();
    }

    private void btnIzbaciPrvu_Click(object sender, EventArgs e)
    {
        if (knjizareRed.Count > 0)
        {
            knjizareRed.Dequeue();
            OsveziPrikaz();
        }
        else
        {
            MessageBox.Show("Knjižara je već prazna!");
        }
    }

    private void btnIzbrisiSve_Click(object sender, EventArgs e)
    {
        DialogResult dr = MessageBox.Show("Da li ste sigurni da želite da ispraznire knjižaru?", "Potvrda", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (dr == DialogResult.OK)
        {
            knjizareRed.Clear();
            OsveziPrikaz();
        }
    }

    private void btnSacuvaj_Click(object sender, EventArgs e)
    {
        List<string> linije = new List<string>();
        foreach (var k in knjizareRed)
        {
            linije.Add($"{k.NazivKnjige}|{k.IzdavackaKuca}|{k.BrojStranica}");
        }
        File.WriteAllLines("Knjiga.txt", linije);
        MessageBox.Show("Podaci su uspešno sačuvani u fajl!");
    }

    private void btnKraj_Click(object sender, EventArgs e)
    {
        DialogResult dr = MessageBox.Show($"Ukupna vrednost knjižare je: {Knjiga.ukupnaCenaSvihKnjiga:F2} RSD.\nDa li ste sigurni da želite da zatvorite program?", "Kraj programa", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (dr == DialogResult.OK)
        {
            Application.Exit();
        }
    }

    private void btnStatistika_Click(object sender, EventArgs e)
    {
        Form3 f3 = new Form3();
        f3.ShowDialog();
    }
}


====================================================================
 3. PROZOR ZA UNOS KNJIGE: Form2.cs
====================================================================

using System;
using System.Windows.Forms;

public partial class Form2 : Form
{
    public Form2()
    {
        InitializeComponent();
    }

    private void Form2_Load(object sender, EventArgs e)
    {
        comboBox1.Items.Clear();
        foreach (var izdavac in Knjiga.CeneStranica.Keys)
        {
            comboBox1.Items.Add(izdavac);
        }
        if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
    }

    private void btnUnesi_Click(object sender, EventArgs e)
    {
        errorProvider1.Clear();
        bool greska = false;

        string naziv = textBox1.Text.Trim();
        
        // Provera dužine naziva (bar 3 karaktera)
        if (naziv.Length < 3)
        {
            errorProvider1.SetError(textBox1, "Naziv knjige mora imati bar tri karaktera!");
            greska = true;
        }

        // Provera da li već postoji knjiga sa istim nazivom
        foreach (var k in Form1.knjizareRed)
        {
            if (k.NazivKnjige.Equals(naziv, StringComparison.OrdinalIgnoreCase))
            {
                errorProvider1.SetError(textBox1, "Knjiga sa istim nazivom već postoji!");
                greska = true;
                break;
            }
        }

        // Provera broja stranica (mora biti ceo broj veći od 1)
        if (!int.TryParse(textBox2.Text, out int brStrana) || brStrana <= 1)
        {
            errorProvider1.SetError(textBox2, "Broj stranica mora biti ceo broj veći od 1!");
            greska = true;
        }

        if (greska) return;

        string izdavac = comboBox1.SelectedItem.ToString();

        // Kreiranje nove knjige i dodavanje u red
        Knjiga novaKnjiga = new Knjiga(naziv, izdavac, brStrana);
        Form1.knjizareRed.Enqueue(novaKnjiga);

        MessageBox.Show("Knjiga je uspešno dodata!", "Uspeh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        textBox1.Clear();
        textBox2.Clear();
        if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
        errorProvider1.Clear();
    }

    private void btnOdustani_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}


====================================================================
 4. PROZOR ZA STATISTIKU I GRAFIK: Form3.cs
====================================================================

using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

public partial class Form3 : Form
{
    public Form3()
    {
        InitializeComponent();
    }

    private void Form3_Load(object sender, EventArgs e)
    {
        // 1. Učitaj sve knjige iz reda u CheckedListBox i podrazumevano ih čekiraj
        checkedListBox1.Items.Clear();
        foreach (var k in Form1.knjizareRed)
        {
            checkedListBox1.Items.Add(k.NazivKnjige, true);
        }

        // 2. Postavi podrazumevani izbor na broj stranica
        radioButton1.Checked = true;

        // 3. Poveži događaje za osvežavanje grafika u realnom vremenu
        radioButton1.CheckedChanged += RadioButton_CheckedChanged;
        radioButton2.CheckedChanged += RadioButton_CheckedChanged;
        checkedListBox1.ItemCheck += CheckedListBox1_ItemCheck;

        // Inicijalno crtanje grafika čim se forma otvori
        OsveziGrafik();
    }

    private void RadioButton_CheckedChanged(object sender, EventArgs e)
    {
        if (((RadioButton)sender).Checked)
        {
            OsveziGrafik();
        }
    }

    private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        // Osiguravamo osvežavanje grafika nakon što se proveri čekiranje
        this.BeginInvoke(new MethodInvoker(OsveziGrafik));
    }

    private void OsveziGrafik()
    {
        chart1.Series.Clear();
        Series serija = new Series("Statistika");
        serija.ChartType = SeriesChartType.Line; // Linijski grafik
        serija.MarkerStyle = MarkerStyle.Circle;  // Tačke na grafiku radi preglednosti
        serija.MarkerSize = 8;

        bool prikaziStranice = radioButton1.Checked;
        serija.Name = prikaziStranice ? "Broj stranica" : "Cena knjige";

        double maxVrednost = 0;

        foreach (var item in checkedListBox1.CheckedItems)
        {
            string nazivKnjige = item.ToString();
            
            Knjiga pronadjenaKnjiga = null;
            foreach (var k in Form1.knjizareRed)
            {
                if (k.NazivKnjige.Equals(nazivKnjige, StringComparison.OrdinalIgnoreCase))
                {
                    pronadjenaKnjiga = k;
                    break;
                }
            }

            if (pronadjenaKnjiga != null)
            {
                double yVrednost = prikaziStranice ? pronadjenaKnjiga.BrojStranica : pronadjenaKnjiga.UkupnaCenaProizvoda;
                serija.Points.AddXY(nazivKnjige, yVrednost);

                if (yVrednost > maxVrednost)
                {
                    maxVrednost = yVrednost;
                }
            }
        }

        chart1.Series.Add(serija);
        
        // Podešavanje osa grafikona
        if (chart1.ChartAreas.Count > 0)
        {
            ChartArea zona = chart1.ChartAreas[0];
            zona.AxisX.Title = "Naziv knjige";
            zona.AxisY.Title = prikaziStranice ? "Broj stranica" : "Ukupna cena (RSD)";

            // Vertikalni nazivi na X osi
            zona.AxisX.LabelStyle.Angle = -90;
            zona.AxisX.Interval = 1;

            // Podešavanje Y ose na podelu po 500
            zona.AxisY.Minimum = 0;
            zona.AxisY.Interval = 500;

            if (maxVrednost > 0)
            {
                double izracunatiMax = Math.Ceiling(maxVrednost / 500.0) * 500;
                zona.AxisY.Maximum = izracunatiMax < maxVrednost + 200 ? izracunatiMax + 500 : izracunatiMax;
            }
            else
            {
                zona.AxisY.Maximum = 1000;
            }
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
