using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float heath;
    [SerializeField] float moveSpeed;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject exlopsionEffect;

    private List<Transform> listWalkingPoints;
    private bool initalized = false;
    private Color originalColor;
    private bool isDied = false;
    private int pointIndex = 1;

    // Update is called once per frame
    void Update()
    {
        if (initalized && !isDied)
        {
            float distance = Vector3.Distance(transform.position, listWalkingPoints[pointIndex].position);
            if (distance > 0)
            {
                var step = moveSpeed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, listWalkingPoints[pointIndex].position, step);
            }
            else 
            {
                transform.position = listWalkingPoints[pointIndex].position;
                if (pointIndex < listWalkingPoints.Count - 1) pointIndex += 1;
                else Destroy(gameObject);
            }
        }
    }

    public void Initialize(List<Transform> listWalkingPoints, float speed)
    {
        this.listWalkingPoints = listWalkingPoints;
        originalColor = meshRenderer.material.color;
        moveSpeed = speed;
        initalized = true;
    }

    private IEnumerator OnHit()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        meshRenderer.material.color = originalColor;
    }

    public void UpdateEnemy(float damage)
    {
        heath -= damage;
        StartCoroutine(OnHit());
        
        if (heath <= 0f)
        {
            GameObject effectIns = Instantiate(exlopsionEffect, transform.position, transform.rotation);
            Destroy(effectIns, 2f);
            Destroy(gameObject);
        }
    }
}
