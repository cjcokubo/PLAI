using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This generates a dynamic mesh during runtime, but for now we should just use a prefabbed plane

public class MyPolyMesh : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verts;
    Vector2[] uvs;
    Vector3[] normals;

    public int halfX = 1;
    public int halfY = 1;
    public float intervalX = 0.25f;
    public float intervalY = 0.25f;
    int totalX, totalY, totalVerts;

    public float backingDistance = 0.25f;
    public float angleRatio = 0.2f;
    public bool AngleOrPos = false;

    public Color myColor;

    float spaceOffWall = 0.02f;

   public GameObject customMeshPrefab;
    GameObject targetMesh;


    void Start()
    {
        totalX = (2 * halfX) + 1;
        totalY = (2 * halfY) + 1;
        totalVerts = totalX * totalY;
        mesh = new Mesh();
        mesh.name = "CustomDecalMesh";
        targetMesh = Instantiate(customMeshPrefab, Vector3.zero, Quaternion.identity);
        if(!TestBounds())
        {
            FindMeshBounds();
            BuildMesh();
        }
        targetMesh.GetComponent<MeshFilter>().mesh = mesh;
        targetMesh.GetComponent<MeshRenderer>().material.color = new Color(myColor.r, myColor.g, myColor.b, 1f);

        spaceOffWall *= Random.Range(0.995f, 1.005f);
    }


    void Update()
    {
       
    }


    bool TestBounds()
    {
        Vector3 pos = transform.position - transform.forward * backingDistance;
        Vector3 D = transform.forward + (-transform.up * angleRatio * halfY) + ( transform.right * angleRatio * halfX);
        Vector3 A = transform.forward + ( transform.up * angleRatio * halfY) + (-transform.right * angleRatio * halfX);
        Vector3 B = transform.forward + ( transform.up * angleRatio * halfY) + ( transform.right * angleRatio * halfX);
        Vector3 C = transform.forward + (-transform.up * angleRatio * halfY) + (-transform.right * angleRatio * halfX);
        RaycastHit hitA, hitB, hitC, hitD;

        Physics.Raycast(pos, A, out hitA, 999);
        if(!hitA.collider)
        {
            return false;
        }
        Physics.Raycast(pos, B, out hitB, 999);
        if (!hitB.collider)
        {
            return false;
        }
        Physics.Raycast(pos, C, out hitC, 999);
        if (!hitC.collider)
        {
            return false;
        }
        Physics.Raycast(pos, D, out hitD, 999);
        if (!hitD.collider)
        {
            return false;
        }
        //all 4 hit!
        if(hitA.collider != hitB.collider || hitB.collider != hitC.collider || hitC.collider != hitD.collider)
        {
            return false;
        }

        //simple mesh!
        Vector3[] verts = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] tris = new int[6];
        verts[0] = hitA.point + (hitA.normal * spaceOffWall);
        verts[1] = hitB.point + (hitB.normal * spaceOffWall);
        verts[2] = hitC.point + (hitC.normal * spaceOffWall);
        verts[3] = hitD.point + (hitD.normal * spaceOffWall);
        normals[0] = hitA.normal;
        normals[1] = hitB.normal;
        normals[2] = hitC.normal;
        normals[3] = hitD.normal;
        uvs[0] = new Vector3(0f,1f);
        uvs[1] = new Vector3(1f,1f);
        uvs[2] = new Vector3(0f,0f);
        uvs[3] = new Vector3(1f,0f);
        //tri 1
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;
        //tri 2
        tris[3] = 1;
        tris[4] = 2;
        tris[5] = 3;
        mesh.vertices = verts;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = tris;

        Color[] colors = new Color[4];
        for(int i = 0; i < 4; i++)
        {
            colors[i] = myColor;
        }
        mesh.colors = colors;

        return true;
    }

    void FindMeshBounds()
    {

        verts = new Vector3[totalVerts];
        normals = new Vector3[totalVerts];
        uvs = new Vector2[totalVerts];
        RaycastHit hit;
        int vertIndex = 0;
        for (int y = -halfY; y <= halfY; y++)
        {
            for (int x = -halfX; x <= halfX; x++)
                {          
                Vector3 pos = transform.position - transform.forward * backingDistance;
                Vector3 fwd = transform.forward;
                if (!AngleOrPos)
                {
                    fwd += (transform.up * y * angleRatio) + (transform.right * x * angleRatio);
                }
                else
                {
                pos += transform.right * x * intervalX;
                pos += transform.up * y * intervalY;
                }

                Physics.Raycast(pos, fwd, out hit, 999);
                if (hit.collider)
                {
                    verts[vertIndex] = hit.point + (hit.normal * spaceOffWall);
                    normals[vertIndex] = hit.normal;
                }
                else
                {
                    float distance = Vector3.Distance(verts[vertIndex - 1], pos);
                    verts[vertIndex] = pos + fwd * distance;
                    normals[vertIndex] = normals[vertIndex - 1];
                }
                 
                vertIndex++;
            }
        }
    }


    void BuildMesh()
    {
        mesh.vertices = verts;



        int squaresX = totalX - 1;
        int squaresY = totalY - 1;
        int totalSquares = squaresX * squaresY;
        float uvIntervalX = 1f / totalX;
        float uvIntervalY = 1f / totalY;

        List<int> myTris = new List<int>();
        for (int y = 0; y < squaresY; y++)
        {
            for (int x = 0; x < squaresX; x++)
            {
                int index = x + (y * totalX);
                int A = index;
                int B = index + 1;
                int C = index + totalY;
                int D = index + totalY + 1;
                uvs[A] = new Vector2(x * uvIntervalX, y * uvIntervalY);
                uvs[B] = new Vector2((x+1) * uvIntervalX, y * uvIntervalY);
                uvs[C] = new Vector2(x * uvIntervalX, (y+1) * uvIntervalY);
                uvs[D] = new Vector2((x+1) * uvIntervalX, (y + 1) * uvIntervalY);

                //tri 1;
                myTris.Add(A);
                myTris.Add(B);
                myTris.Add(C);

                //tri 2;
                myTris.Add(C);
                myTris.Add(B);
                myTris.Add(D);
            }
        }



        mesh.triangles = myTris.ToArray();
        mesh.uv = uvs;
        mesh.normals = normals;
        Color[] colors = new Color[totalVerts];
        for (int i = 0; i < totalVerts; i++)
        {
            colors[i] = myColor;
        }
        mesh.colors = colors;
    }

}
