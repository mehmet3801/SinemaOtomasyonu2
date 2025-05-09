using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using Org.BouncyCastle.Crypto;
namespace SinemaOtomasyonu2
{
    public partial class Satisliste : Form
    {
        SqlConnection bag = new SqlConnection("Data Source=MEHMETT;Initial Catalog=SinemaOtomasyonu1;Integrated Security=True");
        DataTable tablo = new DataTable();

        public Satisliste()
        {
            InitializeComponent();
        }

        private void SatisListesi(string sql)
        {
            bag.Open();
            SqlDataAdapter adtr = new SqlDataAdapter(sql, bag);
            adtr.Fill(tablo);
            dataGridView1.DataSource = tablo;
            bag.Close();
        }

        private void Satisliste_Load(object sender, EventArgs e)
        {
            tablo.Clear();
            SatisListesi("SELECT * FROM satis_bilgileri");
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;


        }

        private void ToplamUcretHesapla()
        {
            int ucrettoplami = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                object cellValue = dataGridView1.Rows[i].Cells["ucret"].Value;
                if (cellValue != DBNull.Value && cellValue != null)
                {
                    ucrettoplami += Convert.ToInt32(cellValue);
                }
            }
            label1.Text = "Toplam Satış: " + ucrettoplami + " TL";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            tablo.Clear();
            SatisListesi("SELECT * FROM satis_bilgileri");
            ToplamUcretHesapla();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            tablo.Clear();
            SatisListesi("SELECT * FROM satis_bilgileri WHERE SatisTarihi LIKE '" + dateTimePicker1.Text + "'");
            ToplamUcretHesapla();
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0) // Seçili satır var mı kontrol et
            {
                DataGridViewRow satir = dataGridView1.SelectedRows[0];

                string musteriAdi = satir.Cells["Ad"].Value.ToString();
                string filmAdi = satir.Cells["FilmAdi"].Value.ToString();
                string salon = satir.Cells["SalonAdi"].Value.ToString();
                string koltuk = satir.Cells["KoltukNo"].Value.ToString();
                string tarih = satir.Cells["SatisTarihi"].Value.ToString();
                string ucret = satir.Cells["Ucret"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "PDF Dosyası|*.pdf",
                    FileName = $"{musteriAdi}_Bilet.pdf"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (PdfWriter writer = new PdfWriter(sfd.FileName))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            Document doc = new Document(pdf);

                            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                            doc.Add(new Paragraph("🎟 SINEMA BİLETİ 🎟")
                                .SetFont(boldFont)
                                .SetFontSize(18));

                            doc.Add(new Paragraph($"📌 Müşteri: {musteriAdi}"));
                            doc.Add(new Paragraph($"🎬 Film: {filmAdi}"));
                            doc.Add(new Paragraph($"📍 Salon: {salon}"));
                            doc.Add(new Paragraph($"🪑 Koltuk: {koltuk}"));
                            doc.Add(new Paragraph($"📅 Tarih: {tarih}"));
                            doc.Add(new Paragraph($"💰 Ücret: {ucret} TL"));

                            doc.Close();
                        }
                    }

                    MessageBox.Show("Bilet başarıyla oluşturuldu!", "Başarılı");
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir satır seçin!", "Hata");
            }
        }
    }
}
