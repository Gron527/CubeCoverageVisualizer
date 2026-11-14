using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// források: Reading a JSON file - an easy how to with Unity3d: https://www.youtube.com/watch?v=XbdnG__wzZ8
public class LoadCubes : MonoBehaviour
{
    public Material outlineMaterial;
    public Material cubeMaterial;
    
    public List<Cube> Cubes=new List<Cube>();   
    public List<GameObject> allCubes= new List<GameObject>();
    
    bool isFirst = true;


    float expand = 4f;


    



    [System.Serializable]
    public class Cube 
    {
        public float x;
        public float y;
        public float z;
        public float x_center;
        public float y_center;
        public float z_center;
        public float size;
    }
    [System.Serializable]
    public class Solution
    {
        public List<Cube> cubes;
        public float result;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSolution();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // a korábbi, fájkból történõ beolvasás (place holder)
    void LoadSolution()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("cubes_example");
        if (jsonFile==null)
        {
            Debug.LogError("Nem található a Resources / cubes_example.json fájl!");
            return;
        }

        string json=jsonFile.text;
        Solution solution = JsonUtility.FromJson<Solution>(json);

        var Cubes=solution.cubes;



        float maxX= float.MinValue;
        float minX= float.MaxValue;

        float maxY= float.MinValue;
        float minY= float.MaxValue;

        float maxZ = float.MinValue;
        float minZ = float.MaxValue;




        Debug.Log("LoadSolution futott!!!");



        Debug.Log("Beolvasott kockák száma: " + solution.cubes.Count);

        foreach(var Cube in Cubes)
        {
            GameObject cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
           
            cubeObj.transform.localScale=Vector3.one * Cube.size ;
            Renderer rend = cubeObj.GetComponent<Renderer>();


            float realsize = Cube.size  / 2f;

            if (isFirst)
            {
                cubeObj.transform.position = new Vector3(Cube.x_center,Cube.y_center,Cube.z_center);
                rend.materials = new Material[] { outlineMaterial };
                cubeObj.name = "MAIN_CUBE";
                isFirst = false;
                minX = Cube.x_center - realsize;
                maxX = Cube.x_center + realsize;
                minY = Cube.y_center - realsize;
                maxY = Cube.y_center + realsize;
                minZ = Cube.z_center - realsize;
                maxZ = Cube.z_center + realsize;

            }
            else
            {
                float j = 0.02f; // 1–2 cm-nyi eltérés Unity-ben
                cubeObj.transform.position = new Vector3(
                    Cube.x_center + Random.Range(-j, j),
                    Cube.y_center + Random.Range(-j, j),
                    Cube.z_center + Random.Range(-j, j)
                );
                Material mat = new Material(cubeMaterial);
                mat.color = new Color(Random.value, Random.value, Random.value);
                rend.material = mat;
                mat.renderQueue = 3001 + allCubes.Count;

            }
            // a mozgástér kiszámításához kell ez adja itt meg a befogó téglatest 6 koordináta határát
            if (Cube.x_center - realsize < minX)
                {
                    minX = Cube.x_center - realsize;
                }
                if (Cube.x_center + realsize > maxX)
                {
                    maxX = Cube.x_center + realsize;
                }
                if (Cube.y_center - realsize < minY)
                {
                    minY = Cube.y_center - realsize;
                }
                if (Cube.y_center + realsize > maxY)
                {
                    maxY = Cube.y_center + realsize;
                }
                if (Cube.z_center - realsize < minZ)
                {
                    minZ = Cube.z_center - realsize;
                }
                if (Cube.z_center + realsize > maxZ)
                {
                    maxZ = Cube.z_center + realsize;
                }

            
           

       



            this.Cubes.Add(Cube);
           this.allCubes.Add(cubeObj);
        }

       
        var x= (maxX + minX) /2;
        var y= (maxY + minY) /2;
        var z = (maxZ + minZ)/ 2;

        var a = (maxX-x) ; // a 3 befogó él  felének hossza
        var b = (maxY-y) ;
        var c= (maxZ-z) ;

