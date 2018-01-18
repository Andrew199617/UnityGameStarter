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
        /// An GameObject that holds the Walls, floors, player, enemies, and anything else in the game.
        /// </summary>
        [SerializeField] private GameObject game;

        /// <summary>
        /// An GameObject that holds the Walls, floors, player, enemies, and anything else in the game.
        /// </summary>
        [SerializeField] private GameObject gamePrefab;

        /// <summary>
        /// A GameObject that holds all the UI for the Start Screen.
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
        /// Initialize as a Singleton.
        /// </summary>
        public void Awake()
        {
            if (SceneManagerInst)
            {
                Destroy(SceneManagerInst);
            }
            SceneManagerInst = this;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded +=
                (scene, mode) =>
                {
                    GameManager.GameManagerInst.ShowStartScreen();
                };
        }
        
        /// <summary>
        /// Spawns the Game Over Screen and Stops Time.
        /// </summary>
        public void ShowGameOverScreen()
        {
            HideScreens();
            SetActiveScene(gameOverScreen);
        }

        private void SetActiveScene(GameObject screen)
        {
            screen.SetActive(true);
            if (Screen.orientation == ScreenOrientation.Landscape || Screen.width > Screen.height)
            {
                screen.transform.Find("Landscape").gameObject.SetActive(true);
            }
            else
            {
                screen.transform.Find("Portrait").gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Sets the Game Screen as the only active screen.
        /// </summary>
        public void ShowGame()
        {
            HideScreens();
            Screen.orientation = ScreenOrientation.Landscape;
            game.SetActive(true);
        }

        /// <summary>
        /// Disables every screen before enabling the start screen.
        /// </summary>
        public void ShowStartScreen()
        {
            HideScreens();
            Screen.orientation = ScreenOrientation.AutoRotation;
            SetActiveScene(startScreen);
        }

        /// <summary>
        /// Reinstantiates the Game GameObject so that the level will restart.
        /// </summary>
        public void ResetGame()
        {
            var gameTransform = game.transform;
            Destroy(game);
            game = Instantiate(gamePrefab, gameTransform.position, gameTransform.rotation,
                gameTransform.parent);
        }

        /// <summary>
        /// Disables every screen before enabling the how to play screen.
        /// </summary>
        public void ShowHowToPlayScreen()
        {
            HideScreens();
            SetActiveScene(howToPlayScreen);
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
                SetActiveScene(screen);
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
