using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public bool isEnabled = true;
  
    public float delayBetweenWaves = 5f;
    private int _enemiesAlive;

    public int waveIndex = 0;
    public float waveDifficulty = 1;

    [Tooltip("The difficulty increase of each consecutive wave. For example, a 1.1f multiplier means each wave is 10% more difficult than the precedent. Modify this value in order to change the pace of the game.")]
    public float waveDifficultyMultiplier = 1.1f;

    public GameObject[] mobPrefabArray;

    [System.Serializable]
    public class MobMasterPool
    {
        public List<GameObject> _mobSubtypePool = new List<GameObject>();
    }

    private List<MobMasterPool> _mobMasterPool = new List<MobMasterPool>();


    public static MobSpawner MobSpawnerInstance;



    void Awake()
    {

        MobSpawnerInstance = this;

        // Init of the pooling lists
        for (int i = 0; i < mobPrefabArray.Length; i++)
        {
            _mobMasterPool.Add(new MobMasterPool());
        }
       
    }


    void Start()
    {
        StartCoroutine(LaunchNextWave());
    }




    IEnumerator LaunchNextWave()
    {
        while (isEnabled)
        {
            // Get the difficulty score of the wave
            waveDifficulty *= waveDifficultyMultiplier;
            waveIndex++;

            Debug.Log("Wave " + waveIndex + " is starting ! Difficulty : " + waveDifficulty);

            float currentWaveDifficulty = 0f;

            // Fill the wave with random units from the prefab array until the difficulty score is reached
            while (currentWaveDifficulty < waveDifficulty)
            {
                int randomIndex = Random.Range(0, mobPrefabArray.Length);

               
                // Get pooled thing
                GameObject newMob = GetPooledMob(randomIndex);
                Mob newMobScript = newMob.GetComponent<Mob>();

                // Increment the current wave difficulty counter
                currentWaveDifficulty += newMobScript.difficultyScore;

                // Scale up the mob with the wave level
                newMobScript.scalingRatio = waveDifficulty;


                yield return new WaitForSeconds(0.25f);

            }

            yield return new WaitForSeconds(delayBetweenWaves);

        }

        // If the mob cap is reached, scale up the amount of mobs.
    }

    private GameObject GetPooledMob(int subtypeIndex)
    {
        // Spawn the enemy along a random horizontal line, within screenspace
        Vector3 spawnPosition = new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(Random.value, 0f)).x, transform.position.y, 0f);

        for (int i = 0; i < _mobMasterPool[subtypeIndex]._mobSubtypePool.Count; i++)
        {
            // Look for an available mob subtype in the pool
            if (!_mobMasterPool[subtypeIndex]._mobSubtypePool[i].activeInHierarchy)
            {
                _mobMasterPool[subtypeIndex]._mobSubtypePool[i].SetActive(true);
                _mobMasterPool[subtypeIndex]._mobSubtypePool[i].transform.position = spawnPosition;
                return _mobMasterPool[subtypeIndex]._mobSubtypePool[i];
            }
        }

        // Otherwise Instantiate a new one
        GameObject newMob = Instantiate(mobPrefabArray[subtypeIndex], spawnPosition, Quaternion.identity, null);

        _mobMasterPool[subtypeIndex]._mobSubtypePool.Add(newMob);

        return newMob;
    }
}
