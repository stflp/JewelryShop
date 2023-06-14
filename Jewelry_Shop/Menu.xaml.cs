using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Jewelry_Shop
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        ObservableCollection<Jewelry> shoppingCartItems = new ObservableCollection<Jewelry>();
        public Menu()
        {
            InitializeComponent();
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

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            Jewelry selectedItem = (Jewelry)JewelryMenu.SelectedItem;
            if (selectedItem != null)
            {
                shoppingCartItems.Add(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select an item to add to the cart", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewCartButton_Click(object sender, RoutedEventArgs e)
        {

            ViewCart viewCart = new ViewCart(shoppingCartItems);
            viewCart.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Jewelry selectedItem = (Jewelry)JewelryMenu.SelectedItem;
            if (selectedItem != null)
            {
                shoppingCartItems.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select an item to remove from the cart", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            PurchaseHistoryWindow purchaseHistoryWindow = new PurchaseHistoryWindow();
            purchaseHistoryWindow.Show();
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
