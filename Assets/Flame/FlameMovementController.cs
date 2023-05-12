using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO solve flame rising problem
// TODO Adjust flame flicker speed in ordinance with flame movement speed

public class FlameMovementController : MonoBehaviour
{
    private RaycastHit hitPlace;
    private RaycastHit hitMove;

    private bool flameOnTable;

    private Vector3 movementDirection;
    public float speed;

    public GameObject flamePrefab;

    private Vector3 lastFlamePosition;
    private float flameSpawnPositionDifference = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.01f;

        PlaceFlameOnTable();
        GenerateRandomMovementDirection();

        lastFlamePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
        KeepFlameOnSurface();
        ExpandFlame();
    }

    private void ExpandFlame()
    {
        // Instantiate new flames as the flame moves (spread).
        float distanceTraveled = Vector3.Distance(transform.position, lastFlamePosition);

        // If the flame has traveled enough, spawn another flame.
        if (distanceTraveled > flameSpawnPositionDifference)
        {
            GameObject newFlame = Instantiate(flamePrefab, transform.position, transform.rotation);
            StartCoroutine(RiseFromTable(newFlame, 0.5f));
            newFlame.AddComponent<FlameNoisyFlickerController>();

            lastFlamePosition = transform.position;
        }
    }

    private void KeepFlameOnSurface()
    {
        Debug.DrawRay(transform.position + movementDirection * speed * Time.deltaTime, -Vector3.up * 10, Color.red);

        // Cast ray straight down (while looking ahead, in order to change course before going off the table).
        if (Physics.Raycast(transform.position + movementDirection * (speed + 0.1f) * Time.deltaTime, -Vector3.up, out hitMove))
        {
            // If it hits the table.
            if (hitMove.collider.gameObject.tag == "Table")
            {
                flameOnTable = true;
                Debug.Log("ON TABLE");
            }
            else
            {
                flameOnTable = false;
                Debug.Log("OFF TABLE" + transform.position + ToString());
            }
        }

        // If the flame is not on the table, pick another random direction.
        if (!flameOnTable)
        {
            GenerateRandomMovementDirection();
            flameOnTable = true;
        }
    }

    void PlaceFlameOnTable()
    {
        // Cast a ray straight down.
        if (Physics.Raycast(transform.position, -Vector3.up, out hitPlace))
        {
            // If it hits the table.
            if (hitPlace.collider.gameObject.tag == "Table")
            {
                // Position flame on table (slightly above).
                transform.position = hitPlace.point + new Vector3(0, 0.001f, 0);
            }
        }
    }

    void GenerateRandomMovementDirection()
    {
        GameObject table = GameObject.FindGameObjectWithTag("Table");
        Vector3 tableCenterPosition = new Vector3(table.transform.position.x, transform.position.y, table.transform.position.z);
        Vector3 directionToTableCenter = (tableCenterPosition - transform.position).normalized;

        // Create a random offset.
        float angleOffset = Random.Range(-45.0f, 45.0f); // Change this range depending on how much randomness you want
        Quaternion rotation = Quaternion.Euler(0, angleOffset, 0);

        // Apply the random offset to the direction.
        movementDirection = rotation * directionToTableCenter;
    }

    IEnumerator RiseFromTable(GameObject flame, float duration)
    {
        float elapsed = 0.0f;
        float initialScale = 0.0f;
        float finalScale = flame.transform.localScale.y;

        Vector3 currentScale = flame.transform.localScale;
        currentScale.y = initialScale;
        flame.transform.localScale = currentScale;

        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            t = t * t * (3.0f - 2.0f * t);

            currentScale.y = Mathf.Lerp(initialScale, finalScale, t);
            flame.transform.localScale = currentScale;
            yield return null;
        }
    }
}
