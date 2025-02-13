using System;
/// <summary>
/// Enables a MonoBehaviour to listen to the beat of a song playing via the SongManager.
/// </summary>
public interface IEasyListener
{
    // Called every beat of an FMOD event playing via the SongManager.
    void OnBeat(EasyEvent currentAudioEvent);
}
