using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ViewCart.xaml
    /// </summary>
    public partial class ViewCart : Window
    {
        ObservableCollection<Jewelry> jewelries = new ObservableCollection<Jewelry>();

        public ViewCart(ObservableCollection<Jewelry> items)
        {
            jewelries = items;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            JewelryMenu.ItemsSource = jewelries;
            decimal totalPrice = 0;
            foreach (Jewelry item in jewelries)
            {
                totalPrice += item.price;
            }
            txtPrice.Text = totalPrice.ToString();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Jewelry selectedItem = (Jewelry)JewelryMenu.SelectedItem;
            if (selectedItem != null)
            {
                jewelries.Remove(selectedItem);
                Window_Loaded(sender, e);
            }
            else
            {
                MessageBox.Show("Please select an item to remove from the cart", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            User currentUser = CurrentUser.user;

            using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;"))
            {
                connection.Open();

                foreach (Jewelry item in jewelries)
                {
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "INSERT INTO Purchases(UserName, JewelryName, JewelryPrice, JewelryImage, PurchaseDate) VALUES (@UserName, @JewelryName, @JewelryPrice, @JewelryImage, @PurchaseDate)";
                        cmd.Parameters.AddWithValue("UserName", currentUser.username);
                        cmd.Parameters.AddWithValue("JewelryName", item.name);
                        cmd.Parameters.AddWithValue("JewelryPrice", item.price);
                        cmd.Parameters.AddWithValue("JewelryImage", item.image);
                        cmd.Parameters.AddWithValue("PurchaseDate", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            jewelries.Clear();
            JewelryMenu.ItemsSource = jewelries;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
