using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetUp : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab_audio_manager;
    private AudioManager audio_manager;

    void Start()
    {
        if (GameObject.Find("Audio Manager(Clone)") == null)
        {
            Instantiate(prefab_audio_manager);
        }

        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();
    }
    
    void Update()
    {
        if (GameObject.Find("Audio Manager(Clone)") != null)
        {
            SceneManager.LoadScene("Cutscene");
        }
    }
}
