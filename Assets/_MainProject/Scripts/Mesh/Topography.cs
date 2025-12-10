using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Topography : MonoBehaviour
{

    public float width = 1.0f;
    public float height = 1.0f;
    public float length = 1.0f;

    [Range(0, 1f)]
    public float h = 0f;
    [Range(0, 1f)]
    public float t = 0f;

    public Vector3 normal;
    public Vector3 normal_up;

    public GameObject marker;

    public bool withoutMarker = false;

    private Vector3[] vectrices;
    private Vector2[] uv;
    private int[] triangles;
    private int[] deep_triangles;
    private Vector3[] normals;

    private Mesh mesh;

    private MeshRenderer mRender;
    private MeshFilter mFilter;

    private Vector3 initPosition;

    public AnimationCurve curve;

    public int resolution = 5;

    public bool deep = false;

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

    public void generateMeshDeep()
    {
        int vectricesNumber;
        vectricesNumber = (resolution * 3) + 3;

        vectrices = new Vector3[vectricesNumber];
        uv = new Vector2[vectricesNumber];
        normals = new Vector3[vectricesNumber];
        triangles = new int[resolution * 6];
        deep_triangles = new int[resolution * 6];

        //    z ^   C(2)  D(3)   (6)     (9)
        //     /     ----  ...   -  .   -         
        //    /    /|  /        /      /     
        //   /    / | /        /      /      
        //       /  |/        /      /     
        //   ^ B(1)  E(4)    (7)    (10)      
        //   |  -----   ...  -      -         
        //   |  |  /|        |      |      
        //   |  | / |        |      |      
        //   |  |/  |        |      |      
        //   |  -----   ...  -      -      
        //   | A(0)  F(5)    (8)    (11)      
        //   |----------> x

        int i = 0;
        int vectrice_idx = 0;
        int triangle_idx = 0;

        float ABx = i / (float)resolution;
        float EFx = (i + 1) / (float)resolution;
        float Ey = curve.Evaluate(EFx);
        float By = curve.Evaluate(ABx);

        // A (0)
        vectrices[vectrice_idx + 0] = new Vector3(ABx * width, 0 * height, 0);
        uv[vectrice_idx + 0] = new Vector2(0f, 0f);
        normals[vectrice_idx + 0] = normal;

        // B (1)
        vectrices[vectrice_idx + 1] = new Vector3(ABx * width, By * height, 0);
        uv[vectrice_idx + 1] = new Vector2(0f, By);
        normals[vectrice_idx + 1] = normal;

        // C (2)
        vectrices[vectrice_idx + 2] = new Vector3(ABx * width, By * height, length);
        uv[vectrice_idx + 2] = new Vector2(0f, 1f);
        normals[vectrice_idx + 2] = normal_up;

        // D (3)
        vectrices[vectrice_idx + 3] = new Vector3(EFx * width, Ey * height, length);
        uv[vectrice_idx + 3] = new Vector2(EFx, 1f);
        normals[vectrice_idx + 3] = normal_up;
        
        // E (4)
        vectrices[vectrice_idx + 4] = new Vector3(EFx * width, Ey * height, 0);
        uv[vectrice_idx + 4] = new Vector2(EFx, Ey);
        normals[vectrice_idx + 4] = normal;

        // F (5)
        vectrices[vectrice_idx + 5] = new Vector3(EFx * width, 0 * height, 0);
        uv[vectrice_idx + 5] = new Vector2(EFx, 0);
        normals[vectrice_idx + 5] = normal;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 4;

        triangles[3] = 0;
        triangles[4] = 4;
        triangles[5] = 5;

        deep_triangles[0] = 1;
        deep_triangles[1] = 2;
        deep_triangles[2] = 3;

        deep_triangles[3] = 1;
        deep_triangles[4] = 3;
        deep_triangles[5] = 4;
 
        for (i = 1; i < resolution; i++)
        {
            ABx = i / (float)resolution;
            EFx = (i + 1) / (float)resolution;
            Ey = curve.Evaluate(EFx);
            By = curve.Evaluate(ABx);

            triangle_idx = 6 * i;
            vectrice_idx = ((i + 1) * 3);

            // D
            vectrices[vectrice_idx + 0] = new Vector3(EFx * width, Ey * height, length);
            uv[vectrice_idx + 0] = new Vector2(EFx, 1f);
            normals[vectrice_idx + 0] = normal_up;

            // E
            vectrices[vectrice_idx + 1] = new Vector3(EFx * width, Ey * height, 0);
            uv[vectrice_idx + 1] = new Vector2(EFx, Ey);
            normals[vectrice_idx + 1] = normal;
            
            // F
            vectrices[vectrice_idx + 2] = new Vector3(EFx * width, 0 * height, 0);
            uv[vectrice_idx + 2] = new Vector2(EFx, 0);
            normals[vectrice_idx + 2] = normal;

            int vect_pos = vectrice_idx;

            
            triangles[triangle_idx + 0] = vect_pos - 1;
            triangles[triangle_idx + 1] = vect_pos - 2;
            triangles[triangle_idx + 2] = vect_pos + 1;

            triangles[triangle_idx + 3] = vect_pos - 1;
            triangles[triangle_idx + 4] = vect_pos + 1;
            triangles[triangle_idx + 5] = vect_pos + 2;

            deep_triangles[triangle_idx + 0] = vect_pos - 2;
            deep_triangles[triangle_idx + 1] = vect_pos - 3;
            deep_triangles[triangle_idx + 2] = vect_pos;

            deep_triangles[triangle_idx + 3] = vect_pos - 2;
            deep_triangles[triangle_idx + 4] = vect_pos;
            deep_triangles[triangle_idx + 5] = vect_pos + 1;          

        }

        mesh = new Mesh();
        mesh.vertices = vectrices;
        mesh.uv = uv;
        //mesh.triangles = triangles;
        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles, 0);
        mesh.SetTriangles(deep_triangles, 1);
        
        mesh.normals = normals;
        mesh.RecalculateBounds();
        mFilter.mesh = mesh;
    
    }


    public void generateMesh()
    {
        if(deep) {
            generateMeshDeep();
            return;
        }

        int vectricesNumber = (resolution * 2) + 2;
        
        vectrices = new Vector3[vectricesNumber];
        uv = new Vector2[vectricesNumber];
        normals = new Vector3[vectricesNumber];
        triangles = new int[resolution * 6];
        deep_triangles = new int[resolution * 6];


        //   ^ B(1)  C(2)    (4)    (6)      
        //   |  -----   ...  -      -         
        //   |  |  /|        |      |      
        //   |  | / |        |      |      
        //   |  |/  |        |      |      
        //   |  -----   ...  -      -      
        //   | A(0)  D(3)    (5)    (7)      
        //   |----------> x

        int i = 0;

        int vectrice_idx = 0;
        int triangle_idx = 0;

        float ABx = i / (float)resolution;
        float DCx = (i + 1) / (float)resolution;
        float Cy = curve.Evaluate(DCx);
        float By = curve.Evaluate(ABx);

        // A (0)
        vectrices[vectrice_idx + 0] = new Vector3(ABx * width, 0 * height, 0);
        uv[vectrice_idx + 0] = new Vector2(0f, 0f);
        normals[vectrice_idx + 0] = normal;

        // B (1)
        vectrices[vectrice_idx + 1] = new Vector3(ABx * width, By * height, 0);
        uv[vectrice_idx + 1] = new Vector2(0f, By);
        normals[vectrice_idx + 1] = normal;

        // C (2)
        vectrices[vectrice_idx + 2] = new Vector3(DCx * width, Cy * height, 0);
        uv[vectrice_idx + 2] = new Vector2(DCx, Cy);
        normals[vectrice_idx + 2] = normal;

        // D (3)
        vectrices[vectrice_idx + 3] = new Vector3(DCx * width, 0 * height, 0);
        uv[vectrice_idx + 3] = new Vector2(DCx, 0);
        normals[vectrice_idx + 3] = normal;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;                    
 
        for (i = 1; i < resolution; i++)
        {
            ABx = i / (float)resolution;
            DCx = (i + 1) / (float)resolution;
            Cy = curve.Evaluate(DCx);
            By = curve.Evaluate(ABx);

            triangle_idx = 6 * i;
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

        mesh = new Mesh();
        mesh.vertices = vectrices;
        mesh.uv = uv;
        //mesh.triangles = triangles;
        mesh.SetTriangles(triangles, 0);
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
