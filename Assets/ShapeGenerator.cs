using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShapeGenerator : MonoBehaviour
{
    //enum ShapeType { Triangle, Square, Pentagon, Hexagon, BarnsleyFern };
    //ShapeType shapeType = ShapeType.Triangle;

    // WTF is this??? https://openprocessing.org/sketch/136642/

    [Range(3, 10)] public int NumVertecies;
    [Min(0)] public float BaseShapeRadius;
    [Range(.001f, 0.05f)] public float PointRadius;
    [Min(0)] public int KiloNumOfPoints;
    int Points => KiloNumOfPoints * 1000;

    [SerializeField] int InstansiatedPoints = 0;

    public GameObject dot;

    Vector3[] vertices;
    Vector3[] pointPos;

    private void Start()
    {
        vertices = CalculateShapePositions();
        CreateObjects(vertices, Color.green, .1f);
        GenerateFractal(Points);
        //Fern();

        //StartCoroutine(YieldGenerateFractal(Points));
        //StartCoroutine(YieldFern());

        Debug.Break();  // Pause editor so computer does not die
    }

    # region Generation
    private void GenerateFractal(int iter)
    {
        Vector3 sPos = transform.position; // Starting position
        pointPos = new Vector3[iter]; // Array to hold points

        Color[] vertexColors = GenColors(); // Array to store colors for each vertex
        float ratio = CalculateOptRatio(NumVertecies); // Ratio calculation

        for (int i = 0; i < iter; i++)
        {
            int index = Random.Range(0, vertices.Length); // Select a random vertex
            Vector3 randomPos = vertices[index]; // Get the position of the selected vertex

            // New point calculation using the custom ratio
            Vector3 newDot = (randomPos + sPos) * (1-ratio);

            pointPos[i] = newDot; // Store the new point
            sPos = newDot; // Move the current position to the new point

            CreateObject(newDot, vertexColors[index], PointRadius);
        }
    }

    void Fern()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float xn, yn = 0.0f;

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
            x = xn;
            y = yn;
        }

        CreateObjects(positions, Color.green, PointRadius);
        //return positions;
    }

    IEnumerator YieldFern()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float xn, yn = 0.0f;

        //Vector3[] positions = new Vector3[Points];

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
            yield return CreateObject(new Vector3(xn, yn, 0f), Color.green, .01f);
            //positions[i] = new Vector3(xn, yn, 0);
            x = xn;
            y = yn;
        }
    }

    float CalculateOptRatio(int N)
    {
        // Calc angle in deg
        int angle = (N - 2) * 180 / N;

        // Calc number of iterations for a
        int n = Mathf.FloorToInt(N / 4);
        
        // Calc a from: ∑_(i=1)^n(cos⁡[i(PI-ϕ)])
        float a = n == 0 ? 0f : Enumerable.Range(1, n).Sum(i => Mathf.Cos(i * (Mathf.PI - angle * Mathf.PI / 180f)));

        // Return ratio
        return (1 + 2 * a) / (2 + 2 * a);
    }

    Color[] GenColors()
    {
        var a = new Color[vertices.Length];
        // Generate a unique color for each vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            a[i] = Random.ColorHSV();
        }
        return a;
    }
    #endregion Generation

    #region BaseRegPolygon
    //private Vector3[] CalculateShapePositions()
    //{
    //    Vector3[] calculatedPositions = shapeType switch
    //    {
    //        ShapeType.Triangle => CalculateRegularPolygonPositions(3),
    //        ShapeType.Square => CalculateRegularPolygonPositions(4),
    //        ShapeType.Pentagon => CalculateRegularPolygonPositions(5),
    //        ShapeType.Hexagon => CalculateRegularPolygonPositions(6),
    //        _ => new Vector3[1],
    //    };
    //    return calculatedPositions;
    //}

    private Vector3[] CalculateShapePositions()
    {
        Vector3[] calculatedPositions = CalculateRegularPolygonPositions(NumVertecies);
        return calculatedPositions;
    }

    private Vector3[] CalculateRegularPolygonPositions(int numberOfSides)
    {
        Vector3[] calculatedPositions = new Vector3[numberOfSides];
        float angleStep = 2 * Mathf.PI / numberOfSides; // Calculate the angle between each vertex

        for (int i = 0; i < numberOfSides; i++)
        {
            float angle = angleStep * i + Mathf.PI / 2; // Adding π/2 to rotate counterclockwise by 90 degrees
            float x = BaseShapeRadius * Mathf.Cos(angle);
            float y = BaseShapeRadius * Mathf.Sin(angle);
            calculatedPositions[i] = transform.position + new Vector3(x, y, 0);
        }

        return calculatedPositions;
    }
    #endregion BaseRegPolygon

    #region CreatePoints
    private void CreateObjects(Vector3[] calculatedPositions, Color color, float rad)
    {
        foreach (Vector3 pos in calculatedPositions)
        {
            CreateObject(pos, color, rad);
        }
    }

    private GameObject CreateObject(Vector3 position, Color color, float rad)
    {
        GameObject emptyGameObject = Instantiate(dot, transform);
        emptyGameObject.GetComponent<DebugDotDrawer>().dotColor = color;
        emptyGameObject.GetComponent<DebugDotDrawer>().dotRadius = rad;
        emptyGameObject.transform.position = position;
        InstansiatedPoints++;
        return emptyGameObject;
    }
    #endregion CreatePoints

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
