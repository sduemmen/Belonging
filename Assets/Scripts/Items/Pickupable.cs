using InventorySystem;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(SphereCollider))]
    public class Pickupable : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private Item _item;
        [SerializeField] private float _pickUpRadius = 1f;
        [SerializeField] private float pickUpDelay;
        [SerializeField] private SphereCollider _collider;

        #region -- Getters --

        public Item GetItem()
        {
            return _item;
        }

        public float GetPickUpRadius()
        {
            return _pickUpRadius;
        }

        public SphereCollider GetCollider()
        {
            return _collider;
        }

        public void SetPickUpDelay(float delay_s)
        {
            pickUpDelay = delay_s;
        }

        #endregion

        private void OnValidate()
        {
            _collider.radius = _pickUpRadius;
        }
        
        private void Update()
        {
            pickUpDelay = Mathf.Max(pickUpDelay - Time.deltaTime, 0);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!CanBePickedUp()) return;
            
            InventoryHolders inventoryHolders = other.transform.GetComponent<InventoryHolders>();
            if (!inventoryHolders) return;
            
            foreach (InventoryHolder inventoryHolder in inventoryHolders.GetHolders()) {
                if (inventoryHolder.GetInventory().AddToInventory(_item, 1)) {
                    Destroy(this.gameObject);
                    return;
                }
            }
        }
        
        private bool CanBePickedUp()
        {
            return pickUpDelay <= 0;
        }

        public void LoadData(GameData data)
        {
            Destroy(this.gameObject);
        }

        public void SaveData(ref GameData data)
        {
            Transform t = transform;
            PersistentItemData persistentData = new PersistentItemData(t.position, t.rotation, _item.GetItemType());
            data.persistentGameObjects.Add(persistentData);
        }
    }
}
