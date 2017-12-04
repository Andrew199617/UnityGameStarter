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
    /// Quit Button pressed close the application.
    /// </summary>
    public void QuitPressed()
    {
        Application.Quit();
    }

    /// <summary>
    /// The Exit button was pressed. Open Main Menu.
    /// </summary>
    public void ExitToMainMenu()
    {
        GameManager.GameManagerInst.ShowStartScreen();
    }

}
