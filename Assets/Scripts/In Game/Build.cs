using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Build : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    private BuildManager build_manager;

    [Header("DATA")]

    [SerializeField]
    private int index = 0;

    private float time_decay = 0;

    private Vector3 click_position;

    [Header("UI")]

    private Button button;

    [SerializeField]
    private GameObject pop_up_build;
    [SerializeField]
    private GameObject pop_up_fix;

    [SerializeField]
    private Sprite[] sprite;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        build_manager = GameObject.Find("Game Manager").GetComponent<BuildManager>();

        button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(build);
    }
    
    void Update()
    {
        if (build_manager.get_build_health(index) > 0)
        {
            change_sprite(build_manager.get_build_health(index) + 1);

            time_decay = time_decay + Time.deltaTime;
        }
        else if (build_manager.get_index() == index)
        {
            change_sprite(1);
        }
        else
        {
            change_sprite(0);
        }

        if (time_decay >= 20)
        {
            if (build_manager.get_build_health(index) == 2 && Random.Range(0, 20) == 0 && build_manager.get_game_over_status() == false)   // 5% chance to decay and cannot destroy
            {
                build_manager.reduce_build_health(index);
            }

            time_decay = 0;
        }
    }

    public void change_sprite(int index)
    {
        gameObject.GetComponent<Image>().sprite = sprite[index];
    }

    void build()
    {
        audio_manager.play_sound("Button Sound");

        if (build_manager.get_bear_active() == false)
        {
            if (build_manager.get_index() == index) // reset
            {
                build_manager.deactivate_all_pop_ups();

                build_manager.set_index(-1);
            }
            else if (build_manager.get_build_health(index) == 1)    // fix
            {
                build_manager.deactivate_all_pop_ups();

                pop_up_fix.SetActive(true);

                if (Application.isMobilePlatform == true)   // mobile anchor
                {
                    click_position = Input.GetTouch(0).position;
                }
                else    // pc anchor
                {
                    click_position = Input.mousePosition;
                }

                pop_up_fix.transform.position = new Vector3(click_position.x, click_position.y + Screen.height / 9, 0);

                build_manager.set_index(index);
            }
            else if (build_manager.get_build_health(index) <= 0) // open pop up
            {
                build_manager.deactivate_all_pop_ups();

                pop_up_build.SetActive(true);

                if (Application.isMobilePlatform == true)   // mobile anchor
                {
                    click_position = Input.GetTouch(0).position;
                }
                else    // pc anchor
                {
                    click_position = Input.mousePosition;
                }

                pop_up_build.transform.position = new Vector3(click_position.x, click_position.y + Screen.height / 9, 0);

                build_manager.set_index(index);
            }
        }
    }
}

