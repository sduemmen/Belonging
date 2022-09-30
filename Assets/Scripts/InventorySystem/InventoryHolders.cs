using UnityEngine;

namespace InventorySystem
{
    public class InventoryHolders : MonoBehaviour
    {
        [SerializeField] private InventoryHolder[] _inventoryHolders;

        #region -- Getters --

        public InventoryHolder[] GetHolders()
        {
            return _inventoryHolders;
        }

        #endregion
    }
}
