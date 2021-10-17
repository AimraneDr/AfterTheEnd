using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerTest : MonoBehaviour
{

    public GameObject Enemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNewEnemy()
    {
        GameObject newEnemy = Instantiate(Enemy, transform.position, Quaternion.identity);
        newEnemy.transform.parent = this.transform;
    }
}
