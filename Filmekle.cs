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
    public partial class Filmekle : Form
    {
        SqlConnection bag = new SqlConnection("Data Source=MEHMETT;Initial Catalog=SinemaOtomasyonu1;Integrated Security=True");
        public Filmekle()
        {
            InitializeComponent();
        }

        private void Filmekle_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(dateTimePicker1.Text) ||
                string.IsNullOrWhiteSpace(pictureBox2.ImageLocation))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı!");
                return;
            }

            bag.Open();

            // **Önce Film Adı var mı kontrol et**
            string kontrolSorgu = "SELECT COUNT(*) FROM Film_Bilgiler WHERE FilmAdi = @filmadi";
            SqlCommand kontrolKomut = new SqlCommand(kontrolSorgu, bag);
            kontrolKomut.Parameters.AddWithValue("@filmadi", textBox1.Text);

            int filmSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());

            if (filmSayisi > 0)
            {
                MessageBox.Show("Bu Film zaten kayıtlarda bulunuyor!", "Uyarı!");
            }
            else
            {
                try
                {
                    string filmekle = "INSERT INTO Film_Bilgiler (FilmAdi, Yonetmen, FilmTuru, FilmSuresi, tarih, YapimYili, Resim) VALUES (@filmadi, @yonetmen, @filmturu, @filmsuresi, @tarih, @yapimyili, @resim)";
                    SqlCommand komut = new SqlCommand(filmekle, bag);

                    komut.Parameters.AddWithValue("@filmadi", textBox1.Text);
                    komut.Parameters.AddWithValue("@yonetmen", textBox2.Text);
                    komut.Parameters.AddWithValue("@filmturu", comboBox1.Text);
                    komut.Parameters.AddWithValue("@filmsuresi", int.TryParse(textBox4.Text, out int sure) ? sure : 0);
                    komut.Parameters.AddWithValue("@yapimyili", int.TryParse(textBox5.Text, out int yil) ? yil : 0);
                    komut.Parameters.AddWithValue("@tarih", dateTimePicker1.Value); // Tarihi DateTime olarak al
                    komut.Parameters.AddWithValue("@resim", pictureBox2.ImageLocation ?? ""); // Eğer resim boşsa hata almamak için boş string ekle

                    komut.ExecuteNonQuery();
                    MessageBox.Show("Film başarıyla eklendi!", "Kayıt");

                    // Form temizleme
                    foreach (Control item in Controls)
                    {
                        if (item is TextBox) item.Text = "";
                    }
                    comboBox1.Text = "";
                    pictureBox2.Image = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata!");
                }
            }

            bag.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            pictureBox2.ImageLocation = openFileDialog1.FileName;
        }
    }
}
