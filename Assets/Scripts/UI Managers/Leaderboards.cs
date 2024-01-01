using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class Leaderboards : MonoBehaviour
{
    [Header("DATA")]

    private string[] leaderboard_name = new string[20];
    private string[] leaderboard_score = new string[20];

    private bool leaderboard_ready = false;

    private int my_score_page = 0;
    private int current_page = 0;
    private int last_page = 0;

    [Header("CLASS")]

    private AudioManager audio_manager;

    [Header("UI")]

    [SerializeField]
    private Text rank;
    [SerializeField]
    private Text name;
    [SerializeField]
    private Text score;

    [SerializeField]
    private GameObject pop_up;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        get_leaderboard();
    }

    // public
    
    public void first()
    {
        audio_manager.play_sound("Button Sound");

        if (leaderboard_ready == true)
        {
            current_page = 0;

            show_leaderboard(current_page);
        }
    }

    public void prev()
    {
        audio_manager.play_sound("Button Sound");

        if (leaderboard_ready == true)
        {
            if (current_page > 0)
            {
                current_page--;

                show_leaderboard(current_page);
            }
        }
    }

    public void next()
    {
        audio_manager.play_sound("Button Sound");

        if (leaderboard_ready == true)
        {
            if (current_page < last_page)
            {
                current_page++;

                show_leaderboard(current_page);
            }
        }
    }

    public void last()
    {
        audio_manager.play_sound("Button Sound");

        if (leaderboard_ready == true)
        {
            current_page = last_page;

            show_leaderboard(current_page);
        }
    }

    public void refresh()
    {
        audio_manager.play_sound("Button Sound");

        if (leaderboard_ready == true)
        {
            SceneManager.LoadScene("Leaderboards");
        }
    }

    public void continue_()
    {
        audio_manager.play_sound("Button Sound");

        if (leaderboard_ready == true)
        {
            pop_up.SetActive(true);
        }
    }

    public void yes()
    {
        audio_manager.play_sound("Button Sound");

        SceneManager.LoadScene("Enter Code");
    }

    public void no()
    {
        audio_manager.play_sound("Button Sound");

        pop_up.SetActive(false);
    }

    // playfab

    void get_leaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = Data.question_code,
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, get_leaderboard_success, error);
    }

    void get_leaderboard_success(GetLeaderboardResult result)
    {
        int i = 0;
        int j = 0;

        foreach (var item in result.Leaderboard)
        {
            if (string.Equals(Data.name, item.DisplayName))
            {
                leaderboard_name[i] = leaderboard_name[i] + "( " + item.DisplayName + " )";

                my_score_page = i;
            }
            else
            {
                leaderboard_name[i] = leaderboard_name[i] + item.DisplayName;
            }

            leaderboard_name[i] = leaderboard_name[i] + "\n";

            leaderboard_score[i] = leaderboard_score[i] + (float)(item.StatValue / 100);
            leaderboard_score[i] = leaderboard_score[i] + "\n";

            if ((j + 1) % 5 == 0 && j > 0)
            {
                i++;
            }

            j++;
        }

        last_page = i;

        leaderboard_ready = true;

        current_page = my_score_page;

        show_leaderboard(current_page);
    }

    void error(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    void show_leaderboard(int index)
    {
        if (leaderboard_ready == true)
        {
            rank.text = "";

            for (int i = (index + 1) * 5 - 5; i < (index + 1) * 5; i++)
            {
                rank.text = rank.text + (i + 1) + "\n";
            }

            name.text = leaderboard_name[index];
            score.text = leaderboard_score[index];
        }
    }
}
