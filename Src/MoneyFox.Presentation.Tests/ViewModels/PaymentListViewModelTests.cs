﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using GenericServices;
using MockQueryable.Moq;
using MoneyFox.Presentation.ViewModels;
using MoneyFox.ServiceLayer.Facades;
using MoneyFox.ServiceLayer.Parameters;
using MoneyFox.ServiceLayer.Services;
using MoneyFox.ServiceLayer.ViewModels;
using Moq;
using Xunit;
using IDialogService = MoneyFox.ServiceLayer.Interfaces.IDialogService;

namespace MoneyFox.Presentation.Tests.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class PaymentListViewModelTests
    {
        public PaymentListViewModelTests()
        {
            crudService = new Mock<ICrudServicesAsync>();
            paymentService = new Mock<IPaymentService>();
            dialogService = new Mock<IDialogService>();
            settingsFacade = new Mock<ISettingsFacade>();
            balanceCalculatorService = new Mock<IBalanceCalculationService>();
            backupService = new Mock<IBackupService>();
            navigationService = new Mock<INavigationService>();

            crudService.SetupAllProperties();
            paymentService.SetupAllProperties();
        }

        private readonly Mock<ICrudServicesAsync> crudService;
        private readonly Mock<IPaymentService> paymentService;
        private readonly Mock<IDialogService> dialogService;
        private readonly Mock<ISettingsFacade> settingsFacade;
        private readonly Mock<IBalanceCalculationService> balanceCalculatorService;
        private readonly Mock<IBackupService> backupService;
        private readonly Mock<INavigationService> navigationService;

        [Fact]
        public async Task Init_NullPassAccountId_AccountIdSet()
        {
            // Arrange
            crudService.Setup(x => x.ReadSingleAsync<AccountViewModel>(It.IsAny<int>()))
                .ReturnsAsync(new AccountViewModel());

            balanceCalculatorService.Setup(x => x.GetEndOfMonthBalanceForAccount(It.IsAny<AccountViewModel>()))
                .ReturnsAsync(0);

            var vm = new PaymentListViewModel(crudService.Object,
                paymentService.Object,
                dialogService.Object,
                settingsFacade.Object,
                balanceCalculatorService.Object,
                backupService.Object,
                navigationService.Object);

            // Act
            vm.Prepare(new PaymentListParameter());
            await vm.Initialize();

            // Assert
            Assert.Equal(0, vm.AccountId);
        }

        [Fact]
        public async Task Init_PassAccountId_AccountIdSet()
        {
            // Arrange
            crudService.Setup(x => x.ReadSingleAsync<AccountViewModel>(It.IsAny<int>()))
                .ReturnsAsync(new AccountViewModel());

            balanceCalculatorService.Setup(x => x.GetEndOfMonthBalanceForAccount(It.IsAny<AccountViewModel>()))
                .ReturnsAsync(0);

            var vm = new PaymentListViewModel(crudService.Object,
                paymentService.Object,
                dialogService.Object,
                settingsFacade.Object,
                balanceCalculatorService.Object,
                backupService.Object,
                navigationService.Object);

            // Act
            vm.Prepare(new PaymentListParameter(42));
            await vm.Initialize();

            // Assert
            Assert.Equal(42, vm.AccountId);
        }

        [Fact]
        public async Task ViewAppearing_DialogShown()
        {
            // Arrange
            dialogService.Setup(x => x.ShowLoadingDialog(It.IsAny<string>()));
            dialogService.Setup(x => x.HideLoadingDialog());

            crudService.Setup(x => x.ReadManyNoTracked<AccountViewModel>())
                .Returns(new List<AccountViewModel>()
                    .AsQueryable()
                    .BuildMock()
                    .Object);
            crudService.Setup(x => x.ReadSingleAsync<AccountViewModel>(It.IsAny<int>()))
                .ReturnsAsync(new AccountViewModel());

            var vm = new PaymentListViewModel(crudService.Object,
                paymentService.Object,
                dialogService.Object,
                settingsFacade.Object,
                balanceCalculatorService.Object,
                backupService.Object,
                navigationService.Object);

            await vm.Initialize();

            // Act
            vm.ViewAppearing();

            // Assert
            dialogService.Verify(x => x.ShowLoadingDialog(It.IsAny<string>()), Times.Once);
        }
    }
}