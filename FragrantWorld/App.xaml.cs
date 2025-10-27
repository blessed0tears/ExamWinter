using System.Windows;
using System.Windows.Controls;
using DatabaseLibrary.Models;
using DatabaseLibrary.Services;

namespace FragrantWorld
{
    public partial class App : Application
    {
        public static Frame CurrentFrame { get; set; }
        public static User CurrentUser { get; set; }
        public static string CurrentUserRole { get; set; }
        public static CartService CartService { get; set; } = new CartService();
    }
}