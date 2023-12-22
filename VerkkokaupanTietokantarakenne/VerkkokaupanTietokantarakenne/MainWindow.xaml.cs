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
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Xenon\\Documents\\VerkkokauppaV2.mdf;Integrated Security=True;Connect Timeout=30";

        public MainWindow()
        {
            InitializeComponent();
            LataaTuotteet();
            LataaAsiakkaat();
            // LisaaAsiakas(new Asiakas { Nimi = "Test", Sahkoposti = "test@example.com", Osoite = "Testosoite", Puhelinnumero = "12345678" });
            LataaTilaukset();

        }

        private void LataaTilaukset()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT * FROM Tilaus";
                    SqlCommand cmd = new SqlCommand(query, con);

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Tilaus> tilaukset = new List<Tilaus>();

                    while (reader.Read())
                    {
                        Tilaus tilaus = new Tilaus()
                        {
                            TilausID = (int)reader["TilausID"],
                            AsiakasID = reader.IsDBNull(reader.GetOrdinal("AsiakasID")) ? 0 : (int)reader["AsiakasID"],
                            Tilauspäivämäärä = reader.IsDBNull(reader.GetOrdinal("Tilauspäivämäärä")) ? DateTime.MinValue : (DateTime)reader["Tilauspäivämäärä"],
                            Toimitusosoite = reader.IsDBNull(reader.GetOrdinal("Toimitusosoite")) ? string.Empty : reader["Toimitusosoite"].ToString(),
                            Kokonaissumma = reader.IsDBNull(reader.GetOrdinal("Kokonaissumma")) ? 0f : (float)reader["Kokonaissumma"]
                        };

                        tilaukset.Add(tilaus);
                    }

                    dataGridTilaukset.ItemsSource = tilaukset;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Virhe tilausten lataamisessa: " + ex.Message);
            }
        }


        private void PoistaTilaus_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridTilaukset.SelectedItem is Tilaus selectedTilaus)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string query = "DELETE FROM Tilaus WHERE TilausID = @TilausID";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@TilausID", selectedTilaus.TilausID);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Tilaus poistettu onnistuneesti");
                    }

                    LataaTilaukset(); // Reload the orders to reflect the deletion
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Virhe: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Valitse ensin poistettava tilaus");
            }
        }



        private void LataaTuotteet()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT * FROM Tuotteet";
                    SqlCommand cmd = new SqlCommand(query, con);

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Tuotteet> tuotteet = new List<Tuotteet>();

                    while (reader.Read())
                    {
                        Tuotteet Tuotteet = new Tuotteet()
                        {
                            TuoteID = (int)reader["TuoteID"],
                            Nimi = reader["Nimi"].ToString(),
                            Kuvaus = reader["Kuvaus"].ToString(),
                            Hinta = Convert.ToSingle(reader["Hinta"]),
                            Varastosaldo = (int)reader["Varastosaldo"]
                        };

                        tuotteet.Add(Tuotteet);
                    }

                    dataGridTuotteet.ItemsSource = tuotteet;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LisaaAsiakas_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.ShowDialog();

            // Refresh the customer list after adding a new customer
            LataaAsiakkaat();
        }


        private void LataaAsiakkaat()
        {
            try
            {
                
                using (SqlConnection con = new SqlConnection(connectionString)) // Luo yhteyden tietokantaan 
                {
                     
                    con.Open(); // Avaa tietokantayhteys.

                    // SQL-kysely, joka hakee kaikki tiedot Asiakkaat-taulusta.
                    string query = "SELECT * FROM Asiakkaat";
                    SqlCommand cmd = new SqlCommand(query, con);

                    
                    SqlDataReader reader = cmd.ExecuteReader(); // Suoritta kysely ja hankkii tulokset SqlDataReader-olion avulla.

                    
                    List<Asiakas> asiakkaat = new List<Asiakas>(); // lista asiakas-olioita varten.


                    while (reader.Read())
                    {
                        // uusi Asiakas-olio ja asetta sen ominaisuudet tulosrivin tietojen perusteella.
                        Asiakas asiakas = new Asiakas()
                        {
                            AsiakasID = (int)reader["AsiakasID"],
                            Nimi = reader["Nimi"].ToString(),
                            Sahkoposti = reader["Sahkoposti"].ToString(),
                            Osoite = reader["Osoite"].ToString(),
                            Puhelinnumero = reader["Puhelinnumero"].ToString()
                        };

                        // Lisää luodun Asiakas olion listaan.
                        asiakkaat.Add(asiakas);
                    }

                    // Asettaa asiakaslistan sisällön DataGrid-komponentin ItemSource-ominaisuudeksi.
                    dataGridAsiakkaat.ItemsSource = asiakkaat;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void LisaaAsiakas(Asiakas uusiAsiakas)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check if the email already exists
                    string checkQuery = "SELECT COUNT(*) FROM Asiakkaat WHERE Sahkoposti = @Sahkoposti";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@Sahkoposti", uusiAsiakas.Sahkoposti);
                    int exists = (int)checkCmd.ExecuteScalar();

                    if (exists > 0)
                    {
                        MessageBox.Show("Sähköpostiosoite on jo käytössä");
                        return;
                    }

                    // Insert new customer
                    string query = "INSERT INTO Asiakkaat (Nimi, Sahkoposti, Osoite, Puhelinnumero) VALUES (@Nimi, @Sahkoposti, @Osoite, @Puhelinnumero)";
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@Nimi", uusiAsiakas.Nimi);
                    cmd.Parameters.AddWithValue("@Sahkoposti", uusiAsiakas.Sahkoposti);
                    cmd.Parameters.AddWithValue("@Osoite", uusiAsiakas.Osoite);
                    cmd.Parameters.AddWithValue("@Puhelinnumero", uusiAsiakas.Puhelinnumero);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void PaivitaAsiakas_Click(object sender, RoutedEventArgs e)
        {
            Asiakas valittuAsiakas = dataGridAsiakkaat.SelectedItem as Asiakas;
            if (valittuAsiakas != null)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string query = "UPDATE Asiakkaat SET Nimi = @Nimi, Sahkoposti = @Sahkoposti, Osoite = @Osoite, Puhelinnumero = @Puhelinnumero WHERE AsiakasID = @AsiakasID";
                        SqlCommand cmd = new SqlCommand(query, con);

                        cmd.Parameters.AddWithValue("@AsiakasID", valittuAsiakas.AsiakasID);
                        cmd.Parameters.AddWithValue("@Nimi", valittuAsiakas.Nimi);
                        cmd.Parameters.AddWithValue("@Sahkoposti", valittuAsiakas.Sahkoposti);
                        cmd.Parameters.AddWithValue("@Osoite", valittuAsiakas.Osoite);
                        cmd.Parameters.AddWithValue("@Puhelinnumero", valittuAsiakas.Puhelinnumero);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Asiakkaan tiedot päivitetty onnistuneesti");
                    LataaAsiakkaat(); // Päivitä asiakaslista
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Virhe: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Valitse ensin päivitettävä asiakas");
            }
        }

        private void PoistaAsiakas_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridAsiakkaat.SelectedItem is Asiakas selectedAsiakas)
            {
                var result = MessageBox.Show("Haluatko varmasti poistaa valitun asiakkaan?", "Vahvista", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            con.Open();
                            string query = "DELETE FROM Asiakkaat WHERE AsiakasID = @AsiakasID";
                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@AsiakasID", selectedAsiakas.AsiakasID);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Asiakas poistettu onnistuneesti");
                        }

                        LataaAsiakkaat(); // Refresh the clients list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Virhe: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Valitse ensin poistettava asiakas");
            }
        }



        private void LisaaUusiTilaus_Click(object sender, RoutedEventArgs e)
        {
            LisaaTilausWindow tilausWindow = new LisaaTilausWindow();
            var result = tilausWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                LataaTilaukset();
            }
        }




        private void LisaaTuote_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO Tuotteet (Nimi, Kuvaus, Hinta, Varastosaldo) VALUES (@Nimi, @Kuvaus, @Hinta, @Varastosaldo)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Nimi", "tuotteen nimi");
                    cmd.Parameters.AddWithValue("@Kuvaus", "tuotteen kuvaus"); // Tarkista tämä rivi
                    cmd.Parameters.AddWithValue("@Hinta", 100);
                    cmd.Parameters.AddWithValue("@Varastosaldo", 50);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Tuote lisätty onnistuneesti");
                LataaTuotteet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Virhe: " + ex.Message);
            }
        }

        private void PoistaTuote_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridTuotteet.SelectedItem is Tuotteet valittuTuote)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string query = "DELETE FROM Tuotteet WHERE TuoteID = @TuoteID";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@TuoteID", valittuTuote.TuoteID);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Tuote poistettu onnistuneesti");
                    LataaTuotteet(); // Päivitä tuotelista
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Virhe: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Valitse ensin poistettava tuote");
            }
        }



        private void PaivitaTuote_Click(object sender, RoutedEventArgs e)
        {
            // Oletetaan, että käyttäjä on valinnut tuotteen DataGridistä
            if (dataGridTuotteet.SelectedItem is Tuotteet valittuTuote)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string query = "UPDATE Tuotteet SET Nimi = @Nimi, Kuvaus = @Kuvaus, Hinta = @Hinta, Varastosaldo = @Varastosaldo WHERE TuoteID = @TuoteID";
                        SqlCommand cmd = new SqlCommand(query, con);

                        // Oletetaan, että olet hankkinut uudet arvot jostakin (esim. dialogista)
                        cmd.Parameters.AddWithValue("@Nimi", valittuTuote.Nimi);
                        cmd.Parameters.AddWithValue("@Kuvaus", valittuTuote.Kuvaus);
                        cmd.Parameters.AddWithValue("@Hinta", valittuTuote.Hinta);
                        cmd.Parameters.AddWithValue("@Varastosaldo", valittuTuote.Varastosaldo);
                        cmd.Parameters.AddWithValue("@TuoteID", valittuTuote.TuoteID);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Tuote päivitetty onnistuneesti");
                    LataaTuotteet(); // Päivitä tuotelista
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Valitse ensin päivitettävä tuote");
            }
        }
    }

    public class Tuotteet
    {
        public int TuoteID { get; set; }
        public string Nimi { get; set; }
        public string Kuvaus { get; set; }
        public float Hinta { get; set; } 
        public int Varastosaldo { get; set; }
    }


    public class Tilaus
    {
        public int TilausID { get; set; }
        public int AsiakasID { get; set; }
        public DateTime Tilauspäivämäärä { get; set; }
        public string Toimitusosoite { get; set; }
        public float Kokonaissumma { get; set; }
    }

    public class Tilausrivi
    {
        public int TilausriviID { get; set; }
        public int TilausID { get; set; }
        public int TuoteID { get; set; }
        public int Määrä { get; set; }
        public decimal RivinSumma { get; set; }
    }

    public class ToimitettuTilaus
    {
        public int TilausID { get; set; }
        public string AsiakkaanNimi { get; set; }
        public decimal Kokonaissumma { get; set; }
        public string Toimituspaivamaara { get; set; }
    }




    public class Asiakas
    {
        public int AsiakasID { get; set; }
        public string Nimi { get; set; }
        public string Sahkoposti { get; set; }
        public string Osoite { get; set; }
        public string Puhelinnumero { get; set; }
    }

}

