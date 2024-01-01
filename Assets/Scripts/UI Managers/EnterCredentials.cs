using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class EnterCredentials : MonoBehaviour
{
    [Header("CLASS")]

    private AudioManager audio_manager;

    [SerializeField]
    private Data data;

    [Header("DATA")]

    private string password = "monztr5123welm4coolz";

    private int index = 1;
    private int digit = 1;

    private bool data_ready = true;

    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSdnVoMC0Xyv0vGo65-TUY3QAy1CQciqA9HhdVtAAehpiPFGNQ/formResponse";

    /*private const string email_pattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";*/

    [Header("UI")]

    [SerializeField]
    private GameObject username_panel;

    [SerializeField]
    private InputField username_input;

    [SerializeField]
    private GameObject password_panel;

    [SerializeField]
    private InputField password_input;

    [SerializeField]
    private Text error_text;

    [SerializeField]
    private GameObject empty;

    [SerializeField]
    private GameObject pop_up_password;
    [SerializeField]
    private Text pop_up_password_title;

    [SerializeField]
    private GameObject pop_up;
    [SerializeField]
    private Text pop_up_text;

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
        if (Input.GetKeyDown(KeyCode.Return) && pop_up.active == false)
        {
            enter();
        }
    }

    public void enter()
    {
        audio_manager.play_sound("Button Sound");

        if (data_ready == false)
        {
            return;
        }

        if (username_input.text.Length < 3)
        {
            error_text.color = Color.red;
            error_text.text = "Username invalid!";
        }
        else
        {
            empty.SetActive(true);  // block input bar to prevent changing data while loading

            if (password_panel.active == true)
            {
                password = password_input.text;
            }

            login();
        }
    }

    public void back()
    {
        audio_manager.play_sound("Button Sound");

        if (data_ready == true)
        {
            username_input.text = "";
            password_input.text = "";
            error_text.text = "";
            
            password = "monztr5123welm4coolz";

            username_panel.SetActive(true);

            password_panel.SetActive(false);
        }
    }

    public void password_ok()
    {
        audio_manager.play_sound("Button Sound");

        if (data_ready == true)
        {
            pop_up_password.SetActive(false);

            initialize_base_data();
        }
    }

    public void ok()
    {
        audio_manager.play_sound("Button Sound");

        if (data_ready == true)
        {
            SceneManager.LoadScene("Leaderboards");
        }
    }

    void login()
    {
        data_ready = false;

        error_text.color = Color.yellow;
        error_text.text = "Menyimpan Data...";

        var request = new LoginWithPlayFabRequest
        {
            Username = username_input.text,
            Password = password,
        };

        PlayFabClientAPI.LoginWithPlayFab(request, get_data, error);   // will get data if successful, will automatically register with previous input if error
    }

    void get_data(LoginResult result)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), update_base_data, error);
    }

    void register()
    {
        data_ready = false;

        error_text.text = "Melakukan Registrasi...";

        var request = new RegisterPlayFabUserRequest
        {
            Username = username_input.text,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, register_success, error);
    }

    void register_success(RegisterPlayFabUserResult result)
    {
        pop_up_password_title.text = "Password: " + password;

        error_text.color = Color.green;
        error_text.text = "Registrasi Sukses!";

        data_ready = true;

        pop_up_password.SetActive(true);
    }

    void initialize_base_data()
    {
        data_ready = false;

        error_text.text = "Menginisialisasikan Data...";

        string[] question_code = new string[Data.quiz_list.Length];
        string[] date = new string[Data.quiz_list.Length];

        for (int i = 0; i < Data.quiz_list.Length; i++)
        {
            question_code[i] = Data.quiz_list[i];   // quiz code
            date[i] = System.DateTime.Today.AddDays(-1).ToString(); // initial date
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Quiz Count", Data.quiz_list.Length.ToString()},
                {"Account Data", JsonConvert.SerializeObject(data.return_account_data(question_code, date))},
            }
        };

        PlayFabClientAPI.UpdateUserData(request, initialize_base_data_success, error);
    }

    void initialize_base_data_success(UpdateUserDataResult result)
    {
        data_ready = true;

        login();
    }

    void update_base_data(GetUserDataResult result) // update data if there are new quizzes
    {
        error_text.text = "Mengupdate Data...";

        if (int.Parse(result.Data["Quiz Count"].Value) == Data.quiz_list.Length)
        {
            set_display_name();
        }
        else
        {
            AccountData account_data = JsonConvert.DeserializeObject<AccountData>(result.Data["Account Data"].Value);

            string[] question_code = new string[Data.quiz_list.Length];
            string[] date = new string[Data.quiz_list.Length];

            for (int i = 0; i < Data.quiz_list.Length; i++) // from number of quizzes in playfab to new total
            {
                if (i < int.Parse(result.Data["Quiz Count"].Value)) // overlap data
                {
                    question_code[i] = account_data.question_code[i];
                    date[i] = account_data.date[i];
                }
                else    // enter new data
                {
                    question_code[i] = Data.quiz_list[i];   // quiz code
                    date[i] = System.DateTime.Today.AddDays(-1).ToString(); // initial date
                }
            }

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    {"Quiz Count", Data.quiz_list.Length.ToString()},
                    {"Account Data", JsonConvert.SerializeObject(data.return_account_data(question_code, date))},
                }
            };

            PlayFabClientAPI.UpdateUserData(request, update_base_data_success, error);
        }
    }

    void update_base_data_success(UpdateUserDataResult result)
    {
        set_display_name();
    }

    void set_display_name() // update name
    {
        error_text.text = "Mengupdate Data...";

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username_input.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, set_display_name_success, error);
    }

    void set_display_name_success(UpdateUserTitleDisplayNameResult result)
    {
        Data.name = username_input.text;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), check_update_leaderboards, error);
    }
    
    void check_update_leaderboards(GetUserDataResult result)    // update score if eligible
    {
        error_text.text = "Mengupdate Leaderboards...";

        AccountData account_data = JsonConvert.DeserializeObject<AccountData>(result.Data["Account Data"].Value);

        for (int i = 0; i < Data.quiz_list.Length; i++)
        {
            if (account_data.question_code[i] == Data.question_code)
            {
                if (System.DateTime.Parse(account_data.date[i]) < System.DateTime.Today)    // replace data & record to gsheet
                {
                    //send_data_to_gform(name.text, email.text, Data.score);

                    update_leaderboards();
                }
                else    // terakhir kerjain hari ini
                {
                    pop_up_text.text = "Skor untuk quiz " + Data.question_code + " hari ini sudah tercatat pada akun anda.\n\nSkor tidak akan diupdate.";

                    data_ready = true;

                    pop_up.SetActive(true);
                }

                break;
            }
        }
    }

    void update_leaderboards()
    {
        error_text.text = "Mengupdate Leaderboards...";

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = Data.question_code,
                    Value = (int)(Data.score * 100)   // to account decimal places because playfab can't store float
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, update_leaderboards_success, error);
    }

    void update_leaderboards_success(UpdatePlayerStatisticsResult result)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), update_data, error);
    }

    void update_data(GetUserDataResult result)
    {
        AccountData account_data = JsonConvert.DeserializeObject<AccountData>(result.Data["Account Data"].Value);

        for (int i = 0; i < Data.quiz_list.Length; i++)
        {
            if (account_data.question_code[i] == Data.question_code)
            {
                account_data.date[i] = System.DateTime.Today.ToString();

                break;
            }
        }

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
                {
                    {"Account Data", JsonConvert.SerializeObject(account_data)},
                }
        };

        PlayFabClientAPI.UpdateUserData(request, update_data_success, error);
    }

    void update_data_success(UpdateUserDataResult result)
    {
        error_text.color = Color.green;
        error_text.text = "Masuk Sukses!";

        data_ready = true;

        SceneManager.LoadScene("Leaderboards");
    }

    void error(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());

        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            password = Random.Range(10000000, 100000000).ToString();

            register();
        }
        else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword && password == "monztr5123welm4coolz")
        {
            error_text.color = Color.green;
            error_text.text = "Username terdeteksi! Mohon masukkan password!";

            password_panel.SetActive(true);

            username_panel.SetActive(false);
        }
        else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
        {
            error_text.color = Color.red;
            error_text.text = "Password invalid!";
        }

        data_ready = true;
        empty.SetActive(false);
    }

    /*void error(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());

        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            register();
        }
        else if (error.Error == PlayFabErrorCode.NameNotAvailable)
        {
            if (index == 1)
            {
                if (no_space.Length + index.ToString().Length + 1 <= 25)    // panjang string + panjang no index + _ index
                {
                    no_space = no_space.Insert(no_space.Length, "_" + index.ToString());
                }
                else
                {
                    no_space = no_space.Remove(no_space.Length - 1 - index.ToString().Length, index.ToString().Length + 1);
                    no_space = no_space.Insert(no_space.Length, "_" + index.ToString());
                }
            }
            else if (index.ToString().Length == digit)
            {
                no_space = no_space.Remove(no_space.Length - digit, digit); // ilangin angka sesuai digit
                no_space = no_space.Insert(no_space.Length, index.ToString());
            }
            else
            {
                digit++;

                no_space = no_space.Remove(no_space.Length - digit);

                if (no_space.Length + index.ToString().Length + 1 <= 25)    // panjang string + panjang no index + _ index
                {
                    no_space = no_space.Insert(no_space.Length, "_" + index.ToString());
                }
                else
                {
                    no_space = no_space.Remove(no_space.Length - 1, 1); // ilangin 1 aja karena udah dicalculate di index pertama
                    no_space = no_space.Insert(no_space.Length, "_" + index.ToString());
                }
            }

            index++;

            set_display_name();
        }
    }*/

    /*bool check_email(string email)
    {
        if (email != null)
        {
            return Regex.IsMatch(email, email_pattern);
        }
        else
        {
            return false;
        }
    }

    void send_data_to_gform(string name, string email, float score)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.1510176575", name);
        form.AddField("entry.704527458", email);
        form.AddField("entry.1280439684", score.ToString());

        byte[] rawdata = form.data;

        WWW www = new WWW(BASE_URL, rawdata);
    }*/
}