using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private RoomListSO roomList;
    [SerializeField] private RoomSO placeholder;

    private RoomSO[][] roomMap;
    private int rows = 5;
    private int cols = 5;
    private int startingIndex = 2;

    // Start is called before the first frame update
    void Start()
    {
        do
        {
            Generate();
        } while (!Validate());

        Create();
    }

    bool Validate()
    {
        return true;
    }

    void Generate()
    {
        System.Random random = new System.Random();
        int row;
        int col;

        List<RoomSO> rooms = roomList.rooms;
        List<RoomSO> startingRooms = roomList.rooms.Where(r => r.startingRoom).ToList();

        Initialize();

        RoomSO start = startingRooms[random.Next(startingRooms.Count)];
        Exits startDirection = start.roomExits[random.Next(start.roomExits.Count)];

        row = startingIndex;
        col = startingIndex;

        switch (startDirection)
        {
            case Exits.NORTH:
                col = cols - 1;
                break;
            case Exits.SOUTH:
                col = 0;
                break;
            case Exits.EAST:
                row = 0;
                break;
            case Exits.WEST:
                row = rows - 1;
                break;
        }

        roomMap[row][col] = start;

        List<RoomPosition> newRooms = new List<RoomPosition>();

        foreach (Exits direction in start.roomExits)
        {
            if (direction == startDirection) continue;
            List<RoomSO> nextRooms = rooms.Where(r => r.roomExits.Contains(Reverse(direction))).ToList();
            while (nextRooms.Count > 0)
            {
                RoomSO nextRoom = nextRooms[random.Next(nextRooms.Count)];
                if (AssignRoom(row, col, direction, nextRoom, true, true))
                {
                    if (AssignRoom(row, col, direction, nextRoom))
                        newRooms.Add(new RoomPosition(NextPosition(row, col, direction), nextRoom));
                    break;
                }
                nextRooms.Remove(nextRoom);
            }
        }

        while (newRooms.Count > 0)
        {
            RoomPosition tmpRoom = newRooms[random.Next(newRooms.Count)];
            newRooms.Remove(tmpRoom);
            row = tmpRoom.pos.Row;
            col = tmpRoom.pos.Col;
            foreach (Exits direction in tmpRoom.room.roomExits)
            {
                List<RoomSO> nextRooms = rooms.Where(r => r.roomExits.Contains(Reverse(direction))).ToList();
                while (nextRooms.Count > 0)
                {
                    RoomSO nextRoom = nextRooms[random.Next(nextRooms.Count)];
                    if (AssignRoom(row, col, direction, nextRoom, true, true))
                    {
                        if (AssignRoom(row, col, direction, nextRoom))
                            newRooms.Add(new RoomPosition(NextPosition(row, col, direction), nextRoom));
                        break;
                    }
                    nextRooms.Remove(nextRoom);
                }
            }
        }
    }

    Exits Reverse(Exits direction)
    {
        switch (direction)
        {
            case Exits.NORTH:
                return Exits.SOUTH;
            case Exits.SOUTH:
                return Exits.NORTH;
            case Exits.EAST:
                return Exits.WEST;
            case Exits.WEST:
                return Exits.EAST;
        }
        return Exits.EAST;
    }

    Position NextPosition(int row, int col, Exits direction)
    {
        int newRow = row;
        int newCol = col;

        switch (direction)
        {
            case Exits.NORTH:
                newCol++;
                break;
            case Exits.SOUTH:
                newCol--;
                break;
            case Exits.EAST:
                newRow--;
                break;
            case Exits.WEST:
                newRow++;
                break;
        }

        return new Position(newRow, newCol);
    }

    bool Inside(Position pos)
    {
        if (pos.Col < 0 || pos.Col >= cols || pos.Row < 0 || pos.Row >= rows) return false;
        return true;
    }

    bool AssignRoom(int row, int col, Exits direction, RoomSO newRoom, bool test = false, bool borders = false)
    {
        Position newPosition = NextPosition(row, col, direction);

        // if (test && borders && !newRoom.roomExits.Contains(direction)) {
        //     if (!Inside(newPosition)) return true;
        //     if (roomMap[newPosition.Row][newPosition.Col] != null &&
        //         roomMap[newPosition.Row][newPosition.Col].roomExits.Contains(Reverse(direction)))
        //         return false;
        //     return true;
        // }

        if (!Inside(newPosition)) return false;
        if (test && roomMap[newPosition.Row][newPosition.Col] != null && roomMap[newPosition.Row][newPosition.Col].roomExits.Contains(Reverse(direction))) return true;
        if (roomMap[newPosition.Row][newPosition.Col] != null) return false;
        if (test && borders)
        {
            foreach (Exits nextDirection in newRoom.roomExits)
            {
                if (!AssignRoom(newPosition.Row, newPosition.Col, nextDirection, null, true)) return false;
            }
        }
        if (!test) roomMap[newPosition.Row][newPosition.Col] = newRoom;
        return true;
    }

    // initialize matrix
    private void Initialize()
    {
        roomMap = new RoomSO[rows][];
        for (int i = 0; i < roomMap.Length; i++)
            roomMap[i] = new RoomSO[cols];
    }

    // create objects in scene
    void Create()
    {
        //Generate();
        CreatePlane(transform, 5, 5);
    }

    void CreatePlane(Transform plane, float planeWidth, float planeLength)
    {
        float rowStep = planeWidth / rows;
        float colStep = planeLength / cols;

        Vector3 origin = plane.position - plane.forward * planeWidth / 2
                                        - plane.right * planeLength / 2
                         + plane.forward * rowStep / 2
                         + plane.right * colStep / 2;

        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
            {
                Vector3 itemOffset = plane.forward * i * rowStep + plane.right * j * colStep;
                if (roomMap[i][j] != null)
                {
                    GameObject room = Instantiate(roomList.roomPrefab, origin + itemOffset + roomMap[i][j].prefabOffset, Quaternion.identity);
                    List<Exits> exits = roomMap[i][j].roomExits;
                    foreach (Exits direction in Enum.GetValues(typeof(Exits)))
                    {
                        GameObject prefab = roomList.noexitPrefab;
                        if (exits.Contains(direction)) prefab = roomList.exitPrefab;
                        Instantiate(prefab, room.transform.position + ExitDirectionOffset(direction), Quaternion.identity);
                    }
                }
                else
                {
                    Instantiate(placeholder.roomPrefab, origin + itemOffset + placeholder.prefabOffset, Quaternion.identity);
                }
            }
    }

    Vector3 ExitDirectionOffset(Exits direction)
    {
        float radius = roomList.exitRadius;
        switch (direction)
        {
            case Exits.EAST:
                return -Vector3.forward * radius;
            case Exits.WEST:
                return Vector3.forward * radius;
            case Exits.NORTH:
                return Vector3.right * radius;
            case Exits.SOUTH:
                return -Vector3.right * radius;
        }
        return Vector3.forward * radius;
    }


}


public class Position
{
    public int Row;
    public int Col;

    public Position(int row, int col)
    {
        Row = row;
        Col = col;
    }
}

public class RoomPosition
{
    public Position pos;
    public RoomSO room;

    public RoomPosition(Position pos, RoomSO room)
    {
        this.pos = pos;
        this.room = room;
    }
}