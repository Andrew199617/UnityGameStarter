using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    /// <summary>
    /// This Screen is shown when the game is over.
    /// </summary>
    public GameObject GameOverScreen;

    /// <summary>
    /// An Empty GameObject that holds the LightCycle, Walls and anything else in the game.
    /// </summary>
    public GameObject Game;

    /// <summary>
    /// An Empty GameObject that holds all the UI for the Start Screen.
    /// </summary>
    [SerializeField] private GameObject startScreen;

    /// <summary>
    /// Refrence to the gameOverText Object in the Unity Scene.
    /// </summary>
    public GameObject GameOverText;


    private bool[] playersAlive = new bool[2];

    /// <summary>
    /// For using the class as a singleton.
    /// </summary>
    public static GameManager GameManagerInst;

    /// <summary>
    /// Initialize as a Singleton.
    /// </summary>
    public void Start()
    {
        GameManagerInst = this;
        SetPlayersAlive();
    }

    /// <summary>
    /// Set every player to be alive.
    /// </summary>
    private void SetPlayersAlive()
    {
        for (var playerIndex = 0; playerIndex < playersAlive.Length; ++playerIndex)
        {
            playersAlive[playerIndex] = true;
        }
    }
    
    public void PlayerDied(int DeadPlayer)
    {
        playersAlive[DeadPlayer] = false;

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
    public void GameEnded(int WinningPlayer)
    {
        GameOverScreen.SetActive(true);

        //Change text to say what player won
        var textComponent = GameOverText.GetComponent<Text>();
        textComponent.text = "Player " + WinningPlayer + " Won!";

        //Change all colors in game over screen to be winning players colors.
        textComponent.color = WinningPlayer == 1 ? Color.cyan : Color.red;
        var childImages = GameOverText.transform.parent.GetComponentsInChildren<Image>();
        foreach (var childImage in childImages)
        {
            childImage.color = WinningPlayer == 1 ? Color.cyan : Color.red;
        }

        //Pause the game so no movement occurs
        Time.timeScale = 0;
    }

    /// <summary>
    /// Spawns the Walls and LightCycle that are in the GameObject Game.
    /// Set TimeScale back to 1
    /// </summary>
    public void StartGame()
    {
        startScreen.SetActive(false);
        Game.SetActive(true);
        Time.timeScale = 1;
    }

    /// <summary>
    /// Set TimeScale back to 1 and Hide GameOverScreen.
    /// Also reset all players.
    /// </summary>
    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

}
