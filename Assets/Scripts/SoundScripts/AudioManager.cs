using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Background Musics")]
    public AudioClip playModeBGM;
    public AudioClip editModeBGM;
    public AudioClip mainMenuBGM;

    [Header("Sound Effects")]
    public Sounds[] soundsUI;
    public Sounds[] soundsPlayMode;
    public Sounds[] soundsEditMode;

    public static AudioManager instance;

    private void Awake()
    {
        //Check if object already exist, destroy if it does
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


    }

    private void Start()
    {
        PopulateSoundArrays(soundsUI);
        PopulateSoundArrays(soundsPlayMode);
        PopulateSoundArrays(soundsEditMode);
    }

    private void PopulateSoundArrays(Sounds[] sArrays)
    {
        foreach (Sounds sound in sArrays)
        {
            //Add Audio source component for sound
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            //Input sound variables into new component
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.ogAudio = sound.volume;
        }
    }

    public void UISounds(string name)
    {
        //Find the sound in the array base on a name parameter
        Sounds s = Array.Find(soundsUI, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound of name: " + name + " is not found");
            return;
        }

        s.source.Play();

    }

    public void PlayModeSound(string name)
    {
        //Find the sound in the array base on a name parameter
        Sounds s = Array.Find(soundsPlayMode, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound of name: " + name + " is not found");
            return;
        }

        s.source.Play();

    }

    public void EditModeSound(string name)
    {
        //Find the sound in the array base on a name parameter
        Sounds s = Array.Find(soundsEditMode, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound of name: " + name + " is not found");
            return;
        }

        s.source.Play();
    }

    public AudioSource FindAudioSource(Sounds[] targetArray, string name)
    {
        Sounds s = Array.Find(targetArray, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound of name: " + name + " is not found in " + targetArray + " sound array" );
            return null;
        }
        return s.source;
    }


}
