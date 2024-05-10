using System.Collections.Generic;
using UnityEngine;

namespace TempleRun { //allows every file under it to access each other

public class TileSpawner : MonoBehaviour
{
    /// <summary>
    /// Number of tiles to spawn at the beginning
    /// </summary>
    [SerializeField]
    private int tileStartCount = 10; 
    
    /// <summary>
    /// Min nb of straight tiles (avoid endlessly going straight)
    /// </summary>
    [SerializeField]
    private int minimumStraightTiles = 3; 

    /// <summary>
    /// Max nb of straight tiles (avoid endlessly going straight)
    /// </summary>
    [SerializeField]
    private int maximumStraightTiles = 15; 
    
    [SerializeField] 
    private GameObject startingTile;
    [SerializeField]
    private List<GameObject> turnTiles;
    [SerializeField]
    private List<GameObject> obstacles;

    /// <summary>
    /// Probability of spawning an obstacle on a tile
    /// </summary>
    private float probaObstacle = 0.4f;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;

    /// <summary>
    /// Keep track of the last tile, at the moment of turning for exemple
    /// </summary>
    private GameObject prevTile;

    private List<GameObject> currentTiles;

    /// <summary>
    /// All obstacles in the scene
    /// </summary>
    private List<GameObject> currentObstacles;

    private void Start() { //initialisation of all variables 
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond); //random seed : chosen the actual date, so always a unique one when the game is run

        for (int i=0; i<tileStartCount; i++) {
            SpawnTile(startingTile.GetComponent<Tile>()); 
            //gives only the tile component, so we don't have to keep referencing every tile spawning
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    /// <summary>
    /// Spawns a tile at the current location and updates the current location
    /// <para> 
    /// Boolean param to indicate if we want an obstacle on the new tile
    /// </para>
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="spawnObstacle"></param>
    private void SpawnTile(Tile tile, bool spawnObstacle = false) {
        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        //current direction, on which we apply the rotation of the next tile so that it points in the correct direction (Vector3.up is the Y axis on which we rotate)

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation); 
        currentTiles.Add(prevTile);

        if (spawnObstacle) SpawnObstacle();

        if (tile.type == TileType.STRAIGHT) { // currentTileLocation already uptaded if TileType != STRAIGHT (i.e, turning) in AddNewDirection 
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection); 
            //moving the "currentLocation" 1 straight tile forward in the current direction
            //calculus is for exemple : (4,1,10)*(0,0,1) -> (0,0,10) for a tile (lenght 10 units, width 4, height 1) that we add on currentTileLocation
            //(0,0,1) is the direction of going straight forward, (1,0,0) is going right
        }
    }

    /// <summary>
    /// Avoid the old tiles to be seen by player if by any chance the player ends up going back
    /// </summary>
    private void DeletePreviousTile() {
        //TO DO: check "object pull" instead of destroying and creating new ones
        while (currentTiles.Count != 1) {
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);
        }
        //removes one by one the tiles that are left behind the player, except the one the player is on (the turn)

        while (currentObstacles.Count != 0) {
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);
        }
    }
    /// <summary>
    /// Sets the new direction
    /// <para>
    /// Deletes the previous tile and determining placement of the new one
    /// </summary>
    /// <param name="direction"></param>
    public void AddNewDirection(Vector3 direction) {
        currentTileDirection = direction;
        DeletePreviousTile();

        Vector3 tilePlacementScale;
        if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS) {
            tilePlacementScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size /2 + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z /2), currentTileDirection);
        }
        else { // left or right tiles
            tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size) -(Vector3.one * 2) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z /2), currentTileDirection);
        }
        //we calculate the position of the next tile depending of the type of the one we just turned on
        //the *Vector3.one is to convert the float to a vector3, so we can multiply it with the direction

        currentTileLocation += tilePlacementScale;

        int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);
        for (int i=0; i<currentPathLength; i++) {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true); //little test to avoid spawning an obstacle directly after a turn
        }

        SpawnTile(SelectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
    }

    private void SpawnObstacle() {
        if (Random.value > probaObstacle) return; //probability of spawning an obstacle

        GameObject obstaclePrefab = SelectRandomGameObjectFromList(obstacles);
        Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
        currentObstacles.Add(obstacle);
    }

    private GameObject SelectRandomGameObjectFromList(List<GameObject> list) {
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

}

}