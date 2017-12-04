using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    /// For using the class as a singleton.
    /// </summary>
    public static GameManager GameManagerInst;

    /// <summary>
    /// Initialize as a Singleton.
    /// </summary>
    public void Start()
    {
        if (!GameManagerInst)
        {
            GameManagerInst = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Spawns the Game Over Screen and Stops Time.
    /// </summary>
    public void GameEnded()
    {
        GameOverScreen.SetActive(true);
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
    /// Spawns the Walls and LightCycle that are in the GameObject Game.
    /// Set TimeScale back to 1
    /// </summary>
    public void ShowStartScreen()
    {
        ResetGameValues();
        startScreen.SetActive(true);
        GameOverScreen.SetActive(false);
        Game.SetActive(false);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Set TimeScale back to 1 and Hide GameOverScreen.
    /// Also reset all players.
    /// </summary>
    public void RetryGame()
    {
        Game.SetActive(true);
        GameOverScreen.SetActive(false);
        ResetGameValues();
        Time.timeScale = 1;
    }

    /// <summary>
    /// Resets Player and anything else needed to replay game.
    /// </summary>
    private static void ResetGameValues()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        players.ToList().ForEach(p => p.GetComponent<Player>().Reset());
    }
}
