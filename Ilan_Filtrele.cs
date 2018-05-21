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
    public partial class Ilan_Filtrele : Form
    {
        public Ilan_Filtrele()
        {
            InitializeComponent();
        }

        public void ara()
        {
            string sorgu = "SELECT * FROM tbl_ilan ";
            List<string> ilan_kelimeler = new List<string>();

            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                ilan_kelimeler.Add("Ilan_Adi LIKE '%" + textBox1.Text + "%'");
            }
            if (Kontrol.boslukKontrol(new string[] { textBox2.Text, textBox3.Text }))
            {
                ilan_kelimeler.Add("Ilan_Fiyat BETWEEN " + textBox2.Text + " AND " + textBox3.Text);
            }
            if (!String.IsNullOrEmpty(textBox4.Text))
            {
                ilan_kelimeler.Add("Ilan_Km = " + textBox4.Text);
            }
            if (comboBox1.SelectedIndex > -1)
            {
                db.baglanti.Open();
                ilan_kelimeler.Add("Ilan_SehirID = " + db.idIleCek("tbl_sehir", "SehirID", "Sehir='" + comboBox1.SelectedItem + "'"));
                db.baglanti.Close();
            }
            if(comboBox2.SelectedIndex > -1)
            {

                string deger = comboBox2.SelectedItem.ToString();
                if(deger == "Bugün")
                {
                    ilan_kelimeler.Add("Ilan_Tarih >= DATE_SUB(CURDATE(), INTERVAL 1 DAY)");
                }else if(deger == "1 Hafta")
                {
                    ilan_kelimeler.Add("Ilan_Tarih >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)");
                }
                else if(deger == "1 Ay")
                {
                    ilan_kelimeler.Add("Ilan_Tarih >= DATE_SUB(CURDATE(), INTERVAL 1 MONTH)");
                }
                else if(deger == "1 Yıl")
                {
                    ilan_kelimeler.Add("Ilan_Tarih >= DATE_SUB(CURDATE(), INTERVAL 1 YEAR)");
                }
               
            }
            List<string> araba_kelimeler = new List<string>();
            if (!String.IsNullOrEmpty(textBox6.Text))
            {
                araba_kelimeler.Add("Araba_Marka LIKE '%" + textBox6.Text + "%'");
            }
            if (!String.IsNullOrEmpty(textBox5.Text))
            {
                araba_kelimeler.Add("Araba_Model LIKE '%" + textBox5.Text + "%'");
            }
            if (comboBox6.SelectedIndex > -1)
            {
                db.baglanti.Open();
                araba_kelimeler.Add("Araba_VitesTuruID = " + db.idIleCek("tbl_vitesturu", "VitesTuruID", "Vites_Turu='" + comboBox6.SelectedItem + "'"));
                db.baglanti.Close();
            }
            if (comboBox5.SelectedIndex > -1)
            {
                db.baglanti.Open();
                araba_kelimeler.Add("Araba_YakitTuruID = " + db.idIleCek("tbl_yakitturu", "YakitTuruID", "Yakit_Turu='" + comboBox5.SelectedItem + "'"));
                db.baglanti.Close();

            }
            if (comboBox4.SelectedIndex > -1)
            {
                db.baglanti.Open();
                araba_kelimeler.Add("Araba_RenkID = " + db.idIleCek("tbl_renk", "RenkID", "Renk='" + comboBox4.SelectedItem + "'"));
                db.baglanti.Close();
            }
            string ilan_aranacak = string.Join(" AND ", ilan_kelimeler.ToArray());
            string araba_aranacak = string.Join(" AND ", araba_kelimeler.ToArray());
            if (!string.IsNullOrEmpty(ilan_aranacak) && !string.IsNullOrEmpty(araba_aranacak))
            {
                sorgu += "WHERE " + ilan_aranacak + " AND Ilan_ArabaID IN (SELECT ArabaID FROM tbl_araba WHERE " + araba_aranacak + ")";
            } else if (!string.IsNullOrEmpty(ilan_aranacak) && string.IsNullOrEmpty(araba_aranacak))
            {
                sorgu += "WHERE " + ilan_aranacak;
            } else if (!string.IsNullOrEmpty(araba_aranacak) && string.IsNullOrEmpty(ilan_aranacak))
            {
                sorgu += "WHERE Ilan_ArabaID IN (SELECT ArabaID FROM tbl_araba WHERE " + araba_aranacak + ")";
            }


            if (comboBox7.SelectedIndex > -1 && comboBox8.SelectedIndex > -1)
            {
                string siralama = " ORDER BY ";
                string k = comboBox7.SelectedItem.ToString();
                if (k == "Ad")
                {
                    siralama += "Ilan_Adi ";
                }
                else if (k == "Fiyat")
                {
                    siralama += "Ilan_Fiyat ";
                }
                else if (k == "Km")
                {
                    siralama += "Ilan_Km ";
                }
                else if (k == "Tarih")
                {
                    siralama += "Ilan_Tarih ";
                }


                if (comboBox8.SelectedItem.ToString() == "Artan")
                {
                    siralama += "ASC";
                }
                else if (comboBox8.SelectedItem.ToString() == "Azalan")
                {
                    siralama += "DESC";
                }
                sorgu += " " + siralama;
            }

            refresh(sorgu, new string[] { });
        }

        private void Ilan_Filtrele_Load(object sender, EventArgs e)
        {
            refresh("SELECT * FROM tbl_ilan", new string[] { });
            textBox2.Text = "0"; textBox3.Text = "999999";
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; comboBox6.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList; comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox7.DropDownStyle = ComboBoxStyle.DropDownList; comboBox8.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            MySqlDataReader r;
            r = db.oku("SELECT Sehir FROM tbl_sehir", new string[] { });
            while (r.Read())
            {
                comboBox1.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            /// combo1 - vites
            /// 
            
            r = db.oku("SELECT Vites_Turu FROM tbl_vitesturu", new string[] { });
            while (r.Read())
            {
                
                comboBox6.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            /// yakit
            r = db.oku("SELECT Yakit_Turu FROM tbl_yakitturu", new string[] { });
            while (r.Read())
            {
                
                comboBox5.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            /// renk
            r = db.oku("SELECT Renk FROM tbl_renk", new string[] { });
            while (r.Read())
            {
               
                comboBox4.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
           


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
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                db.baglanti.Open();
                listView1.Items[i].SubItems[6].Text = (db.idIleCek("tbl_sehir", "Sehir", "SehirID=" + listView1.Items[i].SubItems[6].Text));
                db.baglanti.Close();
                r = db.oku("SELECT ArabaID, Araba_Marka, Araba_Model FROM tbl_araba WHERE ArabaID=@arac_id", new string[] { "@arac_id", listView1.Items[i].SubItems[5].Text });
                while (r.Read())
                {
                    listView1.Items[i].SubItems[5].Text = (r[0] + " " + r[1] + " " + r[2]);

                }
                db.baglanti.Close();
            }
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            ara();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refresh("SELECT * FROM tbl_ilan", new string[] { });
            textBox1.Clear(); textBox2.Clear(); textBox3.Clear();
            textBox4.Clear(); textBox5.Clear(); textBox6.Clear();
            comboBox1.SelectedIndex = -1; comboBox2.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1; comboBox6.SelectedIndex = -1;
            comboBox7.SelectedIndex = -1; comboBox8.SelectedIndex = -1;
        }
        string yakit_id, vites_id, renk_id;
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            label12.Text = "İlan Adı: " + listView1.SelectedItems[0].SubItems[1].Text;
            label15.Text = "İlan Fiyatı: " + listView1.SelectedItems[0].SubItems[2].Text;
            label13.Text = "Km: " + listView1.SelectedItems[0].SubItems[3].Text;
            label14.Text = "Tarih: " + listView1.SelectedItems[0].SubItems[4].Text;
            label16.Text = "Şehir: " + listView1.SelectedItems[0].SubItems[6].Text;
            string id = listView1.SelectedItems[0].SubItems[5].Text.Split(' ')[0];
            
            MySqlDataReader reader = db.oku("SELECT * FROM tbl_araba WHERE ArabaID = @id", new string[] { "@id", id });
            DataTable dt = new DataTable(); dt.Load(reader);
            foreach (DataRow row in dt.Rows)
            {
                label17.Text = "Marka: " + row[1];
                label18.Text = "Model: " + row[2];
                yakit_id = row[4].ToString(); vites_id = row[3].ToString(); renk_id = row[5].ToString(); 
            }
            
                label19.Text = "Vites Türü: " + db.idIleCek("tbl_vitesturu", "Vites_Turu", "VitesTuruID=" + vites_id);
                label20.Text = "Yakıt: " + db.idIleCek("tbl_yakitturu", "Yakit_Turu", "YakitTuruID=" + yakit_id);
                label21.Text = "Renk: " + db.idIleCek("tbl_renk", "Renk", "RenkID=" + renk_id);
            
            db.baglanti.Close();
        }
    }
}
