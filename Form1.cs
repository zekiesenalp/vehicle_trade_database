using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AracAlisSatis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
           
        }     

        private void button1_Click_1(object sender, EventArgs e)
        {
            Ilanlar i = new Ilanlar(); i.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Ilan_Filtrele filtre = new Ilan_Filtrele(); filtre.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Arac a = new Arac(); a.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Sehir s = new Sehir(); s.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Yakit y = new Yakit(); y.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Renk r = new Renk(); r.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            VitesTuru v = new VitesTuru(); v.Show();
        }
    }
}
