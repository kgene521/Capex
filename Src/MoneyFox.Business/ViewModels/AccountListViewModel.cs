﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MoneyFox.Business.Manager;
using MoneyFox.Business.Parameters;
using MoneyFox.Business.ViewModels.Interfaces;
using MoneyFox.DataAccess.DataServices;
using MoneyFox.Foundation.Groups;
using MoneyFox.Foundation.Interfaces;
using MoneyFox.Foundation.Resources;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;

namespace MoneyFox.Business.ViewModels
{
    /// <inheritdoc cref="IAccountListViewModel" />
    public class AccountListViewModel : BaseViewModel, IAccountListViewModel
    {
        private readonly IAccountService accountService;
        private readonly ISettingsManager settingsManager;
        private readonly IDialogService dialogService;
        private readonly IMvxNavigationService navigationService;

        private MvxObservableCollection<AlphaGroupListGroup<AccountViewModel>> accounts;

        /// <summary>
        ///     Constructor
        /// </summary>
        public AccountListViewModel(IAccountService accountService,
                                    IBalanceCalculationManager balanceCalculationManager,
                                    ISettingsManager settingsManager,
                                    IDialogService dialogService, 
                                    IMvxNavigationService navigationService)
        {
            this.accountService = accountService;
            this.settingsManager = settingsManager;
            this.dialogService = dialogService;
            this.navigationService = navigationService;

            BalanceViewModel = new BalanceViewModel(balanceCalculationManager);
            ViewActionViewModel = new AccountListViewActionViewModel(accountService, navigationService);

            Accounts = new MvxObservableCollection<AlphaGroupListGroup<AccountViewModel>>();
        }
        
        #region Properties

        /// <inheritdoc />
        public IBalanceViewModel BalanceViewModel { get; }

        /// <inheritdoc />
        public IAccountListViewActionViewModel ViewActionViewModel { get; }

        /// <inheritdoc />
        public IMvxLanguageBinder TextSource => new MvxLanguageBinder("", GetType().Name);

        /// <inheritdoc />
        public MvxObservableCollection<AlphaGroupListGroup<AccountViewModel>> Accounts
        {
            get => accounts;
            set
            {
                if (accounts == value) return;
                accounts = value;
                RaisePropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool HasAccounts => Accounts.Any();


        #endregion

        #region Commands

        /// <inheritdoc />
        public MvxAsyncCommand<AccountViewModel> OpenOverviewCommand => new MvxAsyncCommand<AccountViewModel>(GoToPaymentOverView);

        /// <inheritdoc />
        public MvxAsyncCommand<AccountViewModel> EditAccountCommand => new MvxAsyncCommand<AccountViewModel>(EditAccount);

        /// <inheritdoc />
        public MvxAsyncCommand<AccountViewModel> DeleteAccountCommand => new MvxAsyncCommand<AccountViewModel>(Delete);

        /// <inheritdoc />
        public MvxAsyncCommand GoToAddAccountCommand => new MvxAsyncCommand(GoToAddAccount);

        #endregion

        public override async Task Initialize()
        {
            await Loaded();
        }

        private async Task EditAccount(AccountViewModel accountViewModel)
        {
            await navigationService.Navigate<ModifyAccountViewModel, ModifyAccountParameter>(new ModifyAccountParameter(accountViewModel.Id));
        }

        private async Task Loaded()
        {
            try
            {
                await BalanceViewModel.UpdateBalanceCommand.ExecuteAsync();

                var includedAccountList = (await accountService.GetNotExcludedAccounts()).ToList();
                var excludedAccountList = (await accountService.GetExcludedAccounts()).ToList();
                
                var includedAlphaGroup = new AlphaGroupListGroup<AccountViewModel>(Strings.IncludedAccountsHeader);
                includedAlphaGroup.AddRange(includedAccountList.Select(x => new AccountViewModel(x)));

                var excludedAlphaGroup = new AlphaGroupListGroup<AccountViewModel>(Strings.ExcludedAccountsHeader);
                includedAlphaGroup.AddRange(excludedAccountList.Select(x => new AccountViewModel(x)));

                if (includedAccountList.Any())
                {
                    Accounts.Add(includedAlphaGroup);
                }

                if (excludedAccountList.Any())
                {
                    Accounts.Add(excludedAlphaGroup);
                }

                RaisePropertyChanged(nameof(HasAccounts));
            }
            catch(Exception ex)
            {
                await dialogService.ShowMessage(Strings.GeneralErrorTitle, ex.ToString());
            }
        }

        private async Task GoToPaymentOverView(AccountViewModel accountViewModel)
        {
            if (accountViewModel == null) return;

            await navigationService.Navigate<PaymentListViewModel, PaymentListParameter>(new PaymentListParameter(accountViewModel.Id));
        }

        private async Task Delete(AccountViewModel accountToDelete)
        {
            if (accountToDelete == null)
            {
                return;
            }

            if (await dialogService.ShowConfirmMessage(Strings.DeleteTitle, Strings.DeleteAccountConfirmationMessage))
            {
                await accountService.DeleteAccount(accountToDelete.Account);

                Accounts.Clear();
                await Loaded();
                
                settingsManager.LastDatabaseUpdate = DateTime.Now;
            }
        }

        private async Task GoToAddAccount()
        {
            await navigationService.Navigate<ModifyAccountViewModel, ModifyAccountParameter>(new ModifyAccountParameter());
        }
    }
}