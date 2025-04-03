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
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            bool buttonPressed = gamepad.aButton.wasPressedThisFrame;
            submitted = submitted || buttonPressed;
        }
        if (submitted) {
            if (selected == 0) OnPlayButton();
            else OnQuitButton();
        }

        bool upPressed = Keyboard.current.upArrowKey.wasPressedThisFrame || gamepad.leftStick.ReadValue().y < 0;
        bool downPressed = Keyboard.current.downArrowKey.wasPressedThisFrame || gamepad.leftStick.ReadValue().y > 0;

        if (upPressed) {
            selected = 1;
        }
        if (downPressed) {
            selected = 0;
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
        SceneManager.LoadScene("OpeningCutScene");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
