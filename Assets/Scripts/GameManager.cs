using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private ObjectListSO objectList;
    [SerializeField] private ObjectSO placeholder;
    [SerializeField] private List<Transform> planes;

    //Creamos la Matrix ¿Cuál es el átomo?: un objeto SO
    private ObjectSO[][] objectsMap;

    private int rows = 5;
    private int cols = 5;
    private int numberofObjects = 4;


    // Start is called before the first frame update
    void Start()
    {
        Generate(); 
        //Validate(); 
        Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Generate()
    {
        int row;
        int col;
        // Para no tener stringify cuando lo usamos abajo
        List<ObjectSO> objects = objectList.objects;

        //Tenemos que inicializar cada vez la "rejilla" para que en diferentes planos genere combinaciones distintas
        Initialize();
        //Genera un número aleatorio que usaremos para varias cosas
        System.Random random = new System.Random();

        for (int i = 0; i < numberofObjects; i++)
        {
            //generates new random empty position --> Desde 0 hasta que llegue al número total de objetos genera una posición aleatoria
            //Al 0 le da un row y col aleatorio, al 1 igual...
            do
            {
                row = random.Next(rows);
                col = random.Next(cols);
            } while (objectsMap[row][col] != null);
            //Esto último evita que ponga dos en la misma posición

            //OPCIONAL: hacemos un debug de ese row y ese col
            Debug.Log($"{row} {col}");

            // put random item in random position generated previously
            // Desde 0 al total de objetos, coge el mapa de posiciones generado y asignale un objeto aleatorio de la lista
            objectsMap[row][col] = objects[random.Next(objects.Count)];
        }


        // console debug map
        // OPCIONAL: después del bucle de generación, para debuggear mapeamos lo que hay en cada posición de la rejilla
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                Debug.Log($"{i} {j} {objectsMap[i][j]}");

    }
    
    //Initialize the matrix --> inicializa esa rejilla
    private void Initialize()
    {
        objectsMap = new ObjectSO[rows][];
        for (int i = 0; i < objectsMap.Length; i++)
            objectsMap[i] = new ObjectSO[cols];
    }

    // create objects in Unity scene
    void Create()
    {
        foreach (var plane in planes)
            
        {
            Generate();
            CreatePlane(plane, 10, 10);
        }

        //planes es una lista de transforms, el transform de un plano, objeto de la escena Unity. Si queremos 6 planos, los asignaremos en el editor a esos elementos de lista serializados
        //los creamos con ancho y alto en unidades Unity de 10x10 en el script de abajo
    }


    void CreatePlane(Transform plane, float planeWidth, float planeLength)
    {

        //El step lo obtenemos dividiendo el ancho o largo del plano (en unidades unity) entre el número de filas o columnas (de la rejilla de la matrix)
        float rowStep = planeWidth / rows;
        float colStep = planeLength / cols;

        //Con esto ajustamos la posición del plano 
        Vector3 origin = plane.position - plane.forward * planeWidth / 2
                                        - plane.right * planeLength / 2
                                        + plane.forward * rowStep / 2
                                        + plane.right * colStep / 2;

        
        //Con esto creamos y colocamos los objetos en la posición teniendo en cuenta el offset
        for (int i = 0; i < rows; i++)
            for (int j=0; j < cols; j++)
            {
                Vector3 objectOffset = plane.forward * i * rowStep + plane.right * j * colStep;
                if (objectsMap[i][j] != null)
                {
                    Instantiate(objectsMap[i][j].prefab, origin + objectOffset + objectsMap[i][j].prefabOffset, Quaternion.identity);
                }
                else 
                {
                    Instantiate(placeholder.prefab, origin + objectOffset + placeholder.prefabOffset, Quaternion.identity);
                }
            }
        //OPCIONAL: para debuggear metemos un placeholder si en la matrix generada en el mapa no hay nada
    }


    

}
