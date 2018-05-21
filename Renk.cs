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
    public partial class Renk : Form
    {
        public Renk()
        {
            InitializeComponent();
        }
        bool duzen = false;
        string id;
        private void Renk_Load(object sender, EventArgs e)
        {
            refresh(); button2.Visible = false;
        }

        public void refresh()
        {
            listView1.Items.Clear();
            MySqlDataReader reader = db.oku("SELECT * FROM `tbl_renk`", new string[] { });
            while (reader.Read())
            {
                ListViewItem ekle = new ListViewItem();
                ekle.Text = reader["RenkID"].ToString();
                ekle.SubItems.Add(reader["Renk"].ToString());
                listView1.Items.Add(ekle);
            }
            db.baglanti.Close();
        }
        public void yazdir(bool durum, string onay)
        {
            if (durum)
            {
                MessageBox.Show(onay,"Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
                textBox1.Text = "";
            }
            else
            {
                MessageBox.Show("Bir Hata Oluştu.","Hata!",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                    yazdir(db.islem("UPDATE `tbl_renk` SET `Renk` = @text WHERE `tbl_renk`.`RenkID` = @id;", new string[] { "@text", textBox1.Text, "@id", id })
                        , "Seçilen renk güncellendi");
                    button2_Click(e, e);


                }
                else
                {
                    yazdir(db.islem("INSERT INTO `tbl_renk` (`RenkID`, `Renk`) VALUES (NULL, @renk);", new string[] { "@renk", textBox1.Text }), "Renk Eklendi");
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
                if (!db.islem("DELETE FROM `tbl_renk` WHERE `tbl_renk`.`RenkID` = @id", new string[] { "@id", listView1.SelectedItems[0].SubItems[0].Text }))
                {
                    MessageBox.Show("Renk silinemedi!");
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
