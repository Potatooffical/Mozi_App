using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace Mozi_App_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //======Globális változok=======
        int Selecteditem;
        private readonly string connectionstring = "server=localhost;user=root;password=;database=mozi";
        //======mufajlista feltoltese======
        List<string> mufajlista = new List<string>() { "kérem válasszon" };
        //======korhatarlista es feltoltese======
        List<string> korhatarlista = new List<string>() { "kérem válasszon" };
        //======Fő ablak=======
        public MainWindow()
        {
            InitializeComponent();

            Korhatarhozzaad();
            Mufajlistahozzaad();
            cb_mufaj.ItemsSource = mufajlista;
            cb_korhatar.ItemsSource = korhatarlista;

            cb_mufaj.SelectedIndex = 0;
            cb_korhatar.SelectedIndex = 0;

            Adatbetolt();
        }
        //======Korhatarhozzaad======
        private void Korhatarhozzaad()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionstring))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT DISTINCT korhatar FROM filmek";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // A listához adjuk hozzá, nem a ComboBoxhoz
                    foreach (DataRow row in dt.Rows)
                    {
                        korhatarlista.Add(row["korhatar"].ToString());
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Adatbázis hiba vagy inaktiv a szerver \n" + ex.Message,
                                    "Hiba", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
        }
        //======Mufajhozzaad======
        private void Mufajlistahozzaad()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionstring))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT DISTINCT mufaj FROM filmek";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                        mufajlista.Add(row["mufaj"].ToString());
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Adatbázis hiba vagy inaktiv a szerver \n" + ex.Message,
                                    "Hiba", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
        }

        //======Adatbetöltése=======
        private void Adatbetolt()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionstring))
                {
                    conn.Open();
                    //MessageBox.Show("Sikeres adat betöltés!");
                    string sql = "SELECT * FROM filmek";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dg_tablazat.ItemsSource = dt.DefaultView;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Adatbázis hiba vagy inaktiv a szerver \n" + ex.Message, "Hiba", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }
        //======Adahozzáadása=======
        private void btn_Hozzaad_Click(object sender, RoutedEventArgs e)
        {
            if (cb_korhatar.SelectedIndex != 0 && cb_mufaj.SelectedIndex != 0)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionstring))
                    {
                        conn.Open();

                        string cim = tbx_cim.Text;
                        if (!int.TryParse(tbx_hossz.Text, out int hossz))
                        {
                            MessageBox.Show("Hossz csak szám lehet!");
                            return;
                        }
                        else if (int.TryParse(tbx_hossz.Text, out hossz) && hossz >= 1000)
                        {
                            MessageBox.Show("Nagyon hosszú a film!");
                        }
                        string korhatar = cb_korhatar.SelectedItem.ToString();
                        string mufaj = cb_mufaj.SelectedItem.ToString();


                        string sql = "INSERT INTO filmek (cim, hossz, korhatar, mufaj) VALUES (@cim, @hossz, @korhatar, @mufaj)";
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@cim", cim);
                            cmd.Parameters.AddWithValue("@hossz", hossz);
                            cmd.Parameters.AddWithValue("@korhatar", korhatar);
                            cmd.Parameters.AddWithValue("@mufaj", mufaj);

                            int rows = cmd.ExecuteNonQuery();
                            //MessageBox.Show(rows > 0 ? "Sikeres hozzáadás!" : "Nem sikerült hozzáadni.");
                        }
                        rb_Osszes.IsChecked = false;
                        rb_hatev.IsChecked = false;
                        rb_tizenkettoev.IsChecked = false;
                        rb_tizenhatev.IsChecked = false;
                        rb_tizennyolcev.IsChecked = false;
                        Adatbetolt();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Adatbázis hiba vagy inaktív a szerver \n" + ex.Message,
                                    "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Hiányos adatok!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //======Adattörlése=======
        private void btn_Torol_Click(object sender, RoutedEventArgs e)
        {
            if (Selecteditem == -1)
            {
                MessageBox.Show("Nincs kiválasztva adat :|");
                return;
            }
            var eredmeny = MessageBox.Show("Biztosan akkarod törölni a sort?", "Törlés", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            if (eredmeny != MessageBoxResult.OK)
            {
                MessageBox.Show("nem történt változás");
                return;
            }
            else
            {
                using (MySqlConnection conn = new MySqlConnection(connectionstring))
                {
                    try
                    {
                        conn.Open();

                        string sql = "DELETE FROM filmek WHERE id=@id";
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", Selecteditem);

                            int rows = cmd.ExecuteNonQuery();
                            MessageBox.Show(rows > 0 ? "Sikeres törlés!" : "Nem sikerült törölni.");
                        }
                        rb_Osszes.IsChecked = false;
                        rb_hatev.IsChecked = false;
                        rb_tizenkettoev.IsChecked = false;
                        rb_tizenhatev.IsChecked = false;
                        rb_tizennyolcev.IsChecked = false;
                        Adatbetolt();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Adatbázis hiba történt!\n" + ex.Message);
                    }
                }
            }
        }
        //======Táblázatbeállitássa jelölésere=======
        private void dg_tablazat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView sor = dg_tablazat.SelectedItem as DataRowView;
            if (sor == null) return;
            int.TryParse(sor["id"].ToString(), out Selecteditem);
            tbx_cim.Text = sor["cim"].ToString();
            tbx_hossz.Text = sor["Hossz"].ToString();
            cb_korhatar.SelectedItem = sor["korhatar"].ToString();
            cb_mufaj.SelectedItem = sor["mufaj"].ToString();
        }
        //======Adat módositása=======
        private void btn_Modosit_Click(object sender, RoutedEventArgs e)
        {
            if (cb_korhatar.SelectedIndex != 0 && cb_mufaj.SelectedIndex != 0)
            {


                if (Selecteditem == -1)
                {
                    MessageBox.Show("Nincs kiválasztva adat :|");
                    return;
                }
                var eredmeny = MessageBox.Show("Biztosan akkarod módositani a sort?", "Modósitás", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (eredmeny != MessageBoxResult.OK)
                {
                    MessageBox.Show("nem történt változás");
                    return;
                }
                
                else
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionstring))
                    {
                        try
                        {
                            conn.Open();
                            string cim = tbx_cim.Text;
                            if (!int.TryParse(tbx_hossz.Text, out int hossz))
                            {
                                MessageBox.Show("Hossz csak szám lehet!");
                                return;
                            }
                            else if (int.TryParse(tbx_hossz.Text, out  hossz) && hossz >= 1000)
                            {
                                MessageBox.Show("Nagyon hosszú a film!");
                            }
                            string korhatar = cb_korhatar.SelectedItem.ToString();
                            string mufaj = cb_mufaj.SelectedItem.ToString();
                            string sql = "update filmek set cim=@cim,hossz=@hossz,korhatar=@korhatar,mufaj=@mufaj where id=@id ";
                            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@cim", cim);
                                cmd.Parameters.AddWithValue("@hossz", hossz);
                                cmd.Parameters.AddWithValue("@korhatar", korhatar);
                                cmd.Parameters.AddWithValue("@mufaj", mufaj);
                                cmd.Parameters.AddWithValue("@id", Selecteditem);

                                int rows = cmd.ExecuteNonQuery();
                                MessageBox.Show(rows > 0 ? "Sikeres módositani!" : "Nem sikerült módositás.");
                            }
                            rb_Osszes.IsChecked = false;
                            rb_hatev.IsChecked = false;
                            rb_tizenkettoev.IsChecked = false;
                            rb_tizenhatev.IsChecked = false;
                            rb_tizennyolcev.IsChecked = false;
                            Adatbetolt();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Adatbázis hiba történt!\n" + ex.Message);
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("Hiányos adatok!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //======Frissités======
        private void btn_Frissites_Click(object sender, RoutedEventArgs e)
        {
            tbx_cim.Text = "";
            tbx_hossz.Text = "";
            cb_korhatar.SelectedItem = 0;
            cb_mufaj.SelectedItem = 0;
            rb_Osszes.IsChecked = false;
            rb_hatev.IsChecked = false;
            rb_tizenkettoev.IsChecked = false;
            rb_tizenhatev.IsChecked = false;
            rb_tizennyolcev.IsChecked = false;
            Adatbetolt();
        }
        //=======Rádió gomb alapján keresés======
        private void rb_Osszes_Checked(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionstring))
            {
                conn.Open();
                string query = "";
                int selectedOption = GetSelectedOption();
                switch (selectedOption)
                {
                    case 1:
                        query = "SELECT * FROM filmek";
                        break;
                    case 2:
                        query = "SELECT * FROM filmek WHERE korhatar='KN'";
                        break;
                    case 3:
                        query = "SELECT * FROM filmek WHERE korhatar=6";
                        break;
                    case 4:
                        query = "SELECT * FROM filmek WHERE korhatar=12";
                        break;
                    case 5:
                        query = "SELECT * FROM filmek WHERE korhatar=16";
                        break;
                    case 6:
                        query = "SELECT * FROM filmek WHERE korhatar=18";
                        break;
                }
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dg_tablazat.ItemsSource = dt.DefaultView;
                }

                conn.Close();
            }
        }
        //======Radiobuttonok altal visszaadtok ertekek=======
        private int GetSelectedOption()
        {
            if (rb_Osszes.IsChecked == true) return 1;
            if (rb_kn.IsChecked == true) return 2;
            if (rb_hatev.IsChecked == true) return 3;
            if (rb_tizenkettoev.IsChecked == true) return 4;
            if (rb_tizenhatev.IsChecked == true) return 5;
            if (rb_tizennyolcev.IsChecked == true) return 6;
            return 0;
        }
    }
}
