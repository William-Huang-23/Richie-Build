using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    [Header("GAME DATA")]

    private int page = 0;
    private int max_page = 4;

    private string[] story = new string[5];

    [Header("CLASS")]
    
    private AudioManager audio_manager;

    [Header("UI")]

    [SerializeField]
    private Text story_text;

    [SerializeField]
    private GameObject[] cutscene;

    [SerializeField]
    private Button prev_button;

    void Start()
    {
        audio_manager = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();

        audio_manager.play_sound("Cutscene BGM");

        set_value();

        story_text.text = story[0];
    }

    // public

    public void skip()
    {
        audio_manager.play_sound("Button Sound");

        sign_in();
    }

    public void next()
    {
        audio_manager.play_sound("Button Sound");

        cutscene[page].SetActive(false);

        if (page != max_page)
        {
            if (page == 0)
            {
                prev_button.gameObject.SetActive(true);
            }

            page++;
        }
        else
        {
            sign_in();
        }

        story_text.text = story[page];

        cutscene[page].SetActive(true);
    }

    public void prev()
    {
        audio_manager.play_sound("Button Sound");

        cutscene[page].SetActive(false);

        if (page != 0)
        {
            page--;

            if (page == 0)
            {
                prev_button.gameObject.SetActive(false);
            }
        }

        story_text.text = story[page];

        cutscene[page].SetActive(true);
    }

    // private

    void set_value()
    {
        story[0] = "Setelah bekerja dengan keras di kota untuk mengumpulkan uang, Richie akhirnya kembali ke gunung untuk bertemu dengan teman-temannya!";
        story[1] = "Richie merasa sangat senang karena akhirnya dapat bersatu kembali dengan teman-temannya setelah sekian lama!";
        story[2] = "Namun, reuni Richie dan teman-temannya harus berhenti tiba tiba karena mereka mendapat kabar bahwa banjir bandang akan datang!";
        story[3] = "Dengan uang yang dikumpulkan oleh Richie di kota, Richie memutuskan untuk membeli bahan-bahan bangunan untuk membangun bendungan yang kokoh sebelum banjir bandang tiba!";
        story[4] = "Mari bantu Richie bangun bendungan yang kokoh untuk para teman-temannya!";
    }

    void sign_in()
    {
        audio_manager.stop_sound("Cutscene BGM");

        SceneManager.LoadScene("Splash Screen");
    }
}
