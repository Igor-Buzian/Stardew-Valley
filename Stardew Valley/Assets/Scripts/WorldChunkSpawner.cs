// File: Scripts/World/WorldChunkSpawner.cs

using UnityEngine;

namespace FarmSim.World
{
    /// <summary>
    /// Spawns one random world chunk prefab at player position on start.
    /// Useful for testing procedural maps / biome chunks.
    /// </summary>
    public class WorldChunkSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;

        [Header("Chunk Prefabs")]
        [SerializeField] private GameObject[] chunkPrefabs;

        [Header("Settings")]
        [SerializeField] private bool spawnOnStart = true;

        private GameObject _currentChunk;

        private void Start()
        {
            if (spawnOnStart)
                SpawnRandomChunk();
        }

        /// <summary>
        /// Spawns random chunk prefab at player position.
        /// </summary>
        public void SpawnRandomChunk()
        {
            if (player == null)
            {
                Debug.LogError("[WorldChunkSpawner] Player reference missing");
                return;
            }

            if (chunkPrefabs == null || chunkPrefabs.Length == 0)
            {
                Debug.LogError("[WorldChunkSpawner] No chunk prefabs assigned");
                return;
            }

            // Remove previous chunk
            if (_currentChunk != null)
                Destroy(_currentChunk);

            // Pick random prefab
            int randomIndex = Random.Range(0, chunkPrefabs.Length);

            GameObject prefab = chunkPrefabs[randomIndex];

            // Spawn at player position
            Vector3 spawnPos = player.position;

            _currentChunk = Instantiate(prefab, spawnPos, Quaternion.identity);

            Debug.Log($"[WorldChunkSpawner] Spawned chunk: {prefab.name}");
        }
    }
}