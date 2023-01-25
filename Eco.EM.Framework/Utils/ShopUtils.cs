using Eco.EM.Framework.Models;
using Eco.Gameplay.Items;
using Eco.Gameplay.Utils;
using Eco.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.EM.Framework.Utils
{
    public class ShopUtils
    {
        public static string GetListedItem(string ItemType)
        {
            Tag tag = null;
            Item item = null;
            List<Item> tagItem = null;
            var itemType = char.ToUpper(ItemType[0]) + ItemType[1..];
            try
            {
                tag = TagManager.GetTagOrFail(itemType);
                tagItem = Item.AllItems.Where(x => x.Tags().Contains(tag)).ToList();
            }
            catch (ArgumentException)
            {
                tag = null;
            }
            if (tag == null)
                item = CommandsUtil.ClosestMatchingEntity(null, itemType, Item.AllItems, x => x.GetType().Name, x => x.DisplayName);

            if (item == null && tag == null)
            {
                return $"[Informatics] Error: Unable to find item {ItemType} or a Tag with that name";
            }
            var sellOffers = new List<OfferedItem>();
            var buyOffers = new List<OfferedItem>();
            if (tagItem != null)
            {
                sellOffers = Informatics.GetSellingByItemType(tagItem);
                buyOffers = Informatics.GetBuyingByItemType(tagItem);
            }
            else
            {
                sellOffers = Informatics.GetSellingByItemType(item);
                buyOffers = Informatics.GetBuyingByItemType(item);
            }
            var message = string.Empty;

            if (sellOffers.Count > 0)
            {
                if (tag != null)
                {
                    message += $"{tag.DisplayName} can be bought from:\n";
                    sellOffers.OrderBy(o => o.Price).ForEach(o =>
                    {
                        if (o.Quantity > 0)
                        {
                            message += $"({o.tagItemName}) {o.Quantity} In Stock, For: <color=green>{o.Price} {o.Currency}</color> \tAt Store: <color=green>{o.StoreName}</color>\tStore Owner: <color=green>{o.StoreOwner}</color>\n";
                        }
                        else
                        {
                            message += $"<color=red>Out of Stock</color> ({o.tagItemName}), was at <color=green>{o.Price} {o.Currency}</color> \tAt Store: <color=green>{o.StoreName}</color>\tStore Owner: <color=green>{o.StoreOwner}</color>\n";
                        }
                    });
                }
                else
                {
                    message += $"{ItemType} can be bought from:\n";
                    sellOffers.OrderBy(o => o.Price).ForEach(o =>
                    {
                        if (o.Quantity > 0)
                        {
                            message += $"{o.Quantity} In Stock, For: <color=green>{o.Price} {o.Currency}</color>, \tAt Store: <color=green>{o.StoreName}</color> \tStore Owner: <color=green>{o.StoreOwner}</color>\n";
                        }
                        else
                        {
                            message += $"<color=red>Out of Stock</color>, was at <color=green>{o.Price} {o.Currency}</color> \tAt Store: <color=green>{o.StoreName}</color>\tStore Owner: <color=green>{o.StoreOwner}</color>\n";
                        }
                    });
                }
                message += "\n\n<color=yellow>----------------</color>\n\n";
            }

            if (buyOffers.Count > 0)
            {
                if (tag != null)
                {
                    message += $"{tag.DisplayName} can be sold at:\n";
                    buyOffers.OrderBy(o => o.Price).ForEach(o =>
                    {
                        message += $"Buying ({o.tagItemName}) {o.Quantity} for {o.Price} {o.Currency} \t At Store: <color=green>{o.StoreName}</color>\t Owner: {o.StoreOwner}\n";
                    });
                }
                else
                {
                    message += $"{ItemType} can be sold at:\n";
                    buyOffers.OrderBy(o => o.Price).ForEach(o =>
                    {
                        message += $"Buying {o.Quantity} for {o.Price} {o.Currency} \t  At Store: <color=green>{o.StoreName}</color>\t Owner: {o.StoreOwner}\n";
                    });
                }
            }

            if (message == string.Empty)
                message = "Unable to find anyone selling this item.";

            return message;
        }

        public static List<OfferedItem> GetAllItems(bool includeOutOfStock = false)
        {
            try
            {
                var sellOffers = new List<OfferedItem>();
                var buyOffers = new List<OfferedItem>();
                List<OfferedItem> Items = new();
                foreach (var item in Item.AllItems)
                {

                    sellOffers = Informatics.GetSellingByItemType(item);
                    buyOffers = Informatics.GetBuyingByItemType(item);
                    if (sellOffers.Count > 0)
                    {
                        sellOffers.OrderBy(o => o.Price).ForEach(o =>
                        {
                            switch (o.Quantity)
                            {
                                case > 0:
                                    Items.Add(new OfferedItem
                                    {
                                        StoreID = o.StoreID,
                                        tagItemName = o.tagItemName,
                                        Quantity = o.Quantity,
                                        Price = o.Price,
                                        Currency = o.Currency,
                                        StoreName = o.StoreName,
                                        StoreOwner = o.StoreOwner,
                                        ForSale = true
                                    });
                                    break;
                                case < 1:
                                    if (includeOutOfStock)
                                        Items.Add(new OfferedItem
                                        {
                                            StoreID = o.StoreID,
                                            tagItemName = o.tagItemName,
                                            Quantity = o.Quantity,
                                            Price = o.Price,
                                            Currency = o.Currency,
                                            StoreName = o.StoreName,
                                            StoreOwner = o.StoreOwner,
                                            ForSale = true
                                        });
                                    break;
                            }
                        });
                    }

                    if (!includeOutOfStock && buyOffers.Count > 0)
                    {
                        buyOffers.OrderBy(o => o.Price).ForEach(o =>
                        {
                            switch (o.Quantity)
                            {
                                case > 0:
                                    Items.Add(new OfferedItem
                                    {
                                        StoreID = o.StoreID,
                                        tagItemName = o.tagItemName,
                                        Quantity = o.Quantity,
                                        Price = o.Price,
                                        Currency = o.Currency,
                                        StoreName = o.StoreName,
                                        StoreOwner = o.StoreOwner,
                                        ForSale = false
                                    });
                                    break;
                                case < 1:
                                    if (includeOutOfStock)
                                        Items.Add(new OfferedItem
                                        {
                                            StoreID = o.StoreID,
                                            tagItemName = o.tagItemName,
                                            Quantity = o.Quantity,
                                            Price = o.Price,
                                            Currency = o.Currency,
                                            StoreName = o.StoreName,
                                            StoreOwner = o.StoreOwner,
                                            ForSale = false
                                        });
                                    break;
                            }
                        });
                    }
                }
                if (Items.Count > 0)
                    return Items;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
