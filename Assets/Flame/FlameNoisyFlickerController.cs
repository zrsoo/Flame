using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameNoisyFlickerController : MonoBehaviour
{
    private float speed = 25.0f;
    private float timePeriod = 100.0f;
    private float time = 0.0f;

    private float timeOffset;

    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        timeOffset = Random.Range(0.0f, timePeriod);
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNoisyAmplitudeSpeed();
    }

    private void GenerateNoisyAmplitudeSpeed()
    {
        time = ((Time.time + timeOffset) % timePeriod) / timePeriod;
        time = Mathf.Sin(time * 2 * Mathf.PI) * speed;

        float noisyAmplitude = Mathf.PerlinNoise(time, 0.0f);
        noisyAmplitude *= 0.6f;

        float noisySpeed = Mathf.PerlinNoise(0.0f, time);
        noisySpeed *= 0.5f;

        meshRenderer.material.SetFloat("_WaveAmplitude", noisyAmplitude);
        meshRenderer.material.SetFloat("_WaveSpeed", noisySpeed);
    }
}
