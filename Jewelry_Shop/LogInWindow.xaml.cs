using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Jewelry_Shop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void forgot_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordWindow window = new ForgotPasswordWindow();
            window.Show();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usrName.Text) || string.IsNullOrEmpty(usrPassword.Password))
            {
                MessageBox.Show("Both fields must be filled out.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "INSERT INTO users (username, password, isAdmin) VALUES (@username, @password, @isAdmin)";
                    cmd.Parameters.AddWithValue("username", usrName.Text);
                    cmd.Parameters.AddWithValue("password", usrPassword.Password);
                    cmd.Parameters.AddWithValue("isAdmin", false);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Registration Successful", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usrName.Text) || string.IsNullOrEmpty(usrPassword.Password))
            {
                MessageBox.Show("Both fields must be filled out.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string connectionString = "Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;";

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    cmd.CommandText = "SELECT password, isadmin FROM users WHERE username = @username";
                    cmd.Parameters.AddWithValue("username", usrName.Text);
                    cmd.Parameters.AddWithValue("password", usrPassword.Password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentUser.user = new User(usrName.Text);
                            string storedPassword = reader.GetString(0);
                            bool isAdmin = reader.GetBoolean(1);

                            if (usrPassword.Password == storedPassword)
                            {
                                if (isAdmin)
                                {
                                    AdminMenu adminMenu = new AdminMenu();
                                    adminMenu.Show();
                                    this.Close();
                                }
                                else
                                {
                                    Menu menu = new Menu();
                                    menu.Show();
                                    this.Close();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Incorrect password", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Username not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                login_Click(sender, e);
            }
        }
    }
}