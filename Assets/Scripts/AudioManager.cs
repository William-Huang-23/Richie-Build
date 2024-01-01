using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [Header("CLASS")]

    [SerializeField]
    private Sound[] sound_list;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        foreach (Sound s in sound_list)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
        }
    }

    public void play_sound(string name)
    {
        Sound s = Array.Find(sound_list, sound => sound.name == name);
        s.source.Play();
    }

    public void play_sound_loop(string name)
    {
        Sound s = Array.Find(sound_list, sound => sound.name == name);
        s.source.Play();
        s.source.loop = true;
    }

    public float get_volume(string name)
    {
        Sound s = Array.Find(sound_list, sound => sound.name == name);

        return s.source.volume;
    }

    public void set_volume(string name, float value)
    {
        Sound s = Array.Find(sound_list, sound => sound.name == name);
        s.source.volume = value;
    }

    public void stop_sound(string name)
    {
        Sound s = Array.Find(sound_list, sound => sound.name == name);
        s.source.Stop();
    }

    public bool is_playing(string name)
    {
        Sound s = Array.Find(sound_list, sound => sound.name == name);

        return s.source.isPlaying;
    }
}
