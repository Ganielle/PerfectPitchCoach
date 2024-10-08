using UnityEngine;

public class Note : MonoBehaviour
{

    [Header("DEBUGGER")]
    public NoteSpawner noteSpawner;
    public bool isHit;


    private LineRenderer lineRenderer;
    private float speed;
    private float xPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.gameObject.CompareTag("Hit") && !isHit)
        {
            isHit = true;
            noteSpawner.score += 1;
        }
    }

    public void SetData(float length, float xPos, float speed, NoteSpawner noteSpawner)
    {
        this.noteSpawner = noteSpawner;
        this.xPos = xPos;
        this.speed = speed;
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 startPosition = new Vector3(0, 0, 0); // Starting position
        Vector3 endPosition = new Vector3(0, length, 0); // Length is added to the Y position

        // Set positions for the line
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        transform.position = new Vector3(xPos, endPosition.y - 1, 0f);
    }

    private void Update()
    {
        // Move the note downwards over time
        transform.position += Vector3.up * Time.deltaTime * speed; // Adjust speed as needed
        DeleteGameObject();
    }

    private void DeleteGameObject()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        // Check if the object is above the top of the camera
        if (viewportPosition.y > 2 - xPos )
        {
            Destroy(gameObject);
        }
    }
}
