using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//The following namespaces reference paid assets or removed public repositories & as such are not included in this sample.
//Were I to re-do this, I'd probably use Triangle.Net : https://github.com/Ranguna/Triangle-NET-Unity-Port
using Delaunay;
using Delaunay.Geo;
using Lexic;

public class MapGenerator : MonoBehaviour
{
    public string mapName;


    public GameObject strategyBoardHolder;
    public GameObject strategyDecorationHolder;
    public Material highlightMaterial;
    public Material detailMaterial;
    public Material waterMaterial;
    public GameObject interestPointUIPrefab;

    public GameObject boardBackground;
    public GameObject titleHolder;
    public Text mapTitle;
    //public NameGenerator cityNameGen;
    //public NameGenerator districtNameGen;

    [Header("Map Generation Variables")]

    [SerializeField]
    private int m_pointCount = 30;

    [SerializeField]
    private float minPointDistance = 2;

    [SerializeField]
    private int maxPointGenAttempts = 50;

    [SerializeField]
    private int minDistricts = 20;


    public float m_mapWidth = 50;

    public float m_mapHeight = 25;

    [SerializeField]
    private float coastChance;

    [SerializeField]
    private float riverDeltaChance;

    public bool hasCoast;
    public bool hasRiver;
    public bool coastOnXAxis;
    public float coastFloat;

    List<InterestPoint> testInterestPoints;


    public float minNameAngle;
    public float roughUnitsToPixels;


    //Mesh Generation Variables
    private List<Vector2> m_points;
    private List<LineSegment> m_edges = null;

    private enum Facing { Up, Forward, Right };

    private void Awake()
    {
        //GenerateMap();
    }

