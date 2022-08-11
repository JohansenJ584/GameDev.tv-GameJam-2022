using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollabseMaster : MonoBehaviour
{
    [SerializeField]
    private float pWidth;
    [SerializeField]
    private float pHeight;
    [SerializeField]
    private float pScale;
    [SerializeField]
    private int seed;
    [SerializeField]
    private float persistance;
    [SerializeField]
    private float lacunarity;
    [SerializeField]
    private int octaves;
    [SerializeField]
    private Vector2 offset;

    private GameObject[,] terrianSquares;

    public bool autoUpdate;


    public void GenerateMap()
    {
        valueValid();
        terrianSquares = new GameObject[(int)pWidth, (int)pHeight];
        while (this.transform.childCount > 0)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float flootHieght = 0;
        float sampleX;
        float sampleY;
        for (int x = 0; x < (int)pWidth; x++)
        {
            for (int y = 0; y < (int)pHeight; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                GameObject tempObject = null;
                //float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    sampleX = (x / pWidth) / pScale * frequency + octaveOffsets[i].x;
                    sampleY = (y / pHeight) / pScale * frequency + octaveOffsets[i].y;
                    flootHieght = (Mathf.PerlinNoise(sampleX, sampleY) - 0.5f) * 10 * amplitude;
                    if (i == 0)
                    {
                        tempObject = makeNewPrimative(flootHieght, x, y, i); // i is octives
                        terrianSquares[x, y] = tempObject;
                    }
                    else
                    {
                       editPrimative(tempObject, flootHieght);
                    }
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
            }
        }
        for (int x = 0; x < (int)pWidth; x++)
        {
            for (int y = 0; y < (int)pHeight; y++)
            {
                makeVertexEven(x, y);
            }
        }
        //this.transform.Rotate(180f, 0f, 0f);
        combineMeshes();
    }

    void combineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        if (transform.GetComponent<MeshFilter>().sharedMesh)
        {
            transform.GetComponent<MeshFilter>().sharedMesh.Clear();
        }
        transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

    void makeVertexEven(int row, int coluomn)
    {
        MeshFilter ObjMesh = terrianSquares[row, coluomn].GetComponent<MeshFilter>();
        Vector3[] ObjVertices = ObjMesh.sharedMesh.vertices;
        bool LeftOk = false, UpOk = false, RightOk = false, DownOk = false;
        if(row > 0)
        {
            LeftOk = true;
        }
        if (row < (int)pWidth - 1)
        {
            RightOk = true;
        }
        if (coluomn > 0)
        {
            DownOk = true;
        }
        if (coluomn < (int)pHeight - 1)
        {
            UpOk = true;
        }
        Vector3[] tempVertices;
        //These are all the side that have double uses
        //Vertex 0 Bottom Left compared to the plane the left
        if (LeftOk)
        {
            tempVertices = terrianSquares[row - 1, coluomn].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[1].y > ObjVertices[0].y)
            {
                ObjVertices[0].y = tempVertices[1].y;
            }
            //Vertex 3 top left compared to the plane the left
            if (tempVertices[2].y > ObjVertices[3].y)
            {
                ObjVertices[3].y = tempVertices[2].y;
            }
        }
        //Vertex 0 Bottom Left compared to the plane below
        if (DownOk)
        {
            tempVertices = terrianSquares[row, coluomn - 1].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[3].y > ObjVertices[0].y)
            {
                ObjVertices[0].y = tempVertices[3].y;
            }
            //Vertex 1 botoom right compared to plane below
            if (tempVertices[2].y > ObjVertices[1].y)
            {
                ObjVertices[1].y = tempVertices[2].y;
            }
        }
        //vertex 2 compared up
        if (UpOk)
        {
            tempVertices = terrianSquares[row, coluomn + 1].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[1].y > ObjVertices[2].y)
            {
                ObjVertices[2].y = tempVertices[1].y;
            }
            //Vertex 3 top Left compared up
            if (tempVertices[0].y > ObjVertices[3].y)
            {
                ObjVertices[3].y = tempVertices[0].y;
            }
        }
        //Vertex 1 Bottom Right
        if (RightOk)
        {
            tempVertices = terrianSquares[row + 1, coluomn].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (RightOk && tempVertices[0].y > ObjVertices[1].y)
            {
                ObjVertices[1].y = tempVertices[0].y;
            }
            //Vertex 2 top Right
            if (RightOk && tempVertices[3].y > ObjVertices[2].y)
            {
                ObjVertices[2].y = tempVertices[3].y;
            }
        }
        //Corners
        //Vertex 0 Bottom left
        if (DownOk && LeftOk)
        {
            tempVertices = terrianSquares[row - 1, coluomn - 1].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[2].y > ObjVertices[0].y)
            {
                ObjVertices[0].y = tempVertices[2].y;
            }
        }
        //Vertex 1 Bottom Right
        if (DownOk && RightOk)
        {
            tempVertices = terrianSquares[row + 1, coluomn - 1].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[3].y > ObjVertices[1].y)
            {
                ObjVertices[1].y = tempVertices[3].y;
            }
        }
        //Vertex 2 top Right
        if (RightOk && UpOk)
        {
            tempVertices = terrianSquares[row + 1, coluomn + 1].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[0].y > ObjVertices[2].y)
            {
                ObjVertices[2].y = tempVertices[0].y;
            }
        }

        //Vertex 3 top left
        if (UpOk && LeftOk)
        {
            tempVertices = terrianSquares[row - 1, coluomn + 1].GetComponent<MeshFilter>().sharedMesh.vertices;
            if (tempVertices[1].y > ObjVertices[3].y)
            {
                ObjVertices[3].y = tempVertices[1].y;
            }
        }
        //Its got sent
        ObjMesh.sharedMesh.SetVertices(ObjVertices);
        float maxY = 0;
        foreach(Vector3 vert in ObjVertices)
        {
            if(maxY == 0)
            {
                maxY = vert.y;
            }
            else if(maxY < vert.y)
            {
                maxY = vert.y;
            }
        }
        float normalizedValue = Mathf.InverseLerp(-5f, 5f, maxY);
        float result = (int)Mathf.Lerp(0f, 255f, normalizedValue)/255f;
        Material newMaterial = new Material(Shader.Find("Standard"));
        // Debug.Log(maxY + " turns into " + result+ " why is thie bad " + new Vector4(result, result, result, 255f));
        newMaterial.color = Color.red;//new Vector4(result, result, result, 1);
        newMaterial.color = new Color(result, result, result, .5f);
        ObjMesh.gameObject.GetComponent<Renderer>().material = newMaterial;
    }

    GameObject makeNewPrimative(float hieghtOrColor, int x, int y, int octaves)
    {
        float size = 1;
        Mesh m = new Mesh();
        m.name = "Plane_New_Mesh";
        m.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(size, 0, 0), new Vector3(size, 0, size), new Vector3(0, 0, size) };
        m.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
        m.triangles = new int[] { 3,2,0,2,1,0};// {0, 1, 2, 0, 2, 3 }; 
        m.RecalculateBounds();
        m.RecalculateNormals();
        GameObject obj = new GameObject("New_Plane_Fom_Script", typeof(MeshRenderer), typeof(MeshFilter));
        obj.GetComponent<MeshFilter>().mesh = m;
        //Material newMaterial = new Material(Shader.Find("Standard"));
        //newMaterial.color = new Vector4(hieghtOrColor, hieghtOrColor, hieghtOrColor, 1);
        //obj.GetComponent<MeshRenderer>().material = newMaterial;
        //obj.transform.Rotate(180f, 0f, 0f);

        obj.transform.position = new Vector3(x, 0, y);
        obj.transform.parent = this.transform;
        editPrimative(obj, hieghtOrColor);
        return obj;
    }

    void editPrimative(GameObject obj, float hieghtOrColor)
    {
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        Vector3[] newVertexList = mf.sharedMesh.vertices;
        for (int i = 0; i < newVertexList.Length; i++)
        {
            newVertexList[i] = new Vector3(newVertexList[i].x, newVertexList[i].y + hieghtOrColor, newVertexList[i].z);
        }
        mf.sharedMesh.SetVertices(newVertexList);
    }

    void valueValid()
    {
        if (pWidth <= 0)
        {
            this.pWidth = 1;
        }
        if (pHeight <= 0)
        {
            this.pHeight = 1;
        }
        if (pScale <= 0)
        {
            this.pScale = .0001f;
        }
        if (seed < 0)
        {
            this.seed = 1;
        }
        if (persistance < 0)
        {
            this.persistance = 0;
        }
        if (lacunarity < 0)
        {
            this.lacunarity = 0;
        }
        if (octaves <= 0)
        {
            this.octaves = 1;
        }

    }
}
