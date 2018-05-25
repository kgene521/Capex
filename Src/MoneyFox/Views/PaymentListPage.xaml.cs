﻿using System;
using MoneyFox.Business.ViewModels;
using MoneyFox.Foundation.Resources;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace MoneyFox.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentListPage
    {
        public PaymentListPage()
        {
            InitializeComponent();

            PaymentList.ItemTapped += (sender, args) =>
            {
                PaymentList.SelectedItem = null;
                ViewModel.EditPaymentCommand.Execute(args.Item);
            };

            PaymentList.On<Android>().SetIsFastScrollEnabled(true);
        }

        protected override void OnAppearing()
        {
            Title = ViewModel.Title;
            base.OnAppearing();
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet(Strings.AddTitle, 
                                                  Strings.CancelLabel, 
                                                  null, 
                                                  Strings.AddExpenseLabel,
                                                  Strings.AddIncomeLabel,
                                                  Strings.AddTransferLabel);

            if (action == Strings.AddExpenseLabel)
            {
                await ViewModel.ViewActionViewModel.GoToAddExpenseCommand.ExecuteAsync();
            }
            else if (action == Strings.AddIncomeLabel)
            {
                await ViewModel.ViewActionViewModel.GoToAddIncomeCommand.ExecuteAsync();
            }
            else if (action == Strings.AddTransferLabel)
            {
                await ViewModel.ViewActionViewModel.GoToAddTransferCommand.ExecuteAsync();
            }
        }

        private void EditPayment(object sender, EventArgs e)
        {
            if (!(sender is MenuItem menuItem)) return;
            ViewModel.EditPaymentCommand.ExecuteAsync(menuItem.CommandParameter as PaymentViewModel);
        }

        private void DeletePayment(object sender, EventArgs e)
        {
            if (!(sender is MenuItem menuItem)) return;
            ViewModel.DeletePaymentCommand.ExecuteAsync(menuItem.CommandParameter as PaymentViewModel);
        }
    }
}