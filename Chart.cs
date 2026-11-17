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

foreach (string naziv in checkedListBox1.CheckedItems)
{
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
                    cena.Points.AddXY(naziv, a.CenaJednogProizvoda);
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
                    kolicina.Points.AddXY(naziv, a.BrojProizvoda);
                }
            }
        }

        chart1.Series.Add(kolicina);
    }

    chart1.ChartAreas[0].RecalculateAxesScale();
}
//PIE
private void prikazi()
{
    if (radioButton1.Checked)
    {
        chart1.Series.Clear();
        Series s = new Series();
        s.IsValueShownAsLabel = true;
        s.ChartType = SeriesChartType.Pie;
        foreach (string item in checkedListBox1.CheckedItems)
        {
            Class1 c = Form1.recnik.Values.FirstOrDefault(x => x.Prezime == item);
            s.Points.AddXY(c.Prezime, c.Brojnocenja);
        }
        chart1.Series.Add(s);
        
    }
    else if (radioButton2.Checked)
    {
        chart1.Series.Clear();
        Series s = new Series();
        s.IsValueShownAsLabel = true;
        s.ChartType = SeriesChartType.Pie;
        foreach (string item in checkedListBox1.CheckedItems)
        {
            Class1 c = Form1.recnik.Values.FirstOrDefault(x => x.Prezime == item);
            s.Points.AddXY(c.Prezime, c.Cena);
        }
        chart1.Series.Add(s);
    }
}
