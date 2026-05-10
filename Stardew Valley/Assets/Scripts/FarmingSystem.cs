using System.Collections.Generic;
using UnityEngine;
using FarmSim.Core;

namespace FarmSim.Farming
{
    public class FarmingSystem : MonoBehaviour
    {
        private Dictionary<Vector3Int, FarmTile> _tiles = new();

        public void Init()
        {
            Debug.Log("[FarmingSystem] Init");
        }
        
        public void TillSoil(Vector3Int pos)
        {
            GetOrCreateTile(pos).state = SoilState.Tilled;
            GameEvents.RaiseSoilTilled(pos);
        }

        public void WaterSoil(Vector3Int pos)
        {
            var tile = GetOrCreateTile(pos);

            if (tile.state == SoilState.Tilled || tile.state == SoilState.Dry)
            {
                tile.state = SoilState.Wet;
                tile.waterLevel = 1f;
                GameEvents.RaiseSoilWatered(pos);
            }
        }

        public void PlantSeed(Vector3Int pos, string cropId)
        {
            var tile = GetOrCreateTile(pos);

            if (tile.state == SoilState.Wet && tile.crop == null)
            {
                tile.crop = new CropData
                {
                    cropId = cropId,
                    growth = 0f,
                    stage = 0
                };

                GameEvents.RaiseSeedPlanted(pos, cropId);
            }
        }

        public void HarvestCrop(Vector3Int pos)
        {
            var tile = GetOrCreateTile(pos);

            if (tile.crop != null && tile.crop.growth >= 1f)
            {
                tile.crop = null;
                tile.state = SoilState.Dry;

                GameEvents.RaiseCropHarvested(pos);
            }
        }

        public void GrowCrop(Vector3Int pos)
        {
            var tile = GetOrCreateTile(pos);

            if (tile.crop == null) return;

            tile.crop.growth += 0.1f;

            if (tile.crop.growth >= 1f)
                tile.crop.growth = 1f;

            tile.crop.stage = Mathf.FloorToInt(tile.crop.growth * 3);

            GameEvents.RaiseCropGrew(pos);
        }
        
        private FarmTile GetOrCreateTile(Vector3Int pos)
        {
            if (!_tiles.TryGetValue(pos, out var tile))
            {
                tile = new FarmTile
                {
                    state = SoilState.Dry
                };

                _tiles[pos] = tile;
            }

            return tile;
        }

        // ─── Save / Load ──────────────────────────────────────────────

        public FarmingData GetSaveData()
        {
            var data = new FarmingData();

            foreach (var kv in _tiles)
            {
                data.tiles.Add(new FarmingTileData
                {
                    x = kv.Key.x,
                    y = kv.Key.y,
                    z = kv.Key.z,

                    state = kv.Value.state,
                    waterLevel = kv.Value.waterLevel,

                    crop = kv.Value.crop != null
                        ? new CropData
                        {
                            cropId = kv.Value.crop.cropId,
                            growth = kv.Value.crop.growth,
                            stage = kv.Value.crop.stage
                        }
                        : null
                });
            }

            return data;
        }

        public void LoadFromData(FarmingData data)
        {
            _tiles.Clear();

            if (data?.tiles == null) return;

            foreach (var t in data.tiles)
            {
                var pos = new Vector3Int(t.x, t.y, t.z);

                _tiles[pos] = new FarmTile
                {
                    state = t.state,
                    waterLevel = t.waterLevel,
                    crop = t.crop
                };
            }
        }
    }
    
    public enum SoilState
    {
        Dry,
        Tilled,
        Wet
    }

    public class FarmTile
    {
        public SoilState state;
        public float waterLevel;
        public CropData crop;
    }

    [System.Serializable]
    public class CropData
    {
        public string cropId;
        public float growth;
        public int stage;    
    }
    
    [System.Serializable]
    public class FarmingData
    {
        public List<FarmingTileData> tiles = new();
    }

    [System.Serializable]
    public class FarmingTileData
    {
        public int x;
        public int y;
        public int z;

        public SoilState state;
        public float waterLevel;
        public CropData crop;
    }
}