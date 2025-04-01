using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartMenu : MonoBehaviour
{
    public Button playButton, quitButton;

    private int selected = 0;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButton);
        playButton.onClick.AddListener(OnQuitButton);
    }

    void Update()
    {
        bool enterPressed = Keyboard.current.enterKey.wasPressedThisFrame;
        bool submitted = enterPressed;
        if (Gamepad.current != null)
        {
            bool buttonPressed = Gamepad.current.aButton.wasPressedThisFrame;
            submitted = submitted || buttonPressed;
        }
        if (enterPressed) {
            if (selected == 0) OnPlayButton();
            else OnQuitButton();
        }

        bool upPressed = Keyboard.current.upArrowKey.wasPressedThisFrame;
        bool downPressed = Keyboard.current.downArrowKey.wasPressedThisFrame;

        if (upPressed) {
            selected = selected == 1 ? 0 : 1;
        }
        if (downPressed) {
            selected = selected == 0 ? 1 : 0;
        }

        if (selected == 0) {
            playButton.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1f);
            quitButton.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        }
        else {
            playButton.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            quitButton.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1f);
        }

    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("SpecificArrowLanes");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
