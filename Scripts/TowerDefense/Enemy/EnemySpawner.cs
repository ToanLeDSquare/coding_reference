using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private List<Transform> listWayPoints;

    [SerializeField] private int wayIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoSpawnEnemies());
    }

    private IEnumerator AutoSpawnEnemies()
    {
        while(true)
        {
            wayIndex++;
            if (wayIndex > 10) wayIndex = 1;
            
            for(int i = 0; i < wayIndex; i++)
            {
                var enemyObject = Instantiate(enemy, listWayPoints[0].transform.position, Quaternion.identity);
                Enemy newEnemy = enemyObject.GetComponent<Enemy>();
                //float speed = Random.Range(1f, 10f);
                newEnemy.Initialize(listWayPoints, 2f);
                yield return new WaitForSeconds(1f);
            }

            float time = Random.Range(5f, 10f); 
            yield return new WaitForSeconds(time);
        }
    }
}
