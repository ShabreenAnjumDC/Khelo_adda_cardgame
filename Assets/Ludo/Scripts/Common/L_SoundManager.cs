using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_SoundManager : MonoBehaviour
{
    public static L_SoundManager instance;

    //[SerializeField] private AudioSource sfxAudioSource;
    //public AudioClip[] sounds;
    [SerializeField]
    private GameObject[] soundPrefabs;
    public int volume;

    [Space(20)]
    public bool isVibrate;
    public bool isSound;
    public bool isNotification;



    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(instance);

        if (!PlayerPrefs.HasKey("Sound"))
        {
            PlayerPrefs.SetString("Sound", "true");
            isSound = true;
            Debug.Log("sound key set");
        }
        else
        {
            SoundCheck();
            Debug.Log("sound key set from PREF");
        }

        if (!PlayerPrefs.HasKey("Vibration"))
        {
            PlayerPrefs.SetString("Vibration", "true");
            isVibrate = true;
        }
        else
        {
            VibrationCheck();
        }

        if (!PlayerPrefs.HasKey("Notification"))
        {
            PlayerPrefs.SetString("Notification", "true");
            isNotification = true;
        }
        else
        {
            NotificationCheck();
        }
    }





    public void SoundCheck()
    {
        isSound = bool.Parse(PlayerPrefs.GetString("Sound"));
        //bool soundsCheck = bool.Parse(PlayerPrefs.GetString("Sound"));
        //isSound = soundsCheck;
        //if (soundsCheck)
        //{
        //    isSound = true;
        //    //volume = 1;
        //}
        //else
        //{
        //    isSound = false;
        //    //volume = 0;
        //}
    }

    public void SetSoundSetting(bool isTrue)
    {
        if (isTrue)
        {
            PlayerPrefs.SetString("Sound", "true");
            isSound = true;
        }
        else
        {
            PlayerPrefs.SetString("Sound", "false");
            isSound = false;
        }
    }





    public void VibrationCheck()
    {
        isVibrate = bool.Parse(PlayerPrefs.GetString("Vibration"));
    }

    public void SetVibrateSetting(bool isTrue)
    {
        if (isTrue)
        {
            PlayerPrefs.SetString("Vibration", "true");
            isVibrate = true;
        }
        else
        {
            PlayerPrefs.SetString("Vibration", "false");
            isVibrate = false;
        }
    }

    public void PlayVibrate(long milliseconds = 0)
    {
        if (!isVibrate)
            return;

        Handheld.Vibrate();

        //if (milliseconds > 0)
        //    Vibration.Vibrate(milliseconds);
        //else
        //    Vibration.Vibrate();
    }






    public void NotificationCheck()
    {
        isNotification = bool.Parse(PlayerPrefs.GetString("Notification"));
    }

    public void SetNotificationSetting(bool isTrue)
    {
        if (isTrue)
        {
            PlayerPrefs.SetString("Notification", "true");
            isNotification = true;
        }
        else
        {
            PlayerPrefs.SetString("Notification", "false");
            isNotification = false;
        }
    }





    public void PlaySound(L_SoundType soundType, Transform content)
    {
        GameObject gm = Instantiate(soundPrefabs[(int)soundType], content) as GameObject;
        //gm.GetComponent<AudioSource>().volume = volume;
        Destroy(gm, gm.GetComponent<AudioSource>().clip.length); ///*soundLength[(int)soundType]*/10
    }

    public void PlayLoopSound(L_SoundType soundType, Transform content)
    {
        if (content.Find(soundType.ToString()) != null)
            Destroy(content.Find(soundType.ToString()).gameObject);

        GameObject gm = Instantiate(soundPrefabs[(int)soundType], content) as GameObject;
        gm.name = soundType.ToString();
        //gm.GetComponent<AudioSource>().volume = volume;  // set from soundPrefabs's audio source volume
    }

    public void StopLoopSound(L_SoundType soundType, Transform content)
    {
        if (content.Find(soundType.ToString()) != null)
            Destroy(content.Find(soundType.ToString()).gameObject);
    }

    public bool IsSoundPlaying(L_SoundType soundType, Transform content)
    {
        if (content.Find(soundType.ToString()) != null)
            return true;

        return false;
    }

    //public enum ClipName
    //{
    //    DiceRoll,
    //    TokenMove
    //}

    //public void PlaySFX(ClipName clipName)
    //{
    //    if (!isSound)
    //        return;

    //    sfxAudioSource.clip = sounds[(int)clipName];
    //    sfxAudioSource.Play();
    //}
}

public enum L_SoundType
{
    MainAppSound,
    BoardGamePlaySound,
    WonSound,
    LoseSound,
    YourTurnSound,
    NotYourTurnSound,
    TimeOutSound,
    GotiCutSound,
    GotiInHomeSound,
    SafeZoneSound,
    ButtonSound
}

