using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralFlame : MonoBehaviour
{
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private float speed = 25.0f;
    private float timePeriod = 100.0f;
    private float time = 0.0f;

    private float timeOffset;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        int segments = 16;

        // Create the rhombus shape by specifying vertex positions
        Vector3[] vertices = new Vector3[segments * 4];

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            float y = t - 0.5f;

            float x_offset;

            if (t < 1.0f / 3.0f)
            {
                x_offset = Mathf.Lerp(0.1f, 0.25f, Mathf.Abs(t));
            }
            else
            {
                x_offset = Mathf.Lerp(0.25f, 0.0f, Mathf.Abs(t));
            }

            float angle = Mathf.PI * Mathf.Abs(t);
            float x_left = 0.5f - x_offset + Mathf.Cos(angle) * 0.07f;
            float x_right = 0.5f + x_offset + Mathf.Cos(angle) * 0.07f;

            vertices[i] = new Vector3(x_left, y, 0);
            vertices[segments + i] = new Vector3(x_right, y, 0);
        }

        // Create the triangles
        int[] triangles = new int[(segments - 1) * 6];
        for (int i = 0; i < segments - 1; i++)
        {
            int idx = i * 6;

            triangles[idx] = i;
            triangles[idx + 1] = i + 1;
            triangles[idx + 2] = segments + i;

            triangles[idx + 3] = i + 1;
            triangles[idx + 4] = segments + i + 1;
            triangles[idx + 5] = segments + i;
        }

        // Assign the vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Generate UVs based on vertex positions
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
        }
        mesh.uv = uvs;

        // Calculate normals
        mesh.RecalculateNormals();

        timeOffset = Random.Range(0.0f, timePeriod);
    }

    private void Update()
    {
        time = ((Time.time + timeOffset) % timePeriod) / timePeriod;
        time = Mathf.Sin(time * 2 * Mathf.PI) * speed;

        float noisyAmplitude = Mathf.PerlinNoise(time, 0.0f);
        noisyAmplitude *= 1.3f;

        float noisySpeed = Mathf.PerlinNoise(0.0f, time);
        noisySpeed *= 0.5f;

        meshRenderer.material.SetFloat("_WaveAmplitude", noisyAmplitude);
        meshRenderer.material.SetFloat("_WaveSpeed", noisySpeed);
    }
}
