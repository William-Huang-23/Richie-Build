using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bear : MonoBehaviour
{
    [Header("CLASS")]

    private BuildManager build_manager;

    [Header("DATA")]

    [SerializeField]
    private int index = 0;

    private Vector3 target_location;

    private int speed = 400;
    private int direction = -1;

    private bool moving = false;
    private bool tantrum = false;

    private float tantrum_time = 3;

    [Header("UI")]

    private Button button;

    [SerializeField]
    private GameObject utility_buttons;

    [Header("ANIMATOR")]

    [SerializeField]
    private Animator animator;

    void Start()
    {
        build_manager = GameObject.Find("Game Manager").GetComponent<BuildManager>();

        button = gameObject.GetComponent<Button>();

        button.onClick.AddListener(bear);

        target_location = new Vector3(Screen.width / 2, transform.position.y, transform.position.z);
    }
    
    void Update()
    {
        if (moving == true && tantrum == false)
        {
            move();

            if (transform.position.x >= Screen.width * 2)
            {
                gameObject.SetActive(false);
            }
            else if ((transform.position.x >= target_location.x && direction == 1) || (transform.position.x <= target_location.x && direction == -1))
            {
                moving = false;

                tantrum_time = 3;

                build_manager.destroy();

                animator.SetBool("Idle", true);

                tantrum = true;
            }
        }
        else if (moving == false && tantrum == true)
        {
            tantrum_time = tantrum_time - Time.deltaTime;

            if (tantrum_time <= 0)
            {
                tantrum = false;

                if (transform.position.x >= target_location.x && direction == 1)
                {
                    target_location = new Vector3(Screen.width / 10, transform.position.y, transform.position.z);
                }
                else if (transform.position.x <= target_location.x && direction == -1)
                {
                    target_location = new Vector3(Screen.width - Screen.width / 10, transform.position.y, transform.position.z);
                }

                speed = 50;

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

    public void approach()
    {
        moving = true;

        utility_buttons.gameObject.SetActive(false);
    }

    public void escape()
    {
        moving = true;
        tantrum = false;

        target_location = new Vector3(Screen.width * 2, transform.position.y, transform.position.z);

        speed = 400;
        direction = 1;

        utility_buttons.gameObject.SetActive(true);

        animator.SetTrigger("Escape");

        build_manager.disable_bear_active();
    }

    void bear()
    {
        build_manager.set_index(index);
    }

    void move()
    {
        transform.position = transform.position + Vector3.right * speed * direction * Time.deltaTime;
    }
}
