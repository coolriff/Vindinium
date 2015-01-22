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
        public List<Node> openList = new List<Node>();
        public List<Node> closedList = new List<Node>();
        public List<Node> neighbors = new List<Node>();
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

        public List<String> FindPath(Pos startPoint, Pos endPoint)
        {
            openList.Clear();
            closedList.Clear();
            List<String> path = new List<String>();
            List<Pos> pathPos = new List<Pos>();



            // initialiser 
            //InitializeSearchNodes(map, startPoint, endPoint);

            //show the walkable maps
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (map[j][i] == Tile.FREE)
                    {
                        Console.Out.Write(" ");
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

            //get neighbors points
            int t = 0;

            while (t < 5)
            {
                if (t == 0)
                {
                    getNeighbors(startPoint, endPoint);
                }
                else
                {
                    getNeighbors(pathPos[t-1], endPoint);
                }

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

                Console.Out.WriteLine("tempPos: x=" + tempPos.x + ", y=" + tempPos.y);
                Console.Out.WriteLine("tempString: " + tempString);
                openList.Add(tnode);
                pathPos.Add(tempPos);
                path.Add(tempString);
                closedList.Add(tnode);
                t++;

                startPoint = pathPos[t-1];

                if (tempString == "Stay")
                {
                    break;
                }

//                 Console.Out.WriteLine("OpenList: " + openList.Count);
// 
//                 for (int i = 0; i < openList.Count(); i++)
//                 {
//                     Console.Out.WriteLine("OpenList: x=" + openList[i].Position.x + ", y=" + openList[i].Position.y);
//                 }
// 
//                 Console.Out.WriteLine("CloseList: " + closedList.Count);
// 
//                 for (int i = 0; i < closedList.Count(); i++)
//                 {
//                     Console.Out.WriteLine("OpenList: x=" + closedList[i].Position.x + ", y=" + closedList[i].Position.y);
//                 }
// 
//                 for (int i = 0; i < openList.Count(); i++)
//                 {
//                     Console.Out.WriteLine("OpenList: f=" + openList[i].F);
//                 }
            }


            
            return path;
        }

        private void getNeighbors(Pos startPos, Pos endPos)
        {
            neighbors.Clear();
            Pos nUP = new Pos();
            Pos nDown = new Pos();
            Pos nRight = new Pos();
            Pos nLift = new Pos();

            //up
            nUP.x = startPos.x - 1;
            nUP.y = startPos.y;

            Node node1 = new Node();
            if ((nUP.x < 0 || nUP.y < 0) || map[nUP.y][nUP.x] != Tile.FREE)
            {
                node1.Position = nUP;
                node1.Walkable = false;
                closedList.Add(node1);
            }
            else
            {
                node1.Position = nUP;
                node1.Walkable = true;
                //node1.Parent.Position = startPos;
                node1.G = 10;
                node1.H = getH(node1.Position, endPos);
                node1.F = node1.G + node1.H;
                node1.direction = "North";
                neighbors.Add(node1);
            }

            //down
            nDown.x = startPos.x + 1;
            nDown.y = startPos.y;

            Node node2 = new Node();
            if ((nDown.x < 0 || nDown.y < 0) || map[nDown.y][nDown.x] != Tile.FREE)
            {
                node2.Position = nDown;
                node2.Walkable = false;
                closedList.Add(node2);
            }
            else
            {
                node2.Position = nDown;
                node2.Walkable = true;
                //node2.Parent.Position = startPos;
                node2.G = 10;
                node2.H = getH(node2.Position, endPos);
                node2.F = node2.G + node2.H;
                node2.direction = "South";
                neighbors.Add(node2);
            }

            //down
            nRight.x = startPos.x;
            nRight.y = startPos.y + 1;

            Node node3 = new Node();
            if ((nRight.x < 0 || nRight.y < 0) || map[nRight.y][nRight.x] != Tile.FREE)
            {
                node3.Position = nRight;
                node3.Walkable = false;
                closedList.Add(node3);
            }
            else
            {
                node3.Position = nRight;
                node3.Walkable = true;
                //node3.Parent.Position = startPos;
                node3.G = 10;
                node3.H = getH(node3.Position, endPos);
                node3.F = node3.G + node3.H;
                node3.direction = "East";
                neighbors.Add(node3);
            }

            //down
            nLift.x = startPos.x;
            nLift.y = startPos.y - 1;

            Node node4 = new Node();
            if ((nLift.x < 0 || nLift.y < 0) || map[nLift.y][nLift.x] != Tile.FREE)
            {
                node4.Position = nLift;
                node4.Walkable = false;
                closedList.Add(node4);
            }
            else
            {
                node4.Position = nLift;
                node4.Walkable = true;
                //node4.Parent.Position = startPos;
                node4.G = 10;
                node4.H = getH(node4.Position, endPos);
                node4.F = node4.G + node4.H;
                node4.direction = "West";
                neighbors.Add(node4);
            }
        }

        private int getH(Pos point1, Pos point2)
        {
            return Math.Abs(point1.x - point2.x) + Math.Abs(point1.y - point2.y);
        }



    }
}
