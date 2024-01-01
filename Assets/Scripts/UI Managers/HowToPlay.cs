using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlay : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    [Header("DATA")]

    private int page = 0;
    [SerializeField]
    private int max_page = 0;

    [Header("UI")]

    [SerializeField]
    private Text number_text;

    [SerializeField]
    private GameObject[] image;

    [SerializeField]
    private GameObject prev_button;
    [SerializeField]
    private GameObject next_button;
    [SerializeField]
    private GameObject play_button;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        number_text.text = "1";

        image[0].SetActive(true);
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            audio_manager.play_sound("Button Sound");

            prev();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            audio_manager.play_sound("Button Sound");

            next();
        }*/
    }

    public void back()
    {
        SceneManager.LoadScene("Enter Code");
    }

    public void prev()
    {
        audio_manager.play_sound("Button Sound");

        image[page].SetActive(false);

        if (page == 0)
        {
            page = max_page;

            prev_button.gameObject.SetActive(true);
            next_button.gameObject.SetActive(false);
            play_button.gameObject.SetActive(true);
        }
        else
        {
            page--;

            if(page == 0)
            {
                prev_button.gameObject.SetActive(false);
                next_button.gameObject.SetActive(true);
                play_button.gameObject.SetActive(false);
            }
            else
            {
                prev_button.gameObject.SetActive(true);
                next_button.gameObject.SetActive(true);
                play_button.gameObject.SetActive(false);
            }
        }

        number_text.text = (page + 1).ToString();

        image[page].SetActive(true);
    }

    public void next()
    {
        audio_manager.play_sound("Button Sound");

        image[page].SetActive(false);

        if (page == max_page)
        {
            page = 0;

            prev_button.gameObject.SetActive(false);
            next_button.gameObject.SetActive(true);
            play_button.gameObject.SetActive(false);
        }
        else
        {
            page++;

            if (page == max_page)
            {
                prev_button.gameObject.SetActive(true);
                next_button.gameObject.SetActive(false);
                play_button.gameObject.SetActive(true);
            }
            else
            {
                prev_button.gameObject.SetActive(true);
                next_button.gameObject.SetActive(true);
                play_button.gameObject.SetActive(false);
            }
        }

        number_text.text = (page + 1).ToString();

        image[page].SetActive(true);
    }

    public void play()
    {
        audio_manager.play_sound("Button Sound");

        audio_manager.stop_sound("Forest Ambience");
        audio_manager.stop_sound("River Sound");

        SceneManager.LoadScene("In Game");
    }
}
