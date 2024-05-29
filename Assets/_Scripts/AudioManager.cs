using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource lavaRun;
    [SerializeField] private AudioSource lavaGun;
    [SerializeField] private AudioSource lavaSlide;
    [SerializeField] private AudioSource lavaMusic;
    [SerializeField] private AudioSource jungleRun;
    [SerializeField] private AudioSource jungleGun;
    [SerializeField] private AudioSource jungleSlide;
    [SerializeField] private AudioSource jungleMusic;
    [SerializeField] private AudioSource futurRun;
    [SerializeField] private AudioSource futurGun;
    [SerializeField] private AudioSource futurSlide;
    [SerializeField] private AudioSource futurMusic;

    public void PlayOrStopRun(bool run)
    {
        switch (PlayerPrefs.GetInt("WorldPreference"))
        {
            case 1:
                if (run && !lavaRun.isPlaying)
                {
                    lavaRun.Play();
                }
                else if (!run)
                {
                    lavaRun.Stop();
                }
                break;
            case 2:
                if (run && !jungleRun.isPlaying)
                {
                    jungleRun.Play();
                }
                else if (!run)
                {
                    jungleRun.Stop();
                }
                break;
            case 3:
                if (run && !futurRun.isPlaying)
                {
                    futurRun.Play();
                }
                else if (!run)
                {
                    futurRun.Stop();
                }
                break;
        }
    }

    public void PlayGun()
    {
        switch (PlayerPrefs.GetInt("WorldPreference"))
        {
            case 1:
                lavaGun.Play();
                break;
            case 2:
                jungleGun.Play();
                break;
            case 3:
                futurGun.Play();
                break;
        }
    }

    public void PlayOrStopMusic(bool play)
    {
        switch (PlayerPrefs.GetInt("WorldPreference"))
        {
            case 1:
                if (play)
                {
                    lavaMusic.loop = true;
                    lavaMusic.Play();
                }
                else
                {
                    lavaMusic.Stop();
                }
                break;
            case 2:
                if (play)
                {
                    jungleMusic.loop = true;
                    jungleMusic.Play();
                }
                else
                {
                    jungleMusic.Stop();
                }
                break;
            case 3:
                if (play)
                {
                    futurMusic.loop = true;
                    futurMusic.Play();
                }
                else
                {
                    futurMusic.Stop();
                }
                break;
        }
    }


    public void PlaySlide()
    {
        switch (PlayerPrefs.GetInt("WorldPreference"))
        {
            case 1:
                lavaSlide.Play();
                break;
            case 2:
                jungleSlide.Play();
                break;
            case 3:
                futurSlide.Play();
                break;
        }
    }
}
