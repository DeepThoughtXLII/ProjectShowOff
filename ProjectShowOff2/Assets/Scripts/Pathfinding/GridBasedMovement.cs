using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridBasedMovement : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundTilemap;

    [SerializeField]
    private Tilemap collisionTilemap;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
            return false;
        return true;
    }
}
