using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Richie : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    private BuildManager build_manager;

    [Header("DATA")]

    private Vector3 target_location;

    private int direction = 1;

    private bool moving = false;
    private bool building = false;

    private float building_time = 0;

    private bool escaping = false;

    [Header("ANIMATOR")]

    [SerializeField]
    private Animator animator;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        build_manager = GameObject.Find("Game Manager").GetComponent<BuildManager>();

        target_location = new Vector3(Random.Range(Screen.width / 10, Screen.width - Screen.width / 10), transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (build_manager.get_game_over_status() == false)
        {
            if (escaping == true)   // stuck when chased by bear
            {
                transform.position = transform.position + Vector3.left * 400 * Time.deltaTime;
            }
            else if (moving == true && building == false)
            {
                move();

                if ((transform.position.x >= target_location.x && direction == 1) || (transform.position.x <= target_location.x && direction == -1))    // build
                {
                    if (building == false)
                    {
                        audio_manager.play_sound("Fix");
                    }

                    moving = false;

                    building_time = Random.Range(2, 5);

                    animator.SetBool("Idle", true);

                    building = true;
                }
            }
            else if (moving == false && building == true)
            {
                building_time = building_time - Time.deltaTime;

                if (building_time <= 0) // start moving after building
                {
                    if (moving == false)
                    {
                        audio_manager.stop_sound("Fix");
                    }

                    building = false;

                    if (Random.Range(1, 9) > 2)
                    {
                        target_location = new Vector3(Random.Range(Screen.width / 10, Screen.width - Screen.width / 10), transform.position.y, transform.position.z);
                    }
                    else
                    {
                        if (Random.Range(1, 5) > 2)
                        {
                            target_location = new Vector3(0 - Screen.width / 5, transform.position.y, transform.position.z);
                        }
                        else
                        {
                            target_location = new Vector3(Screen.width + Screen.width / 5, transform.position.y, transform.position.z);
                        }
                    }

                    if (target_location.x > transform.position.x)
                    {
                        direction = 1;

                        animator.SetInteger("Direction", 1);
                    }
                    else
                    {
                        direction = -1;

                        animator.SetInteger("Direction", -1);
                    }

                    animator.SetBool("Idle", false);

                    moving = true;
                }
            }
        }
    }

    public void approach()
    {
        escaping = false;

        moving = true;

        transform.position = new Vector3(0 - Screen.width / 2, transform.position.y, transform.position.z); // teleport from stuck zone

        target_location = new Vector3(Random.Range(Screen.width / 10, Screen.width - Screen.width / 10), transform.position.y, transform.position.z);

        direction = 1;

        animator.SetInteger("Direction", 1);
    }

    public void escape()
    {
        escaping = true;

        moving = true;
        building = false;

        building_time = 0;

        target_location = new Vector3(0 - Screen.width, transform.position.y, transform.position.z);

        direction = -1;

        animator.SetInteger("Direction", -1);
        animator.SetBool("Idle", false);
    }

    public void game_over(bool win)
    {
        moving = false;
        building = false;

        if (win == true)
        {
            animator.SetBool("Win", true);
        }
        else
        {
            animator.SetBool("Lose", true);
        }
    }

    void move()
    {
        if (build_manager.get_bear_active() == true)
        {
            transform.position = transform.position + Vector3.right * 400 * direction * Time.deltaTime;
        }
        else
        {
            transform.position = transform.position + Vector3.right * 200 * direction * Time.deltaTime;
        }
    }
}
