using System.Collections;
using UnityEngine;

public class FootstepSpawner : MonoBehaviour
{
    public GameObject footstepPrefab;
    public Transform TrFootstepPicMain;
    public float moveSpeed = 2f;
    public float stepDistance = 0.5f;
    public float minWait = 0.2f, maxWait = 0.5f;

    private Vector3 targetPos;

    void Start()
    {
        SetNewRandomTarget();
        StartCoroutine(WalkToRandomPoints());
    }

    void SetNewRandomTarget()
    {
        Vector2 screenPos = new Vector2(Random.Range(0f, Screen.width), Random.Range(0f, Screen.height));
        targetPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10));
    }

    IEnumerator WalkToRandomPoints()
    {
        while (true)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPos);

            while (distance > stepDistance)
            {
                Vector3 step = transform.position + dir * stepDistance;
                Instantiate(footstepPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, dir),this.transform.parent);
                float t = 0;
                while (t < 1f)
                {
                    t += Time.deltaTime * moveSpeed;
                    transform.position = Vector3.Lerp(transform.position, step, t);
                    yield return null;
                }

                yield return new WaitForSeconds(Random.Range(minWait, maxWait));
                TrFootstepPicMain.rotation = Quaternion.LookRotation(Vector3.forward, targetPos);
                distance = Vector3.Distance(transform.position, targetPos);
            }

            SetNewRandomTarget();
            yield return null;
        }
    }
}
