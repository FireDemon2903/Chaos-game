//using System;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour
{
    public enum ShapeType { Triangle, Square, Pentagon, Hexagon, Fern };
    public ShapeType shapeType = ShapeType.Triangle;
    [Min(0)] public float radius;

    [Min(0)] public int KiloNumOfPoints;
    int Points => KiloNumOfPoints * 1000;

    public GameObject dot;

    Vector3[] vertices;
    Vector3[] pointPos;

    private void Start()
    {
        GenerateShape();
    }

    private void GenerateShape()
    {
        //vertices = CalculateShapePositions();
        //CreateObjects(vertices, Color.green, .1f);
        //GenerateFractal(Points);

        Fern();

        Debug.Break();  // Pause editor so computer does not die
    }

    private void GenerateFractal(int iter)
    {
        Vector3 sPos = transform.position;

        pointPos = new Vector3[iter];

        Color[] vertexColors = new Color[vertices.Length]; // Array to store colors for each vertex

        // Generate a unique color for each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            vertexColors[i] = Random.ColorHSV();
        }

        for (int i = 0; i < iter; i++)
        {
            int index = Random.Range(0, vertices.Length);
            Vector3 randomPos = vertices[index];

            Vector3 newDot = (sPos + randomPos) * .5f;

            pointPos[i] = newDot;

            sPos = newDot;

            CreateObject(newDot, vertexColors[index], .01f);
        }
    }

    private Vector3[] CalculateShapePositions()
    {
        Vector3[] calculatedPositions = shapeType switch
        {
            ShapeType.Triangle => CalculateRegularPolygonPositions(3),
            ShapeType.Square => CalculateRegularPolygonPositions(4),
            ShapeType.Pentagon => CalculateRegularPolygonPositions(5),
            ShapeType.Hexagon => CalculateRegularPolygonPositions(6),
            //ShapeType.Fern => Fern(),
            _ => new Vector3[1],
        };
        return calculatedPositions;
    }

    private Vector3[] CalculateRegularPolygonPositions(int numberOfSides)
    {
        Vector3[] calculatedPositions = new Vector3[numberOfSides];
        float angleStep = 2 * Mathf.PI / numberOfSides; // Calculate the angle between each vertex

        for (int i = 0; i < numberOfSides; i++)
        {
            float angle = angleStep * i + Mathf.PI / 2; // Adding π/2 to rotate counterclockwise by 90 degrees
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            calculatedPositions[i] = transform.position + new Vector3(x, y, 0);
        }

        return calculatedPositions;
    }

    private void CreateObjects(Vector3[] calculatedPositions, Color color, float rad)
    {
        foreach (Vector3 pos in calculatedPositions)
        {
            CreateObject(pos, color, rad);
        }
    }

    private void CreateObject(Vector3 position, Color color, float rad)
    {
        GameObject emptyGameObject = Instantiate(dot, transform);
        emptyGameObject.GetComponent<DebugDotDrawer>().dotColor = color;
        emptyGameObject.GetComponent<DebugDotDrawer>().dotRadius = rad;
        emptyGameObject.transform.position = position;
    }

    void Fern()
    {
        float x = transform.position.x;
        float y = transform.position.x;
        float xn = 0.0f;
        float yn = 0.0f;

        Vector3[] positions = new Vector3[Points];

        for (int i = 0; i < Points; i++)
        {
            float r = Random.value;

            if (r < .01)
            {
                xn = 0.0f;
                yn = 0.16f * y;
            }
            else if (r < 0.86)
            {
                xn = 0.85f * x + 0.04f * y;
                yn = -0.04f * x + 0.85f * y + 1.6f;
            }
            else if (r < 0.93)
            {
                xn = 0.2f * x - 0.26f * y;
                yn = 0.23f * x + 0.22f * y + 1.6f;
            }
            else
            {
                xn = -0.15f * x + 0.28f * y;
                yn = 0.26f * x + 0.24f * y + 0.44f;
            }
            //CreateObject(new Vector3(xn, yn, 0f), Color.green, .01f);
            positions[i] = new Vector3(xn, yn, 0);
            //Console.WriteLine($"xn: {xn}, yn:\t\t{yn}");
            x = xn;
            y = yn;
        }

        CreateObjects(positions, Color.green, .01f);
        //return positions;
    }

    private void Update()
    {
        // Draw lines between the empty game objects
        if (vertices != null)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 start = vertices[i];
                Vector3 end = vertices[(i + 1) % vertices.Length];
                Debug.DrawRay(start, end - start, Color.red);
            }
        }
    }
}
