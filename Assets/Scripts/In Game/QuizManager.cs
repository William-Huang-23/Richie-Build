using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class QuizManager : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    private BuildManager build_manager;

    [Header("DATA")]

    private string[] question = new string[1000];

    private string[] answer_1 = new string[1000];
    private string[] answer_2 = new string[1000];
    private string[] answer_3 = new string[1000];
    private string[] answer_4 = new string[1000];

    private int[] correct_answer = new int[1000];

    private int total_question = 0;
    private int current_question = 0;

    private float time = 16;

    private float[] cooldown = { 61, 61, 61 };

    private bool hint_50_used = false;
    private bool hint_ask_used = false;
    private bool hint_poll_used = false;

    private bool deactivate = false;
    private float deactivate_delay = 0;

    [Header("UI")]

    [SerializeField]
    private GameObject quiz;

    [SerializeField]
    private Text question_text;

    [SerializeField]
    private GameObject[] answer_button;
    [SerializeField]
    private Text[] answer_text;
    [SerializeField]
    private GameObject answer_highlight;

    [SerializeField]
    private GameObject correct_highlight;
    [SerializeField]
    private GameObject wrong_highlight;

    [SerializeField]
    private Button hint_50_button;
    [SerializeField]
    private Button hint_ask_button;
    [SerializeField]
    private Button hint_poll_button;

    [SerializeField]
    private Text[] hint_cooldown;

    [SerializeField]
    private Text[] poll_result;

    [SerializeField]
    private Text time_limit;

    [SerializeField]
    private Toggle hint_toggle;

    [SerializeField]
    private Sprite[] sprite;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        build_manager = GameObject.Find("Game Manager").GetComponent<BuildManager>();

        read_file(Data.question_code);

        shuffle_question();

        display_question();
    }

    void Update()
    {
        if (quiz.active == true)
        {
            time = time - Time.deltaTime;

            time_limit.text = ((int)time).ToString();

            if (time <= 1 && deactivate == false)
            {
                build_manager.wrong();

                reset_question();
            }
        }

        if (hint_50_used == true)
        {
            cooldown[0] = cooldown[0] - Time.deltaTime;

            if (cooldown[0] <= 0)
            {
                hint_cooldown[0].text = "";
                cooldown[0] = 61;

                hint_50_button.gameObject.GetComponent<Image>().sprite = sprite[0];

                hint_50_used = false;
            }
            else
            {
                hint_cooldown[0].text = ((int)cooldown[0]).ToString();

                hint_50_button.gameObject.GetComponent<Image>().sprite = sprite[1];
            }
        }

        if (hint_ask_used == true)
        {
            cooldown[1] = cooldown[1] - Time.deltaTime;

            if (cooldown[1] <= 0)
            {
                hint_cooldown[1].text = "";
                cooldown[1] = 61;

                hint_ask_button.gameObject.GetComponent<Image>().sprite = sprite[2];

                hint_ask_used = false;
            }
            else
            {
                hint_cooldown[1].text = ((int)cooldown[1]).ToString();

                hint_ask_button.gameObject.GetComponent<Image>().sprite = sprite[3];
            }
        }

        if (hint_poll_used == true)
        {
            cooldown[2] = cooldown[2] - Time.deltaTime;

            if (cooldown[2] <= 0)
            {
                hint_cooldown[2].text = "";
                cooldown[2] = 61;

                hint_poll_button.gameObject.GetComponent<Image>().sprite = sprite[4];

                hint_poll_used = false;
            }
            else
            {
                hint_cooldown[2].text = ((int)cooldown[2]).ToString();

                hint_poll_button.gameObject.GetComponent<Image>().sprite = sprite[5];
            }
        }

        if (deactivate == true)
        {
            deactivate_delay = deactivate_delay + Time.deltaTime;

            if (deactivate_delay >= 1)
            {
                quiz.SetActive(false);
            }
        }
    }

    public void activate_quiz()
    {
        audio_manager.play_sound("Button Sound");

        display_question();

        quiz.SetActive(true);

        time = 16;

        deactivate = false;
        deactivate_delay = 0;

        answer_button[0].gameObject.SetActive(true);
        answer_button[1].gameObject.SetActive(true);
        answer_button[2].gameObject.SetActive(true);
        answer_button[3].gameObject.SetActive(true);

        activate_hint();

        if (hint_50_used == false || hint_ask_used == false || hint_poll_used == false)
        {
            hint_toggle.isOn = true;
        }
        else
        {
            hint_toggle.isOn = false;
        }

        poll_result[0].text = "";
        poll_result[1].text = "";
        poll_result[2].text = "";
        poll_result[3].text = "";

        answer_highlight.SetActive(false);

        correct_highlight.SetActive(false);
        wrong_highlight.SetActive(false);
    }

    public void answer(int correct_answer)
    {
        if (deactivate == false)
        {
            if (correct_answer == this.correct_answer[current_question])
            {
                correct_highlight.SetActive(true);
                correct_highlight.transform.position = answer_button[this.correct_answer[current_question] - 1].transform.position;

                build_manager.right();
            }
            else
            {
                wrong_highlight.SetActive(true);
                wrong_highlight.transform.position = answer_button[correct_answer - 1].transform.position;

                build_manager.wrong();
            }

            answer_highlight.SetActive(false);

            reset_question();
        }
    }

    public void hint_50()
    {
        audio_manager.play_sound("Button Sound");

        if (hint_50_used == false)
        {
            int answer = 1;
            int eliminated = 0;

            do
            {
                if (answer != correct_answer[current_question]) // random eliminate 2 wrong answers
                {
                    if (Random.Range(0, 3) == 0 && answer_button[answer - 1].gameObject.active == true)     // 33% chance to eliminate answers
                    {
                        answer_button[answer - 1].gameObject.SetActive(false);

                        eliminated++;
                    }
                }

                answer++;

                if (answer >= 5)
                {
                    answer = 1;
                }
            }
            while (eliminated < 2);
            
            deactivate_hint();

            hint_50_used = true;
        }
    }

    public void hint_ask()
    {
        audio_manager.play_sound("Button Sound");

        if (hint_ask_used == false)
        {
            answer_highlight.SetActive(true);

            if (Random.Range(0, 10) < 8)    // 80% chance hint shows right answer
            {
                answer_highlight.transform.position = answer_button[correct_answer[current_question] - 1].transform.position;
            }
            else    // hint chooses wrong answer (can choose right answer too by chance)
            {
                answer_highlight.transform.position = answer_button[Random.Range(0, 4)].transform.position;
            }

            deactivate_hint();

            hint_ask_used = true;
        }
    }

    public void hint_poll()
    {
        audio_manager.play_sound("Button Sound");

        if (hint_poll_used == false)
        {
            int[] poll = { 0, 0, 0, 0 };

            for (int respondents = 0; respondents < 20; respondents++) // 20 respondents
            {
                if (Random.Range(0, 10) < 4)    // 40% chance for respondents to choose right answer
                {
                    poll[correct_answer[current_question] - 1]++;
                }
                else    // respondents choose wrong answer (can choose right answer too by chance)
                {
                    poll[Random.Range(0, 4)]++;
                }
            }

            poll_result[0].text = ((float)poll[0] / 20 * 100).ToString() + "%\n---------->";
            poll_result[1].text = ((float)poll[1] / 20 * 100).ToString() + "%\n---------->";
            poll_result[2].text = ((float)poll[2] / 20 * 100).ToString() + "%\n---------->";
            poll_result[3].text = ((float)poll[3] / 20 * 100).ToString() + "%\n---------->";

            deactivate_hint();

            hint_poll_used = true;
        }
    }

    void activate_hint()
    {
        hint_toggle.gameObject.SetActive(true);

        hint_50_button.gameObject.SetActive(true);
        hint_ask_button.gameObject.SetActive(true);
        hint_poll_button.gameObject.SetActive(true);
    }

    void deactivate_hint()
    {
        hint_toggle.gameObject.SetActive(false);

        hint_50_button.gameObject.SetActive(false);
        hint_ask_button.gameObject.SetActive(false);
        hint_poll_button.gameObject.SetActive(false);
    }

    void reset_question()
    {
        current_question++;

        deactivate = true;
    }

    void read_file(string question_code)
    {
        TextAsset file = Resources.Load<TextAsset>(question_code);    // load file dari folder resources

        string[] line = file.text.Split("\n"[0]);

        total_question = 0;

        for (int index = 0; index < line.Length; total_question++)    // index array baris
        {
            correct_answer[total_question] = 1;

            question[total_question] = line[index];    // baca soal
            index++;

            answer_1[total_question] = line[index];   // baca jawaban 1
            index++;
            answer_2[total_question] = line[index];   // baca jawaban 2
            index++;
            answer_3[total_question] = line[index];   // baca jawaban 3
            index++;
            answer_4[total_question] = line[index];   // baca jawaban 4
            index++;

            index++;    // spasi kosong di text file
        }
    }

    void shuffle_question()
    {
        string temp_string;
        int temp_int;

        for (int i = 0; i < total_question - 1; i++)   // acak soal sesuai jumlah soalnya
        {
            int random = Random.Range(i, total_question);

            temp_string = question[random];
            question[random] = question[i];
            question[i] = temp_string;

            temp_string = answer_1[random];
            answer_1[random] = answer_1[i];
            answer_1[i] = temp_string;

            temp_string = answer_2[random];
            answer_2[random] = answer_2[i];
            answer_2[i] = temp_string;

            temp_string = answer_3[random];
            answer_3[random] = answer_3[i];
            answer_3[i] = temp_string;

            temp_string = answer_4[random];
            answer_4[random] = answer_4[i];
            answer_4[i] = temp_string;

            temp_int = correct_answer[random];
            correct_answer[random] = correct_answer[i];
            correct_answer[i] = temp_int;

            shuffle_answers(i);
        }
    }

    void shuffle_answers(int index)
    {
        string temp_string;

        for (int i = 0; i < 10; i++)   // acak jawaban 10x
        {
            int random = Random.Range(i, 4);    // random index 0-3

            switch(random)
            {
                case 0:
                    {
                        temp_string = answer_1[index];
                        answer_1[index] = answer_2[index];
                        answer_2[index] = temp_string;

                        if (correct_answer[index] == 1)
                        {
                            correct_answer[index] = 2;
                        }
                        else if (correct_answer[index] == 2)
                        {
                            correct_answer[index] = 1;
                        }

                        break;
                    }

                case 1:
                    {
                        temp_string = answer_2[index];
                        answer_2[index] = answer_3[index];
                        answer_3[index] = temp_string;

                        if (correct_answer[index] == 2)
                        {
                            correct_answer[index] = 3;
                        }
                        else if (correct_answer[index] == 3)
                        {
                            correct_answer[index] = 2;
                        }


                        break;
                    }

                case 2:
                    {
                        temp_string = answer_3[index];
                        answer_3[index] = answer_4[index];
                        answer_4[index] = temp_string;

                        if (correct_answer[index] == 3)
                        {
                            correct_answer[index] = 4;
                        }
                        else if (correct_answer[index] == 4)
                        {
                            correct_answer[index] = 3;
                        }


                        break;
                    }

                case 3:
                    {
                        temp_string = answer_4[index];
                        answer_4[index] = answer_1[index];
                        answer_1[index] = temp_string;

                        if (correct_answer[index] == 4)
                        {
                            correct_answer[index] = 1;
                        }
                        else if (correct_answer[index] == 1)
                        {
                            correct_answer[index] = 4;
                        }


                        break;
                    }
            }
        }
    }

    void display_question()
    {
        if (current_question == total_question) // kalau udah sampai akhir array
        {
            current_question = 0;

            shuffle_question();
        }

        question_text.text = question[current_question];

        answer_text[0].text = answer_1[current_question];
        answer_text[1].text = answer_2[current_question];
        answer_text[2].text = answer_3[current_question];
        answer_text[3].text = answer_4[current_question];
    }
}