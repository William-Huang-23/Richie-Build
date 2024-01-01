using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class EnterCode : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    [Header("UI")]

    [SerializeField]
    private InputField code;

    [SerializeField]
    private Text error_text;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            enter();
        }
    }

    public void enter()
    {
        audio_manager.play_sound("Button Sound");

        if (code.text != "")
        {
            var file = Resources.Load<TextAsset>(code.text.ToUpper());

            if (file != null)
            {
                Data.question_code = code.text.ToUpper();

                SceneManager.LoadScene("How to Play");
            }
            else
            {
                error_text.color = Color.red;
                error_text.text = "Kode soal invalid. Mohon dicoba ulang.";
            }
        }
    }

    public void credits()
    {
        audio_manager.play_sound("Button Sound");

        SceneManager.LoadScene("Credits");
    }
}
