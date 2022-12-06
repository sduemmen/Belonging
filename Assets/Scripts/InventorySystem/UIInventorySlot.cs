using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class UIInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _stackSizeLabel;
        [SerializeField] private int _index;
        [SerializeField] private bool _clickable = true;

        public TextMeshProUGUI StackSizeLabel => _stackSizeLabel;
        public int Index => _index;

        public void OnSlotClicked()
        {
            if (_clickable)
            {
                InventoryController.Instance.OnSlotClickedDelegate?.Invoke(this);
            }
        }

        public void Initialize(InventorySlot inventorySlot)
        {
            if (inventorySlot == null || inventorySlot.IsEmpty())
            {
                ClearSlot();
            }
            else
            {
                _image.sprite = inventorySlot.Item.Icon;
                _image.color = Color.white;
                _stackSizeLabel.text = inventorySlot.StackSize > 0 ? inventorySlot.StackSize.ToString() : "";
            }
        }

        public void Initialize(InventorySlot inventorySlot, bool clickable)
        {
            _clickable = clickable;
            Initialize(inventorySlot);
        }
        
        public void Initialize(InventorySlot inventorySlot, bool clickable, int index)
        {
            _index = index;
            Initialize(inventorySlot, clickable);
        }

        public void ClearSlot()
        {
            _image.sprite = null;
            _image.color = Color.clear;
            _stackSizeLabel.text = "";
        }
    }
}