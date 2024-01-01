using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        if (audio_manager.is_playing("Forest Ambience") == false)
        {
            audio_manager.play_sound_loop("Forest Ambience");
        }

        if (audio_manager.is_playing("River Sound") == false)
        {
            audio_manager.play_sound_loop("River Sound");
        }

        read_file("Quiz List");
    }

    public void play()
    {
        audio_manager.play_sound("Button Sound");

        SceneManager.LoadScene("Enter Code");
    }

    void read_file(string code)
    {
        TextAsset file = Resources.Load<TextAsset>(code);    // load file dari folder resources

        string[] line = file.text.Split(' ');

        Data.quiz_list = line;
    }
}
