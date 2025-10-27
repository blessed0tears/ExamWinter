using DatabaseLibrary.Models;
using DatabaseLibrary.Services;
using System;
using System.Linq;
using System.Windows;

namespace FragrantWorld.Pages
{
    public partial class CheckoutWindow : Window
    {
        private readonly OrderService _orderService = new();

        public CheckoutWindow()
        {
            InitializeComponent();
            Loaded += CheckoutWindow_Loaded;
        }

        private async void CheckoutWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загружаем пункты выдачи
                var pickupPoints = await _orderService.GetPickupPointsAsync();
                PickupPointComboBox.ItemsSource = pickupPoints;
                PickupPointComboBox.SelectedIndex = 0;

                // Показываем товары в заказе
                var cartItems = App.CartService.GetCartItems();
                OrderItemsListView.ItemsSource = cartItems;

                // Обновляем итоговую сумму
                UpdateTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void UpdateTotal()
        {
            var total = App.CartService.GetTotalPrice();
            TotalTextBlock.Text = $"Общая сумма: {total} руб.";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PickupPointComboBox.SelectedItem is not PickupPoint selectedPoint)
                {
                    MessageBox.Show("Выберите пункт выдачи");
                    return;
                }

                var cartItems = App.CartService.GetCartItems();
                if (!cartItems.Any())
                {
                    MessageBox.Show("Корзина пуста");
                    return;
                }

                // Создаем заказ
                var order = new Order
                {
                    UserId = App.CurrentUser?.UserId,
                    PickupPointId = selectedPoint.PickupPointId,
                    Status = "Новый",
                    Date = DateTime.Now,
                    DeliveryDate = DateTime.Now.AddDays(7),
                    PickupCode = new Random().Next(100, 1000)
                };

                // Создаем товары заказа
                var orderProducts = cartItems.Select(item => new OrderProduct
                {
                    ProductId = item.ProductId,
                    Amount = (short)item.Quantity
                }).ToList();

                // Сохраняем заказ в БД
                var createdOrder = await _orderService.CreateOrderAsync(order, orderProducts);

                MessageBox.Show($"Заказ №{createdOrder.OrderId} успешно оформлен!\n" +
                              $"Код получения: {createdOrder.PickupCode}\n" +
                              $"Пункт выдачи: {selectedPoint.City}, {selectedPoint.Street}, {selectedPoint.HomeNumber}",
                              "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}