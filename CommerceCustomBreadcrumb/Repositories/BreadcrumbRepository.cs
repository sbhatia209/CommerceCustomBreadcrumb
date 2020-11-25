using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.XA.Foundation.Common.Providers;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.XA.Feature.Navigation.Models;
using Sitecore.XA.Foundation.Multisite;
using System.Collections.Generic;
using System.Linq;

namespace CommerceCustomBreadcrumb.Repositories
{
    public class BreadcrumbRepository : Sitecore.XA.Feature.Navigation.Repositories.Breadcrumb.BreadcrumbRepository
    {
        public IItemTypeProvider _itemTypeProvider { get; set; }
        public BreadcrumbRepository(IItemTypeProvider itemTypeProvider)
        {
            this._itemTypeProvider = itemTypeProvider;
        }

        protected override BreadcrumbRenderingModel CreateBreadcrumbModel(
            Item item,
            int index,
            int count,
            IEnumerable<BreadcrumbRenderingModel> children,
            string name)
        {
            BreadcrumbRenderingModel breadcrumbRenderingModel =
                base.CreateBreadcrumbModel(item, index, count, children, name);

            return breadcrumbRenderingModel;
        }

        public override IEnumerable<Item> BuildBreadcrumb(Item currentItem, Item rootItem)
        {
            List<Item> list = base.BuildBreadcrumb(currentItem, rootItem).ToList();

            //Add product node if current item is commerce product or category
            AddProductNode(currentItem, list);

            return list.AsEnumerable();
        }

        private void AddProductNode(Item currentItem, List<Item> list)
        {
            var isCommerceCategoryItem = IsCommerceCategoryItem(currentItem);
            var isProductItem = IsProductItem(currentItem);
            if (isCommerceCategoryItem || isProductItem)
            {
                var catalogRootItem = GetCatalogRootItem(currentItem);

                if (catalogRootItem != null)
                {
                    list.Insert(1, catalogRootItem);
                }
            }
        }

        private bool IsCommerceCategoryItem(Item item)
        {
            Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes itemType = this._itemTypeProvider.GetItemType(item);
            return itemType == Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes.Category;
        }

        private bool IsProductItem(Item item)
        {
            Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes itemType = this._itemTypeProvider.GetItemType(item);
            return itemType == Sitecore.Commerce.XA.Foundation.Common.Constants.ItemTypes.Product;
        }

        private Item GetCatalogRootItem(Item item = null)
        {
            var rootItem = GetSiteRootItem();

            if (rootItem == null)
            {
                return null;
            }

            var settingsItem = ServiceLocator.ServiceProvider.GetService<IMultisiteContext>().GetSettingsItem(item);

            if (settingsItem == null)
            {
                return null;
            }

            var rootItemField = settingsItem.Fields["Catalog Root Item"];

            if (rootItemField != null)
            {
                var rootItemFieldValue = rootItemField.Value;

                if (!string.IsNullOrEmpty(rootItemFieldValue))
                {
                    return Sitecore.Context.Database.GetItem(rootItemFieldValue);
                }
            }

            return null;
        }

        private Item GetSiteRootItem()
        {
            if (Sitecore.Context.Database != null || Sitecore.Context.Site != null)
            {
                return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);
            }

            return null;
        }
    }
}