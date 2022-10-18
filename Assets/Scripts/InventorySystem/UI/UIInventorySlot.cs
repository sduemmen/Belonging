using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class UIInventorySlot : MonoBehaviour
    {
        [SerializeField] protected Image _image;
        [SerializeField] protected TextMeshProUGUI _stackSizeLabel;
        [SerializeField] public InventoryDisplay parentDisplay;
        [SerializeField] private int _index;
        public bool clickable = true;

        public int Index {
            get => _index;
            set => _index = value;
        }

        public void OnInventorySlotClicked()
        {
            parentDisplay.OnSlotClicked?.Invoke(this);
            Debug.Log("slot clicked");
        }
        
        public virtual void Initialize(InventorySlot inventorySlot)
        {
            if (inventorySlot == null) {
                ClearSlot();
                Debug.LogWarning($"initialized UI slot with null on {this.gameObject.name}");
                return;
            }
            
            if (inventorySlot.IsEmpty()) {
                ClearSlot();
                Debug.LogWarning($"inventory slot is empty on {this.gameObject.name}");
                return;
            }
            
            _image.sprite = inventorySlot.Item.icon;
            _image.color = Color.white;
            _stackSizeLabel.text = inventorySlot.StackSize > 1 ? inventorySlot.StackSize.ToString() : "";
        }

        public void ClearSlot()
        {
            _image.sprite = null;
            _image.color = Color.clear;
            _stackSizeLabel.text = "";
        }
    }
}