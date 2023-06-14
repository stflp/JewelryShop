using Npgsql;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for PurchaseHistoryWindow.xaml
    /// </summary>
    public partial class PurchaseHistoryWindow : Window
    {
        List<PurchaseHistory> purchases;

        public PurchaseHistoryWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            purchases = GetPurchases();

            foreach (var purchase in purchases)
            {
                PurchasesListView.Items.Add(new { PurchaseId = purchase.purchaseId, PurchaseDate = purchase.purchaseDate });
            }

        }

        private void PurchasesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = PurchasesListView.SelectedItem;

            if (selectedItem != null)
            {
                var selectedPurchase = purchases.FirstOrDefault(p => p.purchaseId == ((dynamic)selectedItem).PurchaseId);

                DetailsTextBlock.Text = "Item: " + selectedPurchase.jewelryName + "\nPrice: " + selectedPurchase.jewelryPrice;

                using (MemoryStream stream = new MemoryStream(selectedPurchase.jewelryImage))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    DetailsImage.Source = image;
                }

                DetailsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DetailsPanel.Visibility = Visibility.Hidden;
            }
        }

        public List<PurchaseHistory> GetPurchases()
        {
            var purchases = new List<PurchaseHistory>();

            using (var connection = new NpgsqlConnection("Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;"))
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand("SELECT * FROM Purchases WHERE UserName = @Username", connection))
                {
                    cmd.Parameters.AddWithValue("Username", CurrentUser.user.username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var purchase = new PurchaseHistory
                            {
                                purchaseId = (int)reader["PurchaseId"],
                                username = (string)reader["UserName"],
                                jewelryName = (string)reader["JewelryName"],
                                jewelryPrice = (decimal)reader["JewelryPrice"],
                                jewelryImage = (byte[])reader["JewelryImage"],
                                purchaseDate = (DateTime)reader["PurchaseDate"]
                            };

                            purchases.Add(purchase);
                        }
                    }
                }
            }

            return purchases;
        }

    }
}
