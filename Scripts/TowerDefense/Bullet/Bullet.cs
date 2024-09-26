using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float speed = 70f;
    [SerializeField] private GameObject impactEffect;

    private Transform target;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if(dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    private void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        enemy.UpdateEnemy(damage);
        Destroy(gameObject);
    }
}
