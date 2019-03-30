using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public bool isEnabled = true;
    private List<Mob> _mobPool = new List<Mob>();

    public float delayBetweenWaves = 5f;
    private int _enemiesAlive;

    public int waveIndex = 0;
    public float waveDifficulty = 1;

    [Tooltip("The difficulty increase of each consecutive wave. For example, a 1.1f multiplier means each wave is 10% more difficult than the precedent. Modify this value in order to change the pace of the game.")]
    public float waveDifficultyMultiplier = 1.1f;

    [Tooltip("The amount of mobs on screen is capped, otherwise the device will eventually melt")]
    public int maxMobsOnScreen;

    public GameObject[] mobPrefabArray;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LaunchNextWave());
    }

    // Update is called once per frame
    void Update()
    {
        
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

                // Spawn the enemy along a random horizontal line, within screenspace
                Vector3 spawnPosition = new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(Random.value, 0f)).x,transform.position.y,0f);

                GameObject newMob = Instantiate(mobPrefabArray[randomIndex],spawnPosition, Quaternion.identity, null);
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

    // NB : Will need pooling system in order to prevent performance drop
}
