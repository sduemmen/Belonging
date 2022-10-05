using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public class PlayerInventory : InventoryHolder, IInteractableInventory
    {
        public UnityAction<IInteractableInventory> OnInteractionComplete { get; set; }
        private bool isInteracting;

        #region -- Getters --

        public bool IsInteracting()
        {
            return isInteracting;
        }

        #endregion

        public void Interact()
        {
            isInteracting = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            OnDynamicInventoryDisplayContextRequested?.Invoke(_inventory);
        }

        public void EndInteraction()
        {
            isInteracting = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            OnDynamicInventoryDisplayContextClosed?.Invoke();
        }

        public bool InteractionTriggered()
        {
            return Keyboard.current.tabKey.wasPressedThisFrame;
        }

        private void Update()
        {
            bool interactionTriggered = InteractionTriggered();
            if (interactionTriggered && !isInteracting) Interact();
            else if (interactionTriggered && isInteracting) EndInteraction();
        }
    }
}