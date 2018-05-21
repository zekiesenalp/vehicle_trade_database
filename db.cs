using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace AracAlisSatis
{
    class db
    {

        public static MySqlConnection baglanti = null;

        private static MySqlDataAdapter da;
        private static MySqlCommand command;
        private static MySqlCommandBuilder cmb;
        private static MySqlDataReader reader;
        private static DataTable dt;
        public static string err;
        private string query;

        public static void baglantiKontrol()
        {
            if (baglanti == null) baglanti = new MySqlConnection("Server=localhost;Database=aracsatis;Uid=root;Pwd='';Encrypt=false;AllowUserVariables=True;UseCompression=True");

            //        if (baglanti.State == ConnectionState.Closed) baglanti.Open();

        }

        public db()
        {
            baglantiKontrol();
        }

        public static bool islem(string q, string[] parametre)
        {
            parametreEkle(q, parametre);
            try
            {
                command.ExecuteNonQuery();
                baglanti.Close();
                return true;
            }
            catch (MySqlException e)
            {
                err = e.Message;
                baglanti.Close();
                return false;
            }
        }

        public static MySqlDataReader oku(string q, string[] parametre)
        {

            parametreEkle(q, parametre);
            try
            {
                reader = command.ExecuteReader();
            }
            catch (MySqlException e)
            {
                err = e.Message;
            }
            return reader;
        }

        private static void parametreEkle(string q, string[] parametre)
        {
            baglantiKontrol();
            baglanti.Open();
            command = new MySqlCommand(q, baglanti);

            for (int i = 0; i < parametre.Length; i += 2)
            {
                command.Parameters.AddWithValue(parametre[i], parametre[i + 1]);
            }
        }

        public static string tekli(string q, string[] parametre)
        {
            parametreEkle(q, parametre);
            try
            {
                string data = (command.ExecuteScalar() ?? string.Empty).ToString();

                baglanti.Close();
                return data;
            }
            catch (MySqlException e)
            {
                
                err = e.Message;
                baglanti.Close();
                return err;             
            }
           
        }

        public static string idIleCek(string tablo, string kolon, string id)
        {
            baglantiKontrol();
            //baglanti.Open();
            command = new MySqlCommand("SELECT "+kolon+" FROM `"+tablo+"` WHERE "+id, baglanti);

            try
            {
                string data = (command.ExecuteScalar() ?? string.Empty).ToString();
                return data;
            }
            catch (MySqlException e)
            {

                err = e.Message;
                return err;
            }

        }

    }
}
