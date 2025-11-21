using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameResult : MonoBehaviour
{
    private int highScore;
    public Text resultTime;
    public Text bestTime;
    public GameObject parts;

    void Start()
    {
        if (PlayerPrefs.HasKey("HightScore"))
        {
            highScore = PlayerPrefs.GetInt("HightScore");
        }
        else
        {
            highScore = 999;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GoalArea.goal)
        {
            parts.SetActive(true);
            int result = Mathf.FloorToInt(Time.time);
            resultTime.text = "ResultTime : " + result;
            bestTime.text = "BestTime : " + highScore;

            if(highScore>result)
            {
                PlayerPrefs.SetInt("HightSocre",result);
            }
        }
    }
    public void OnRetry()
    {
        SceneManager.LoadScene("Main");
    }
}
