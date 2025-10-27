using DatabaseLibrary.Models;
using DatabaseLibrary.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FragrantWorld.Pages
{
    public partial class ShopPage : Page
    {
        private readonly ProductService _productService = new();
        private readonly UserService _userService = new();
        private List<Product> AllProducts = new();
        private List<Product> FilteredProducts = new();

        public ShopPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProductsAsync();
            LoadManufacturers();
            DisplayUserInfo();
            UpdateCartButton();
        }

        private void UpdateCartButton()
        {
            var totalItems = App.CartService.GetTotalItems();
            CartButton.Content = totalItems > 0 ? $"Корзина ({totalItems})" : "Корзина";
        }

        private void DisplayUserInfo()
        {
            if (App.CurrentUser != null)
            {
                var userInfoTextBlock = new TextBlock
                {
                    Text = $"{App.CurrentUser.Surname} {App.CurrentUser.Name} ({App.CurrentUserRole})",
                    FontFamily = new FontFamily("Comic Sans MS"),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10, 0, 0, 0),
                    Foreground = new SolidColorBrush(Color.FromRgb(199, 21, 133))
                };

                var topPanel = BackButton.Parent as StackPanel;
                if (topPanel != null)
                {
                    topPanel.Children.Insert(0, userInfoTextBlock);
                }
            }
            else
            {
                var guestTextBlock = new TextBlock
                {
                    Text = "Гость",
                    FontFamily = new FontFamily("Comic Sans MS"),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10, 0, 0, 0),
                    Foreground = new SolidColorBrush(Color.FromRgb(199, 21, 133))
                };

                var topPanel = BackButton.Parent as StackPanel;
                if (topPanel != null)
                {
                    topPanel.Children.Insert(0, guestTextBlock);
                }
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                AllProducts = await _productService.GetProductsAsync();
                if (AllProducts == null) throw new Exception("Failed to load products.");
                FilteredProducts = new List<Product>(AllProducts);
                UpdateDisplayedProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadManufacturers()
        {
            try
            {
                ManufacturerComboBox.Items.Clear();
                ManufacturerComboBox.Items.Add(new ComboBoxItem { Content = "Все производители" });

                var manufacturers = new List<string>
                {
                    "Arôme de France", "BioNatural", "Parfum Élégance", "Herbal Essence",
                    "BellaCosmetic", "AromaLux", "EyesPerfect", "SkinSoft", "AquaCare",
                    "FleurD'or", "BeautyLook", "NailStar", "LipSoft", "TimeRestore"
                };

                foreach (var manufacturer in manufacturers.OrderBy(m => m))
                {
                    ManufacturerComboBox.Items.Add(new ComboBoxItem { Content = manufacturer });
                }

                ManufacturerComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке производителей: {ex.Message}");
            }
        }

        private void UpdateDisplayedProducts()
        {
            ProductStackPanel.Children.Clear();

            foreach (var product in FilteredProducts)
            {
                CreateProductContainer(product);
            }

            CountTextBlock.Text = $"{FilteredProducts.Count} из {AllProducts.Count}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            App.CurrentUserRole = null;

            if (App.CurrentFrame.CanGoBack)
                App.CurrentFrame.GoBack();
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new CartPage());
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                if (AllProducts == null) throw new Exception("Products not loaded.");

                FilteredProducts = AllProducts.Where(product =>
                    (ManufacturerComboBox.SelectedIndex == 0 ||
                     product.Manufacturer == (string)((ComboBoxItem)ManufacturerComboBox.SelectedItem)?.Content) &&
                    (string.IsNullOrWhiteSpace(SearchTextBox.Text) ||
                     product.Name.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) ||
                     product.Description.Contains(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase)) &&
                    (decimal.TryParse(MinPriceTextBox.Text, out decimal minPrice) ? product.Cost >= minPrice : true) &&
                    (decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice) ? product.Cost <= maxPrice : true)
                ).ToList();

                ApplySorting();
                UpdateDisplayedProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ApplySorting()
        {
            if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Цена (по возрастанию)":
                        FilteredProducts = FilteredProducts.OrderBy(p => p.Cost).ToList();
                        break;
                    case "Цена (по убыванию)":
                        FilteredProducts = FilteredProducts.OrderByDescending(p => p.Cost).ToList();
                        break;
                }
            }
        }

        private void CreateProductContainer(Product productItem)
        {
            try
            {
                StackPanel panel = new()
                {
                    Width = 630,
                    Margin = new Thickness(15),
                    Background = new SolidColorBrush(Color.FromRgb(255, 182, 193)),
                };

                Grid grid = new();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock ProductTextBlock = new TextBlock()
                {
                    Text = productItem.Name,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(199, 21, 133))
                };
                Grid.SetRow(ProductTextBlock, 0);
                Grid.SetColumn(ProductTextBlock, 0);
                grid.Children.Add(ProductTextBlock);

                TextBlock DescriptionTextBlock = new TextBlock
                {
                    Text = productItem.Description,
                    FontFamily = new FontFamily("Comic Sans MS"),
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Left,
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 105, 180))
                };
                Grid.SetRow(DescriptionTextBlock, 1);
                Grid.SetColumn(DescriptionTextBlock, 0);
                grid.Children.Add(DescriptionTextBlock);

                TextBlock ManufacturerTextBlock = new TextBlock
                {
                    Text = $"Производитель: {productItem.Manufacturer}",
                    FontFamily = new FontFamily("Comic Sans MS"),
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 105, 180))
                };
                Grid.SetRow(ManufacturerTextBlock, 2);
                Grid.SetColumn(ManufacturerTextBlock, 0);
                grid.Children.Add(ManufacturerTextBlock);

                TextBlock PriceTextBlock = new TextBlock
                {
                    Text = $"Цена: {productItem.Cost} руб.",
                    FontFamily = new FontFamily("Comic Sans MS"),
                    Foreground = new SolidColorBrush(Color.FromRgb(199, 21, 133))
                };
                Grid.SetRow(PriceTextBlock, 3);
                Grid.SetColumn(PriceTextBlock, 0);
                grid.Children.Add(PriceTextBlock);

                Button OrderButton = new Button
                {
                    Content = "Заказать",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontFamily = new FontFamily("Comic Sans MS"),
                    Background = new SolidColorBrush(Color.FromRgb(255, 105, 180)),
                    Foreground = System.Windows.Media.Brushes.White
                };
                OrderButton.Click += (s, e) => OrderButton_Click(productItem);
                Grid.SetRow(OrderButton, 3);
                Grid.SetColumn(OrderButton, 1);
                grid.Children.Add(OrderButton);

                panel.Children.Add(grid);
                ProductStackPanel.Children.Add(panel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OrderButton_Click(Product product)
        {
            App.CartService.AddToCart(product, 1);

            UpdateCartButton();

            string userInfo = App.CurrentUserRole switch
            {
                "Менеджер" => "\n(Менеджер)",
                "Администратор" => "\n(Администратор)",
                _ => ""
            };

            MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!\nЦена: {product.Cost} руб.{userInfo}");
        }
    }
}