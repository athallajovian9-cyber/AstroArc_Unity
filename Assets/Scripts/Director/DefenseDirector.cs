using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstroArc
{
    public class DefenseDirector : MonoBehaviour
    {
        [Header("Configuration")]
        public DifficultySettings difficulty = new DifficultySettings();
        public int currentWave = 1;
        public int totalWaves = 100;
        public int baseHealth = 100;
        public int currentHealth;
        public int sigmaCoins = 0;

        [Header("Spawn Parameters")]
        public float spawnInterval = 1.5f;
        public float laneWidth = 7.0f;

        private int remainingToSpawn;
        private int activeEnemies = 0;
        private bool waveInProgress = false;

        private void Start()
        {
            currentHealth = baseHealth;
            StartWave(currentWave);
        }

        public void StartWave(int wave)
        {
            currentWave = wave;
            remainingToSpawn = Mathf.Min(difficulty.maxToilets, Mathf.RoundToInt(difficulty.toiletsBase + difficulty.toiletsGrowth * (wave - 1)));
            waveInProgress = true;
            Debug.Log($"[ASTRO ARC] Starting Wave {wave} - Enemies: {remainingToSpawn}");
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (remainingToSpawn > 0)
            {
                SpawnEnemy();
                remainingToSpawn--;
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            // Procedural geometric representation (no missing asset crash)
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.transform.position = new Vector3(Random.Range(-laneWidth, laneWidth), 7.0f, 0.0f);
            go.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            EnemyAgent agent = go.AddComponent<EnemyAgent>();
            
            // Determine type
            EnemyType type = EnemyType.AstroTrooper;
            float hp = 2.0f;
            float spd = 2.5f;
            int reward = 10;

            if (currentWave % 5 == 0 && remainingToSpawn == 1)
            {
                type = EnemyType.GManBoss;
                hp = 50.0f + (currentWave * 5);
                spd = 1.2f;
                reward = 250;
                go.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
            }
            else if (currentWave >= 6 && Random.value < 0.25f)
            {
                type = EnemyType.AstroAssailant;
                hp = 3.0f;
                spd = 3.8f;
                reward = 20;
            }

            EnemyStats stats = new EnemyStats
            {
                type = type,
                hp = Mathf.RoundToInt(hp * difficulty.hpMult),
                maxHp = Mathf.RoundToInt(hp * difficulty.hpMult),
                speed = spd * difficulty.speedMult,
                sigmaReward = reward,
                isBoss = (type == EnemyType.GManBoss)
            };

            agent.Initialize(stats);
            agent.OnDeath += HandleEnemyDeath;
            agent.OnBreach += HandleEnemyBreach;
            activeEnemies++;
        }

        private void HandleEnemyDeath(EnemyAgent agent)
        {
            activeEnemies--;
            sigmaCoins += agent.stats.sigmaReward;
            CheckWaveCompletion();
        }

        private void HandleEnemyBreach(EnemyAgent agent)
        {
            activeEnemies--;
            currentHealth -= difficulty.breachDamage;
            Debug.LogWarning($"[DEFENSE BREACH] Health Remaining: {currentHealth}");
            CheckWaveCompletion();
        }

        private void CheckWaveCompletion()
        {
            if (remainingToSpawn <= 0 && activeEnemies <= 0 && waveInProgress)
            {
                waveInProgress = false;
                Debug.Log($"[WAVE {currentWave} CLEARED!] Unlocked Sigma: {sigmaCoins}");
                if (currentWave < totalWaves)
                {
                    StartCoroutine(NextWaveCooldown());
                }
                else
                {
                    Debug.Log("[VICTORY] Campaign Completed! All 100 Waves Cleared!");
                }
            }
        }

        private IEnumerator NextWaveCooldown()
        {
            yield return new WaitForSeconds(3.0f);
            StartWave(currentWave + 1);
        }
    }
}
