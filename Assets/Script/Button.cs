using UnityEngine.SceneManagement;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject startUI;

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void EnterSettingsMenu()
    {
        settingsMenu.SetActive(true);
        startUI.SetActive(false);
    }

    public void ExitSettingsMenu()
    {
        startUI.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Start");
    }
}
