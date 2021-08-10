using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] List<CharacterManager> enemies;

    public CharacterManager GetRandomEnemy()
    {
        var enemy = enemies[Random.Range(0, enemies.Count)];
        enemy.Initialize();
        return enemy;
    }
    
}
