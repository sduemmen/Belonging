using BuildSystem;
using InventorySystem;
using QuestSystem;
using UnityEngine.SceneManagement;

namespace Flags
{
    public static class GameFlags
    {
        public static bool MAIN_MENU_ACTIVE => SceneManager.GetActiveScene().name == "MainMenuScene";
        
        
        public static bool GAME_PAUSED => GameStateController.Instance.GamePaused;
        public static bool GAME_RUNNING => !GameStateController.Instance.GamePaused;

        
        public static bool INVENTORY_OPEN => InventoryController.Instance.DisplayContextActive;
        public static bool INVENTORY_CLOSED => !InventoryController.Instance.DisplayContextActive;
        public static bool BUILD_MENU_OPEN => BuildingController.Instance.DisplayContextActive;
        public static bool BUILD_MENU_CLOSED => !BuildingController.Instance.DisplayContextActive;
        public static bool QUEST_DISPLAY_OPEN => QuestController.Instance.DisplayContextActive;
        public static bool QUEST_DISPLAY_CLOSED => !QuestController.Instance.DisplayContextActive;

        public static bool AXE_EQUIPPED;
        public static bool PICKAXE_EQUIPPED;
        public static bool HAMMER_EQUIPPED;

        public static bool SLOT_EQUIPPED => AXE_EQUIPPED || PICKAXE_EQUIPPED || HAMMER_EQUIPPED;
        public static bool UI_ELEMENT_OPEN => BUILD_MENU_OPEN || INVENTORY_OPEN || QUEST_DISPLAY_OPEN;
    }
}