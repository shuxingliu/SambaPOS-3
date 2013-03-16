﻿using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using Samba.Domain.Models.Entities;
using Samba.Domain.Models.Inventory;
using Samba.Localization.Properties;
using Samba.Presentation.Common;
using Samba.Presentation.Common.ModelBase;
using Samba.Presentation.Services.Common;
using Samba.Services;

namespace Samba.Modules.InventoryModule
{
    [ModuleExport(typeof(InventoryModule))]
    public class InventoryModule : VisibleModuleBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ICacheService _cacheService;
        private readonly WarehouseInventoryView _warehouseInventoryView;
        private readonly WarehouseInventoryViewModel _warehouseInventoryViewModel;

        [ImportingConstructor]
        public InventoryModule(IRegionManager regionManager, ICacheService cacheService,
            WarehouseInventoryView resourceInventoryView, WarehouseInventoryViewModel resourceInventoryViewModel)
            : base(regionManager, AppScreens.InventoryView)
        {
            _regionManager = regionManager;
            _cacheService = cacheService;
            _warehouseInventoryView = resourceInventoryView;
            _warehouseInventoryViewModel = resourceInventoryViewModel;

            AddDashboardCommand<EntityCollectionViewModelBase<WarehouseTypeViewModel, WarehouseType>>(Resources.WarehouseType.ToPlural(), Resources.Inventory, 35);
            AddDashboardCommand<EntityCollectionViewModelBase<WarehouseViewModel, Warehouse>>(Resources.Warehouse.ToPlural(), Resources.Inventory, 35);
            AddDashboardCommand<EntityCollectionViewModelBase<TransactionTypeViewModel, InventoryTransactionType>>(Resources.TransactionType.ToPlural(), Resources.Inventory, 35);
            AddDashboardCommand<EntityCollectionViewModelBase<TransactionDocumentTypeViewModel, InventoryTransactionDocumentType>>(Resources.DocumentType.ToPlural(), Resources.Inventory, 35);
            AddDashboardCommand<TransactionListViewModel>(Resources.Transaction.ToPlural(), Resources.Inventory, 35);

            AddDashboardCommand<EntityCollectionViewModelBase<InventoryItemViewModel, InventoryItem>>(Resources.InventoryItems, Resources.Inventory, 35);
            AddDashboardCommand<EntityCollectionViewModelBase<RecipeViewModel, Recipe>>(Resources.Recipes, Resources.Inventory, 35);
            AddDashboardCommand<PeriodicConsumptionListViewModel>(Resources.EndOfDayRecords, Resources.Inventory, 36);

            SetNavigationCommand(Resources.Warehouses, Resources.Common, "Images/box.png", 40);

            EventServiceFactory.EventService.GetEvent<GenericEvent<Entity>>().Subscribe(OnResourceEvent);
            EventServiceFactory.EventService.GetEvent<GenericEvent<Warehouse>>().Subscribe(OnWarehouseEvent);
        }

        private void OnResourceEvent(EventParameters<Entity> obj)
        {
            if (obj.Topic == EventTopicNames.DisplayInventory)
            {
                var warehouse = _cacheService.GetWarehouses().Single(x => x.Id == obj.Value.WarehouseId);
                _warehouseInventoryViewModel.Refresh(warehouse.Id);
                ActivateInventoryView();
            }
        }


        private void OnWarehouseEvent(EventParameters<Warehouse> obj)
        {
            if (obj.Topic == EventTopicNames.DisplayInventory)
            {
                _warehouseInventoryViewModel.Refresh(obj.Value.Id);
                ActivateInventoryView();
            }
        }

        protected override void OnNavigate(string obj)
        {
            base.OnNavigate(obj);
            _warehouseInventoryViewModel.Refresh(_cacheService.GetWarehouses().First().Id);
        }

        protected override void OnInitialization()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(WarehouseInventoryView));
        }

        private void ActivateInventoryView()
        {
            _regionManager.Regions[RegionNames.MainRegion].Activate(_warehouseInventoryView);
        }

        public override object GetVisibleView()
        {
            return _warehouseInventoryView;
        }
    }
}
