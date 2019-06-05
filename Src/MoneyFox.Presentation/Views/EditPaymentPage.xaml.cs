﻿using MoneyFox.Foundation.Resources;
using MoneyFox.ServiceLayer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoneyFox.Presentation.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditPaymentPage 
	{
		public EditPaymentPage ()
		{
			InitializeComponent ();

            ToolbarItems.Add(new ToolbarItem
            {
                Command = new Command(() => (BindingContext as EditPaymentViewModel)?.SaveCommand.Execute()),
                Text = Strings.SavePaymentLabel,
                Priority = 0,
                Order = ToolbarItemOrder.Primary,
                Icon = "ic_save.png"
            });

            ToolbarItems.Add(new ToolbarItem
            {
                Command = new Command(() => (BindingContext as EditPaymentViewModel)?.DeleteCommand.Execute()),
                Text = Strings.DeleteLabel,
                Priority = 1,
                Order = ToolbarItemOrder.Secondary
            });
        }
    }
}