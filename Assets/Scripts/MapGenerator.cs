using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    [Header("Level Options")]
    public int bigRooms;
    public int startclosingRooms;
    [Range(0, 100)]
    public int enemySpawnChance = 50;


    [Header("Room Prefabs")]
    public GameObject startRoom;
    public GameObject horizontalCorridor;
    public GameObject verticalCorridor;
    public GameObject bigRoomCross;



    [Header("Level Object")]
    public GameObject levelObject;
    [HideInInspector]
    //public NavMeshSurface levelMeshSurface;
    private GameObject tempRoomObject;

    Room currentRoom;

    public List<Room> roomsToConnect = new List<Room>();
    GameObject startRoomObject;
    int rnd;
    public int roomsCreated = 0;
    private int bigRoomsCreated = 0;

    public List<Bounds> roomList = new List<Bounds>();

    void Start()
    {
        if (Instance == null)
            Instance = this;

        //levelMeshSurface = levelObject.GetComponent<NavMeshSurface>();

        currentRoom = startRoom.GetComponent<Room>();

        StartGeneratingDungeon();
    }

    private void Update()
    {
        if (!currentRoom.roomConnected)
            CreateConnection();
    }

    void StartGeneratingDungeon()
    {
        roomsToConnect.Clear();
        roomList.Clear();
        bigRoomsCreated = 0;

        startRoomObject = Instantiate(startRoom, new Vector3(0, 0, 0), Quaternion.identity * Quaternion.Euler(0, 0, 0));
        startRoomObject.transform.parent = levelObject.transform;
        currentRoom.roomType = Room.RoomType.StartRoom;
        roomsCreated = 1;
    }

    

    void CreateConnection()
    {
        rnd = Random.Range(0, 4);

        // East Connection
        if (!currentRoom.eastConnected)
        {
            
        }

        // West Connection
        if (!currentRoom.westConnected)
        {
            
        }

        //// North Connection
        if (!currentRoom.northConnected)
        {
            
        }

        // South Connection
        if (!currentRoom.southConnected)
        {
            
        }
    }

    void SpawnRoom(Room.RoomType type, string direction)
    {

        Vector3 offset = new Vector3();

        switch (type)
        {
            case Room.RoomType.VerticalCorridor:
                tempRoomObject = Instantiate(verticalCorridor, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                break;
            case Room.RoomType.HorizontalCorridor:
                tempRoomObject = Instantiate(horizontalCorridor, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                break;
            case Room.RoomType.BigRoomCross:
                rnd = Random.Range(0, 4);
                tempRoomObject = Instantiate(bigRoomCross, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                bigRoomsCreated++;
                break;
            default:
                break;
        }

        //Align room to connection points
        switch (direction)
        {
            case "east":
                offset = tempRoomObject.GetComponent<Room>().westConnection.transform.position - currentRoom.GetComponent<Room>().eastConnection.transform.position;
                tempRoomObject.transform.position -= offset;

                // Confirm connection
                currentRoom.eastConnected = true;
                tempRoomObject.GetComponent<Room>().westConnected = true;
                break;
            case "west":
                offset = tempRoomObject.GetComponent<Room>().eastConnection.transform.position - currentRoom.GetComponent<Room>().westConnection.transform.position;
                tempRoomObject.transform.position -= offset;

                // Confirm connection
                currentRoom.westConnected = true;
                tempRoomObject.GetComponent<Room>().eastConnected = true;
                break;
            case "north":
                offset = tempRoomObject.GetComponent<Room>().southConnection.transform.position - currentRoom.GetComponent<Room>().northConnection.transform.position;
                tempRoomObject.transform.position -= offset;

                // Confirm connection
                currentRoom.northConnected = true;
                tempRoomObject.GetComponent<Room>().southConnected = true;
                break;
            case "south":
                offset = tempRoomObject.GetComponent<Room>().northConnection.transform.position - currentRoom.GetComponent<Room>().southConnection.transform.position;
                tempRoomObject.transform.position -= offset;

                // Confirm connection
                currentRoom.southConnected = true;
                tempRoomObject.GetComponent<Room>().northConnected = true;
                break;
            case "start":
                break;
        }

        tempRoomObject.transform.parent = levelObject.transform;
        roomsToConnect.Add(tempRoomObject.GetComponent<Room>());

        roomsCreated++;
    }
}