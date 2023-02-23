using UnityEngine;
using UnityEngine.UI;

public class UI_MuteAudio : MonoBehaviour
{
    private AudioManager audioManager;
    private AudioSource bgmAudioSource;
    private bool bgm_muted = false;

    private AudioListener aL;

    // Start is called before the first frame update
    void Start()
    {
        aL = FindObjectOfType<AudioListener>();
        audioManager = FindObjectOfType<AudioManager>();
        bgmAudioSource = GameObject.Find("AudioManager").GetComponent<AudioSource>();
    }

    public void ToggleMuteAllAudio()
    {
        ToggleMute_BGM();
        ToggleMute_SoundEffects();
    }

    private void ToggleMute_BGM()
    {
        if(bgm_muted == false)
        {
            bgmAudioSource.mute = true;
            bgm_muted = true;
        }
        else
        {
            bgmAudioSource.mute = false;
            bgm_muted = false;
        }
        
    }

    private void ToggleMuteAll()
    {
        if(aL.enabled)
        {
            aL.enabled = false;
            return;
        }

        aL.enabled = true;

    }

    private void ToggleMute_SoundEffects()
    {
        if (audioManager.soundsUI[0].muted == false) //If not muted, set muted
        {
            foreach (Sounds sound in audioManager.soundsUI)
            {
                sound.source.mute = true;
                sound.muted = true;
            }
            foreach (Sounds sound in audioManager.soundsPlayMode)
            {
                sound.source.mute = true;
                sound.muted = true;
            }
            foreach (Sounds sound in audioManager.soundsEditMode)
            {
                sound.source.mute = true;
                sound.muted = true;
            }
        }
        else //Else if already muted, set back to original audio
        {
            foreach (Sounds sound in audioManager.soundsUI)
            {
                sound.source.mute = false;
                sound.muted = false;
            }
            foreach (Sounds sound in audioManager.soundsPlayMode)
            {
                sound.source.mute = false;
                sound.muted = false;
            }
            foreach (Sounds sound in audioManager.soundsEditMode)
            {
                sound.source.mute = false;
                sound.muted = false;
            }
        }
    }


    private void OnEnable()
    {
        if(bgm_muted)
        {
            Color color = GetComponent<VisualOnInteract>().toggledColor;
            GetComponent<VisualOnInteract>().FadeColor(color);
            Debug.Log("on enable toggeld color");
        }
    }

}
