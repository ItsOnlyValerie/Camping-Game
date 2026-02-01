using UnityEngine;
using System.Collections.Generic;

public class GrassSpawner : MonoBehaviour
{
    public Mesh GrassMesh;
    public Material GrassMaterial;
    public Texture2D GrassMask;

    public int GrassCount = 1000;
    public Vector2 WorldSize = new Vector2(50, 50);

    public List<Matrix4x4> matrices = new List<Matrix4x4>();

    void Start()
    {
        SpawnGrass();
    }

    void SpawnGrass()
    {
        matrices.Clear();

        for (int i = 0; i < GrassCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-WorldSize.x / 2f, WorldSize.x / 2f), 0, Random.Range(-WorldSize.y / 2f, WorldSize.y / 2f));

            float u = (pos.x / WorldSize.x) + 0.5f;
            float v = (pos.z / WorldSize.y) + 0.5f;

            float MaskValue = GrassMask.GetPixelBilinear(u, v).r;

            if (MaskValue < 0.5f)
                continue;

            Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);

            matrices.Add(Matrix4x4.TRS(pos, rot, scale));
        }
    }

    void Update()
    {
        Graphics.DrawMeshInstanced(GrassMesh, 0, GrassMaterial, matrices);
    }
}
