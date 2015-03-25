using UnityEngine;

// Manages level wide stuff that can be used by both Master and Client,
// like individual rooms
public class Level : MonoBehaviour
{
    private SpawnPoint[] spawnPoints;

    void Start()
    {
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }

    public SpawnPoint[] GetSpawnPointsShuffled()
    {
        SpawnPoint[] newSpawnPoints = new SpawnPoint[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
            newSpawnPoints[i] = spawnPoints[i];

        for (int i = 0; i < newSpawnPoints.Length; i++)
        {
            int j = Random.Range(0, spawnPoints.Length);
            SpawnPoint temp = newSpawnPoints[i];
            newSpawnPoints[i] = newSpawnPoints[j];
            newSpawnPoints[j] = temp;
        }

        return newSpawnPoints;
    }
}
