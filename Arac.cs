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
using System.IO;
namespace AracAlisSatis
{
    public partial class Arac : Form
    {
        public Arac()
        {
            InitializeComponent();
        }
        bool duzen = false;
        string id;
        private void Arac_Load(object sender, EventArgs e)
        {
            refresh("SELECT * FROM tbl_araba", new string[] { });
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; comboBox6.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList; comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList; comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox7.DropDownStyle = ComboBoxStyle.DropDownList; comboBox8.DropDownStyle = ComboBoxStyle.DropDownList;

            /// combo1 - vites
            /// 
            MySqlDataReader r;
            r = db.oku("SELECT Vites_Turu FROM tbl_vitesturu", new string[] { });
            while (r.Read())
            {
                comboBox1.Items.Add(r[0].ToString());
                comboBox6.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            /// yakit
            r = db.oku("SELECT Yakit_Turu FROM tbl_yakitturu", new string[] { });
            while (r.Read())
            {
                comboBox2.Items.Add(r[0].ToString());
                comboBox5.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            /// renk
            r = db.oku("SELECT Renk FROM tbl_renk", new string[] { });
            while (r.Read())
            {
                comboBox3.Items.Add(r[0].ToString());
                comboBox4.Items.Add(r[0].ToString());
            }
            db.baglanti.Close();
            button2.Visible = false;
        }

        public void refresh(string q, string[] parametre)
        {
            listView1.Items.Clear();
            MySqlDataReader reader = db.oku(q,parametre);
            DataTable dt = new DataTable(); dt.Load(reader);
                foreach(DataRow row in dt.Rows)
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = row["ArabaID"].ToString();
                    ekle.SubItems.Add(row["Araba_Marka"].ToString());
                    ekle.SubItems.Add(row["Araba_Model"].ToString());
                    ekle.SubItems.Add(row["Araba_VitesTuruID"].ToString());
                    ekle.SubItems.Add(row["Araba_YakitTuruID"].ToString());
                    ekle.SubItems.Add(row["Araba_RenkID"].ToString());
                    listView1.Items.Add(ekle);
                }
                //reader.Close();
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    listView1.Items[i].SubItems[3].Text = (db.idIleCek("tbl_vitesturu", "Vites_Turu", "VitesTuruID=" + listView1.Items[i].SubItems[3].Text));
                    listView1.Items[i].SubItems[4].Text = (db.idIleCek("tbl_yakitturu", "Yakit_Turu", "YakitTuruID=" + listView1.Items[i].SubItems[4].Text));
                    listView1.Items[i].SubItems[5].Text = (db.idIleCek("tbl_renk", "Renk", "RenkID=" + listView1.Items[i].SubItems[5].Text));
                }
                db.baglanti.Close();
            
        }

      

