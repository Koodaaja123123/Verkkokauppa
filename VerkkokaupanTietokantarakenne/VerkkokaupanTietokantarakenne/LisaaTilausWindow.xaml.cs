using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace VerkkokaupanTietokantarakenne
{
    public partial class LisaaTilausWindow : Window
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Xenon\\Documents\\VerkkokauppaV2.mdf;Integrated Security=True;Connect Timeout=30";

        public LisaaTilausWindow()
        {
            InitializeComponent();

        }

        private void LisaaTilaus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO Tilaus (AsiakasID, Tilauspäivämäärä, Toimitusosoite, Kokonaissumma) VALUES (@AsiakasID, @Tilauspaivamaara, @Toimitusosoite, @Kokonaissumma)";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@AsiakasID", txtAsiakasID.Text);
                    cmd.Parameters.AddWithValue("@Tilauspaivamaara", dpTilauspaivamaara.SelectedDate);
                    cmd.Parameters.AddWithValue("@Toimitusosoite", txtToimitusosoite.Text);
                    cmd.Parameters.AddWithValue("@Kokonaissumma", txtKokonaissumma.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Tilaus lisätty onnistuneesti");

                    this.DialogResult = true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Virhe: " + ex.Message);
            }
        }
    }
}

