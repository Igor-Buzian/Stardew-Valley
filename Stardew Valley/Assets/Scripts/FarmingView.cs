using UnityEngine;
using UnityEngine.Tilemaps;
using FarmSim.Core;

namespace FarmSim.Farming
{
    public class FarmingView : MonoBehaviour
    {
        [SerializeField] private Tilemap soilTilemap;
        [SerializeField] private TileBase tilledTile;
        [SerializeField] private TileBase wetTile;

        private void OnEnable()
        {
            GameEvents.OnSoilTilled += OnTilled;
            GameEvents.OnSoilWatered += OnWatered;
        }

        private void OnDisable()
        {
            GameEvents.OnSoilTilled -= OnTilled;
            GameEvents.OnSoilWatered -= OnWatered;
        }

        private void OnTilled(Vector3Int pos)
        {
            soilTilemap.SetTile(pos, tilledTile);
        }

        private void OnWatered(Vector3Int pos)
        {
            soilTilemap.SetTile(pos, wetTile);
        }
    }
}