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
    public GameObject endNorth;
    public GameObject endEast;
    public GameObject endSouth;
    public GameObject endWest;
    public GameObject cornerEN;
    public GameObject cornerES;
    public GameObject cornerWN;
    public GameObject cornerWS;
    public GameObject machineRoomNorth;
    public GameObject machineRoomEast;
    public GameObject machineRoomSouth;
    public GameObject machineRoomWest;




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
    public int bigRoomsCreated = 0;

    public List<Bounds> roomList = new List<Bounds>();

    public RaycastHit hit;

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
        // Connect rooms
        if (currentRoom.roomConnected)
        {
            roomsToConnect.Remove(currentRoom);

            if (roomsToConnect.Count > 0)
                currentRoom = roomsToConnect[0];
        }
        else
        {
            CreateConnection();
        }
    }

    void StartGeneratingDungeon()
    {
        roomsToConnect.Clear();
        roomList.Clear();
        bigRoomsCreated = 0;

        startRoomObject = Instantiate(startRoom, new Vector3(0, 0, 0), Quaternion.identity * Quaternion.Euler(0, 0, 0));
        startRoomObject.transform.parent = levelObject.transform;
        currentRoom = startRoomObject.GetComponent<Room>();
        roomsCreated = 1;
    }

    

    void CreateConnection()
    {
        // East Connection
        if (!currentRoom.eastConnected)
        {
            ChooseRoom("east");
        }

        // West Connection
        if (!currentRoom.westConnected)
        {
            ChooseRoom("west");    
        }

        //// North Connection
        if (!currentRoom.northConnected)
        {
            ChooseRoom("north");
        }

        // South Connection
        if (!currentRoom.southConnected)
        {
            ChooseRoom("south");
        }
    }

    void ChooseRoom(string direction)
    {
        rnd = Random.Range(0, 10);

        if (direction == "east")
        {
            if (roomsCreated < startclosingRooms)
            {
                if (currentRoom.roomType == Room.RoomType.HorizontalCorridor)
                {
                    if (rnd < 5)
                    {
                        SpawnRoom(Room.RoomType.HorizontalCorridor, direction);
                    }
                    else
                    {
                        if (currentRoom.bigRoomArmNr == 1)
                        {
                            SpawnRoom(Room.RoomType.CornerEN, direction);
                        }

                        if (currentRoom.bigRoomArmNr == bigRooms)
                        {
                            SpawnRoom(Room.RoomType.CornerES, direction);
                        }
                    }
                }
                else
                {
                    SpawnRoom(Room.RoomType.HorizontalCorridor, direction);
                }
            }
            else
            {
                SpawnRoom(Room.RoomType.MachineRoomEast, direction);
            }
        }

        if (direction == "west")
        {
            if (roomsCreated < startclosingRooms)
            {
                if (currentRoom.roomType == Room.RoomType.HorizontalCorridor)
                {
                    if (rnd < 5)
                    {
                        SpawnRoom(Room.RoomType.HorizontalCorridor, direction);
                    }
                    else
                    {
                        if (currentRoom.bigRoomArmNr == 1)
                        {
                            SpawnRoom(Room.RoomType.CornerWN, direction);
                        }

                        if (currentRoom.bigRoomArmNr == bigRooms)
                        {
                            SpawnRoom(Room.RoomType.CornerWS, direction);
                        }
                    }
                }
                else
                {
                    SpawnRoom(Room.RoomType.HorizontalCorridor, direction);
                }
            }
            else
            {
                SpawnRoom(Room.RoomType.MachineRoomWest, direction);
            }
        }
        

        if (direction == "south")
        {
            if (roomsCreated < startclosingRooms)
            {
                if (rnd < 5)
                {
                    SpawnRoom(Room.RoomType.VerticalCorridor, direction);
                }
                else
                {
                    if (bigRoomsCreated < bigRooms)
                        SpawnRoom(Room.RoomType.BigRoomCross, direction);
                }
            }
            else
            {
                SpawnRoom(Room.RoomType.MachineRoomSouth, direction);
            }
        }

        if (direction == "north")
        {
            if (roomsCreated < startclosingRooms)
            {
                SpawnRoom(Room.RoomType.VerticalCorridor, direction);
            }
            else
            {
                SpawnRoom(Room.RoomType.MachineRoomNorth, direction);
            }
        }
    }

    void SpawnRoom(Room.RoomType type, string direction)
    {

        Vector3 offset = new Vector3();

        switch (type)
        {
            case Room.RoomType.VerticalCorridor:
                tempRoomObject = Instantiate(verticalCorridor, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());
                break;
            case Room.RoomType.HorizontalCorridor:
                tempRoomObject = Instantiate(horizontalCorridor, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                break;
            case Room.RoomType.EndNorth:
                tempRoomObject = Instantiate(endNorth, new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
                break;
            case Room.RoomType.EndEast:
                tempRoomObject = Instantiate(endEast, new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
                break;
            case Room.RoomType.EndSouth:
                tempRoomObject = Instantiate(endSouth, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                break;
            case Room.RoomType.EndWest:
                tempRoomObject = Instantiate(endWest, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                break;
            case Room.RoomType.CornerEN:
                tempRoomObject = Instantiate(cornerEN, new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
                break;
            case Room.RoomType.CornerES:
                tempRoomObject = Instantiate(cornerES, new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
                break;
            case Room.RoomType.CornerWN:
                tempRoomObject = Instantiate(cornerWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                break;
            case Room.RoomType.CornerWS:
                tempRoomObject = Instantiate(cornerWS, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                break;
            case Room.RoomType.MachineRoomNorth:
                tempRoomObject = Instantiate(machineRoomNorth, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                break;
            case Room.RoomType.MachineRoomEast:
                tempRoomObject = Instantiate(machineRoomEast, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                break;
            case Room.RoomType.MachineRoomSouth:
                tempRoomObject = Instantiate(machineRoomSouth, new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
                break;
            case Room.RoomType.MachineRoomWest:
                tempRoomObject = Instantiate(machineRoomWest, new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
                break;
            case Room.RoomType.BigRoomCross:
                tempRoomObject = Instantiate(bigRoomCross, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                bigRoomsCreated++;
                tempRoomObject.GetComponent<Room>().bigRoomArmNr = bigRoomsCreated;
                break;
            default:
                break;
        }

        CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());

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
            default:
                break;
        }


        tempRoomObject.transform.parent = levelObject.transform;
        roomsToConnect.Add(tempRoomObject.GetComponent<Room>());
        roomsCreated++;
    }

    void CountRoom(Room currentRoom, Room tempRoom)
    {
        if (currentRoom.roomType == Room.RoomType.BigRoomCross && tempRoom.roomType == Room.RoomType.HorizontalCorridor)
        {
            tempRoom.bigRoomArmNr = currentRoom.bigRoomArmNr;
            tempRoom.corridoorNr = 1;
        }

        if (currentRoom.roomType == Room.RoomType.HorizontalCorridor && tempRoom.roomType == Room.RoomType.HorizontalCorridor)
        {
            tempRoom.bigRoomArmNr = currentRoom.bigRoomArmNr;
            tempRoom.corridoorNr = currentRoom.corridoorNr + 1;
        }
    }
}