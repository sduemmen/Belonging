using InventorySystem;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(SphereCollider))]
    public class Pickupable : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private MaterialItemObject _item;
        [SerializeField] private float _pickUpRadius = 1f;
        [SerializeField] private float _pickUpDelay;
        [SerializeField] private SphereCollider _collider;

        #region -- Getters --

        public float PickupDelay {
            get => _pickUpDelay;
            set => _pickUpDelay = value;
        }

        #endregion

        private void OnValidate()
        {
            _collider.radius = _pickUpRadius;
        }
        
        private void Update()
        {
            PickupDelay = Mathf.Max(PickupDelay - Time.deltaTime, 0);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!CanBePickedUp()) return;
            
            InventoryController inventoryController = other.transform.GetComponent<InventoryController>();
            if (!inventoryController) return;

            inventoryController.Inventory.AddItem(_item, 1); 
            Destroy(this.gameObject);
        }
        
        private bool CanBePickedUp()
        {
            return PickupDelay <= 0;
        }

        public void LoadData(GameData data)
        {
            Destroy(this.gameObject);
        }

        public void SaveData(ref GameData data)
        {
            Transform t = transform;
            PersistentItemData persistentData = new PersistentItemData(t.position, t.rotation, _item.prefab.name);
            data.persistentItems.Add(persistentData);
        }
    }
}
