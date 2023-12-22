using System.Windows;
using System.Data.SqlClient;
using System;

namespace VerkkokaupanTietokantarakenne
{
    public partial class AddCustomerWindow : Window
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Xenon\\Documents\\VerkkokauppaV2.mdf;Integrated Security=True;Connect Timeout=30";

        public AddCustomerWindow()
        {
            InitializeComponent();
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string email = txtEmail.Text;
            string address = txtAddress.Text;
            string phoneNumber = txtPhoneNumber.Text;


            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO Asiakkaat (Nimi, Sahkoposti, Osoite, Puhelinnumero) VALUES (@Nimi, @Sahkoposti, @Osoite, @Puhelinnumero)";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@Nimi", name);
                    cmd.Parameters.AddWithValue("@Sahkoposti", email);
                    cmd.Parameters.AddWithValue("@Osoite", address);
                    cmd.Parameters.AddWithValue("@Puhelinnumero", phoneNumber);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Asiakas lisätty onnistuneesti");

                    this.DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Virhe: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}