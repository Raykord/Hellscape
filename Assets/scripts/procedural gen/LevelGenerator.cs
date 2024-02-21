using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Empty = 0,
    Player = 2,
    Enemy,
    Wall =1,
    Door,
    Key,
    Dagger,
    End
}
public class LevelGenerator : MonoBehaviour
{
    public GameObject[] tiles;

    public const int MIN_DIV_WIDTH  = 10;
    public const int MIN_DIV_HEIGHT = 10;
    public const int MIN_ROOM_WIDTH = 5;
    public const int MIN_ROOM_HEIGHT = 5;
    public const int MAX_NB_ENEMIES = 20;

    BSPNode root;

    public enum Orientation
    {
        VERTICAL,
        HORIZONTAL,
        NB_ORIENTATIONS
    }

    public const int POS_LEFT = 0x1;
    public const int POS_RIGHT = 0x2;
    public const int POS_TOP = 0x4;
    public const int POS_BOTTOM = 0x8;

    private class Room
    {
        public Vector2 Position { get => new Vector2(x, y); set { x = (int)value.x; y = (int)value.y; } }
        public Vector2 Size { get => new Vector2(width, height); set { width = (int)value.x; height = (int)value.y; } }
        public Vector2 Middle { get => new Vector2(x + width / 2, y + height / 2); }


        public int x, y, width, height;
        public List<TileType> containedTiles;
        public List<Path> paths;

        public Room(int x = 0, int y = 0, int width = 0, int height = 0)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            containedTiles = new List<TileType>();
            paths = new List<Path>();
        }

        public void Draw(TileType[,] grid)
        {
            FillBlock(grid, x, y, width, height, TileType.Empty);

            List<Vector2Int> used = new List<Vector2Int>();
            Vector2Int tmp = new Vector2Int();
            foreach(var tile in containedTiles)
            {
                if (tile != TileType.Player && tile != TileType.End)
                {
                    do
                    {
                        tmp.x = Random.Range(x, x + width);
                        tmp.y = Random.Range(y, y + height);
                    } while (used.Contains(tmp));
                } else
                    tmp = new Vector2Int((int)Middle.x, (int)Middle.y);

                FillBlock(grid, tmp.x, tmp.y, 1, 1, tile);

                used.Add(tmp);
            }
        }

        public Vector2[] ClosestPoints(Room other)
        {
            Vector2 dif = other.Middle - Middle;
            dif.Normalize();
            Vector2[] proj = new Vector2[2];
            if(dif.x > dif.y)
            {
                proj[0] = Middle + dif * width / 2.0f;
                proj[1] = other.Middle - dif * other.width / 2.0f;
            } else
            {
                proj[0] = Middle + dif * height / 2.0f;
                proj[1] = other.Middle - dif * other.height/ 2.0f;
            }

            proj[0].x = Mathf.RoundToInt(proj[0].x);
            proj[0].y = Mathf.RoundToInt(proj[0].y);
            proj[1].x = Mathf.RoundToInt(proj[1].x);
            proj[1].y = Mathf.RoundToInt(proj[1].y);

            if (proj[0].x < x) proj[0].x = x;
            if (proj[0].x > x + width - 1)  proj[0].x = x + width - 1;
            if (proj[0].y < y) proj[0].y = y;
            if (proj[0].y > y + height - 1) proj[0].y = y + height - 1;

            if (proj[1].x < other.x) proj[1].x = other.x;
            if (proj[1].x > other.x + other.width - 1)  proj[1].x = other.x + other.width - 1;
            if (proj[1].y < other.y) proj[1].y = other.y;
            if (proj[1].y > other.y + other.height - 1) proj[1].y = other.y + other.height - 1;

            return proj;
        }

        public List<Room> PathToPlayer(Path from = null)
        {
            if (containedTiles.Contains(TileType.Player))
                return new List<Room>() { this };

            foreach(Path p in paths)
            {
                if (p == from)      continue;

                Room next = p.DestFrom(this);
                if (next == null) continue;
                List<Room> rooms = next.PathToPlayer(p);
                if(rooms != null)
                {
                    rooms.Insert(0, this);
                    return rooms;
                }
                
            }
            return null;
        }

