using UnityEngine.UI;

namespace UI.InventorySystem
{
    public class ToolbarInventoryDisplay : InventoryDisplay
    {
        public Image highlightImage;
        
        protected override void Start()
        {
            InitializeInventorySlots(_inventory);
            highlightImage.gameObject.SetActive(false);
        }

        public void EnableHighlightAtIndex(int index)
        {
            highlightImage.gameObject.SetActive(true);
            highlightImage.transform.SetParent(_UIInventorySlots[index].transform, false);
        }
        
        public void DisableHighlight()
        {
            highlightImage.gameObject.SetActive(false);
        }
    }
}