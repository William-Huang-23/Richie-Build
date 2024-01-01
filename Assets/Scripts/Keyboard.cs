using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour
{
    [Header("CLASS")]
    
    private AudioManager audio_manager;

    [Header("DATA")]

    private int index = 0;

    [Header("UI")]

    [SerializeField]
    private InputField[] input_field;

    [SerializeField]
    private GameObject small, big, symbol;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();
    }

    public void input_alphabet(string alphabet)
    {
        audio_manager.play_sound("Keyboard Click");

        input_field[index].text = input_field[index].text + alphabet;
    }

    public void input_caps_alphabet(string alphabet)
    {
        audio_manager.play_sound("Keyboard Click");

        input_field[index].text = input_field[index].text + alphabet;
    }

    public void backspace()
    {
        audio_manager.play_sound("Keyboard Click");

        if (input_field[index].text.Length > 0) input_field[index].text = input_field[index].text.Remove(input_field[index].text.Length - 1);
    }

    public void close_all_layouts()
    {
        small.SetActive(false);
        big.SetActive(false);
        symbol.SetActive(false);
    }

    public void show_layout(GameObject SetLayout)
    {
        audio_manager.play_sound("Keyboard Click");

        close_all_layouts();
        SetLayout.SetActive(true);
    }

    public void activate()
    {
        if(Application.isMobilePlatform == true)
        {
            small.SetActive(true);
        }
    }

    public void set_index(int index)
    {
        this.index = index;
    }

    public int get_index()
    {
        return index;
    }
}
