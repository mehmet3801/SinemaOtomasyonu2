using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SinemaOtomasyonu2
{
    public partial class salonekle : Form
    {
        SqlConnection bag = new SqlConnection("Data Source=MEHMETT;Initial Catalog=SinemaOtomasyonu1;Integrated Security=True");
        public salonekle()
        {
            InitializeComponent();
        }

        private void salonekle_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Lütfen Salon Adı Giriniz!", "Uyarı!");
                return;
            }

            bag.Open();

            // Önce aynı isimde salon var mı kontrol et
            string kontrolSorgu = "SELECT COUNT(*) FROM Salon_Bilgileri WHERE SalonAdi = @salonadi";
            SqlCommand kontrolKomut = new SqlCommand(kontrolSorgu, bag);
            kontrolKomut.Parameters.AddWithValue("@salonadi", textBox1.Text);

            int salonSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());

            if (salonSayisi > 0)
            {
                MessageBox.Show("Bu salon zaten eklenmiş!", "Uyarı!");
            }
            else
            {
                try
                {
                    // Yeni salonu ekle
                    string salonekle = "INSERT INTO Salon_Bilgileri(SalonAdi) VALUES (@salonadi)";
                    SqlCommand komut = new SqlCommand(salonekle, bag);
                    komut.Parameters.AddWithValue("@salonadi", textBox1.Text);
                    komut.ExecuteNonQuery();

                    MessageBox.Show("Salon başarıyla eklendi!", "Kayıt");
                    textBox1.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata!");
                }
            }

            bag.Close();
        }
    }
}
