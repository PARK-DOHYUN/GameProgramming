using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour
{
    public static float time;
    public Text uiText;
    void Start()
    {
        time = 0;
        uiText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GoalArea.goal == false)
        {
            time += Time.deltaTime;
        }

        int t = Mathf.FloorToInt(time);
     
        
        uiText.text = "Time : " + t.ToString();
    }
}
