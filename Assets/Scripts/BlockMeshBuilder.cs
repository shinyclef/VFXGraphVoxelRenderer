using UnityEngine;

namespace DefaultNamespace
{
    public static class BlockMeshBuilder
    {
        public static Mesh BuildBlockMesh()
        {
            Vector3[] verts = new Vector3[]
            {
                // right
                new( 0.5f, -0.5f, -0.5f),
                new( 0.5f,  0.5f, -0.5f),
                new( 0.5f,  0.5f,  0.5f),
                new( 0.5f, -0.5f,  0.5f),
                
                // up
                new(-0.5f,  0.5f, -0.5f),
                new(-0.5f,  0.5f,  0.5f),
                new( 0.5f,  0.5f,  0.5f),
                new( 0.5f,  0.5f, -0.5f),
                
                // forward
                new( 0.5f, -0.5f,  0.5f),
                new( 0.5f,  0.5f,  0.5f),
                new(-0.5f,  0.5f,  0.5f),
                new(-0.5f, -0.5f,  0.5f)
            };

            Vector3[] normals = new Vector3[]
            {
                // right
                new(1, 0, 0),
                new(1, 0, 0),
                new(1, 0, 0),
                new(1, 0, 0),
                
                // up
                new(0, 1, 0),
                new(0, 1, 0),
                new(0, 1, 0),
                new(0, 1, 0),
                
                // forward
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1),
                new(0, 0, 1)
            };
            
            Vector4[] tangents = new Vector4[]
            {
                // right
                new(0, 0, 1, 1),
                new(0, 0, 1, 1),
                new(0, 0, 1, 1),
                new(0, 0, 1, 1),
                
                // up
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                
                // forward
                new(-1, 0, 0, 1),
                new(-1, 0, 0, 1),
                new(-1, 0, 0, 1),
                new(-1, 0, 0, 1)
            };
                
            Vector3[] flippedPositions = new Vector3[]
            {
                // left
                new(-0.5f, -0.5f,  0.5f),
                new(-0.5f,  0.5f,  0.5f),
                new(-0.5f,  0.5f, -0.5f),
                new(-0.5f, -0.5f, -0.5f),
                
                // down
                new(-0.5f, -0.5f,  0.5f),
                new(-0.5f, -0.5f, -0.5f),
                new( 0.5f, -0.5f, -0.5f),
                new( 0.5f, -0.5f,  0.5f),
                
                // back
                new(-0.5f, -0.5f, -0.5f),
                new(-0.5f,  0.5f, -0.5f),
                new( 0.5f,  0.5f, -0.5f),
                new( 0.5f, -0.5f, -0.5f)
            };
            
            Vector4[] flippedTangents = new Vector4[]
            {
                // left
                new(0, 0, -1, 1),
                new(0, 0, -1, 1),
                new(0, 0, -1, 1),
                new(0, 0, -1, 1),
                
                // down
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                
                // back
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                new(1, 0, 0, 1),
                new(1, 0, 0, 1)
            };
            

            int[] tris = new int[3 * 6];
            for (int i = 0; i < 3; i++)
            {
                tris[i * 6 + 0] = i * 4 + 0;
                tris[i * 6 + 1] = i * 4 + 1;
                tris[i * 6 + 2] = i * 4 + 2;
                
                tris[i * 6 + 3] = i * 4 + 0;
                tris[i * 6 + 4] = i * 4 + 2;
                tris[i * 6 + 5] = i * 4 + 3;
            }

            Vector2[] uvs = new Vector2[3 * 4];
            for (int i = 0; i < 3; i++)
            {
                uvs[i * 4 + 0] = new(0f, 0f);
                uvs[i * 4 + 1] = new(0f, 1f);
                uvs[i * 4 + 2] = new(1f, 1f);
                uvs[i * 4 + 3] = new(1f, 0f);    
            }

            Mesh mesh = new();
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.SetUVs(0, uvs);
            mesh.SetUVs(1, flippedPositions);
            mesh.SetUVs(2, flippedTangents);
            return mesh;
        }
    }
}
