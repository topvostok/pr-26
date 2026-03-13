using System;
using System.Windows;
using System.Windows.Controls;
using MySqlX.XDevAPI;
using WpfApp1.Classes;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class RentalFormPage : Page
    {
        private int _id = 0;

        public RentalFormPage(int id = 0)
        {
            InitializeComponent();
            _id = id;
            Loaded += (s, e) => Init();
        }

        private void Init()
        {
            // Fill combos
            CbClient.ItemsSource = ClientRepo.GetAll();
            CbEquip.ItemsSource = EquipmentRepo.GetAll();
            CbStatus.SelectedIndex = 0;
            DpStart.SelectedDate = DateTime.Today;
            DpEnd.SelectedDate = DateTime.Today.AddDays(1);

            if (_id > 0)
            {
                FormTitle.Text = "Р Р•Р”РђРљРўРР РћР’РђРўР¬ РђР Р•РќР”РЈ";
                BtnDelete.Visibility = Visibility.Visible;
                var rn = RentalRepo.GetById(_id);
                if (rn == null) return;

                SelectById(CbClient, rn.ClientId);
                SelectById(CbEquip, rn.EquipmentId);
                DpStart.SelectedDate = rn.RentDate;
                DpEnd.SelectedDate = rn.ReturnPlan;
                TbDeposit.Text = rn.Deposit.ToString();
                TbNotes.Text = rn.Notes;
                SetCombo(CbStatus, rn.Status);
                RecalcTotal();
            }
        }

        // Select item by Id property
        private void SelectById(ComboBox cb, int id)
        {
            foreach (var item in cb.Items)
            {
                if (item is RentalClient cl && cl.Id == id) { cb.SelectedItem = item; return; }
                if (item is Equipment eq && eq.Id == id) { cb.SelectedItem = item; return; }
            }
        }

        private void SetCombo(ComboBox cb, string val)
        {
            foreach (ComboBoxItem item in cb.Items)
                if (item.Content?.ToString() == val) { cb.SelectedItem = item; return; }
        }

        private void CbEquip_Changed(object sender, SelectionChangedEventArgs e) => RecalcTotal();
        private void DpDate_Changed(object sender, SelectionChangedEventArgs e) => RecalcTotal();

        private void RecalcTotal()
        {
            if (DpStart.SelectedDate == null || DpEnd.SelectedDate == null) return;
            int days = (int)(DpEnd.SelectedDate.Value - DpStart.SelectedDate.Value).TotalDays;
            if (days < 1) days = 1;
            TbDays.Text = days.ToString();

            if (CbEquip.SelectedItem is Equipment eq)
            {
                decimal total = eq.PricePerDay * days;
                TbTotal.Text = total.ToString("N0");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CbClient.SelectedItem == null || CbEquip.SelectedItem == null)
            { MessageBox.Show("Р’С‹Р±РµСЂРёС‚Рµ РєР»РёРµРЅС‚Р° Рё СЃРЅР°СЂСЏР¶РµРЅРёРµ.", "РћС€РёР±РєР°"); return; }

            if (DpStart.SelectedDate == null || DpEnd.SelectedDate == null)
            { MessageBox.Show("РЈРєР°Р¶РёС‚Рµ РґР°С‚С‹ Р°СЂРµРЅРґС‹.", "РћС€РёР±РєР°"); return; }

            if (!decimal.TryParse(TbDeposit.Text, out decimal dep)) dep = 0;
            if (!int.TryParse(TbDays.Text, out int days)) days = 1;
            if (!decimal.TryParse(TbTotal.Text.Replace(" ", "").Replace(",", ""), out decimal total)) total = 0;

            var rn = new Rental
            {
                Id = _id,
                //ClientId = ((Client)CbClient.SelectedItem).Id,
                EquipmentId = ((Equipment)CbEquip.SelectedItem).Id,
                RentDate = DpStart.SelectedDate.Value,
                ReturnPlan = DpEnd.SelectedDate.Value,
                DaysCount = days,
                TotalPrice = total,
                Deposit = dep,
                Status = (CbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "РђРєС‚РёРІРЅР°",
                Notes = TbNotes.Text.Trim(),
            };

            try
            {
                if (_id == 0) RentalRepo.Insert(rn);
                else RentalRepo.Update(rn);
                AdminWindow.Instance.GoTo("RnList", AdminWindow.Instance.NavRnList);
            }
            catch (Exception ex)
            { MessageBox.Show("РћС€РёР±РєР°: " + ex.Message, "РћС€РёР±РєР°", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("РЈРґР°Р»РёС‚СЊ Р·Р°РїРёСЃСЊ РѕР± Р°СЂРµРЅРґРµ?", "РџРѕРґС‚РІРµСЂР¶РґРµРЅРёРµ",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                try { RentalRepo.Delete(_id); AdminWindow.Instance.GoTo("RnList", AdminWindow.Instance.NavRnList); }
                catch (Exception ex)
                { MessageBox.Show("РћС€РёР±РєР°: " + ex.Message, "РћС€РёР±РєР°", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
            => AdminWindow.Instance.GoTo("RnList", AdminWindow.Instance.NavRnList);
    }
}
