using UnityEngine.SceneManagement;

namespace Flags
{
    public static class GameFlags
    {
        public static bool GAME_PAUSED;
        public static bool MAIN_MENU_ACTIVE => SceneManager.GetActiveScene().name == "MainMenuScene";
        public static bool INVENTORY_OPEN;
        public static bool INVENTORY_CLOSED => !INVENTORY_OPEN;

        public static bool INVENTORY_SLOT1_EQUIPPED;
        public static bool INVENTORY_SLOT2_EQUIPPED;
        public static bool INVENTORY_SLOT3_EQUIPPED;

        public static bool SLOT_EQUIPPED => INVENTORY_SLOT1_EQUIPPED || INVENTORY_SLOT2_EQUIPPED || INVENTORY_SLOT3_EQUIPPED;
    }
}