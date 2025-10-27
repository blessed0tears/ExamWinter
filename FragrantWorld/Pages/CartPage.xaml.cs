using DatabaseLibrary.Models;
using DatabaseLibrary.Services;
using System.Windows;
using System.Windows.Controls;

namespace FragrantWorld.Pages
{
    public partial class CartPage : Page
    {
        private readonly OrderService _orderService = new();

        public CartPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayCartItems();
        }

        private void DisplayCartItems()
        {
            CartItemsPanel.Children.Clear();

            var cartItems = App.CartService.GetCartItems();

            if (!cartItems.Any())
            {
                var emptyText = new TextBlock
                {
                    Text = "Корзина пуста",
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                CartItemsPanel.Children.Add(emptyText);
                CheckoutButton.IsEnabled = false;
                return;
            }

            CheckoutButton.IsEnabled = true;

            foreach (var item in cartItems)
            {
                CreateCartItemContainer(item);
            }

            UpdateTotal();
        }

        private void CreateCartItemContainer(CartItem item)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10),
                Background = System.Windows.Media.Brushes.White
            };

            // Информация о товаре
            var productInfoPanel = new StackPanel
            {
                Width = 400,
                Margin = new Thickness(10)
            };

            productInfoPanel.Children.Add(new TextBlock
            {
                Text = item.Name,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap
            });

            productInfoPanel.Children.Add(new TextBlock
            {
                Text = $"Цена: {item.Cost} руб.",
                FontSize = 12
            });

            // Управление количеством
            var quantityPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10)
            };

            var minusButton = new Button
            {
                Content = "-",
                Width = 30,
                Tag = item.ProductId
            };
            minusButton.Click += MinusButton_Click;

            var quantityText = new TextBlock
            {
                Text = item.Quantity.ToString(),
                Width = 30,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var plusButton = new Button
            {
                Content = "+",
                Width = 30,
                Tag = item.ProductId
            };
            plusButton.Click += PlusButton_Click;

            var removeButton = new Button
            {
                Content = "Удалить",
                Margin = new Thickness(10, 0, 0, 0),
                Tag = item.ProductId
            };
            removeButton.Click += RemoveButton_Click;

            quantityPanel.Children.Add(minusButton);
            quantityPanel.Children.Add(quantityText);
            quantityPanel.Children.Add(plusButton);
            quantityPanel.Children.Add(removeButton);

            panel.Children.Add(productInfoPanel);
            panel.Children.Add(quantityPanel);

            CartItemsPanel.Children.Add(panel);
        }

        private void MinusButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = (int)((Button)sender).Tag;
            var item = App.CartService.GetCartItems().FirstOrDefault(p => p.ProductId == productId);
            if (item != null && item.Quantity > 1)
            {
                App.CartService.UpdateQuantity(productId, item.Quantity - 1);
                DisplayCartItems();
            }
        }

        private void PlusButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = (int)((Button)sender).Tag;
            var item = App.CartService.GetCartItems().FirstOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                App.CartService.UpdateQuantity(productId, item.Quantity + 1);
                DisplayCartItems();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var productId = (int)((Button)sender).Tag;
            App.CartService.RemoveFromCart(productId);
            DisplayCartItems();
        }

        private void UpdateTotal()
        {
            var total = App.CartService.GetTotalPrice();
            var totalItems = App.CartService.GetTotalItems();
            TotalTextBlock.Text = $"Итого: {totalItems} товаров на сумму {total} руб.";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new ShopPage());
        }

        private async void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Для оформления заказа необходимо авторизоваться", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var checkoutWindow = new CheckoutWindow();
                if (checkoutWindow.ShowDialog() == true)
                {
                    MessageBox.Show("Заказ успешно оформлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    App.CartService.ClearCart();
                    App.CurrentFrame.Navigate(new ShopPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}