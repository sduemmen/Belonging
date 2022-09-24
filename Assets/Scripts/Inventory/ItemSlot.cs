using Items;

namespace Inventory
{
    public class ItemSlot
    {
        private Item _item;
        private int _quantity;
        public const int MAX_QUANTITY = 32;

        public ItemSlot(Item item, int quantity)
        {
            this._item = item;
            this._quantity = quantity;
        }

        public void SetItem(Item item)
        {
            this._item = item;
        }

        public void SetItem(Item item, int quantity)
        {
            this._item = item;
            this._quantity = quantity;
        }

        public void SetQuantity(int quantity)
        {
            this._quantity = quantity;
        }

        public bool CanAddAmount(int amount)
        {
            return this._quantity + amount <= MAX_QUANTITY;
        }

        public void AddAmount(int amount)
        {
            if (!CanAddAmount(amount)) return;
            this._quantity += amount;
        }

        public Item GetItem()
        {
            return _item;
        }

        public int GetQuantity()
        {
            return _quantity;
        }

        public void CheckEmpty()
        {
            if (this._quantity == 0) {
                this._item = new EmptyItem();
            }
        }

        public bool IsEmpty()
        {
            return _item.GetItemType() == ItemType.Empty;
        }
    }
}
