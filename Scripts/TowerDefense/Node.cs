using UnityEngine;

public class Node : MonoBehaviour
{
    private GameObject turretPoint;
    private GameObject turret;
    private MeshRenderer meshRenderer;
    private Color originalColor;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        turretPoint = new GameObject();
        turretPoint.transform.parent = transform;
        turretPoint.transform.localPosition = new Vector3(0f, 0.5f, 0f);
    }

    private void OnMouseEnter()
    {
        if(turret != null)
        {
            return;
        }
        meshRenderer.material.color = Color.green;
    }

    private void OnMouseDown()
    {
        if (turret != null)
        {
            return;
        }

        GameObject turretToBuild = BuildManager.Instance.GetTurretToBuild();
        turret = Instantiate(turretToBuild, turretPoint.transform.position, turretPoint.transform.rotation);
    }

    private void OnMouseExit()
    {
        meshRenderer.material.color = originalColor;
    }
}
