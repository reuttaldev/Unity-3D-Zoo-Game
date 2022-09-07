using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //[Header("Buttons"), SerializeField]
    //private Button pauseButton, returnToMenuButton, exitGameButton;
    [Header("Panels"), SerializeField]
    private GameObject pauseMenu,optionsMenu, qualityPanel, keySettingsPanel,audioPanel;

    internal bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region PAUSE MENU
    private void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

    }
    public void OpenOptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
    #region OPTIONS MENU
    public void BackToPauseMenu()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);

    }
    public void OpenQualityPanel()
    {
        qualityPanel.SetActive(true);
        keySettingsPanel.SetActive(false);
        audioPanel.SetActive(false);
    }
    public void OpenKeySettingPanel()
    {
        qualityPanel.SetActive(false);
        keySettingsPanel.SetActive(true);
        audioPanel.SetActive(false);
    }
    public void OpenAudioPanel()
    {
        qualityPanel.SetActive(false);
        keySettingsPanel.SetActive(false);
        audioPanel.SetActive(true);
    }
    #endregion
}