        private void button1_Click(object sender, EventArgs e)
        {

            if (!Kontrol.boslukKontrol(new string[] { textBox1.Text, textBox2.Text }) && comboBox1.SelectedIndex > -1 && comboBox2.SelectedIndex > -1 && comboBox3.SelectedIndex > -1)
            {
                MessageBox.Show("Tüm Alanları Doldurun", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                db.baglanti.Open();
                string sec_yakit = db.idIleCek("tbl_yakitturu", "YakitTuruID", "Yakit_Turu='" + comboBox2.SelectedItem + "'");
                string sec_vites = db.idIleCek("tbl_vitesturu", "VitesTuruID", "Vites_Turu='" + comboBox1.SelectedItem + "'");
                string sec_renk = db.idIleCek("tbl_renk", "RenkID", "Renk='" + comboBox3.SelectedItem + "'");
                db.baglanti.Close();
                string[] parametreler = { "@marka", textBox1.Text, "@model", textBox2.Text, "@vitesid", sec_vites, "@yakitid", sec_yakit, "@renkid", sec_renk };
                if (duzen)
                {


                    yazdir(db.islem("UPDATE `tbl_araba` SET `Araba_Marka` = @marka, `Araba_Model` = @model, `Araba_YakitTuruID` = @yakitid, `Araba_VitesTuruID` = @vitesid, `Araba_RenkID` = @renkid WHERE `tbl_araba`.`ArabaID` = " + id + ";", parametreler)
                           , "Seçilen araç güncellendi");
                    button2_Click(e, e);


                }
                else
                {
                    yazdir(db.islem("INSERT INTO `tbl_araba` (`ArabaID`, `Araba_Marka`, `Araba_Model`, `Araba_VitesTuruID`, `Araba_YakitTuruID`, `Araba_RenkID`) VALUES (NULL, @marka, @model, @vitesid, @yakitid, @renkid);", parametreler), "Araba Eklendi");
                }

            }
        }
        
        public void temizle()
        {

            textBox1.Text = ""; textBox2.Text = ""; comboBox1.SelectedIndex = -1; comboBox2.SelectedIndex = -1; comboBox3.SelectedIndex = -1;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            duzen = false;
            button1.Text = "Ekle"; groupBox1.Text = "Ekle";
            temizle();
            button2.Visible = false;
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
            refresh("SELECT * FROM tbl_araba", new string[] { });
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult sor = new DialogResult();
            sor = MessageBox.Show(listView1.SelectedItems[0].SubItems[1].Text + " silinsin mi?", "Silmeyi Onayla", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (sor == DialogResult.Yes)
            {
                if (!db.islem("DELETE FROM `tbl_araba` WHERE `tbl_araba`.`ArabaID` = @id", new string[] { "@id", listView1.SelectedItems[0].SubItems[0].Text }))
                {
                    MessageBox.Show("Araba silinemedi!");
                }
                else
                {
                    refresh("SELECT * FROM tbl_araba", new string[] { });
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            id = listView1.SelectedItems[0].SubItems[0].Text;
            duzen = true; button1.Text = "Düzenle"; groupBox1.Text = "Düzenle";
            textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox2.Text = listView1.SelectedItems[0].SubItems[2].Text;
            comboBox1.SelectedItem = listView1.SelectedItems[0].SubItems[3].Text;
            comboBox2.SelectedItem = listView1.SelectedItems[0].SubItems[4].Text;
            comboBox3.SelectedItem = listView1.SelectedItems[0].SubItems[5].Text;
            button2.Visible = true;
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox7.SelectedIndex > -1) arama();
        }

        public void arama()
        {
            List<string> kelimeler = new List<string>();
            if (!String.IsNullOrEmpty(textBox4.Text))
            {
                kelimeler.Add("Araba_Marka LIKE '%" + textBox4.Text + "%'");
            }
            if (!String.IsNullOrEmpty(textBox3.Text))
            {
                kelimeler.Add("Araba_Model LIKE '%" + textBox3.Text + "%'");
            }
            if (comboBox6.SelectedIndex > -1)
            {
                db.baglanti.Open();
                kelimeler.Add("Araba_VitesTuruID = " + db.idIleCek("tbl_vitesturu", "VitesTuruID", "Vites_Turu='" + comboBox6.SelectedItem + "'"));
                db.baglanti.Close();
            }
            if (comboBox5.SelectedIndex > -1)
            {
                db.baglanti.Open();
                kelimeler.Add("Araba_YakitTuruID = " + db.idIleCek("tbl_yakitturu", "YakitTuruID", "Yakit_Turu='" + comboBox5.SelectedItem + "'"));
                db.baglanti.Close();

            }
            if (comboBox4.SelectedIndex > -1)
            {
                db.baglanti.Open();
                kelimeler.Add("Araba_RenkID = " + db.idIleCek("tbl_renk", "RenkID", "Renk='" + comboBox4.SelectedItem + "'"));
                db.baglanti.Close();
            }
            string aranacak = string.Join(" AND ", kelimeler.ToArray());
           
            if(comboBox7.SelectedIndex > -1 && comboBox8.SelectedIndex > -1)
            {
                string siralama = " ORDER BY ";
                string k = comboBox7.SelectedItem.ToString();
                if(k == "Model")
                {
                    siralama += "Araba_Model ";
                }else if(k == "Marka")
                {
                    siralama += "Araba_Marka ";
                }else if(k == "Vites")
                {
                    siralama += "Araba_VitesTuruID ";
                }else if(k == "Yakıt")
                {
                    siralama += "Araba_YakitTuruID ";
                }else if(k == "Renk")
                {
                    siralama += "Araba_  RenkID "; 
                }

                if(comboBox8.SelectedItem.ToString() == "Artan")
                {
                    siralama += "ASC";
                }else if(comboBox8.SelectedItem.ToString() == "Azalan")
                {
                    siralama += "DESC";
                }
                aranacak += siralama;
            }
            if (String.IsNullOrEmpty(aranacak))
            {
                refresh("SELECT * FROM tbl_araba", new string[] { });
            }
            else
            {
                if (aranacak.Contains("=") || aranacak.Contains("%"))
                {
                    refresh("SELECT * FROM tbl_araba WHERE " + aranacak, new string[] { });
                }else
                {
                    refresh("SELECT * FROM tbl_araba " + aranacak, new string[] { });
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            arama();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            arama();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            arama();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            arama();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            arama();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox8.SelectedIndex > -1) arama();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            refresh("SELECT * FROM tbl_araba", new string[] { });
            textBox4.Clear(); textBox3.Clear(); comboBox4.SelectedIndex = -1; comboBox5.SelectedIndex = -1; comboBox6.SelectedIndex = -1;
            comboBox7.SelectedIndex = -1; comboBox8.SelectedIndex = -1;
        }
    }
}
