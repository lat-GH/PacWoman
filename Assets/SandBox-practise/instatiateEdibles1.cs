using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstatiateEdibles : MonoBehaviour {

    public GameObject edibleGameObjcet;

    public Vector2 whereX;

    void Start()
    {   
        for (int i = 1; i <= 5; i++)
        {
            whereX = new Vector2(i, 0.0f);
        }

        Instantiate(edibleGameObjcet, whereX, transform.rotation);
    }

}

//Instantiate accepts any component type, because it instantiates the GameObject 

/*public Transform tempInstantiate;

void Start()
{
    for (int y = 0; y < 5; y++)
    {
        for (int x = 0; x < 5; x++)
        {
            Instantiate(tempInstantiate, new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}*/

/*public class instatiateEdibles : MonoBehaviour {

    //public class Instantiation : MonoBehaviour
    
        void Start()
        {
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.AddComponent<Rigidbody>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                cube.transform.position = new Vector2(x, y);
                }
            }
        }
    
    */
    

