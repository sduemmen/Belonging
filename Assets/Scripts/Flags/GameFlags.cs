using UnityEngine.SceneManagement;

namespace Flags
{
    public static class GameFlags
    {
        public static bool GAME_PAUSED;
        public static bool MAIN_MENU_ACTIVE => SceneManager.GetActiveScene().name == "MainMenuScene";
        public static bool INVENTORY_OPEN;
        public static bool INVENTORY_CLOSED => !INVENTORY_OPEN;

        public static bool AXE_EQUIPPED;
        public static bool PICKAXE_EQUIPPED;
        public static bool HAMMER_EQUIPPED;

        public static bool SLOT_EQUIPPED => AXE_EQUIPPED || PICKAXE_EQUIPPED || HAMMER_EQUIPPED;
    }
}