using System.Collections;
using UnityEngine;

public class FootstepSpawner : MonoBehaviour
{
    public GameObject footstepPrefab;
    public Transform[] pathPoints;
    public Transform trFather;
    public float stepInterval = 0.2f;
    public bool loop = true;

    private void Start()
    {
        StartCoroutine(SpawnFootsteps());
    }

    IEnumerator SpawnFootsteps()
    {
        int index = 0;
        while (loop || index < pathPoints.Length)
        {
            Instantiate(footstepPrefab, pathPoints[index].position, pathPoints[index].rotation,trFather);
            index = (index + 1) % pathPoints.Length;
            yield return new WaitForSeconds(stepInterval);
        }
    }
}
