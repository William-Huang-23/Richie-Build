using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BuildManager : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    private QuizManager quiz_manager;

    [Header("DATA")]

    private float countdown = 6;

    private int index = -1;
    
    private int current = 0;
    private int total = 20;

    private float time = 301;   // 301 (5 mins)

    private int total_right = 0;
    private int total_wrong = 0;

    private bool game_start_status = false;
    private bool game_over_status = false;

    private int[] build_health = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int total_health = 0;

    private int total_nails = 0;
    private float insurance_duration = 0;
    private float insurance_cooldown = 0;
    
    private bool bear_active = false;
    private int bear_health = -10;  // -10 indicates bear have not been activated ever

    [Header("OBJECT")]

    [SerializeField]
    private GameObject[] build;

    [SerializeField]
    private GameObject richie;

    [SerializeField]
    private GameObject bear;

    [Header("UI")]

    [SerializeField]
    private GameObject quiz;

    [SerializeField]
    private GameObject pop_up_build;
    [SerializeField]
    private GameObject pop_up_fix;
    [SerializeField]
    private GameObject pop_up_utility;
    [SerializeField]
    private GameObject pop_up_highlight;

    [SerializeField]
    private Text build_left;
    [SerializeField]
    private Slider progress_bar;
    [SerializeField]
    private Text time_left;

    [SerializeField]
    private Text total_nails_text;
    [SerializeField]
    private Text insurance_duration_text;
    [SerializeField]
    private Text insurance_cooldown_text;

    [SerializeField]
    private Text countdown_text;

    [SerializeField]
    private GameObject game_over;
    [SerializeField]
    private Text announcement_text;
    [SerializeField]
    private Text score_text;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        quiz_manager = GameObject.Find("Game Manager").GetComponent<QuizManager>();

        audio_manager.play_sound("Countdown");

        if (audio_manager.is_playing("In Game") == false)
        {
            audio_manager.play_sound_loop("In Game");
        }
    }

    void Update()
    {
        if (game_start_status == false)
        {
            countdown = countdown - Time.deltaTime;

            if ((int)countdown <= 2)
            {
                countdown_text.text = "Mulai!";
            }
            else
            {
                countdown_text.text = "" + ((int)countdown - 2).ToString();
            }

            if (countdown <= 0.5)
            {
                countdown_text.gameObject.SetActive(false);

                richie.GetComponent<Richie>().approach();

                game_start_status = true;
            }
        }
        else if (game_over_status == false)
        {
            time = time - Time.deltaTime;

            if (insurance_duration > 0)
            {
                insurance_duration = insurance_duration - Time.deltaTime;
            }
            else
            {
                insurance_duration = 0;

                insurance_cooldown = insurance_cooldown - Time.deltaTime;
            }
            
            build_left.text = current + "/" + total;
            progress_bar.value = ((float)total_health / 40 * 100) / 100;
            time_left.text = ((int)time / 60).ToString() + "m " + ((int)time % 60).ToString() + "s";

            total_nails_text.text = total_nails.ToString();

            if (insurance_duration > 0)
            {
                insurance_duration_text.text = ((int)insurance_duration).ToString();
            }
            else
            {
                insurance_duration_text.text = "";
            }

            if (insurance_cooldown > 0 && insurance_duration == 0)
            {
                insurance_cooldown_text.text = ((int)insurance_cooldown).ToString();
            }
            else
            {
                insurance_cooldown_text.text = "";
            }

            if (current >= 8 && current <= 12 && bear_health == -10)    // activate bear
            {
                if (current >= 12)
                {
                    richie.GetComponent<Richie>().escape();

                    bear_active = true;
                    bear_health = 5;

                    audio_manager.play_sound("Panic");

                    bear.GetComponent<Bear>().approach();
                }
                else if (Random.Range(0, 3) == 1)   // 33%
                {
                    richie.GetComponent<Richie>().escape();

                    bear_active = true;
                    bear_health = 5;

                    audio_manager.play_sound("Panic");

                    bear.GetComponent<Bear>().approach();
                }
            }

            if (bear_active == true && audio_manager.get_volume("In Game") > 0) // reduce volume gradually when bear
            {
                audio_manager.set_volume("In Game", audio_manager.get_volume("In Game") - 0.01f);
            }

            if (bear_active == false && audio_manager.get_volume("In Game") < 1) // increase volume gradually when no bear
            {
                audio_manager.set_volume("In Game", audio_manager.get_volume("In Game") + 0.01f);
            }

            if (current == total && total_health == 40 && game_over_status == false)
            {
                win();
            }

            if (time <= 1 && game_over_status == false)
            {
                lose();
            }
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

    public bool get_game_start_status()
    {
        return game_start_status;
    }

    public bool get_game_over_status()
    {
        return game_over_status;
    }

    public void reduce_build_health(int index)
    {
        audio_manager.play_sound("Decay");

        build_health[index]--;
        total_health--;
    }

    public int get_build_health(int index)
    {
        return build_health[index];
    }

    public int get_total_nails()
    {
        return total_nails;
    }

    public float get_insurance_cooldown()
    {
        return insurance_cooldown;
    }

    public void disable_bear_active()
    {
        bear_active = false;
    }

    public bool get_bear_active()
    {
        return bear_active;
    }

    public void right()
    {
        audio_manager.play_sound("Correct");

        total_right++;

        if (bear_active == true)
        {
            bear_health--;

            if (bear_health <= 0)
            {
                audio_manager.stop_sound("Panic");

                richie.GetComponent<Richie>().approach();

                bear.GetComponent<Bear>().escape();
            }
        }
        else if (index < 20) // build
        {
            build_health[index] = 2;

            current++;
            total_health = total_health + 2;
        }
        else if (index == 20)   // nails
        {
            total_nails = total_nails + 5;
        }
        else if (index == 21)   // insurance
        {
            insurance_duration = 31;
            insurance_cooldown = 11;
        }

        deactivate_all_pop_ups();

        index = -1;
    }

    public void wrong()
    {
        total_wrong++;

        if (insurance_duration > 0)  // no penalty
        {
            audio_manager.play_sound("Shield");

            insurance_duration = 0;
        }
        else    // penalty
        {
            audio_manager.play_sound("Wrong");
            audio_manager.play_sound("Break");

            time = time - 10;

            if (time < 0)
            {
                time = 0;
            }

            for (int index = 0; index < 20; index++)
            {
                if (Random.Range(1, 4) == 1 && build_health[index] > 0)
                {
                    build_health[index]--;
                    total_health--;

                    if (build_health[index] == 0)
                    {
                        current--;
                    }
                }
            }
        }

        deactivate_all_pop_ups();

        index = -1;
    }

    public void destroy()
    {
        total_wrong++;

        if (insurance_duration > 0)  // no penalty
        {
            audio_manager.play_sound("Shield");

            insurance_duration = 0;
        }
        else    // penalty
        {
            audio_manager.play_sound("Break");

            for (int index = 0; index < 20; index++)
            {
                if (Random.Range(1, 4) == 1 && build_health[index] > 0)
                {
                    build_health[index]--;
                    total_health--;

                    if (build_health[index] == 0)
                    {
                        current--;
                    }
                }
            }
        }

        deactivate_all_pop_ups();

        index = -1;
    }

    public void fix()
    {
        if (total_nails > 0)
        {
            audio_manager.play_sound("Fix");

            build_health[index] = 2;
            total_health++;

            total_nails--;

            deactivate_all_pop_ups();
        }
    }

    public void deactivate_all_pop_ups()
    {
        pop_up_build.SetActive(false);
        pop_up_fix.SetActive(false);
        pop_up_utility.SetActive(false);
        pop_up_highlight.SetActive(false);
    }

    public void continue_()
    {
        SceneManager.LoadScene("Enter Credentials");
    }

    void win()
    {
        audio_manager.play_sound("Win");
        audio_manager.stop_sound("In Game");

        richie.GetComponent<Richie>().game_over(true);

        Data.score = 1000 * time / 60 - total_wrong * 20;

        announcement_text.text = "Kamu Menang!";
        score_text.text = "Skor Akhir: " + (int)Data.score;

        quiz.SetActive(false);
        game_over.SetActive(true);

        game_over_status = true;
    }

    void lose()
    {
        audio_manager.play_sound("Lose");
        audio_manager.stop_sound("In Game");

        richie.GetComponent<Richie>().game_over(false);

        Data.score = total_right * 20;

        announcement_text.text = "Kamu Kalah!";
        score_text.text = "Skor Akhir: " + (int)Data.score;

        quiz.SetActive(false);
        game_over.SetActive(true);

        game_over_status = true;
    }
}
