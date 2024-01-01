using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utility : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    private BuildManager build_manager;

    [Header("DATA")]

    [SerializeField]
    private int index = 0;

    [Header("UI")]

    private Button button;

    [SerializeField]
    private GameObject pop_up_build;
    [SerializeField]
    private GameObject pop_up_fix;
    [SerializeField]
    private GameObject pop_up_utility;

    [SerializeField]
    private Sprite[] sprite;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        build_manager = GameObject.Find("Game Manager").GetComponent<BuildManager>();

        button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(utility);
    }
    
    void Update()
    {
        if (build_manager.get_index() == index)
        {
            change_sprite(0);
        }
        else
        {
            change_sprite(1);
        }
    }

    public void change_sprite(int index)
    {
        gameObject.GetComponent<Image>().sprite = sprite[index];
    }

    void utility()
    {
        audio_manager.play_sound("Button Sound");

        if (build_manager.get_bear_active() == false)
        {
            if (build_manager.get_index() == index) // reset
            {
                build_manager.deactivate_all_pop_ups();

                build_manager.set_index(-1);
            }
            else if (index == 21 && build_manager.get_insurance_cooldown() > 0) // cooldown
            {
                build_manager.deactivate_all_pop_ups();

                build_manager.set_index(-1);
            }
            else    // open pop up
            {
                build_manager.deactivate_all_pop_ups();

                pop_up_utility.SetActive(true);
                //pop_up_highlight.SetActive(true);

                pop_up_utility.transform.position = new Vector3(gameObject.transform.position.x + Screen.height / 20, gameObject.transform.position.y + Screen.height / 7, 0);
                //pop_up_highlight.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);

                build_manager.set_index(index);
            }
        }
    }
}
