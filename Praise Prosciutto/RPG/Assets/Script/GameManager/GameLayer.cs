using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    [SerializeField] LayerMask stopMovement;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask enemyLayer;

    public static GameLayer Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public LayerMask SolidLayer
    {
        get => stopMovement;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask EnemyLayer
    {
        get => enemyLayer;
    }
}
