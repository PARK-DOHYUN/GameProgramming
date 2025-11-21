using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    public float speed = 1.0f;
    protected float timeCount = 0f;   // ← protected로 바꿔주면 자식도 사용 가능하게 좋음

    protected virtual void Update()    // ← 반드시 virtual!
    {
        // 타이머 체크
        timeCount += Time.deltaTime;
        if (timeCount > 3f)
        {
            Debug.Log("돌을던져라");
            timeCount = 0f;
        }

        // 이동
        float delta = speed * Time.deltaTime;
        transform.localPosition += new Vector3(delta, 0, 0);

        // 좌우 범위 체크 후 방향 반전
        if (transform.localPosition.x < -9f)
        {
            speed = Mathf.Abs(speed);
        }
        else if (transform.localPosition.x > 9f)
        {
            speed = -Mathf.Abs(speed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;

        if (rb != null)
        {
            Vector3 direction = collision.transform.position - transform.position;
            direction.Normalize();

            float forceMagnitude = 500f;
            rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
        }
    }
}
