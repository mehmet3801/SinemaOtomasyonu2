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
    public partial class seansliste : Form
    {
        SqlConnection bag = new SqlConnection("Data Source=MEHMETT;Initial Catalog=SinemaOtomasyonu1;Integrated Security=True");
        DataTable tablo = new DataTable();
       
        private void SeansListesi(string sql)
        {
            bag.Open();
            SqlDataAdapter adtr = new SqlDataAdapter(sql, bag);
            adtr.Fill(tablo);
            dataGridView1.DataSource = tablo;
            bag.Close();
            
        }
        public seansliste()
        {
            InitializeComponent();
        }

        private void seansliste_Load(object sender, EventArgs e)
        {
            tablo.Clear();
          SeansListesi("select * from seans_bilgileri where tarih like '" + dateTimePicker1.Text + "'");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            tablo.Clear();
            SeansListesi("select * from seans_bilgileri where tarih like '" + dateTimePicker1.Text + "'");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tablo.Clear();
            SeansListesi("select * from seans_bilgileri");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
        }
    }
}
