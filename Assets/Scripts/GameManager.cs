using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private ObjectListSO objectList;
    [SerializeField] private List<Transform> planes;

    //Creamos la Matrix ¿Cuál es el átomo? Un objeto SO
    private ObjectSO[][] objectsMap;

    private int rows = 5;
    private int cols = 5;

    private int numberofObjects = 4;


    // Start is called before the first frame update
    void Start()
    {
        Generate(); 
        //Validate(); 
        //Create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Generate()
    {
        Initialize();
    
    }
    
    //Initialize the matrix
    private void Initialize()
    {
        objectsMap = new ObjectSO[this.rows][];
        for (int i = 0; i < objectsMap.Length; i++)
            objectsMap[i] = new ObjectSO[this.cols];
    }


}
