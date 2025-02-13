using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour, IEasyListener
{
    public Text helloText;
    public Text beatText;
    public GameObject twentySeconds;

    // If a marker in FMOD is passed that contains the string "SayHello" (or "Say Hello", single spaces are allowed), this method will be called
    // The EasyEvent parameter is optional, however can be useful to get info from or to control the audio event, especially if the audio event is not from this object
    // *TIP* Don't forget to build in FMOD (F7) after adding a new marker & method!
    public void SayHello(EasyEvent audioEvent)
    {
        helloText.text = "Hello from bar " + audioEvent.CurrentBar + " of '" + audioEvent.EventName + "'!";
    }

    // Called from FMOD Example Project
    public void DisplayTwentySeconds()
    {
        twentySeconds.SetActive(true);
    }

    // This method HAS to be implemented to use IEasyListener.
    // IF you don't want anything behaviour on the beat, simply leave OnBeat's body blank.
    public void OnBeat(EasyEvent audioEvent)
    {
        // Text is updated on every beat
        beatText.text = audioEvent.CurrentBeat.ToString();
    }
}
