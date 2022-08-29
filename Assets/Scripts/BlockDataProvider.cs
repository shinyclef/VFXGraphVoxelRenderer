using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VoxReader;
using VoxReader.Interfaces;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public static class BlockDataProvider
    {
        public static void GetVoxelDataRandom(NativeArray<float3> positions, NativeArray<Color32> colors, float3 startOffset, float3 spawnRange)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = startOffset + new float3(Random.value * spawnRange.x, Random.value *  spawnRange.y, Random.value * spawnRange.z);
                colors[i] = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 0);
            }
        }
        
        public static int GetVoxelDataVox(NativeArray<float3> positions, NativeArray<Color32> colors)
        {
            TextAsset voxFile = Resources.Load("Dimas Trees") as TextAsset;
            byte[] bytes = voxFile.bytes;
            IVoxFile vox = VoxReader.VoxReader.Read(bytes);
            IModel model = vox.Models[0];
            
            if (model.Voxels.Length > positions.Length)
            {
                Debug.LogError($"BlockCount must support {model.Voxels.Length} blocks.");
                return -1;
            }

            Voxel[] voxels = model.Voxels;
            for (int i = 0; i < voxels.Length; i++)
            {
                var p = voxels[i].Position;
                var c = voxels[i].Color;
                
                positions[i] = new(p.X, p.Z, p.Y);
                colors[i] = new(c.R, c.G, c.B, c.A);
            }

            return model.Voxels.Length;
        }
    }
}