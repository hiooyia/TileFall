using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    [SerializeField] GameObject wallBrick;

    void Start()
    {
        for(int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++) 
            {
                Instantiate(wallBrick, new Vector3(-4.5f + j, -20 + i, 4.5f), Quaternion.identity);
            }
        }

        for(int i = 0;i < 100; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Instantiate(wallBrick, new Vector3(-4.5f, -20 + i, -3.5f + j), Quaternion.identity);
                Instantiate(wallBrick, new Vector3(4.5f, -20 + i, -3.5f + j), Quaternion.identity);
            }
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Instantiate(wallBrick, new Vector3(-3.5f + i, -20, -3.5f + j), Quaternion.identity);
            }
        }
    }

}
