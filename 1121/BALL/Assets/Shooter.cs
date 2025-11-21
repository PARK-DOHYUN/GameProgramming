using UnityEngine;

public class Shooter : ObstacleMove
{
    public GameObject stone;
    float timeCount = 0;

    protected override void Update()
    {
        base.Update();  // 부모 Update 먼저 실행

        timeCount += Time.deltaTime;
        if (timeCount > 3)
        {
            Instantiate(stone, transform.position, Quaternion.identity);
            timeCount = 0;
        }
    }
}
