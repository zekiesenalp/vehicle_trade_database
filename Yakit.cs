using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace AracAlisSatis
{
    public partial class Yakit : Form
    {
        public Yakit()
        {
            InitializeComponent();
        }

        bool duzen = false;
        string id;

       

        private void Yakit_Load(object sender, EventArgs e)
        {
            refresh();
            button2.Visible = false;
        }
        public void refresh()
        {
            listView1.Items.Clear();
            MySqlDataReader reader = db.oku("SELECT * FROM `tbl_yakitturu`", new string[] { });
            while (reader.Read())
            {
                ListViewItem ekle = new ListViewItem();
                ekle.Text = reader["YakitTuruID"].ToString();
                ekle.SubItems.Add(reader["Yakit_Turu"].ToString());
                listView1.Items.Add(ekle);
            }
            db.baglanti.Close();
        }
        public void yazdir(bool durum, string onay)
        {
            if (durum)
            {
                MessageBox.Show(onay, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox1.Text = "";
            }
            else
            {
                MessageBox.Show("Bir Hata Oluştu.", "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Kontrol.boslukKontrol(new string[] { textBox1.Text }))
            {
                MessageBox.Show("Tüm Alanları Doldurun", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (duzen)
                {
                    yazdir(db.islem("UPDATE `tbl_yakitturu` SET `Yakit_Turu` = @text WHERE `tbl_yakitturu`.`YakitTuruID` = @id;", new string[] { "@text", textBox1.Text, "@id", id })
                        , "Seçilen yakıt türü güncellendi");
                    button2_Click(e, e);


                }
                else
                {
                    yazdir(db.islem("INSERT INTO `tbl_yakitturu` (`YakitTuruID`, `Yakit_Turu`) VALUES (NULL, @yakit);", new string[] { "@yakit", textBox1.Text }), "Yakıt Türü Eklendi");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            duzen = false;
            button1.Text = "Ekle";
            textBox1.Text = "";
            button2.Visible = false;

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult sor = new DialogResult();
            sor = MessageBox.Show(listView1.SelectedItems[0].SubItems[1].Text + " silinsin mi?", "Silmeyi Onayla", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (sor == DialogResult.Yes)
            {
                if (!db.islem("DELETE FROM `tbl_yakitturu` WHERE `tbl_yakitturu`.`YakitTuruID` = @id", new string[] { "@id", listView1.SelectedItems[0].SubItems[0].Text }))
                {
                    MessageBox.Show("Yakıt Türü silinemedi!");
                }
                else
                {
                    refresh();
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            id = listView1.SelectedItems[0].SubItems[0].Text;
            duzen = true; button1.Text = "Düzenle"; textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
            button2.Visible = true;
        }
    }
}
