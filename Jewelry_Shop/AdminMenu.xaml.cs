using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace Jewelry_Shop
{
    /// <summary>
    /// Interaction logic for AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Window
    {
        string name;
        decimal price;
        byte[] image;

        public AdminMenu()
        {
            InitializeComponent();
        }

        private void AddJewelryButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(itemName.Text) || string.IsNullOrWhiteSpace(itemPrice.Text) || string.IsNullOrWhiteSpace(itemImage.Text))
            {
                MessageBox.Show("Please fill in all the fields.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Jewelry newJewelryItem = new Jewelry
            {
                name = itemName.Text,
                price = decimal.Parse(itemPrice.Text),
                image = File.ReadAllBytes(itemImage.Text)
            };

            using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;"))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "INSERT INTO Jewelry(Name, Price, Image) VALUES (@Name, @Price, @Image)";
                    cmd.Parameters.AddWithValue("Name", newJewelryItem.name);
                    cmd.Parameters.AddWithValue("Price", newJewelryItem.price);
                    cmd.Parameters.AddWithValue("Image", newJewelryItem.image);

                    cmd.ExecuteNonQuery();
                }

                Window_Loaded(sender, e);
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                itemImage.Text = imagePath;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JewelryMenu.ItemsSource = GetJewelryItems();
        }

        public List<Jewelry> GetJewelryItems()
        {
            var items = new List<Jewelry>();

            using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;"))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM Jewelry", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Jewelry
                        {
                            Id = (int)reader["Id"],
                            name = (string)reader["Name"],
                            price = (decimal)reader["Price"],
                            image = (byte[])reader["Image"]
                        });
                    }
                }
            }

            return items;
        }

        private void DeleteJewelryButton_Click(object sender, RoutedEventArgs e)
        {
            Jewelry selectedItem = (Jewelry)JewelryMenu.SelectedItem;

            if (selectedItem != null)
            {
                using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;"))
                {
                    connection.Open();

                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;

                        cmd.CommandText = "DELETE FROM Jewelry WHERE Id = @Id";
                        cmd.Parameters.AddWithValue("Id", selectedItem.Id);

                        cmd.ExecuteNonQuery();
                    }
                }

                Window_Loaded(sender, e);
            }
            else
            {
                MessageBox.Show("Please select an item to delete", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                base.OnClosing(e);
                LogInWindow login = new LogInWindow();
                login.Show();
            }
        }
    }
}