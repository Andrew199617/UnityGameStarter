using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    /// <summary>
    /// This Class takes care of all logic for score and ending a game.
    /// Anything that has to do with the game concept.
    /// </summary>
    [RequireComponent(typeof(SceneManager))]
    public class GameManager : MonoBehaviour {

        /// <summary>
        /// GameManager keeps track of how many players are in the game.
        /// </summary>
        public bool[] playersAlive;

        /// <summary>
        /// For using the class as a singleton.
        /// </summary>
        public static GameManager GameManagerInst;

        /// <summary>
        /// Initialize as a Singleton.
        /// </summary>
        public void Awake()
        {
            if (GameManagerInst)
            {
                Destroy(GameManagerInst);
            }
            GameManagerInst = this;
            SetPlayersAlive(true);
        }

        /// <summary>
        /// Set every player to be isAlive
        /// </summary>
        private void SetPlayersAlive(bool isAlive)
        {
            for (var playerIndex = 0; playerIndex < playersAlive.Length; ++playerIndex)
            {
                playersAlive[playerIndex] = isAlive;
            }
        }
    
        /// <summary>
        /// Player will tell the GameManager when he died.
        /// GameManager will check if all players are dead before Ending the game.
        /// </summary>
        /// <param name="deadPlayer">The players index or possesingPlayer</param>
        public void PlayerDied(int deadPlayer)
        {
            playersAlive[deadPlayer] = false;

            int numPlayerAlive = 0;
            int playerAlive = 0;
            for (var playerIndex = 0; playerIndex < playersAlive.Length; ++playerIndex)
            {
                if (playersAlive[playerIndex])
                {
                    numPlayerAlive++;
                    playerAlive = playerIndex;
                }
            }
            if (numPlayerAlive == 1)
            {
                GameEnded(playerAlive + 1);
            }
        }

        /// <summary>
        /// Spawns the Game Over Screen and Stops Time.
        /// </summary>
        /// <param name="winningPlayer">Will print out player who won like this if winningPlayer = 1 "Player 1 Won!"</param>
        private void GameEnded(int winningPlayer)
        {
            SceneManager.SceneManagerInst.ShowGameOverScreen();

            ////Change text to say what player won
            //var textComponent = gameOverText.GetComponent<Text>();
            //textComponent.text = "Player " + winningPlayer + " Won!";

            ////Change all colors in game over screen to be winning players colors.
            //textComponent.color = winningPlayer == 1 ? Color.cyan : Color.red;
            //var childImages = gameOverScreen.GetComponentsInChildren<Image>();
            //foreach (var childImage in childImages)
            //{
            //    childImage.color = winningPlayer == 1 ? Color.cyan : Color.red;
            //}

            //Pause the game so no movement occurs
            Time.timeScale = 0;
        }

        /// <summary>
        /// Calls RetryGame.
        /// Set TimeScale back to 1
        /// </summary>
        public void StartGame()
        {
            RetryGame();
        }

        /// <summary>
        /// Set TimeScale back to 1 and Hide GameOverScreen.
        /// Also reset all players.
        /// </summary>
        public void RetryGame()
        {
            SceneManager.SceneManagerInst.ShowGame();

            ResetPlayers();
            StartCoroutine(AudioManager.audioManager.PlayRandomBackgroundMusic());

            Time.timeScale = 1;
        }

        private void ResetPlayers()
        {
            SetPlayersAlive(true);
            var players = FindObjectsOfType<Player>().ToList();
            players.ForEach(player => player.Reset());
        }

        /// <summary>
        /// Shows the start screen. Resets all players. Plays Random Menu Music.
        /// </summary>
        public void ShowStartScreen()
        {
            SceneManager.SceneManagerInst.ShowStartScreen();
            
            ResetPlayers();
            StartCoroutine(AudioManager.audioManager.PlayRandomMenuMusic());

            Time.timeScale = 0;
        }
    }
}
