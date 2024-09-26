using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [SerializeField] private GameObject turretToBuild;

    private void Awake()
    {
        if(Instance != null)
        {
            Instance = null;
        }
        Instance = this;
    }

    public GameObject GetTurretToBuild()
    {
        return turretToBuild;
    }
}
