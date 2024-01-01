using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountData
{
    public string[] question_code;
    public string[] date;

    public AccountData(string[] question_code, string[] date)
    {
        this.question_code = question_code;
        this.date = date;
    }
}

public class Data : MonoBehaviour
{
    public static string[] quiz_list;// = {"QUIZZ"};   // list of question codes

    public static string question_code;// = "QUIZZ"; // code used to generate game

    public static string name;  // name
    public static float score;// = 1000;  // score

    public AccountData return_account_data(string[] question_code, string[] date)
    {
        return new AccountData(question_code, date);
    }
}
