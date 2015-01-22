using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vindinium
{

    class Node
    {
        public Node Parent;
        public int F;
        public int G;
        public int H;
        public String direction;
        public Pos Position;
        public bool Walkable;
    }

    //Neighbors

    class AStar
    {
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

        public String FindPath(Pos startPos, Pos endPos)
        {

            displayMapEntity();

            if(startPos == endPos)
            {
                return "Stay";
            }

            List<Node> openNode = new List<Node>();
            List<Node> closedNode = new List<Node>();
            List<Node> neighbors = new List<Node>();


            List<Pos> pathPos = new List<Pos>();

            List<Pos> openPos = new List<Pos>();
            List<Pos> closePos = new List<Pos>();

            Node startNode = new Node();
            Node endNode = new Node();
            startNode.Position = startPos;
            endNode.Position = endPos;

            //get neighbors points
            openNode.Add(startNode);
            openPos.Add(startPos);

            startNode.F = 0;
            startNode.G = 0;
            startNode.H = 0;

            //neighbors points for start point
            neighbors.Clear();
            Pos nUP = new Pos();
            Pos nDown = new Pos();
            Pos nRight = new Pos();
            Pos nLift = new Pos();


            //up north
            nUP.x = startPos.x - 1;
            nUP.y = startPos.y;
            Node node1 = new Node();
            if (nUP.x >= 0 && nUP.y >= 0 && nUP.x <= width && nUP.y <= width)
            {
//                 if (map[nUP.y][nUP.x] == Tile.FREE || map[nUP.y][nUP.x] == Tile.GOLD_MINE_NEUTRAL ||
//                     map[nUP.y][nUP.x] == Tile.GOLD_MINE_1 || map[nUP.y][nUP.x] == Tile.GOLD_MINE_2 ||
//                     map[nUP.y][nUP.x] == Tile.GOLD_MINE_3 || map[nUP.y][nUP.x] == Tile.GOLD_MINE_4)
                if (map[nUP.y][nUP.x] == Tile.FREE)
                {
                    //Console.Out.WriteLine("Map entity = " + map[nUP.y][nUP.x]);
                    node1.Position = nUP;
                    node1.Walkable = true;
                    node1.G = 10;
                    node1.H = getH(node1.Position, endPos);
                    node1.F = node1.G + node1.H;
                    node1.direction = "North";
                    neighbors.Add(node1);
                }
                else
                {
                    closedNode.Add(node1);
                    closePos.Add(node1.Position);
                }
            }

            //down South
            nDown.x = startPos.x + 1;
            nDown.y = startPos.y;
            Node node2 = new Node();
            if (nDown.x >= 0 && nDown.y >= 0 && nDown.x <= width && nDown.y <= width)
            {
//                 if (map[nDown.y][nDown.x] == Tile.FREE || map[nDown.y][nDown.x] == Tile.GOLD_MINE_NEUTRAL ||
//                     map[nDown.y][nDown.x] == Tile.GOLD_MINE_1 || map[nDown.y][nDown.x] == Tile.GOLD_MINE_2 ||
//                     map[nDown.y][nDown.x] == Tile.GOLD_MINE_3 || map[nDown.y][nDown.x] == Tile.GOLD_MINE_4)
                if (map[nDown.y][nDown.x] == Tile.FREE)
                {
                    //Console.Out.WriteLine("Map entity = " + map[nUP.y][nUP.x]);
                    node2.Position = nDown;
                    node2.Walkable = true;
                    node2.G = 10;
                    node2.H = getH(node2.Position, endPos);
                    node2.F = node2.G + node2.H;
                    node2.direction = "South";
                    neighbors.Add(node2);
                }
                else
                {
                    closedNode.Add(node2);
                    closePos.Add(node2.Position);
                }
            }

            //right East
            nRight.x = startPos.x;
            nRight.y = startPos.y + 1;
            Node node3 = new Node();
            if (nRight.x >= 0 && nRight.y >= 0 && nRight.x <= width && nRight.y <= width)
            {
//                 if (map[nRight.y][nRight.x] == Tile.FREE || map[nRight.y][nRight.x] == Tile.GOLD_MINE_NEUTRAL ||
//                     map[nRight.y][nRight.x] == Tile.GOLD_MINE_1 || map[nRight.y][nRight.x] == Tile.GOLD_MINE_2 ||
//                     map[nRight.y][nRight.x] == Tile.GOLD_MINE_3 || map[nRight.y][nRight.x] == Tile.GOLD_MINE_4)
                if (map[nRight.y][nRight.x] == Tile.FREE)
                {
                    //Console.Out.WriteLine("Map entity = " + map[nUP.y][nUP.x]);
                    node3.Position = nRight;
                    node3.Walkable = true;
                    node3.G = 10;
                    node3.H = getH(node3.Position, endPos);
                    node3.F = node3.G + node3.H;
                    node3.direction = "East";
                    neighbors.Add(node3);
                }
                else
                {
                    closedNode.Add(node3);
                    closePos.Add(node3.Position);
                }
            }

            //lift West
            nLift.x = startPos.x;
            nLift.y = startPos.y - 1;
            Node node4 = new Node();
            if (nLift.x >= 0 && nLift.y >= 0 && nLift.x <= width && nLift.y <= width)
            {
//                 if (map[nLift.y][nLift.x] == Tile.FREE || map[nLift.y][nLift.x] == Tile.GOLD_MINE_NEUTRAL ||
//                     map[nLift.y][nLift.x] == Tile.GOLD_MINE_1 || map[nLift.y][nLift.x] == Tile.GOLD_MINE_2 ||
//                     map[nLift.y][nLift.x] == Tile.GOLD_MINE_3 || map[nLift.y][nLift.x] == Tile.GOLD_MINE_4)
                if (map[nLift.y][nLift.x] == Tile.FREE)
                {
                    //Console.Out.WriteLine("Map entity = " + map[nUP.y][nUP.x]);
                    node4.Position = nLift;
                    node4.Walkable = true;
                    node4.G = 10;
                    node4.H = getH(node4.Position, endPos);
                    node4.F = node4.G + node4.H;
                    node4.direction = "West";
                    neighbors.Add(node4);
                }
                else
                {
                    closedNode.Add(node4);
                    closePos.Add(node4.Position);
                }
            }

            closedNode.Add(startNode);
            closePos.Add(startPos);

            Pos tempPos = new Pos();
            String tempString = "";
            Node tnode = new Node();
            int temp = 2000;
            for (int i = 0; i < neighbors.Count(); i++)
            {
                if (neighbors[i].F <= temp)
                {
                    if (neighbors[i].F == 1)
                    {
                        tempString = "Stay";
                    }
                    else
                    {
                        tempString = neighbors[i].direction;
                    }
                    tempPos = neighbors[i].Position;
                    tnode = neighbors[i];
                    temp = neighbors[i].F;
                }
            }

            //neighbors.OrderBy(Node => Node.F).ToList();
            //neighbors.Reverse();
            Console.Out.WriteLine("Movable Neighbors " + neighbors.Count());
            Console.Out.WriteLine("Moving to: x=" + tnode.direction);
            Console.Out.WriteLine("Moving to: x=" + tnode.Position.x + ", y=" + tnode.Position.y);
            return tnode.direction;
        }

        private void displayMapEntity()
        {
            //show the walkable maps
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[j][i] == Tile.FREE)
                    {
                        Console.Out.Write(" ");
                    }
                    else if (map[j][i] == Tile.GOLD_MINE_NEUTRAL || map[j][i] == Tile.GOLD_MINE_1 || map[j][i] == Tile.GOLD_MINE_2 || map[j][i] == Tile.GOLD_MINE_3 || map[j][i] == Tile.GOLD_MINE_4)
                    {
                        Console.Out.Write("G");
                    }
                    else if (map[j][i] == Tile.TAVERN)
                    {
                        Console.Out.Write("T");
                    }
                    else if (map[j][i] == Tile.HERO_1)
                    {
                        Console.Out.Write("1");
                    }
                    else if (map[j][i] == Tile.HERO_2)
                    {
                        Console.Out.Write("2");
                    }
                    else if (map[j][i] == Tile.HERO_3)
                    {
                        Console.Out.Write("3");
                    }
                    else if (map[j][i] == Tile.HERO_4)
                    {
                        Console.Out.Write("4");
                    }
                    else
                    {
                        Console.Out.Write("#");
                    }
                }
                Console.Out.WriteLine("");
            }
            Console.Out.WriteLine("");
            Console.Out.WriteLine("");
        }

        private int getH(Pos point1, Pos point2)
        {
            return Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);
        }
    }
}
