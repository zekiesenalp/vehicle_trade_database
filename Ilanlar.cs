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
    public partial class Ilanlar : Form
    {
        public Ilanlar()
        {
            InitializeComponent();
        }
        bool duzen = false;
        string id;
        private void Ilanlar_Load(object sender, EventArgs e)
        {

            refresh("SELECT * FROM tbl_ilan", new string[] { });
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;



            /// combo1 - araç
            /// 
            // SELECT ArabaID, Araba_Marka, Araba_Model FROM tbl_araba WHERE ArabaID NOT IN (SELECT Ilan_ArabaID FROM tbl_ilan)
            MySqlDataReader r;
            r = db.oku("SELECT ArabaID, Araba_Marka, Araba_Model FROM tbl_araba", new string[] { });
            while (r.Read())
            {
                comboBox1.Items.Add(r[0] + " " + r[1] + " " + r[2]);
                
            }
            db.baglanti.Close();
            /// sehir
            r = db.oku("SELECT Sehir FROM tbl_sehir", new string[] { });
            while (r.Read())
            {
                comboBox2.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            
            button2.Visible = false;

        }

        public void refresh(string q, string[] parametre)
        {
            listView1.Items.Clear();
            MySqlDataReader reader = db.oku(q, parametre),r;
            DataTable dt = new DataTable(); dt.Load(reader);
            foreach (DataRow row in dt.Rows)
            {
                ListViewItem ekle = new ListViewItem();
                ekle.Text = row["IlanID"].ToString();
                ekle.SubItems.Add(row["Ilan_Adi"].ToString());
                ekle.SubItems.Add(row["Ilan_Fiyat"].ToString());
                ekle.SubItems.Add(row["Ilan_Km"].ToString());
                ekle.SubItems.Add(row["Ilan_Tarih"].ToString());
                ekle.SubItems.Add(row["Ilan_ArabaID"].ToString());
                ekle.SubItems.Add(row["Ilan_SehirID"].ToString());
                listView1.Items.Add(ekle);
            }
            db.baglanti.Close();
            //reader.Close();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                db.baglanti.Open();
                listView1.Items[i].SubItems[6].Text = (db.idIleCek("tbl_sehir", "Sehir", "SehirID=" + listView1.Items[i].SubItems[6].Text));
                db.baglanti.Close();
                r = db.oku("SELECT ArabaID, Araba_Marka, Araba_Model FROM tbl_araba WHERE ArabaID=@arac_id", new string[] { "@arac_id", listView1.Items[i].SubItems[5].Text});
                while (r.Read())
                {
                    listView1.Items[i].SubItems[5].Text = (r[0] + " " + r[1] + " " + r[2]);

                }
                db.baglanti.Close();
            }
            

        }

        private void button1_Click(object sender, EventArgs e)
        {


            if (!Kontrol.boslukKontrol(new string[] { textBox1.Text, textBox2.Text, textBox3.Text }) && comboBox1.SelectedIndex > -1 && comboBox2.SelectedIndex > -1)
            {
                MessageBox.Show("Tüm Alanları Doldurun", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                db.baglanti.Open();
                string sec_sehir = db.idIleCek("tbl_sehir", "SehirID", "Sehir='" + comboBox2.SelectedItem + "'");
                string arac_id = comboBox1.Text.Split(' ')[0];
                db.baglanti.Close();
          string[] parametreler = { "@ad", textBox1.Text, "@fiyat", textBox2.Text, "@km", textBox3.Text, "@a_id", arac_id, "@s_id", sec_sehir };
                if (duzen)
                {


                yazdir(db.islem("UPDATE `tbl_ilan` SET `Ilan_Adi` = @ad, `Ilan_Fiyat` = @fiyat, `Ilan_Km` = @km, `Ilan_ArabaID` = @a_id, `Ilan_SehirID` = @s_id WHERE `tbl_ilan`.`IlanID` = "+id+";", parametreler)
                   , "Seçilen ilan güncellendi");
                   button2_Click(e, e);


                }
                else
                {
                    yazdir(db.islem("INSERT INTO `tbl_ilan` (`IlanID`, `Ilan_Adi`, `Ilan_Fiyat`, `Ilan_Km`, `Ilan_Tarih`, `Ilan_ArabaID`, `Ilan_SehirID`) VALUES (NULL, @ad, @fiyat, @km, CURRENT_TIMESTAMP, @a_id, @s_id);", parametreler), "İlan Eklendi");
                }

            }

        }

        public void yazdir(bool durum, string onay)
        {
            if (durum)
            {
                MessageBox.Show(onay, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                temizle();
            }
            else
            {
                MessageBox.Show("Bir Hata Oluştu. " + db.err, "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            refresh("SELECT * FROM tbl_ilan", new string[] { });
        }

        public void temizle()
        {

            textBox1.Text = ""; textBox2.Text = ""; textBox3.Text = ""; comboBox1.SelectedIndex = -1; comboBox2.SelectedIndex = -1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            duzen = false;
            button1.Text = "Ekle";
            temizle();
            button2.Visible = false;
            groupBox1.Text = "Ekle";
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult sor = new DialogResult();
            sor = MessageBox.Show(listView1.SelectedItems[0].SubItems[1].Text + " silinsin mi?", "Silmeyi Onayla", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (sor == DialogResult.Yes)
            {
                if (!db.islem("DELETE FROM `tbl_ilan` WHERE `tbl_ilan`.`IlanID` = @id", new string[] { "@id", listView1.SelectedItems[0].SubItems[0].Text }))
                {
                    MessageBox.Show("araç silinemedi!");
                }
                else
                {
                    refresh("SELECT * FROM tbl_ilan", new string[] { });
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            id = listView1.SelectedItems[0].SubItems[0].Text;
            duzen = true; button1.Text = "Düzenle";
            groupBox1.Text = "Düzenle";
            textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox2.Text = listView1.SelectedItems[0].SubItems[2].Text;
            textBox3.Text = listView1.SelectedItems[0].SubItems[3].Text;
            
            comboBox2.SelectedItem = listView1.SelectedItems[0].SubItems[6].Text;

            MySqlDataReader r;
            r = db.oku("SELECT ArabaID, Araba_Marka, Araba_Model FROM tbl_araba WHERE ArabaID = @id", new string[] {"@id", listView1.SelectedItems[0].SubItems[5].Text.Split(' ')[0] });
            while (r.Read())
            {
                comboBox1.SelectedItem = (r[0] + " " + r[1] + " " + r[2]);

            }
            db.baglanti.Close();

            button2.Visible = true;
        }
    }
}
