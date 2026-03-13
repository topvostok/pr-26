using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Classes;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class EquipFormPage : Page
    {
        private int _id = 0;

        public EquipFormPage(int id = 0)
        {
            InitializeComponent();
            _id = id;
            Loaded += (s, e) => Init();
        }

        private void Init()
        {
            CbCategory.SelectedIndex = 0;
            CbCondition.SelectedIndex = 1;

            if (_id > 0)
            {
                FormTitle.Text = "РЕДАКТИРОВАТЬ СНАРЯЖЕНИЕ";
                BtnDelete.Visibility = Visibility.Visible;
                var eq = EquipmentRepo.GetById(_id);
                if (eq == null) return;
                TbName.Text    = eq.Name;
                TbBrand.Text   = eq.Brand;
                TbSize.Text    = eq.SizeValue;
                TbPrice.Text   = eq.PricePerDay.ToString();
                TbStock.Text   = eq.StockQty.ToString();
                TbImage.Text   = eq.ImageUrl;
                TbNotes.Text   = eq.Notes;
                SetCombo(CbCategory,  eq.Category);
                SetCombo(CbCondition, eq.ConditionLvl);
            }
        }

        private void SetCombo(ComboBox cb, string val)
        {
            foreach (ComboBoxItem item in cb.Items)
                if (item.Content?.ToString() == val)
                { cb.SelectedItem = item; return; }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text) ||
                string.IsNullOrWhiteSpace(TbBrand.Text))
            { MessageBox.Show("Заполните название и бренд.", "Ошибка"); return; }

            if (!decimal.TryParse(TbPrice.Text, out decimal price) || price <= 0)
            { MessageBox.Show("Введите корректную цену.", "Ошибка"); return; }

            if (!int.TryParse(TbStock.Text, out int stock) || stock < 0)
            { MessageBox.Show("Введите корректное количество.", "Ошибка"); return; }

            var eq = new Equipment
            {
                Id           = _id,
                Name         = TbName.Text.Trim(),
                Brand        = TbBrand.Text.Trim(),
                Category     = (CbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Лыжи",
                ConditionLvl = (CbCondition.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Хорошее",
                SizeValue    = TbSize.Text.Trim(),
                PricePerDay  = price,
                StockQty     = stock,
                ImageUrl     = TbImage.Text.Trim(),
                Notes        = TbNotes.Text.Trim(),
            };

            try
            {
                if (_id == 0) EquipmentRepo.Insert(eq);
                else          EquipmentRepo.Update(eq);
                AdminWindow.Instance.GoTo("EqList", AdminWindow.Instance.NavEqList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Удалить это снаряжение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                try
                {
                    EquipmentRepo.Delete(_id);
                    AdminWindow.Instance.GoTo("EqList", AdminWindow.Instance.NavEqList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => AdminWindow.Instance.GoTo("EqList", AdminWindow.Instance.NavEqList);
    }
}
