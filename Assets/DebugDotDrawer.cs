using UnityEngine;

public class DebugDotDrawer : MonoBehaviour
{
    public float dotRadius = 0.1f; // The size of the dot
    public Color dotColor = Color.red; // The color of the dot

    private void Update()
    {
        // Draw a dot at the GameObject's position
        Debug.DrawLine(transform.position - new Vector3(dotRadius, 0, 0), transform.position + new Vector3(dotRadius, 0, 0), dotColor);
        Debug.DrawLine(transform.position - new Vector3(0, dotRadius, 0), transform.position + new Vector3(0, dotRadius, 0), dotColor);
        Debug.DrawLine(transform.position - new Vector3(0, 0, dotRadius), transform.position + new Vector3(0, 0, dotRadius), dotColor);
    }
}
