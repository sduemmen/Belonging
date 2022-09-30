namespace InventorySystem
{
    public interface IInteractableInventory
    {
        public void Interact();
        public void EndInteraction();
        public bool InteractionTriggered();
    }
}