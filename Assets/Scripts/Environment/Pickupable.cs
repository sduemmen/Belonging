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
        [SerializeField] public bool droppedByPlayer;
        [SerializeField] private float _pickUpRadius = 1f;
        [SerializeField] public float pickUpDelay;
        [SerializeField] private SphereCollider _collider;

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
            
            InventoryController inventoryController = other.transform.GetComponent<InventoryController>();
            if (!inventoryController) return;

            bool itemCanBeCollected = inventoryController.Inventory.AddItem(_item, 1, droppedByPlayer);
            if (!itemCanBeCollected) return;
            
            Destroy(this.gameObject);
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
            PersistentItemData persistentData = new PersistentItemData(t.position, t.rotation, _item.prefab.name, droppedByPlayer, pickUpDelay);
            data.persistentItems.Add(persistentData);
        }
    }
}