        public List<Room> AccessibleRooms(Path from = null)
        {
            List<Room> rooms = new List<Room>();
            rooms.Add(this);

            foreach (Path p in paths)
            {
                if (p == from) continue;   
                if (p.containedTiles.Contains(TileType.Door))
                    continue;

                Room next = p.DestFrom(this);
                if (next == null) continue;

                var tmp = next.AccessibleRooms(p);
                foreach (Room r in tmp)
                    rooms.Add(r);
            }
            return rooms;
        }
    }

    private class Path
    {
        public Vector2 p0, p1;
        public List<TileType> containedTiles;
        public Room r0, r1;

        public Path(Room r0, Room r1)
        {
            this.r0 = r0;
            this.r0.paths.Add(this);
            this.r1 = r1;
            this.r1.paths.Add(this);
            var points = r0.ClosestPoints(r1);
            p0 = points[0];
            p1 = points[1];
            containedTiles = new List<TileType>();
        }

        public void Draw(TileType[,] grid)
        {
            Vector2 dif = p1 - p0;
            int size = 1;

            if (dif.y == 0)
            {
                FillBlock(grid, (int)p0.x, (int)p0.y - size / 2, (int)dif.x, size, TileType.Empty);

                foreach (TileType tile in containedTiles)
                {
                    if (tile == TileType.Door)
                        FillBlock(grid, (int)p0.x + (int)dif.x / 2, (int)p0.y, 1, 1, tile);
                }
                return;
            }
            else if (dif.x == 0)
            {
                FillBlock(grid, (int)p0.x - size / 2, (int)p0.y, size, (int)dif.y, TileType.Empty);

                foreach (TileType tile in containedTiles)
                {
                    if (tile == TileType.Door)
                        FillBlock(grid, (int)p0.x, (int)p0.y + (int)dif.y / 2, 1, 1, tile);
                }
                return;
            }

            int threshold1 = 4;
            int threshold2 = 3;


            if (dif.x > dif.y && dif.x >= threshold1 && dif.y >= threshold2)
            {
                FillBlock(grid, (int)p0.x - size / 2, (int)p0.y, (int)dif.x / 2 + 1, size, TileType.Empty);
                FillBlock(grid, (int)p0.x + (int)dif.x / 2, (int)p0.y, size, (int)dif.y, TileType.Empty);
                FillBlock(grid, (int)p0.x + (int)dif.x / 2, (int)p1.y, (int)dif.x / 2 + 1, size, TileType.Empty);

                foreach (TileType tile in containedTiles)
                {
                    if (tile == TileType.Door)
                        FillBlock(grid, (int)p0.x + (int)dif.x / 2, (int)p0.y + (int)dif.y / 2, 1, 1, tile);
                }
            }
            else if (dif.y > dif.x && dif.y >= threshold1 && dif.x >= threshold2)
            {
                FillBlock(grid, (int)p0.x - size / 2, (int)p0.y, size, (int)dif.y / 2 + 1, TileType.Empty);
                FillBlock(grid, (int)p0.x, (int)p0.y + (int)dif.y / 2, (int)dif.x + 1, size, TileType.Empty);
                FillBlock(grid, (int)p1.x, (int)p0.y + (int)dif.y / 2, size, (int)dif.y / 2 + 1, TileType.Empty);

                foreach (TileType tile in containedTiles)
                {
                    if (tile == TileType.Door)
                        FillBlock(grid, (int)p0.x + (int)dif.x / 2, (int)p0.y + (int)dif.y / 2, 1, 1, tile);
                }
            }
            else
            {
                FillBlock(grid, (int)p0.x - size / 2, (int)p0.y, size, (int)dif.y + 1, TileType.Empty);
                FillBlock(grid, (int)p0.x, (int)p1.y - size / 2, (int)dif.x, size, TileType.Empty);

                foreach (TileType tile in containedTiles)
                {
                    if (tile == TileType.Door)
                        FillBlock(grid, (int)p0.x, (int)p1.y, 1, 1, tile);
                }
            }
        }

        public Room DestFrom(Room r)
        {
            if (r == r0) return r1;
            else if (r == r1) return r0;
            return null;
        }
    }

    private class BSPNode
    {
        public int x, y, width, height;
        public BSPNode[] children;
        public BSPNode parent;
        public Room room;
        public Path path;
        public Orientation orientation;

        public BSPNode(int x = 0, int y = 0, int width = 0, int height = 0)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.children = null;
            this.parent = null;
        }

        public Room CreateRoom(int width, int height)
        {
            Room r = new Room();
            r.width = Random.Range(MIN_ROOM_WIDTH, width);
            r.height = Random.Range(MIN_ROOM_HEIGHT, height);
            r.x = x + Random.Range(0, width - r.width);
            r.y = y + Random.Range(0, height - r.height);

            return r;
        }

        public Path CreatePath()
        {
            if (children == null) return null;
            if (children[0] == null) return null;
            if (children[1] == null) return null;

            Room[] rooms = GetAdjacentRooms();

            if (rooms[0] == null) return null;
            if (rooms[1] == null) return null;

            return new Path(rooms[0], rooms[1]);
        }

        public void Split()
        {
            orientation = (Orientation)Random.Range((int)Orientation.VERTICAL, (int)Orientation.NB_ORIENTATIONS);

            if (width > 1.25f * height)
                orientation = Orientation.HORIZONTAL;
            else if (width < 0.75f * height)
                orientation = Orientation.VERTICAL;

            if ((orientation == Orientation.HORIZONTAL && width / 2 <= MIN_DIV_WIDTH) || (orientation == Orientation.VERTICAL && height / 2 <= MIN_DIV_HEIGHT))
            {
                room = CreateRoom(width, height);
                return;
            }

            children = new BSPNode[2];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = new BSPNode(0, 0, 0, 0);
                children[i].parent = this;
            }
            if (orientation == Orientation.VERTICAL)
            {
                children[0].width = children[1].width = width;
                children[0].x = children[1].x = x;
                children[0].height = Random.Range(MIN_DIV_HEIGHT, height - MIN_DIV_HEIGHT);
                children[0].y = y;
                children[1].height = height - children[0].height;
                children[1].y = y + children[0].height;
            }
            else
            {
                children[0].height = children[1].height = height;
                children[0].y = children[1].y = y;
                children[0].width = Random.Range(MIN_DIV_WIDTH, width - MIN_DIV_WIDTH);
                children[0].x = x;
                children[1].width = width - children[0].width;
                children[1].x = x + children[0].width;
            }
            foreach (BSPNode child in children)
                child.Split();

            if (children != null && children[0] != null && children[1] != null)
            {
                path = CreatePath();
            }
        }

        public Room[] GetAdjacentRooms()
        {
            if (children[0] == null || children[1] == null) return null;

            Room[] res = new Room[2];

            if (orientation == Orientation.HORIZONTAL) {
                res[0] = children[0].GetRoom(POS_TOP | POS_RIGHT);
                res[1] = children[1].GetRoom(POS_TOP | POS_LEFT);
            }
            else
            {
                res[0] = children[0].GetRoom(POS_BOTTOM | POS_RIGHT);
                res[1] = children[1].GetRoom(POS_TOP | POS_RIGHT);
            }

            return res;
        }

        public BSPNode GetNode(int targetPosition)
        {
            if (children == null) return this;
            if (children[0] == null || children[1] == null) return this;

            if (orientation == Orientation.HORIZONTAL)
            {
                if ((targetPosition & POS_LEFT) != 0)
                    return children[0].GetNode(targetPosition);
                else if ((targetPosition & POS_RIGHT) != 0)
                    return children[1].GetNode(targetPosition);
            }

            if ((targetPosition & POS_TOP) != 0)
                return children[0].GetNode(targetPosition);
            else if ((targetPosition & POS_BOTTOM) != 0)
                return children[1].GetNode(targetPosition);

            return null;
        }

        public Room GetRoom(int targetPosition)
        {
            return GetNode(targetPosition).room;
        }

        public Room GetRandomRoom()
        {
            if (children == null) return room;
            if (children[0] == null || children[1] == null) return room;

            if (Random.Range(0, 1) < 0.5f)
                return children[0].GetRandomRoom();
            return children[1].GetRandomRoom();
        }

        public Room[] GetRooms()
        {
            if (children == null) return new Room[1] { room };

            Room[][] childRooms = new Room[children.Length][];
            int nbRooms = 0;

            for (int i = 0; i < childRooms.Length; i++)
            {
                childRooms[i] = children[i].GetRooms();
                nbRooms += childRooms[i].Length;
            }

            Room[] rooms = new Room[nbRooms];

            int curIndex = 0;
            for (int i = 0; i < children.Length; i++)
                for (int j = 0; j < childRooms[i].Length; j++, curIndex++)
                    rooms[curIndex] = childRooms[i][j];

            return rooms;
        }

        public Path[] GetPaths()
        {
            if (children == null) return null;

            Path[][] childPaths = new Path[children.Length][];
            int nbPaths = 0;

            for (int i = 0; i < childPaths.Length; i++)
            {
                childPaths[i] = children[i].GetPaths();
                if (childPaths[i] != null)
                    nbPaths += childPaths[i].Length;
            }

            if (path != null) nbPaths++;

            Path[] paths = new Path[nbPaths];

            int curIndex = 0;
            for (int i = 0; i < children.Length; i++)
            {
                if (childPaths[i] == null) continue;

                for (int j = 0; j < childPaths[i].Length; j++, curIndex++)
                    paths[curIndex] = childPaths[i][j];
            }

            if (path != null)
                paths[curIndex] = path;

            return paths;
        }
    }

    private TileType[,] GenerateTerrain(int width, int height)
    {
        Random.InitState((int)System.DateTime.Now.ToFileTime());

        root = new BSPNode(1, 1, width - 2, height - 2);
        root.Split();

        Room[] rooms = root.GetRooms();
        Path[] paths = root.GetPaths();

        TileType[,] grid = new TileType[height, width];
        FillBlock(grid, 0, 0, width, height, TileType.Wall);

        Room playerRoom = root.GetRoom(POS_BOTTOM | POS_LEFT);
        Room exitRoom;
        if (root.orientation == Orientation.VERTICAL)
            exitRoom = root.GetRoom(POS_TOP | POS_LEFT);
        else
            exitRoom = root.GetRoom(POS_BOTTOM | POS_RIGHT);

        playerRoom.containedTiles.Add(TileType.Player);
        exitRoom.containedTiles.Add(TileType.End);

        var playerPath = exitRoom.PathToPlayer(); 
        int nb_doors = 0;   
        for(int i = 0; i < playerPath.Count - 1; i++)
        {
            Path path = null;  
            foreach(Path p in playerPath[i].paths)
                if(p.DestFrom(playerPath[i]) == playerPath[i+1])
                {
                    path = p;
                    break;
                }

            if (path == null) continue;

            if(Random.Range(0, 1.0f) < 0.2f || (i == playerPath.Count - 2 && nb_doors <= 0))
            {
                nb_doors++;
                path.containedTiles.Add(TileType.Door);

                var accessibleRooms = playerPath[i + 1].AccessibleRooms();
                accessibleRooms[Random.Range(0, accessibleRooms.Count)].containedTiles.Add(TileType.Key);
            }
        }

        
        int nb_enemies = Random.Range(1, MAX_NB_ENEMIES);
        for(int i = 0; i < nb_enemies; i++)
        {
            Room room;

            do
                room = rooms[Random.Range(0, rooms.Length)];
            while (room == playerRoom);

            var accessibleRooms = room.AccessibleRooms();

            room.containedTiles.Add(TileType.Enemy);
            accessibleRooms[Random.Range(0, accessibleRooms.Count)].containedTiles.Add(TileType.Dagger);
        }
        
        foreach (Room r in rooms)
            r.Draw(grid);

        foreach (Path p in paths)
            p.Draw(grid);

        return grid;
    }

    protected void Start()
    {
        int width = 64;
        int height = 64;
        TileType[,] grid = new TileType[height, width];

        FillBlock(grid, 0, 0, width, height, TileType.Wall);
        FillBlock(grid, 26, 26, 12, 12, TileType.Empty);
        FillBlock(grid, 32, 28, 1, 1, TileType.Player);
        FillBlock(grid, 30, 30, 1, 1, TileType.Dagger);
        FillBlock(grid, 34, 30, 1, 1, TileType.Key);
        FillBlock(grid, 32, 32, 1, 1, TileType.Door);
        FillBlock(grid, 32, 36, 1, 1, TileType.Enemy);
        FillBlock(grid, 32, 34, 1, 1, TileType.End);


        grid = GenerateTerrain(height, width);

        CreateTilesFromArray(grid);
    }
    static void FillBlock(TileType[,] grid, int x, int y, int width, int height, TileType fillType)
    {
        if (width < 0)
        {
            x += width;
            width = -width;
        }

        if (height < 0)
        {
            y += height;
            height = -height;
        }

        for (int tileY = 0; tileY < height; tileY++)
        {
            for (int tileX = 0; tileX < width; tileX++)
            {
                grid[tileY + y, tileX + x] = fillType;
            }
        }
    }
    private void CreateTilesFromArray(TileType[,] grid)
    {
        int height = grid.GetLength(0);
        int width = grid.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileType tile = grid[y, x];
                if (tile != TileType.Empty)
                {
                    CreateTile(x, y, tile);
                }
            }
        }
    }
    private GameObject CreateTile(int x, int y, TileType type)
    {
        int tileID = ((int)type) - 1;
        if (tileID >= 0 && tileID < tiles.Length)
        {
            GameObject tilePrefab = tiles[tileID];
            if (tilePrefab != null)
            {
                GameObject newTile = GameObject.Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                newTile.transform.SetParent(transform);
                return newTile;
            }

        }

        return null;
    }

}
