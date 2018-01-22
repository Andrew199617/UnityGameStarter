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
        /// An GameObject that holds the player, enemies, and anything else that can change transform.
        /// </summary>
        [SerializeField] private GameObject movableObjects;

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
        /// Refrence to UICanvas onject in Unity.
        /// </summary>
        private GameObject uiCanvas;

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
#if !UNITY_EDITOR
            UnityEngine.SceneManagement.SceneManager.sceneLoaded +=
                (scene, mode) =>
                {
                    GameManager.GameManagerInst.ShowStartScreen();
                };
#else
            uiCanvas = GameObject.Find("UICanvas");
            uiCanvas.SetActive(false);
#endif
        }
        
        /// <summary>
        /// Spawns the Game Over Screen and Stops Time.
        /// </summary>
        public void ShowGameOverScreen()
        {
            HideScreens();
            ShowUiCanvas(true);
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
            ShowUiCanvas(false);
        }

        /// <summary>
        /// Finds the UICanvas in the scene and sets its active status to the bool param.
        /// </summary>
        /// <param name="active">Wether to hide or show canvas.</param>
        private void ShowUiCanvas(bool active)
        {
            if (!uiCanvas)
            {
                uiCanvas = GameObject.Find("UICanvas");
            }
            uiCanvas.gameObject.SetActive(active);
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
            var respawnArea = GameObject.Find("MovableObjects");
            if (!respawnArea)
            {
                respawnArea = GameObject.Find("MovableObjects(Clone)");
            }
            var oldParent = respawnArea.transform.parent;
            Destroy(respawnArea.gameObject);
            var newRespawnGameObject = Instantiate(movableObjects, oldParent);

            var followCamera = GameObject.Find("FollowCam");
            var cinemachineVirtualCamera = followCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = newRespawnGameObject.transform.Find("Player");
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
            howToPlayScreen.SetActive(false);
            settingsScreen.SetActive(false);
        }

    }
}
