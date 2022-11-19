using InventorySystem;
using InventorySystem.Items;
using SaveSystem;
using SaveSystem.Data;
using UnityEngine;

namespace World
{
    [RequireComponent(typeof(SphereCollider))]
    public class Pickupable : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private MaterialItemObject _item;
        [SerializeField] private bool _droppedByPlayer;
        [SerializeField] private float _pickUpRadius = 1f;
        [SerializeField] private float _pickUpDelay;
        [SerializeField] private SphereCollider _collider;

        public bool DroppedByPlayer => _droppedByPlayer;
        public float PickUpDelay => _pickUpDelay;
        public bool CanBePickedUp => _pickUpDelay <= 0;

        public void Initialize(PersistentItemData persistentItemData)
        {
            _pickUpDelay = persistentItemData.pickUpDelay;
            _droppedByPlayer = persistentItemData.droppedByPlayer;
        }
        
        public void Initialize(float pickUpDelay, bool droppedByPlayer)
        {
            _pickUpDelay = pickUpDelay;
            _droppedByPlayer = droppedByPlayer;
        }
        
        private void Update()
        {
            _pickUpDelay = Mathf.Max(_pickUpDelay - Time.deltaTime, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!this.CanBePickedUp || !other.tag.Equals("Player")) return;

            bool itemHasBeenAddedToInventory = InventoryController.Instance.PlayerInventory.AddItem(_item, 1, _droppedByPlayer);
            
            if (itemHasBeenAddedToInventory)
            {
                this._collider.enabled = false;
                Destroy(this.gameObject);
            }
        }

        private void OnValidate()
        {
            _collider.radius = _pickUpRadius;
        }

        public void LoadData(GameData data)
        {
            Destroy(this.gameObject);
        }

        public void SaveData(ref GameData data)
        {
            Transform t = transform;
            PersistentItemData persistentData = new PersistentItemData(t.position, t.rotation, _item.Prefab.name, _droppedByPlayer, _pickUpDelay);
            data.persistentItems.Add(persistentData);
        }
    }
}