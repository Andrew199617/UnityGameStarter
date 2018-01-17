using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Managers
{
    /// <summary>
    /// Handles the changing of "Scenes" in Unity. Scenes are just game objects set
    /// inside the unity editor.
    /// </summary>
    public class SceneManager : MonoBehaviour
    {
        /// <summary>
        /// This Screen is shown when the game is over.
        /// </summary>
        [SerializeField] private GameObject gameOverScreen;

        /// <summary>
        /// An Empty GameObject that holds the LightCycle, Walls and anything else in the game.
        /// </summary>
        [SerializeField] private GameObject game;

        /// <summary>
        /// An Empty GameObject that holds all the UI for the Start Screen.
        /// </summary>
        [SerializeField] private GameObject startScreen;

        /// <summary>
        /// An Empty GameObject that holds all the UI for the How To Play Screen.
        /// </summary>
        [SerializeField] private GameObject howToPlayScreen;

        /// <summary>
        /// An Empty GameObject that holds all the UI for the Settings.
        /// </summary>
        [SerializeField] private GameObject settingsScreen;

        /// <summary>
        /// For using the class as a singleton.
        /// </summary>
        public static SceneManager SceneManagerInst;

        /// <summary>
        /// Refrence to the gameOverText Object in the Unity Scene.
        /// </summary>
        private GameObject gameOverText;

        /// <summary>
        /// Initialize as a Singleton.
        /// </summary>
        public void Start()
        {
            if (SceneManagerInst)
            {
                Destroy(SceneManagerInst);
            }
            gameOverText = gameOverScreen.transform.Find("GameOverText").gameObject;
            SceneManagerInst = this;
        }
        
        /// <summary>
        /// Spawns the Game Over Screen and Stops Time.
        /// </summary>
        public void ShowGameOverScreen()
        {
            HideScreens();
            gameOverScreen.SetActive(true);
        }

        /// <summary>
        /// Sets the Game Screen as the only active screen.
        /// </summary>
        public void ShowGame()
        {
            HideScreens();
            game.SetActive(true);
        }

        /// <summary>
        /// Disables every screen before enabling the start screen.
        /// </summary>
        public void ShowStartScreen()
        {
            HideScreens();
            startScreen.SetActive(true);
        }

        /// <summary>
        /// Disables every screen before enabling the how to play screen.
        /// </summary>
        public void ShowHowToPlayScreen()
        {
            HideScreens();
            howToPlayScreen.SetActive(true);
        }

        /// <summary>
        /// Finds the screen variable of screenName in SceneManager and sets it as the only active screen
        /// </summary>
        /// <param name="screenName">Name of variable to set active. See Unity Serialized fields.</param>
        public void ShowScreen(string screenName)
        {
            var screen = Reflection.GetField(typeof(SceneManager), screenName).GetValue(this) as GameObject;
            
            if (screen)
            {
                HideScreens();
                screen.SetActive(true);
            }
            else
            {
                Debug.LogError("Could not find field " + screenName + " in SceneManager");
            }
            
        }

        /// <summary>
        /// Hides every screen.
        /// </summary>
        private void HideScreens()
        {
            gameOverScreen.SetActive(false);
            startScreen.SetActive(false);
            game.SetActive(false);
            howToPlayScreen.SetActive(false);
            settingsScreen.SetActive(false);
        }

    }
}