    public void GenerateMap()
    {

        foreach (Transform child in strategyBoardHolder.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in strategyDecorationHolder.transform)
        {
            Destroy(child.gameObject);
        }

        boardBackground.transform.position = new Vector3(m_mapWidth / 2, m_mapHeight / 2, 0.1f);
        boardBackground.transform.localScale = new Vector3((m_mapWidth / 2.5f), m_mapHeight / 2.5f);

        titleHolder.transform.position = new Vector3(m_mapWidth / 2, m_mapHeight);

        testInterestPoints = new List<InterestPoint>();

        //nameGen = GetComponent<NameGenerator>();

        List<uint> colors = new List<uint>();
        m_points = new List<Vector2>();
        List<List<Vector2>> m_sites = new List<List<Vector2>>();

        //Figure out what kind of map we're makin' here
        if (riverDeltaChance > (1 - coastChance))
        {
            riverDeltaChance = (1 - coastChance);
        }

        float riverRand = UnityEngine.Random.Range(0f, 1f);

        //Randomly set Water Feature Bools
        if (riverRand <= coastChance)
        {
            hasCoast = true;
            if (riverRand <= riverDeltaChance)
            {
                hasRiver = true;
            }
            else
            {
                hasRiver = false;
            }
        }
        else
        {
            hasCoast = false;
            hasRiver = true;
        }

        if (hasCoast)
        {
            coastOnXAxis = (UnityEngine.Random.Range(0, 2) == 0);
            if (coastOnXAxis)
            {
                coastFloat = (UnityEngine.Random.Range(0, 2) * m_mapWidth);
            }
            else
            {
                coastFloat = (UnityEngine.Random.Range(0, 2) * m_mapHeight);
            }
        }

        //What's in a name?
        if(cityNameGen != null)
        {
            mapName = cityNameGen.GetNextRandomName();
            mapTitle.text = mapName;
        }
        
        /*
        //Start Voronoi-in' (Vanilla Voronoi)
        for (int i = 0; i < m_pointCount; i++)
        {
            colors.Add(0);
            m_points.Add(new Vector2(
                UnityEngine.Random.Range(0, m_mapWidth),
                UnityEngine.Random.Range(0, m_mapHeight))
            );

        }
        */

        //Start Voronoi-in'(Min Distance Verstion)
        for (int i = 0; i < m_pointCount; i++)
        {
            colors.Add(0);
            bool success = false;
            int pointGenEscape = 0;
            while(success == false && pointGenEscape <= maxPointGenAttempts)
            {
                Vector2 pointToAdd = new Vector2(UnityEngine.Random.Range(0, m_mapWidth), UnityEngine.Random.Range(0, m_mapHeight));
                bool hasOverlap = false;
                foreach (Vector2 point in m_points)
                {
                    if(Vector2.Distance(pointToAdd, point) <= minPointDistance)
                    {

                        hasOverlap = true;
                    }
                }

                if (!hasOverlap)
                {
                    m_points.Add(pointToAdd);
                    success = true;
                } else
                {
                    pointGenEscape++;
                    if (pointGenEscape == maxPointGenAttempts)
                    {
                        Debug.Log("Max point generation attempts exceeded!");
                    }
                }
            }
    }

        Debug.Log("Points: " + m_points.Count);

        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, m_mapWidth, m_mapHeight));
        m_edges = v.VoronoiDiagram();
        m_sites = v.Regions();


        int interestPointIndex = 0;

        //Turn Voronoi graph into a real map
        List<List<Vector2>> noHoles = new List<List<Vector2>>();
        List<Vector3> tempVerts = new List<Vector3>();
        List<int> tempTris = new List<int>();

        foreach (Vector2 point in m_points)
        {
            List<Vector2> verts = v.Region(point);

            if (hasCoast)
            {
                if (coastOnXAxis)
                {
                    if (verts.Any(i => i.x == coastFloat))
                    {
                        GameObject coast = new GameObject();
                        coast.name = "Coast ";


                        Mesh mesh = new Mesh();
                        coast.AddComponent<MeshFilter>().mesh = mesh;
                        coast.AddComponent<MeshRenderer>().material = waterMaterial;

                        Triangulation.triangulate(verts, noHoles, out tempTris, out tempVerts);

                        Vector2[] newUvs = CalculateUVs(tempVerts.ToArray(), 1.0f);

                        mesh.vertices = tempVerts.ToArray();
                        mesh.triangles = tempTris.ToArray();
                        mesh.uv = newUvs;

                        coast.transform.SetParent(strategyDecorationHolder.transform);
                    }
                }
                else if (!coastOnXAxis)
                {
                    if (verts.Any(i => i.y == coastFloat))
                    {
                        GameObject coast = new GameObject();
                        coast.name = "Coast ";


                        Mesh mesh = new Mesh();
                        coast.AddComponent<MeshFilter>().mesh = mesh;
                        coast.AddComponent<MeshRenderer>().material = waterMaterial;

                        Triangulation.triangulate(verts, noHoles, out tempTris, out tempVerts);

                        Vector2[] newUvs = CalculateUVs(tempVerts.ToArray(), 1.0f);

                        mesh.vertices = tempVerts.ToArray();
                        mesh.triangles = tempTris.ToArray();
                        mesh.uv = newUvs;

                        coast.transform.SetParent(strategyDecorationHolder.transform);
                    }
                }
            }

            if( !verts.Any(i => i.x == m_mapWidth) && !verts.Any(i => i.x == 0) && !verts.Any(i => i.y == m_mapHeight) && !verts.Any(i => i.y == 0))
            {

                GameObject newInterestPoint = new GameObject();

                newInterestPoint.transform.position = new Vector3(point.x, point.y, 0);

                string districtName = districtNameGen.GetNextRandomName();

                newInterestPoint.name = districtNameGen.GetNextRandomName();



                Mesh mesh = new Mesh();
                newInterestPoint.AddComponent<MeshFilter>().mesh = mesh;
                newInterestPoint.AddComponent<MeshRenderer>().material = highlightMaterial;
                InterestPoint info = newInterestPoint.AddComponent<InterestPoint>();
                //InterestPointHUD newHUD = newInterestPoint.AddComponent<InterestPointHUD>();

                info.interestPointName = newInterestPoint.name;
                info.voronoiPoint = point;

                List<Vector2> localVerts = new List<Vector2>();
                //swap verts from world space to local space around the voronoi point
                foreach(Vector2 vert in verts)
                {
                    Vector2 localVert = newInterestPoint.transform.InverseTransformPoint(vert);
                    localVerts.Add(localVert);
                }


                //Triangulation.triangulate(verts, noHoles, out tempTris, out tempVerts);
                Triangulation.triangulate(localVerts, noHoles, out tempTris, out tempVerts);

                Vector2[] newUvs = CalculateUVs(tempVerts.ToArray(), 1.0f);

                mesh.vertices = tempVerts.ToArray();
                mesh.triangles = tempTris.ToArray();
                mesh.uv = newUvs;

                //Collider col = newInterestPoint.AddComponent<MeshCollider>();
                PolygonCollider2D col2D = newInterestPoint.AddComponent<PolygonCollider2D>();
                //col2D.points = verts.ToArray();
                col2D.points = localVerts.ToArray();
                Vector3 colCenter = col2D.bounds.center;

                newInterestPoint.transform.SetParent(strategyBoardHolder.transform);

                GameObject interestPointHighlighter = new GameObject();
                interestPointHighlighter.name = newInterestPoint.name + "_Highlighter";

                interestPointHighlighter.transform.SetParent(newInterestPoint.transform, false);


                Mesh highlightMesh = new Mesh();
                interestPointHighlighter.AddComponent<MeshFilter>().mesh = highlightMesh;
                interestPointHighlighter.AddComponent<MeshRenderer>().material = detailMaterial;

                //This is the attempt to manually move the verts towards the center point by a constant
                /*
                List<Vector2> tempHighlightVerts = ResizeBy(tempVerts, point, 0.1f);

                List<Vector3> highlightVerts = new List<Vector3>();
                List<int> highlightTris = new List<int>();

                Triangulation.triangulate(tempHighlightVerts, noHoles, out highlightTris, out highlightVerts);

                Vector2[] highlightUVs = CalculateUVs(highlightVerts.ToArray(), 0.5f);

                highlightMesh.vertices = highlightVerts.ToArray();
                highlightMesh.triangles = highlightTris.ToArray();
                highlightMesh.uv = highlightUVs;
                */

                //This is the attempt to scale the mesh around a center point as evenly as possible
                highlightMesh.vertices = tempVerts.ToArray();
                highlightMesh.triangles = tempTris.ToArray();
                highlightMesh.uv = newUvs;

                Vector3 newScale = new Vector3(0.95f, 0.95f);
                ScaleAround(interestPointHighlighter, interestPointHighlighter.transform.position, newScale);
                interestPointHighlighter.transform.position = new Vector3(point.x, point.y, -0.01f);

                /*
                Vector3 highlighterScale = new Vector3(0.9f, 0.9f);
                ScaleAround(interestPointHighlighter, colCenter, highlighterScale);


                */

                //interestPointHighlighter.transform.position = new Vector3(transform.position.x, transform.position.y, -0.01f);
                GameObject HUD = Instantiate(interestPointUIPrefab, interestPointHighlighter.transform, false);
                Vector2 centroid = Centroid(tempVerts);
                HUD.transform.localPosition = centroid;
                //HUD.transform.localPosition = interestPointHighlighter.transform.InverseTransformPoint(col2D.bounds.center);

                InterestPointHUD newHUD = HUD.GetComponent<InterestPointHUD>();

                newHUD.minAngle = minNameAngle;
                newHUD.unitsToPixels = roughUnitsToPixels;
                info.hud = newHUD;


                Text HUDText = HUD.GetComponentInChildren<Text>();
                if (HUDText != null)
                {
                    HUDText.text = newInterestPoint.name;
                }

                testInterestPoints.Add(info);
            }

            interestPointIndex++;
        }

        foreach(InterestPoint i in testInterestPoints)
        {
            Vector2 sitePoint = i.voronoiPoint;
            List<Vector2> site = v.Region(sitePoint);
            List<Vector2> neighboursByPoint = v.NeighborSitesForSite(sitePoint);
            List<InterestPoint> neighbours = new List<InterestPoint>();
            foreach (Vector2 center in neighboursByPoint)
            {
                InterestPoint neighbour = testInterestPoints.Find(p => p.voronoiPoint == center);
                neighbours.Add(neighbour);
            }

            i.neighbours = neighbours.ToArray();
        }
    }

    public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relative scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.localPosition = FP;
    }

    public List<Vector2> ResizeBy(List<Vector3> inputVerts, Vector3 centerPoint, float targetResize)
    {
        List<Vector2> outputVerts = new List<Vector2>();

        foreach(Vector3 vert in inputVerts)
        {
            //TODO: Manipulate the lil' bastards
            Vector3 translationDir = Vector3.Normalize(vert - centerPoint); //targetResize;

            outputVerts.Add(translationDir);
        }

        return outputVerts;
    }

    public Vector3 Centroid(List<Vector3> points)
    {
        Vector3 result = points.Aggregate(Vector3.zero, (current, point) => current + point);
        result /= points.Count;

        return result;
    }

    #region UV Calculator
    public static Vector2[] CalculateUVs(Vector3[] v/*vertices*/, float scale)
    {
        var uvs = new Vector2[v.Length];

        for (int i = 0; i < uvs.Length; i += 3)
        {
            int i0 = i;
            int i1 = i + 1;
            int i2 = i + 2;

            //Special handling if vertex count isn't a multiple of 3
            if (i == uvs.Length - 1)
            {
                i1 = 0;
                i2 = 1;
            }
            if (i == uvs.Length - 2)
            {
                i2 = 0;
            }

            Vector3 v0 = v[i0];
            Vector3 v1 = v[i1];
            Vector3 v2 = v[i2];

            Vector3 side1 = v1 - v0;
            Vector3 side2 = v2 - v0;
            var direction = Vector3.Cross(side1, side2);
            var facing = FacingDirection(direction);
            switch (facing)
            {
                case Facing.Forward:
                    uvs[i0] = ScaledUV(v0.x, v0.y, scale);
                    uvs[i1] = ScaledUV(v1.x, v1.y, scale);
                    uvs[i2] = ScaledUV(v2.x, v2.y, scale);
                    break;
                case Facing.Up:
                    uvs[i0] = ScaledUV(v0.x, v0.z, scale);
                    uvs[i1] = ScaledUV(v1.x, v1.z, scale);
                    uvs[i2] = ScaledUV(v2.x, v2.z, scale);
                    break;
                case Facing.Right:
                    uvs[i0] = ScaledUV(v0.y, v0.z, scale);
                    uvs[i1] = ScaledUV(v1.y, v1.z, scale);
                    uvs[i2] = ScaledUV(v2.y, v2.z, scale);
                    break;
            }
        }
        return uvs;
    }

    private static bool FacesThisWay(Vector3 v, Vector3 dir, Facing p, ref float maxDot, ref Facing ret)
    {
        float t = Vector3.Dot(v, dir);
        if (t > maxDot)
        {
            ret = p;
            maxDot = t;
            return true;
        }
        return false;
    }

    private static Facing FacingDirection(Vector3 v)
    {
        var ret = Facing.Up;
        float maxDot = Mathf.NegativeInfinity;

        if (!FacesThisWay(v, Vector3.right, Facing.Right, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.left, Facing.Right, ref maxDot, ref ret);

        if (!FacesThisWay(v, Vector3.forward, Facing.Forward, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.back, Facing.Forward, ref maxDot, ref ret);

        if (!FacesThisWay(v, Vector3.up, Facing.Up, ref maxDot, ref ret))
            FacesThisWay(v, Vector3.down, Facing.Up, ref maxDot, ref ret);

        return ret;
    }

    private static Vector2 ScaledUV(float uv1, float uv2, float scale)
    {
        return new Vector2(uv1 / scale, uv2 / scale);
    }
    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (m_points != null)
        {
            for (int i = 0; i < m_points.Count; i++)
            {
                Gizmos.DrawSphere(m_points[i], 0.2f);
                Handles.DrawWireDisc(m_points[i], new Vector3(0, 0, 1), minPointDistance);
            }
        }

        if (m_edges != null)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < m_edges.Count; i++)
            {
                Vector2 left = (Vector2)m_edges[i].p0;
                Vector2 right = (Vector2)m_edges[i].p1;
                Gizmos.DrawLine((Vector3)left, (Vector3)right);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(0, m_mapHeight));
        Gizmos.DrawLine(new Vector2(0, 0), new Vector2(m_mapWidth, 0));
        Gizmos.DrawLine(new Vector2(m_mapWidth, 0), new Vector2(m_mapWidth, m_mapHeight));
        Gizmos.DrawLine(new Vector2(0, m_mapHeight), new Vector2(m_mapWidth, m_mapHeight));
    }
    #endregion
}
