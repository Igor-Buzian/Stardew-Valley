// File: Scripts/World/InfiniteChunkGenerator.cs

using System.Collections.Generic;
using UnityEngine;

namespace FarmSim.World
{
    /// <summary>
    /// Infinite world chunk streaming system.
    /// Spawns chunks around player while moving.
    /// </summary>
    public class InfiniteChunkGenerator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;

        [Header("Chunk Prefabs")]
        [SerializeField] private GameObject[] chunkPrefabs;

        [Header("Chunk Settings")]
        [SerializeField] private int chunkSize = 32;
        [SerializeField] private int renderDistance = 1;

        // Spawned chunks cache
        private readonly Dictionary<Vector2Int, GameObject> _spawnedChunks = new();

        // Current player chunk
        private Vector2Int _currentPlayerChunk;

        private void Start()
        {
            UpdateChunks(force: true);
        }

        private void Update()
        {
            Vector2Int newChunk = GetChunkCoord(player.position);

            // Only update when entering another chunk
            if (newChunk != _currentPlayerChunk)
            {
                _currentPlayerChunk = newChunk;
                UpdateChunks();
            }
        }

        /// <summary>
        /// Spawn/despawn chunks around player.
        /// </summary>
        private void UpdateChunks(bool force = false)
        {
            HashSet<Vector2Int> requiredChunks = new();

            // Generate chunks around player
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int y = -renderDistance; y <= renderDistance; y++)
                {
                    Vector2Int coord = new Vector2Int(
                        _currentPlayerChunk.x + x,
                        _currentPlayerChunk.y + y
                    );

                    requiredChunks.Add(coord);

                    // Spawn if missing
                    if (!_spawnedChunks.ContainsKey(coord))
                    {
                        SpawnChunk(coord);
                    }
                }
            }

            // Remove distant chunks
            List<Vector2Int> toRemove = new();

            foreach (var chunk in _spawnedChunks)
            {
                if (!requiredChunks.Contains(chunk.Key))
                {
                    Destroy(chunk.Value);
                    toRemove.Add(chunk.Key);
                }
            }

            foreach (var coord in toRemove)
            {
                _spawnedChunks.Remove(coord);
            }
        }

        /// <summary>
        /// Spawn random chunk prefab at chunk coordinate.
        /// </summary>
        private void SpawnChunk(Vector2Int coord)
        {
            if (chunkPrefabs.Length == 0)
                return;

            int randomIndex = Random.Range(0, chunkPrefabs.Length);

            GameObject prefab = chunkPrefabs[randomIndex];

            Vector3 worldPos = new Vector3(
                coord.x * chunkSize,
                coord.y * chunkSize,
                0f
            );

            GameObject chunk = Instantiate(prefab, worldPos, Quaternion.identity);

            chunk.name = $"Chunk_{coord.x}_{coord.y}";

            _spawnedChunks.Add(coord, chunk);
        }

        /// <summary>
        /// Convert world position into chunk coordinate.
        /// </summary>
        private Vector2Int GetChunkCoord(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / chunkSize),
                Mathf.FloorToInt(position.y / chunkSize)
            );
        }
    }
}