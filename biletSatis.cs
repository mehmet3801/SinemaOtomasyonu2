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
using System.Globalization;

namespace SinemaOtomasyonu2
{
    public partial class biletSatis : Form
    {
        SqlConnection bag = new SqlConnection("Data Source=MEHMETT;Initial Catalog=SinemaOtomasyonu1;Integrated Security=True");
        public biletSatis()
        {
            InitializeComponent();
        }
        int sayac = 0;

        private void FilmveSalonGetir(ComboBox combo, string sql1, string sql2)
        {
            bag.Open();
            SqlCommand komut = new SqlCommand(sql1, bag);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                combo.Items.Add(read[sql2].ToString());

            }
            bag.Close();
        }

        private void combo_dolu_koltuklar()
        {
            comboBox9.Items.Clear();
            comboBox9.Text = "";

            foreach (Control item in groupBox3.Controls)
            {
                if (item is Button)
                {
                    if (item.BackColor == Color.Red)
                    {
                        comboBox9.Items.Add(item.Text);
                    }
                }
            }
        }
        private List<string> secilenKoltuklar = new List<string>(); // Seçilen koltukları tutacak liste

        private void Btn_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            // Koltuk zaten satılmışsa (kırmızı) işlem yapma
            if (b.BackColor == Color.Red)
            {
                MessageBox.Show("Bu koltuk satılmıştır!", "Uyarı");
                return;
            }

            // Ctrl tuşuna basılıysa çoklu seçim yap
            if (Control.ModifierKeys == Keys.Control)
            {
                if (b.BackColor == Color.Yellow) // Seçiliyse iptal et
                {
                    b.BackColor = Color.White;
                    secilenKoltuklar.Remove(b.Text);
                }
                else // Seçili değilse ekle
                {
                    b.BackColor = Color.Yellow;
                    secilenKoltuklar.Add(b.Text);
                }
            }
            else // Tekli seçim
            {
                // Önceki seçimleri temizle
                foreach (Control item in groupBox3.Controls)
                {
                    if (item is Button btn && btn.BackColor == Color.Yellow)
                    {
                        btn.BackColor = Color.White;
                    }
                }
                secilenKoltuklar.Clear();

                // Yeni seçimi yap
                b.BackColor = Color.Yellow;
                secilenKoltuklar.Add(b.Text);
            }

            // Seçilen koltukları TextBox'ta göster (opsiyonel)
            textBox1.Text = string.Join(", ", secilenKoltuklar);

