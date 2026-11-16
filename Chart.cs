//LINE
private void PrikaziGrafik()
{
    chart1.Series.Clear();
    chart1.ChartAreas.Clear();

    chart1.ChartAreas.Add(new ChartArea());

    chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
    chart1.ChartAreas[0].AxisY.Minimum = 0;
    chart1.ChartAreas[0].AxisY.Interval = 500;

    Series s = new Series();
    s.ChartType = SeriesChartType.Line;
    s.IsValueShownAsLabel = true;

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
//COLUMN
private void PrikaziStatistiku()
{
    chart1.Series.Clear();

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
//PIE
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
