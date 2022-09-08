using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIController : MonoBehaviour
{
    [Header("Panels"), SerializeField]
    private GameObject pauseMenu,optionsMenu, qualityPanel, keySettingsPanel,audioPanel;
    [SerializeField]
    private TMP_Dropdown resolutionDropdown, shadowsDropdown;
    [SerializeField]
    private AudioMixer audioMixer;
    // getting the player input action so we can remap it when needed 
    private PlayerInput playerInput;
    private InputAction playerMovementAction;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private Resolution[] resolutions; 
    private CustomUIInput UIInput;
    private bool isPaused = false;


    private void Awake()
    {
        UIInput = new CustomUIInput();

        // using the new input system to listen for pause
        //  _ => syntax as to not pass parameters to pause
        UIInput.PauseMenu.PauseGame.performed += _ => Pause();
    }
    private void Start()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);


        // get resolutions that are available to us 
        resolutions = Screen.resolutions;
        // add the available resolutions to our dropdown
        resolutionDropdown.ClearOptions();
        List<string> temp = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution r = resolutions[i];
            // check what is the defult resolution is 
            if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
                currentResIndex = i;
            temp.Add(r.width+" x "+r.height);
        }
        resolutionDropdown.AddOptions(temp);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
        playerInput = new PlayerInput();
        playerMovementAction = playerInput.asset.FindAction("Move");

    }
    private void OnEnable()
    {
        UIInput.PauseMenu.Enable();

    }
    private void OnDisable()
    {
        UIInput.PauseMenu.Disable();
    }
    #region PAUSE MENU
    private void Pause()
    {
        if(isPaused)
        {
            Resume();
            return;
        }
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        //AudioListener.pause = true;
    }
    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        //AudioListener.pause = false;

    }
    public void OpenOptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        OpenQualityPanel();
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

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("Volume", volume);
    }
    public void FullScreen(bool val)
    {
        Screen.fullScreen = val;    
    }
    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetShadowQuality(int index)
    {
        // changing the shadows in URP
        QualitySettings.SetQualityLevel(index, false);
    }

    public void SetKeyBinding(int index)
    {
        switch(index)
        {
            // Keyboard
            case 0: {;break; }
            //WASD
            case 1: {;break; }
        }
        // cannot change binding while we are using that key
        playerMovementAction.Disable();

        rebindingOperation = playerMovementAction.PerformInteractiveRebinding().OnComplete(operation => RebindComplete());
        rebindingOperation.Start();
    }

    private void RebindComplete()
    {
        rebindingOperation.Dispose();
        playerMovementAction.Enable();

    }

    #endregion
}