            ListBoxGuncelle();
        }

        private void ListBoxGuncelle()
        {
            listBox1.Items.Clear();

            // Tüm koltukları tek bir string olarak birleştir
            string tumKoltuklar = string.Join(", ", secilenKoltuklar);

            // ListBox'a tek öğe olarak ekle
            if (!string.IsNullOrEmpty(tumKoltuklar))
            {
                listBox1.Items.Add(tumKoltuklar);
            }
        }
        private void vt_dolu_koltuklar()
        {
            try
            {
                // Eksik seçim varsa işlem yapma
                if (string.IsNullOrEmpty(comboBox1.Text) ||
                    string.IsNullOrEmpty(comboBox2.Text) ||
                    comboBox3.SelectedItem == null ||
                    comboBox4.SelectedItem == null)
                {
                    return;
                }

                bag.Open();
                SqlCommand komut = new SqlCommand(
                    "SELECT koltukno FROM satis_bilgileri " +
                    "WHERE filmadi=@filmadi AND salonadi=@salonadi " +
                    "AND tarih=@tarih AND saat=@saat", bag);

                komut.Parameters.AddWithValue("@filmadi", comboBox1.Text);
                komut.Parameters.AddWithValue("@salonadi", comboBox2.Text);
                komut.Parameters.AddWithValue("@tarih", comboBox3.SelectedItem.ToString());
                komut.Parameters.AddWithValue("@saat", comboBox4.SelectedItem.ToString());

                SqlDataReader reader = komut.ExecuteReader();

                // Tüm koltukları beyaz yap (boş)
                foreach (Control item in groupBox3.Controls)
                {
                    if (item is Button btn)
                        btn.BackColor = Color.White;
                }

                // Dolu koltukları kırmızı yap
                while (reader.Read())
                {
                    string doluKoltuk = reader["koltukno"].ToString();
                    foreach (Control item in groupBox3.Controls)
                    {
                        if (item is Button btn && btn.Text == doluKoltuk)
                        {
                            btn.BackColor = Color.Red;
                            break;
                        }
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                if (bag.State == ConnectionState.Open)
                    bag.Close();
            }
        }





        private void yenidenRenklendir()
        {
            foreach (Control item in groupBox3.Controls)
            {
                if (item is Button)
                {
                    item.BackColor = Color.White;
                }
            }
        }
        private void FilmAfisiGoster()
        {
            bag.Open();
            SqlCommand komut = new SqlCommand("select * from film_bilgiler where filmadi='" + comboBox1.SelectedItem + "'", bag);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                pictureBox1.ImageLocation = read["resim"].ToString();
            }
            bag.Close();
        }
        private void biletSatis_Load(object sender, EventArgs e)
        {
            Bos_Koltuklar();
            FilmveSalonGetir(comboBox1, "select*from film_bilgiler", "filmadi");
            FilmveSalonGetir(comboBox2, "select*from salon_bilgileri", "salonadi");

        }

        private void Bos_Koltuklar()
        {
            sayac = 1;
            groupBox3.Controls.Clear();  // **Eski butonları temizle**

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(30, 30);
                    btn.BackColor = Color.White;
                    btn.Location = new Point(j * 30 + 20, i * 30 + 30);
                    btn.Name = sayac.ToString();
                    btn.Text = sayac.ToString();

                    if (j == 4)
                    {
                        continue;
                    }
                    sayac++;

                    this.groupBox3.Controls.Add(btn);
                    btn.Click += Btn_Click;
                }
            }

            vt_dolu_koltuklar();
        }




        private void button1_Click(object sender, EventArgs e)
        {
            if (secilenKoltuklar.Count == 0)
            {
                MessageBox.Show("Lütfen en az bir koltuk seçin!", "Uyarı");
                return;
            }

            try
            {
                bag.Open();

                // Tüm satışlar için ortak parametreleri bir kez tanımla
                string filmAdi = comboBox1.Text;
                string salonAdi = comboBox2.Text;
                string tarih = comboBox3.Text;
                string saat = comboBox4.Text;
                string ad = textBox2.Text;
                string soyad = textBox3.Text;
                string ucret = comboBox8.Text;
                DateTime satisTarihi = DateTime.Now;

                int basariliSatisSayisi = 0;
                List<string> satilmisKoltuklar = new List<string>();

                foreach (string koltukNo in secilenKoltuklar)
                {
                    try
                    {
                        // Koltuk daha önce satılmış mı kontrol et
                        using (SqlCommand kontrolKomut = new SqlCommand(
                            "SELECT COUNT(*) FROM Satis_Bilgileri " +
                            "WHERE KoltukNo=@koltukno AND FilmAdi=@filmadi " +
                            "AND SalonAdi=@salonadi AND Tarih=@tarih AND Saat=@saat", bag))
                        {
                            kontrolKomut.Parameters.AddWithValue("@koltukno", koltukNo);
                            kontrolKomut.Parameters.AddWithValue("@filmadi", filmAdi);
                            kontrolKomut.Parameters.AddWithValue("@salonadi", salonAdi);
                            kontrolKomut.Parameters.AddWithValue("@tarih", tarih);
                            kontrolKomut.Parameters.AddWithValue("@saat", saat);

                            if ((int)kontrolKomut.ExecuteScalar() > 0)
                            {
                                satilmisKoltuklar.Add(koltukNo);
                                continue;
                            }
                        }

                        // Satışı kaydet
                        using (SqlCommand satisKomut = new SqlCommand(
                            "INSERT INTO Satis_Bilgileri " +
                            "(KoltukNo, FilmAdi, SalonAdi, Tarih, Saat, Ad, Soyad, Ucret, SatisTarihi) " +
                            "VALUES (@koltukno, @filmadi, @salonadi, @tarih, @saat, @ad, @soyad, @ucret, @satistarihi)", bag))
                        {
                            satisKomut.Parameters.AddWithValue("@koltukno", koltukNo);
                            satisKomut.Parameters.AddWithValue("@filmadi", filmAdi);
                            satisKomut.Parameters.AddWithValue("@salonadi", salonAdi);
                            satisKomut.Parameters.AddWithValue("@tarih", tarih);
                            satisKomut.Parameters.AddWithValue("@saat", saat);
                            satisKomut.Parameters.AddWithValue("@ad", ad);
                            satisKomut.Parameters.AddWithValue("@soyad", soyad);
                            satisKomut.Parameters.AddWithValue("@ucret", ucret);
                            satisKomut.Parameters.AddWithValue("@satistarihi", satisTarihi);

                            satisKomut.ExecuteNonQuery();
                            basariliSatisSayisi++;

                            // Koltuk rengini kırmızı yap
                            foreach (Control item in groupBox3.Controls)
                            {
                                if (item is Button btn && btn.Text == koltukNo)
                                {
                                    btn.BackColor = Color.Red;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Tek bir koltuk satışında hata olursa diğerlerini etkilemesin
                        MessageBox.Show($"{koltukNo} numaralı koltuk satışında hata: {ex.Message}", "Hata");
                    }
                }

                // Sonuç bildirimi
                string mesaj = $"{basariliSatisSayisi} koltuk satışı başarılı!";
                if (satilmisKoltuklar.Count > 0)
                {
                    mesaj += $"\n\nŞu koltuklar zaten satılmış: {string.Join(", ", satilmisKoltuklar)}";
                }
                MessageBox.Show(mesaj, "Satış Sonucu");

                // Temizlik işlemleri
                secilenKoltuklar.Clear();
                textBox1.Text = "";
                listBox1.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Genel satış hatası: " + ex.Message, "Hata!");
            }
            finally
            {
                if (bag.State == ConnectionState.Open)
                    bag.Close();
            }
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Text = "";
            comboBox3.Text = "";
            foreach (Control item in groupBox3.Controls) if (item is TextBox) item.Text = "";
            FilmAfisiGoster();
            yenidenRenklendir();
            combo_dolu_koltuklar(); // Dolu koltukları güncelle
            vt_dolu_koltuklar(); // Satılan koltukları güncelle
        }



        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Film_Tarihi_getir()
        {
            comboBox3.Items.Clear();
            comboBox3.Text = "";

            bag.Open();
            SqlCommand komut = new SqlCommand("select * from seans_bilgileri where filmadi=@filmadi and salonadi=@salonadi", bag);
            komut.Parameters.AddWithValue("@filmadi", comboBox1.SelectedItem.ToString());
            komut.Parameters.AddWithValue("@salonadi", comboBox2.SelectedItem.ToString());
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                if (DateTime.Parse(read["tarih"].ToString()) >= DateTime.Now.Date)
                {
                    if (!comboBox3.Items.Contains(read["tarih"].ToString()))
                    {
                        comboBox3.Items.Add(read["tarih"].ToString());
                    }
                }
            }
            bag.Close();

            // Seçili tarih varsa, vt_dolu_koltuklar'ı çağır
            if (comboBox3.SelectedItem != null)
            {
                vt_dolu_koltuklar();
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Film_Tarihi_getir();
        }
        private void Film_Seansi_Getir()
        {
            comboBox4.Text = "";
            comboBox4.Items.Clear();

            bag.Open();
            SqlCommand komut = new SqlCommand("SELECT * FROM seans_bilgileri WHERE filmadi = @filmadi AND salonadi = @salonadi AND tarih = @tarih", bag);
            komut.Parameters.AddWithValue("@filmadi", comboBox1.SelectedItem.ToString());
            komut.Parameters.AddWithValue("@salonadi", comboBox2.SelectedItem.ToString());
            komut.Parameters.AddWithValue("@tarih", Convert.ToDateTime(comboBox3.SelectedItem));
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                if (DateTime.Parse(read["tarih"].ToString()) == DateTime.Parse(DateTime.Now.ToShortDateString()))
                {

                    if (DateTime.Parse(read["seans"].ToString()) > DateTime.Parse(DateTime.Now.ToShortTimeString()))
                    {
                        comboBox4.Items.Add(read["seans"].ToString());
                    }
                }
                else if (DateTime.Parse(read["tarih"].ToString()) > DateTime.Parse(DateTime.Now.ToShortDateString()))
                {
                    comboBox4.Items.Add(read["seans"].ToString());
                }

            }
            bag.Close();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Film_Seansi_Getir();
            if (comboBox4.SelectedItem != null) // Seçili seans varsa
            {
                vt_dolu_koltuklar();
            }
            combo_dolu_koltuklar();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            vt_dolu_koltuklar(); // Satılmış koltukları güncelle
            combo_dolu_koltuklar();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlCommand veriSil = new SqlCommand("DELETE FROM Satis_Bilgileri WHERE KoltukNo=@koltukno AND Filmadi=@filmadi AND Salonadi=@salonadi AND Tarih=@tarih AND Saat=@saat", bag);
            bag.Open();
            veriSil.Parameters.AddWithValue("@koltukno", comboBox9.Text);
            veriSil.Parameters.AddWithValue("@filmadi", comboBox1.Text);
            veriSil.Parameters.AddWithValue("@salonadi", comboBox2.Text);
            veriSil.Parameters.AddWithValue("@tarih", comboBox3.Text);
            veriSil.Parameters.AddWithValue("@saat", comboBox4.Text);
            veriSil.ExecuteNonQuery();
            bag.Close();
            MessageBox.Show("Bilet İptal Edildi", "İşlem Başarılı");
            // Seçili koltuğun rengini beyaz yap
            foreach (Control item in groupBox3.Controls)
            {
                if (item is Button btn && btn.Text == comboBox9.Text)
                {
                    btn.BackColor = Color.White;
                    break;
                }
            }
            // Seçili koltuk listesini güncelle
            secilenKoltuklar.Remove(comboBox9.Text);
            textBox1.Text = string.Join(", ", secilenKoltuklar);
            listBox1.Items.Clear();
            ListBoxGuncelle();
            // Dolu koltukları güncelle
            vt_dolu_koltuklar();
            combo_dolu_koltuklar();

        }
    }
}