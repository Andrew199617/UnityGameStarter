using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMethods : MonoBehaviour {

    /// <summary>
    /// The Play button was pressed. Open Level 1.
    /// </summary>
    public void PlayPressed()
    {
        GameManager.GameManagerInst.StartGame();
    }

    /// <summary>
    /// Set TimeScale back to 1 and Hide GameOverScreen.
    /// Also reset all players.
    /// </summary>
    public void RetryButton()
    {
        SceneManager.LoadScene("Game1");
    }

    /// <summary>
    /// Quit Button pressed close the application.
    /// </summary>
    public void QuitPressed()
    {
        Application.Quit();
    }

    /// <summary>
    /// Loads the Single Player Game Mode.
    /// </summary>
    public void SinglePlayerPressed()
    {
        //SceneManager.LoadScene("Game1");
    }

    /// <summary>
    /// Loads the Single Player Game Mode.
    /// </summary>
    public void TwoPlayerPressed()
    {
        SceneManager.LoadScene("Game1");
    }

    /// <summary>
    /// Opens up the main menu.
    /// </summary>
    public void OpenTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
