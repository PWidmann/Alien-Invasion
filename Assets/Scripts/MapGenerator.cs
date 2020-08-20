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

    [Header("Character Prefab")]
    public GameObject alienPrefab;
    public GameObject[] alienSpawns;


    [Header("Room Prefabs")]
    public GameObject startRoom;
    public GameObject horizontalCorridor;
    public GameObject verticalCorridor;
    public GameObject bigRoomCross;
    public GameObject bossRoom;
    public GameObject cornerEN;
    public GameObject cornerES;
    public GameObject cornerWN;
    public GameObject cornerWS;
    public GameObject machineRoomNorth;
    public GameObject machineRoomEast;
    public GameObject machineRoomSouth;
    public GameObject machineRoomWest;
    public GameObject breakRoomNorth;
    public GameObject breakRoomEast;
    public GameObject breakRoomSouth;
    public GameObject breakRoomWest;




    [Header("Level Object")]
    public GameObject levelObject;
    [HideInInspector]
    public NavMeshSurface levelMeshSurface;
    private GameObject tempRoomObject;
    private bool navMeshBuilt = false;

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
        levelMeshSurface = levelObject.GetComponent<NavMeshSurface>();
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

        // Build nav mesh when level is finished building
        if (roomsToConnect.Count == 0 && !navMeshBuilt)
        {
            levelMeshSurface.BuildNavMesh();
            navMeshBuilt = true;
            SpawnEnemies();
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
                    if (rnd < 3) // 30%
                    {
                        SpawnRoom(Room.RoomType.HorizontalCorridor, direction);
                    }
                    else
                    {
                        //turn corridor to the north if the first horizontal level arm
                        if (currentRoom.bigRoomArmNr == 1)
                        {
                            SpawnRoom(Room.RoomType.CornerEN, direction);
                        }

                        //turn corridor to the south if last horizontal room arm
                        if (currentRoom.bigRoomArmNr == bigRooms)
                        {
                            SpawnRoom(Room.RoomType.CornerES, direction);
                        }

                        if (currentRoom.bigRoomArmNr != 1 && currentRoom.bigRoomArmNr != bigRooms)
                        {
                            // Close off Arm
                            if (rnd < 5)
                            {
                                SpawnRoom(Room.RoomType.MachineRoomEast, direction);
                            }
                            else
                            {
                                SpawnRoom(Room.RoomType.BreakRoomEast, direction);
                            }
                            
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
                // Close off Arm
                if (rnd < 5)
                {
                    SpawnRoom(Room.RoomType.MachineRoomEast, direction);
                }
                else
                {
                    SpawnRoom(Room.RoomType.BreakRoomEast, direction);
                }
            }
        }

        if (direction == "west")
        {
            if (roomsCreated < startclosingRooms)
            {
                if (currentRoom.roomType == Room.RoomType.HorizontalCorridor)
                {
                    
                    if (rnd < 3) // 30%
                    {
                        
                        SpawnRoom(Room.RoomType.HorizontalCorridor, direction);
                    }
                    else
                    {
                        //turn corridor to the north if it's the first horizontal room arm
                        if (currentRoom.bigRoomArmNr == 1)
                        {
                            SpawnRoom(Room.RoomType.CornerWN, direction);
                        }

                        //turn corridor to the south if it's the last horizontal room arm
                        if (currentRoom.bigRoomArmNr == bigRooms)
                        {
                            SpawnRoom(Room.RoomType.CornerWS, direction);
                        }

                        if (currentRoom.bigRoomArmNr != 1 && currentRoom.bigRoomArmNr != bigRooms)
                        {
                            // Close off Arm
                            if (rnd < 5)
                            {
                                SpawnRoom(Room.RoomType.MachineRoomWest, direction);
                            }
                            else
                            {
                                SpawnRoom(Room.RoomType.BreakRoomWest, direction);
                            }
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
                // Close off Arm
                if (rnd < 5)
                {
                    SpawnRoom(Room.RoomType.MachineRoomWest, direction);
                }
                else
                {
                    SpawnRoom(Room.RoomType.BreakRoomWest, direction);
                }
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
            else // Corridor close room
            {
                // If all big cross rooms are created
                if (bigRoomsCreated == bigRooms)
                {
                    if (currentRoom.bigRoomArmNr == 0 || currentRoom.roomType == Room.RoomType.BigRoomCross)
                    {
                        // Spawn boss room
                        SpawnRoom(Room.RoomType.BossRoom, direction);
                    }
                    else
                    {
                        if (rnd < 5)
                        {
                            SpawnRoom(Room.RoomType.BreakRoomSouth, direction);
                        }
                        else
                        {
                            SpawnRoom(Room.RoomType.MachineRoomSouth, direction);
                        }
                        
                    }
                        
                    
                }
                else
                {
                    SpawnRoom(Room.RoomType.BigRoomCross, direction);
                }
            }
        }

        if (direction == "north")
        {
            // Only first corridor arm can get here
            if (roomsCreated < startclosingRooms)
            {
                if (rnd < 5)
                {
                    SpawnRoom(Room.RoomType.VerticalCorridor, direction);
                }
                else
                {
                    // Close off Arm
                    if (rnd < 8)
                    {
                        SpawnRoom(Room.RoomType.MachineRoomNorth, direction);
                    }
                    else
                    {
                        SpawnRoom(Room.RoomType.BreakRoomNorth, direction);
                    }
                }
                
            }
            else
            {
                // Close off Arm
                if (rnd < 5)
                {
                    SpawnRoom(Room.RoomType.MachineRoomNorth, direction);
                }
                else
                {
                    SpawnRoom(Room.RoomType.BreakRoomNorth, direction);
                }
            }
        }
    }

    void SpawnRoom(Room.RoomType type, string direction)
    {

        Vector3 offset = new Vector3();

        //Instantiate room object
        switch (type)
        {
            case Room.RoomType.VerticalCorridor:
                tempRoomObject = Instantiate(verticalCorridor, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());
                break;
            case Room.RoomType.HorizontalCorridor:
                tempRoomObject = Instantiate(horizontalCorridor, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                break;
            case Room.RoomType.CornerEN:
                tempRoomObject = Instantiate(cornerEN, new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
                CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());
                break;
            case Room.RoomType.CornerES:
                tempRoomObject = Instantiate(cornerES, new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
                CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());
                break;
            case Room.RoomType.CornerWN:
                tempRoomObject = Instantiate(cornerWN, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());
                break;
            case Room.RoomType.CornerWS:
                tempRoomObject = Instantiate(cornerWS, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                CountRoom(currentRoom, tempRoomObject.GetComponent<Room>());
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
            case Room.RoomType.BreakRoomNorth:
                tempRoomObject = Instantiate(breakRoomNorth, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                break;
            case Room.RoomType.BreakRoomEast:
                tempRoomObject = Instantiate(breakRoomEast, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
                break;
            case Room.RoomType.BreakRoomSouth:
                tempRoomObject = Instantiate(breakRoomSouth, new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
                break;
            case Room.RoomType.BreakRoomWest:
                tempRoomObject = Instantiate(breakRoomWest, new Vector3(0, 0, 0), Quaternion.Euler(0, -90, 0));
                break;
            case Room.RoomType.BossRoom:
                tempRoomObject = Instantiate(bossRoom, new Vector3(0, 0, 0), Quaternion.Euler(0, 180, 0));
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

        //Parent object to level object
        tempRoomObject.transform.parent = levelObject.transform;
        
        roomsToConnect.Add(tempRoomObject.GetComponent<Room>());
        roomsCreated++;
    }

    public void SpawnEnemies()
    {
        if (navMeshBuilt)
        {
            alienSpawns = GameObject.FindGameObjectsWithTag("AlienSpawn");

            foreach (GameObject spawn in alienSpawns)
            {
                int rnd = Random.Range(0, 101);

                if (rnd > (100 - enemySpawnChance))
                    Instantiate(alienPrefab, spawn.transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
        }
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

        if (tempRoom.roomType == Room.RoomType.VerticalCorridor)
        {
            tempRoom.bigRoomArmNr = currentRoom.bigRoomArmNr;
            tempRoom.corridoorNr = currentRoom.corridoorNr + 1;
        }

        if (tempRoom.roomType == Room.RoomType.CornerEN || tempRoom.roomType == Room.RoomType.CornerES || tempRoom.roomType == Room.RoomType.CornerWN || tempRoom.roomType == Room.RoomType.CornerWS)
        {
            tempRoom.bigRoomArmNr = currentRoom.bigRoomArmNr;
            tempRoom.corridoorNr = currentRoom.corridoorNr + 1;
        }

        if (tempRoom.roomType == Room.RoomType.VerticalCorridor && currentRoom.roomType == Room.RoomType.BigRoomCross && bigRoomsCreated == bigRooms)
        {
            tempRoom.bigRoomArmNr = 0;
            tempRoom.corridoorNr = 0;
        }
    }
}