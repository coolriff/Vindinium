using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vindinium
{

    class SearchNode
    {
        public SearchNode Parent;
        public bool InOpenList;
        public bool InClosedList;
        public float DistanceToGoal;
        public float DistanceTraveled;

        public Pos Position;
        public bool Walkable;
        public SearchNode[] Neighbors;
    }

    class AStar
    {
        private SearchNode[,] searchNodes;
        private List<SearchNode> openList = new List<SearchNode>();
        private List<SearchNode> closedList = new List<SearchNode>();

        private int width;
        private int height;
        private Tile[][] map;
        private ServerStuff serverStuff;

        public AStar(Tile[][] board, ServerStuff aServerStuff)
        {
            width = board.Length;
            height = board.Length;
            map = board;
            serverStuff = aServerStuff;
        }

        public List<Pos> FindPath(Pos startPoint, Pos endPoint)
        {
            // initialiser 
            InitializeSearchNodes(map, startPoint, endPoint);

            if (startPoint == endPoint)
            {
                return new List<Pos>();
            }
            ///////////////////////////////////////////////////////////////////
            // Step 1: Clear the open and closed lists and reset all of the  //
            // nodes F and G values incase they're still set from last time. //
            ///////////////////////////////////////////////////////////////////
            ResetSearchNodes();

            // Store references to start and end nodes for convience.
            SearchNode startNode = searchNodes[startPoint.x, startPoint.y];
            SearchNode endNode = searchNodes[endPoint.x, endPoint.y];

            ///////////////////////////////////////////////////////////////////
            // Step 2: Set the start node's G value to 0 and its F value to  //
            //         the estimated distance between the start node and goal//
            //         node.  (This is where the heuristic comes in) and add //
            //         it to the open list                                   //
            ///////////////////////////////////////////////////////////////////
            startNode.InOpenList = true;
            startNode.DistanceToGoal = Heuristic(startPoint, endPoint);
            startNode.DistanceTraveled = 0;
            openList.Add(startNode);

            ///////////////////////////////////////////////////////////////////
            // Step 3: While there are still nodes on the open list...       //
            ///////////////////////////////////////////////////////////////////
            while (openList.Count > 0)
            {
                // Find the node with the lowest F value
                SearchNode currentNode = FindBestNode();

                // If the open list is empty or no node can be found
                if (currentNode == null)
                {
                    break;
                }

                // If we've reached our goal
                if (currentNode == endNode)
                {
                    return FindFinalPath(startNode, endNode);
                }

                // If not, keep going through the open list
                for (int i = 0; i < currentNode.Neighbors.Length; i++)
                {
                    SearchNode neighbor = currentNode.Neighbors[i];
                    if (neighbor == null || neighbor.Walkable == false)
                    {
                        continue;
                    }

                    float distanceTraveled = currentNode.DistanceTraveled + 1;
                    float heuristic = Heuristic(neighbor.Position, endPoint);

                    // If the neighbor isn't in the closed or open list
                    if (neighbor.InOpenList == false && neighbor.InClosedList == false)
                    {
                        neighbor.DistanceTraveled = distanceTraveled;
                        neighbor.DistanceToGoal = distanceTraveled + heuristic;
                        neighbor.Parent = currentNode;
                        neighbor.InOpenList = true;
                        openList.Add(neighbor);
                    }
                    else if (neighbor.InOpenList || neighbor.InClosedList)
                    {
                        if (neighbor.DistanceTraveled > distanceTraveled)
                        {
                            neighbor.DistanceTraveled = distanceTraveled;
                            neighbor.DistanceToGoal = distanceTraveled + heuristic;
                            neighbor.Parent = currentNode;
                        }
                    }
                }
                openList.Remove(currentNode);
                currentNode.InClosedList = true;
            }

            return new List<Pos>();
        }

        private float Heuristic(Pos point1, Pos point2)
        {
            return Math.Abs(point1.x - point2.x)
                 + Math.Abs(point1.y - point2.y);
        }

        private void ResetSearchNodes()
        {
            openList.Clear();
            closedList.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SearchNode node = searchNodes[x, y];
                    if (node == null)
                    {
                        continue;
                    }
                    node.InOpenList = false;
                    node.InClosedList = false;
                    node.DistanceTraveled = float.MaxValue;
                    node.DistanceToGoal = float.MaxValue;
                }
            }
        }

        private List<Pos> FindFinalPath(SearchNode startNode, SearchNode endNode)
        {
            closedList.Add(endNode);
            SearchNode parentTile = endNode.Parent;
            while (parentTile != startNode)
            {
                closedList.Add(parentTile);
                parentTile = parentTile.Parent;
            }

            List<Pos> finalPath = new List<Pos>();

            for (int i = closedList.Count - 1; i >= 0; i--)
            {
                finalPath.Add(closedList[i].Position);
            }
            return finalPath;
        }

        private SearchNode FindBestNode()
        {
            SearchNode currentTile = openList[0];
            float smallestDistanceToGoal = float.MaxValue;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].DistanceToGoal < smallestDistanceToGoal)
                {
                    currentTile = openList[i];
                    smallestDistanceToGoal = currentTile.DistanceToGoal;
                }
            }
            return currentTile;
        }

        /// <summary>
        /// 4 directions of other heros are not walkable to keep safe
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="searchNodes"></param>
        private void NotWalkableHero(Pos endPoint, SearchNode[,] searchNodes)
        {
            // other heros' positions
            List<Pos> HeroPos = new List<Pos>();
            for (int i = 0; i < 4; i++)
            {
                if (i + 1 != serverStuff.myHero.id && serverStuff.heroes[i].crashed == false)
                {
                    HeroPos.Add(serverStuff.heroes[i].pos);
                }
            }
            foreach (Pos pos in HeroPos)
            {
                // the others are not the target
                if (endPoint.x != pos.x && endPoint.y != pos.y)
                {
                    if (pos.y + 1 < height)
                    {
                        // east
                        searchNodes[pos.x, pos.y + 1] = null;
                    }
                    if (pos.y - 1 >= 0)
                    {
                        //west
                        searchNodes[pos.x, pos.y - 1] = null;
                    }
                    if (pos.x + 1 < width)
                    {
                        // south
                        searchNodes[pos.x + 1, pos.y] = null;
                    }
                    if (pos.x - 1 >= 0)
                    {
                        //north
                        searchNodes[pos.x - 1, pos.y] = null;
                    }
                }
            }
        }
        private void InitializeSearchNodes(Tile[][] map, Pos startPoint, Pos endPoint)
        {
            searchNodes = new SearchNode[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SearchNode node = new SearchNode();
                    Pos temp = new Pos();
                    temp.x = x;
                    temp.y = y;
                    node.Position = temp;
                    node.Walkable = map[x][y] == Tile.FREE;
                    if (node.Walkable)
                    {
                        node.Neighbors = new SearchNode[4];
                        searchNodes[x, y] = node;
                    }
                }
            }

            //-----------------------------------------------------------------------
            // Option: ajouter la fonction set 4 directions des heros notwalkable
            NotWalkableHero(endPoint, searchNodes);
            //-----------------------------------------------------------------------

            // ajouter endPoint
            SearchNode endNode = new SearchNode();
            endNode.Position = endPoint;
            endNode.Walkable = true;
            endNode.Neighbors = new SearchNode[4];
            searchNodes[endPoint.x, endPoint.y] = endNode;
            // ajouter startPoint
            SearchNode startNode = new SearchNode();
            startNode.Position = startPoint;
            startNode.Walkable = true;
            startNode.Neighbors = new SearchNode[4];
            searchNodes[startPoint.x, startPoint.y] = startNode;

            // ajouter neighbors
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SearchNode node = searchNodes[x, y];
                    if (node == null || node.Walkable == false)
                    {
                        continue;
                    }

                    Pos n1 = new Pos();
                    Pos n2 = new Pos();
                    Pos n3 = new Pos();
                    Pos n4 = new Pos();
                    n1.x = x; n1.y = y - 1;
                    n2.x = x; n2.y = y + 1;
                    n3.x = x - 1; n3.y = y;
                    n4.x = x + 1; n4.y = y;

                    Pos[] neighbors = new Pos[]
                    {
                        n1, n2, n3, n4
                    };

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        Pos position = neighbors[i];
                        if (position.x < 0 || position.x > width - 1 ||
                           position.y < 0 || position.y > height - 1)
                        {
                            continue;
                        }
                        SearchNode neighbor = searchNodes[position.x, position.y];
                        if (neighbor == null || neighbor.Walkable == false)
                        {
                            continue;
                        }
                        node.Neighbors[i] = neighbor;
                    }
                }
            }
        }
    }
}
