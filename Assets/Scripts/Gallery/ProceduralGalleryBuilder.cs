using System;
using System.Collections;
using System.Collections.Generic;
using Gallery.Builder;
using UnityEngine;

[System.Serializable]
public class IVector2
{
    public IVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
}

public class ProceduralGalleryBuilder : MonoBehaviour
{
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float edge_length;
    [SerializeField] private IVector2[] positions;

    private Transform _walls;
    private Transform _floors;

    private void Awake()
    {
        _walls = transform.Find("walls");
        _floors = transform.Find("floors");
    }

    private void Start()
    {
        Build();
    }

    public void Build()
    {
        
        BuildBlock(new IVector2(0, 0));
        for(var i=0; i<positions.Length; ++i)
            BuildBlock(positions[i]);
    }

    private void BuildBlock(IVector2 pos)
    {
        var position = new Vector3(pos.x * edge_length, 0, pos.y * edge_length);
        var floor = Instantiate(floorPrefab, _floors);
        floor.transform.position = position;
        
        var wallY = wallPrefab.transform.position.y;
        // x axis walls
        if (!Has(pos.x - 1, pos.y))
        {
            var wall = Instantiate(wallPrefab, _walls);
            var wallPos = new Vector3(position.x - edge_length / 2.0f, wallY, position.z);
            wall.transform.position = wallPos;
        }
        if (!Has(pos.x + 1, pos.y))
        {
            var wall = Instantiate(wallPrefab, _walls);
            var wallPos = new Vector3(position.x + edge_length / 2.0f, wallY, position.z);
            wall.transform.position = wallPos;
        }
        
        // y axis walls
        if (!Has(pos.x, pos.y - 1))
        {
            var wall = Instantiate(wallPrefab, _walls);
            var wallPos = new Vector3(position.x, wallY, position.z - edge_length / 2.0f);
            wall.transform.position = wallPos;
            wall.transform.Rotate(Vector3.up, 90.0f);
        }
        if (!Has(pos.x, pos.y + 1))
        {
            var wall = Instantiate(wallPrefab, _walls);
            var wallPos = new Vector3(position.x, wallY, position.z + edge_length / 2.0f);
            wall.transform.position = wallPos;
            wall.transform.Rotate(Vector3.up, -90.0f);
        }
    }

    private bool Has(int x, int y)
    {
        if (x == 0 && y == 0) return true;
        for (var i = 0; i < positions.Length; ++i)
        {
            if (positions[i].x == x && positions[i].y == y)
                return true;
        }
        return false;
    }
}
