using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Npgsql;

namespace Jewelry_Shop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ConnString = "Server=localhost;Port=5432;Database=JewelryDB;User Id=postgres;Password=1234;";
    }
}
