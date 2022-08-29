using System.Diagnostics;
using DefaultNamespace;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class VfxGraphVoxelUpdater : MonoBehaviour
{
    [SerializeField] private float blockSize = 0.5f;
    [SerializeField] private int blockCount = 1000;
    [SerializeField] private float3 spawnRange = new(50, 50, 50);
    
    private readonly int blockCountProp = Shader.PropertyToID("BlockCount");
    private readonly int blockDataProp = Shader.PropertyToID("BlockData");
    private readonly int blockSizeProp = Shader.PropertyToID("BlockSize");
    private readonly int blockMeshProp = Shader.PropertyToID("BlockMesh");
    
    private VisualEffect effect;
    private GraphicsBuffer blockDataBuf;

    private NativeArray<float3> positions;
    private NativeArray<Color32> colors;
    private NativeArray<BlockData> blockData;
    private int usedBlockCount;
    

    private unsafe void Start()
    {
        effect = GetComponent<VisualEffect>();
        blockDataBuf = new(GraphicsBuffer.Target.Structured, blockCount, sizeof(BlockData));
        effect.SetGraphicsBuffer(blockDataProp, blockDataBuf);
        effect.SetFloat(blockSizeProp,  blockSize);
        effect.SetMesh(blockMeshProp, BlockMeshBuilder.BuildBlockMesh());
        
        blockData = new(blockCount, Allocator.Persistent);
        positions = new(blockCount, Allocator.Persistent);
        colors = new(blockCount, Allocator.Persistent);
        
        //SetBlockData();
    }

    private void OnDestroy()
    {
        blockDataBuf.Dispose();
        positions.Dispose();
        colors.Dispose();
        blockData.Dispose();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetBlockData();
            Stopwatch sw = Stopwatch.StartNew();
            UpdateBlocksVfx(positions, colors);
            sw.Stop();
            
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"Update Blocks: {sw.Elapsed.TotalMilliseconds}ms");    
            }
        }
    }

    private unsafe void SetBlockData()
    {
        if (positions.Length != blockCount)
        {
            blockData.ResizeArray(blockCount);
            positions.ResizeArray(blockCount);
            colors.ResizeArray(blockCount);
            
            blockDataBuf.Release();
            blockDataBuf.Dispose();
            blockDataBuf = new(GraphicsBuffer.Target.Structured, blockCount, sizeof(BlockData));
            effect.SetGraphicsBuffer(blockDataProp, blockDataBuf);
        }

        //float3 startOffset = -spawnRange / 2f;
        usedBlockCount = BlockDataProvider.GetVoxelDataVox(positions, colors);
        Debug.Log($"Imported: {usedBlockCount}");
    }

    private void UpdateBlocksVfx(NativeArray<float3> positions, NativeArray<Color32> colors)
    {
        Assert.AreEqual(positions.Length, colors.Length);
        int len = usedBlockCount;

        new PopulateJob
        {
            Length = usedBlockCount,
            Data = blockData,
            Positions = positions,
            Colors = colors,
        }.Run();

        effect.Reinit();
        effect.SetUInt(blockCountProp, (uint)len);

        Profiler.BeginSample("Set Buffer Data");
        blockDataBuf.SetData(blockData, 0, 0, len);
        Profiler.EndSample();
    }

    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    private struct BlockData
    {
        public Vector3 WorldPos;
        public uint Color; // 4 bytes color rgba

        public Color32 Color32
        {
            get => new((byte)(Color >> 24), (byte)(Color >> 16), (byte)(Color >> 8), (byte)Color);
            set => Color = (uint)value.r << 24 | (uint)value.g << 16 | (uint)value.b << 8 | value.a;
        }
    }

    [BurstCompile]
    private struct PopulateJob : IJob
    {
        public int Length;
        public NativeArray<BlockData> Data;
        public NativeArray<float3> Positions;
        public NativeArray<Color32> Colors;
        
        public void Execute()
        {
            for (int i = 0; i < Length; i++)
            {
                Data[i] = new BlockData
                {
                    WorldPos = Positions[i],
                    Color32 = Colors[i]
                };
            }
        }
    }
}