        // itt pedig a téglalap éleit beszorzzuk
        maxX = x + (a) *expand;  
        minX= x - (a) *expand;
        maxY= y + (b)  *expand;
        minY= y - (b)  * expand;
        maxZ= z + (c) *  expand;
        minZ= z - (c) * expand;
        


        var cam = FindFirstObjectByType<FreeCameraController>();

        if (cam != null)
        {
            cam.minX = minX;
            cam.maxX = maxX;
            cam.minY = minY;
            cam.maxY = maxY;
            cam.minZ = minZ;
            cam.maxZ = maxZ;
        }

    }

    public void LoadSolutionFromJson(string json)
    {
       

        foreach (var c in allCubes)
            Destroy(c);
        allCubes.Clear();
        Cubes.Clear();
        isFirst = true;

        // JSON parse ugyanúgy
        Solution solution = JsonUtility.FromJson<Solution>(json);
        var cubes = solution.cubes;

        float maxX = float.MinValue;
        float minX = float.MaxValue;

        float maxY = float.MinValue;
        float minY = float.MaxValue;

        float maxZ = float.MinValue;
        float minZ = float.MaxValue;

        Debug.Log("Beolvasott kockák száma: " + cubes.Count);

        foreach (var Cube in cubes)
        {
       

            GameObject cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
           
            cubeObj.transform.localScale = Vector3.one * Cube.size;
            Renderer rend = cubeObj.GetComponent<Renderer>();
            Debug.Log($"Cube: x={Cube.x}, y={Cube.y}, z={Cube.z}, size={Cube.size}");


            float realsize = Cube.size / 2f;

            if (isFirst)
            {
                cubeObj.transform.position = new Vector3(Cube.x_center,Cube.y_center,Cube.z_center);
                rend.materials = new Material[] { outlineMaterial };
                
                cubeObj.name = "MAIN_CUBE";
                isFirst = false;

                minX = Cube.x_center - realsize;
                maxX = Cube.x_center + realsize;
                minY = Cube.y_center - realsize;
                maxY = Cube.y_center + realsize;
                minZ = Cube.z_center - realsize;
                maxZ = Cube.z_center + realsize;
            }
            else
            {
                float j = 0.02f;

                cubeObj.transform.position = new Vector3(
                    Cube.x_center + Random.Range(-j, j),
                    Cube.y_center + Random.Range(-j, j),
                    Cube.z_center + Random.Range(-j, j)
                );
                Material mat = new Material(cubeMaterial);
                mat.color = new Color(Random.value, Random.value, Random.value);
                mat.renderQueue = 3001 + allCubes.Count;
                rend.material = mat;

            }
            minX = Mathf.Min(minX, Cube.x_center - realsize);
                maxX = Mathf.Max(maxX, Cube.x_center + realsize);
                minY = Mathf.Min(minY, Cube.y_center - realsize);
                maxY = Mathf.Max(maxY, Cube.y_center + realsize);
                minZ = Mathf.Min(minZ, Cube.z_center - realsize);
                maxZ = Mathf.Max(maxZ, Cube.z_center + realsize);
            

            Cubes.Add(Cube);
            allCubes.Add(cubeObj);
        }
        // hasonlóképpen a mozgástér megállapítása
        float cx = (maxX + minX) / 2;
        float cy = (maxY + minY) / 2;
        float cz = (maxZ + minZ) / 2;

        var a = (maxX - cx); 
        var b = (maxY - cy);
        var ce = (maxZ - cz);

        maxX = cx + (a) * expand;
        minX = cx - (a) * expand;
        maxY = cy + (b) * expand;
        minY = cy - (b) * expand;
        maxZ = cz + (ce) * expand;
        minZ = cz - (ce) * expand;


        var cam = FindFirstObjectByType<FreeCameraController>();
        if (cam != null)
        {
            cam.minX = minX; 
            cam.maxX = maxX;
            cam.minY = minY;
            cam.maxY = maxY;
            cam.minZ = minZ; 
            cam.maxZ = maxZ;
        }
        int mainSize = (int)Cubes[0].size;

    
     

        Debug.Log("Szerveres megoldás betöltve!");
        var hotkey = FindFirstObjectByType<CubeTransparencyHotkey>();
        if (hotkey != null)
            hotkey.ResetStateToTransparent();
    }

    
}
