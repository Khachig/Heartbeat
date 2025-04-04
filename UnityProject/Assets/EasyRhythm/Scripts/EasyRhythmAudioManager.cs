using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class EasyRhythmAudioManager : MonoBehaviour
{
    // FMOD

    [EventRef] public string myEventPath; // A reference to the FMOD event we want to use
    public EasyEvent myAudioEvent; // EasyEvent is the object that will play the FMOD audio event, and provide our callbacks and other related info
    public PulsableManager pulsableManager;

    // You can pass an array of IEasyListeners through to the FMOD event, but we have to serialize them as objects.
    // You have to drag the COMPONENT that implements the IEasyListener into the object, or it won't work properly
    [RequireInterface(typeof(IEasyListener))]
    public Object[] myEventListeners;

    void Start()
    {
        // Passes the EventReference so EasyEvent can create the FMOD Event instance
        // Passes an array of listeners through (IEasyListener) so the audio event knows which objects want to listen to the callbacks
        // myAudioEvent = new EasyEvent(myEventPath, myEventListeners);
        // myAudioEvent.start();
    }

    public void ChangeTrack(string newEventPath)
    {
        if (myAudioEvent != null)
        {
            Debug.Log("audioEvenet isnt null");
            myAudioEvent.stop();
            myAudioEvent.RemoveAllListeners();
        } else {
            Debug.Log("audioEvent is null");
        }
        myAudioEvent = new EasyEvent(newEventPath, myEventListeners);
        myAudioEvent.start();
        pulsableManager.Init();
    }

    public void AddListener(IEasyListener listener)
    {
        myAudioEvent.AddListener(listener);
    }

    public void Reset()
    {
        // Reset the audio event to the beginning
        myAudioEvent.stop();
    }
}
