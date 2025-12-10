using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Topography : MonoBehaviour
{

    public float width = 1.0f;
    public float height = 1.0f;

    [Range(0, 1f)]
    public float h = 0f;
    [Range(0, 1f)]
    public float t = 0f;

    public Vector3 normal;

    public GameObject marker;

    public bool withoutMarker = false;

    private Vector3[] vectrices;
    private Vector2[] uv;
    private int[] triangles;
    private Vector3[] normals;

    private Mesh mesh;

    private MeshRenderer mRender;
    private MeshFilter mFilter;

    private Vector3 initPosition;

    public AnimationCurve curve;

    public int resolution = 5;

    private void Awake()
    {
        mRender = GetComponent<MeshRenderer>();
        if (mRender == null)
        {
            mRender = gameObject.AddComponent<MeshRenderer>();
        }

        mFilter = GetComponent<MeshFilter>();
        if (mFilter == null)
        {
            mFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (resolution < 1)
        {
            resolution = 1;
        }
        if (curve == null)
        {
            curve = new AnimationCurve();
        }
        generateMesh();
    }

    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
    }


    public void generateMesh()
    {

        int vectricesNumber = (resolution * 2) + 2;

        vectrices = new Vector3[vectricesNumber];
        uv = new Vector2[vectricesNumber];
        normals = new Vector3[vectricesNumber];
        triangles = new int[resolution * 6];

        for (int i = 0; i < resolution; i++)
        {
            //   y
            // 
            //   ^  B   C
            //   |  -----
            //   |  |  /|
            //   |  | / |
            //   |  |/  |
            //   |  -----
            //   |  A   D
            //   |----------> x

            float ABx = i / (float)resolution;
            float DCx = (i + 1) / (float)resolution;

            float Cy = curve.Evaluate(DCx);

            int vectrice_idx;

            if (i == 0)
            {
                vectrice_idx = 0;

                float By = curve.Evaluate(ABx);

                // A
                vectrices[vectrice_idx + 0] = new Vector3(ABx * width, 0 * height, 0);
                uv[vectrice_idx + 0] = new Vector2(0f, 0f);
                normals[vectrice_idx + 0] = normal;

                // B
                vectrices[vectrice_idx + 1] = new Vector3(ABx * width, By * height, 0);
                uv[vectrice_idx + 1] = new Vector2(0f, By);
                normals[vectrice_idx + 1] = normal;

                // C
                vectrices[vectrice_idx + 2] = new Vector3(DCx * width, Cy * height, 0);
                uv[vectrice_idx + 2] = new Vector2(DCx, Cy);
                normals[vectrice_idx + 2] = normal;

                // D
                vectrices[vectrice_idx + 3] = new Vector3(DCx * width, 0 * height, 0);
                uv[vectrice_idx + 3] = new Vector2(DCx, 0);
                normals[vectrice_idx + 3] = normal;

                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;

                triangles[3] = 0;
                triangles[4] = 2;
                triangles[5] = 3;

            }
            else
            {
                int triangle_idx = 6 * i;

                vectrice_idx = 2 + (i * 2);

                // C
                vectrices[vectrice_idx + 0] = new Vector3(DCx * width, Cy * height, 0);
                uv[vectrice_idx + 0] = new Vector2(DCx, Cy);
                normals[vectrice_idx + 0] = normal;
                // D
                vectrices[vectrice_idx + 1] = new Vector3(DCx * width, 0 * height, 0);
                uv[vectrice_idx + 1] = new Vector2(DCx, 0);
                normals[vectrice_idx + 1] = normal;

                int vect_pos = vectrice_idx;

                triangles[triangle_idx + 0] = vect_pos - 1;
                triangles[triangle_idx + 1] = vect_pos - 2;
                triangles[triangle_idx + 2] = vect_pos;

                triangles[triangle_idx + 3] = vect_pos - 1;
                triangles[triangle_idx + 4] = vect_pos;
                triangles[triangle_idx + 5] = vect_pos + 1;

            }

        }
        mesh = new Mesh();
        mesh.vertices = vectrices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        //mesh.SetTriangles(triangles, 1);
        mesh.normals = normals;
        mesh.RecalculateBounds();
        mFilter.mesh = mesh;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if(withoutMarker)
        {
            transform.position = new Vector3(initPosition.x - (t * width), initPosition.y, initPosition.z);
        } else
        {
            marker.transform.localPosition = new(t * width, h * height, marker.transform.localPosition.z);
        }
    }

}
