using BuildSystem;
using InventorySystem;
using QuestSystem;

public static class Flags
{
    public static bool MAIN_MENU_ACTIVE => SceneManager.GetActiveScene().name == SceneManager.Scenes.MainMenuScene;

    public static bool GAME_PAUSED => GameStateController.Instance != null && GameStateController.Instance.GamePaused;
    public static bool GAME_RUNNING => GameStateController.Instance != null && !GameStateController.Instance.GamePaused;

    public static bool INVENTORY_OPEN => InventoryController.Instance != null && InventoryController.Instance.DisplayContextActive;
    public static bool INVENTORY_CLOSED => InventoryController.Instance != null && !InventoryController.Instance.DisplayContextActive;
    public static bool BUILD_MENU_OPEN => BuildingController.Instance != null && BuildingController.Instance.DisplayContextActive;
    public static bool BUILD_MENU_CLOSED => BuildingController.Instance != null && !BuildingController.Instance.DisplayContextActive;
    public static bool QUEST_DISPLAY_OPEN => QuestController.Instance != null && QuestController.Instance.DisplayContextActive;
    public static bool QUEST_DISPLAY_CLOSED => QuestController.Instance != null && !QuestController.Instance.DisplayContextActive;

    public static bool AXE_EQUIPPED => Player.Instance != null && Player.Instance.m_equippedSlot == 0;
    public static bool PICKAXE_EQUIPPED => Player.Instance != null && Player.Instance.m_equippedSlot == 1;
    public static bool HAMMER_EQUIPPED => Player.Instance != null && Player.Instance.m_equippedSlot == 2;

    public static bool SLOT_EQUIPPED => Player.Instance != null && Player.Instance.m_equippedSlot != -1;
    public static bool UI_ELEMENT_OPEN => BUILD_MENU_OPEN || INVENTORY_OPEN || QUEST_DISPLAY_OPEN;
}