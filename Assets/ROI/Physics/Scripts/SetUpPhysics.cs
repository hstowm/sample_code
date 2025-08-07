using System.Collections.Generic;
using UnityEngine;

public class SetUpPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> gameObjects;
    public float gravity = -50f;
    
    void Start()
    {
        // Physics.gravity = new Vector3(0f, -9.81f, 0f);
        Physics.gravity = new Vector3(0f, gravity, 0f);

        foreach(GameObject go in gameObjects)
        {
            go.SetActive(true);
        }
    }
    
}
