using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vindinium
{
    class GreatestBot
    {
        enum Mode { Attack, Mine, Heal };

        private ServerStuff serverStuff;
        private AStar aStar;
        List<Pos> taverns = new List<Pos>(4);
        List<Pos> mines = new List<Pos>();
        List<Pos> enemies = new List<Pos>(3);
        List<int> mineCounter = new List<int>(4);
        List<int> lifeCounter = new List<int>(4);

        Mode mode = Mode.Attack;

        public GreatestBot(ServerStuff serverStuff)
        {
            this.serverStuff = serverStuff;
        }

        //starts everything
        public void run()
        {
            Console.Out.WriteLine("Greatest bot running");

            serverStuff.createGame();


            if (serverStuff.errored == false)
            {
                //opens up a webpage so you can view the game, doing it async so we dont time out

                new Thread(delegate()
                {
                    System.Diagnostics.Process.Start(serverStuff.viewURL);
                }).Start();
            }

            Random random = new Random();
            // List<Pos> path = new List<Pos>();
            List<List<Pos>> path = new List<List<Pos>>();



            while (serverStuff.finished == false && serverStuff.errored == false)
            {
                aStar = new AStar(serverStuff.board, serverStuff);
                Console.Out.WriteLine("======================================================");
                Console.Out.WriteLine("Hero x=" + serverStuff.myHero.pos.x + ", y=" + serverStuff.myHero.pos.y);
                mines = sortList(getMineLocations());
                enemies = sortList(getEnemyLocations());
                taverns = sortList(getBeerLocations());

                sortByMineCount();
                sortByLife();

                List<String> temps = new List<String>();

                switch (mode)
                {
                    case Mode.Attack:

                        //path.Add(aStar.FindPath(serverStuff.myHero.pos, getNearestEnemy()));
                        //Console.Out.WriteLine("Attack" + " " + path[0].Count + " " + getDirection(path) + " Pos:" + getNearestEnemy());
                        serverStuff.moveHero(aStar.FindPath(serverStuff.myHero.pos, getNearestEnemy()));
                        Console.Out.WriteLine("Attack x=" + enemies[0].x + ", y=" + enemies[0].y);
                        break;
                    case Mode.Mine:

                        //path.Add(aStar.FindPath(serverStuff.myHero.pos, getNearestMine()));
                       // Console.Out.WriteLine("Mine" + " " + path[0].Count + " " + getDirection(path) + " Pos:" + getNearestMine());
                        serverStuff.moveHero(aStar.FindPath(serverStuff.myHero.pos, getNearestMine()));
                        Console.Out.WriteLine("Mine x=" + mines[0].x + ", y=" + mines[0].y);
                        break;
                    case Mode.Heal:

                        //path.Add(aStar.FindPath(serverStuff.myHero.pos, getNearestTavern()));
                        //Console.Out.WriteLine("Heal" + " " + path[0].Count + " " + getDirection(path) + " Pos:" + getNearestTavern());
                        serverStuff.moveHero(aStar.FindPath(serverStuff.myHero.pos, getNearestMine()));
                        Console.Out.WriteLine("Heal x=" + taverns[0].x + ", y=" + taverns[0].y);
                        break;
                }

                Console.Out.WriteLine("======================================================");
                /* Criteria for the modes: 

                    * Attack - Enemy has more than one mine
                    *        - Enemy is within a distance of say 7. 
                    *        - If we have the most mines, only attack with health greater than 80.  
                    *        -
                    * 
                    * 
                    * Mine - Default behaviour
                    *      - Use A* to determine path to closest mines
                    *      - Mine if health is less than nearby enemy, otherwise we can attack (if Attack criteria is met)
                    *      - Health must be greater than or equal 21 - Otherwise goblin defending mine kills us
                    * 
                    * Heal - If health is 21 or less
                    *      - If we have enough wealth
                    *      - If we are close to tavern and health is below 41, go heal up
                    *      - 
                    * 
                    * 
                */
                /*

                if (serverStuff.myHero.life < 90 || (serverStuff.myHero.life < 41 && (distanceTo(getNearestTavern()) < 7)))
                {
                    mode = Mode.Heal;
                }

                else if (distanceTo(getNearestEnemy()) < 8 && (serverStuff.myHero.life >= lifeCounter[1]) && (serverStuff.myHero.mineCount <= mineCounter[1]))
                {
                    if ((serverStuff.myHero.mineCount >= mineCounter[0]) && (serverStuff.myHero.life < 80))
                    {
                        mode = Mode.Mine;
                    }
                    else
                    {
                        mode = Mode.Attack;
                    }

                }

                else
                {
                    mode = Mode.Mine;
                }
                */
            }

            if (serverStuff.errored)
            {
                Console.Out.WriteLine("error: " + serverStuff.errorText);
            }

            Console.Out.WriteLine("The Greatest.. is still the greatest.");
        }



        public List<Pos> getBeerLocations()
        {
            taverns.Clear();

            for (int xPos = 0; xPos < serverStuff.board.Length; xPos++)
            {
                for (int yPos = 0; yPos < serverStuff.board.Length; yPos++)
                {
                    switch (serverStuff.board[xPos][yPos])
                    {
                        case Tile.TAVERN:
                            Pos tavPos = new Pos();
                            tavPos.x = xPos;
                            tavPos.y = yPos;

                            taverns.Add(tavPos);

                            break;
                    }
                }
            }

            return taverns;
        }

        private List<Pos> getEnemyLocations()
        {
            enemies.Clear();

            for (int xPos = 0; xPos < serverStuff.board.Length; xPos++)
            {
                for (int yPos = 0; yPos < serverStuff.board.Length; yPos++)
                {
                    switch (serverStuff.board[xPos][yPos])
                    {
                        case Tile.HERO_1:
                            if (serverStuff.myHero.id != 1)
                            {
                                Pos enemPos = new Pos();

                                enemPos.x = xPos;
                                enemPos.y = yPos;

                                enemies.Add(enemPos);

                                break;
                            }
                            else
                            {
                                break;
                            }
                        case Tile.HERO_2:
                            if (serverStuff.myHero.id != 2)
                            {
                                Pos enemPos = new Pos();

                                enemPos.x = xPos;
                                enemPos.y = yPos;

                                enemies.Add(enemPos);

                                break;
                            }
                            else
                            {
                                break;
                            }

                        case Tile.HERO_3:
                            if (serverStuff.myHero.id != 3)
                            {
                                Pos enemPos = new Pos();

                                enemPos.x = xPos;
                                enemPos.y = yPos;

                                enemies.Add(enemPos);

                                break;
                            }
                            else
                            {
                                break;
                            }
                        case Tile.HERO_4:
                            if (serverStuff.myHero.id != 4)
                            {
                                Pos enemPos = new Pos();

                                enemPos.x = xPos;
                                enemPos.y = yPos;

                                enemies.Add(enemPos);

                                break;
                            }
                            else
                            {
                                break;
                            }
                    }
                }
            }

            return enemies;
        }

        public List<Pos> getMineLocations()
        {
            mines.Clear();

            for (int xPos = 0; xPos < serverStuff.board.Length; xPos++)
            {
                for (int yPos = 0; yPos < serverStuff.board.Length; yPos++)
                {
                    switch (serverStuff.board[xPos][yPos])
                    {
                        case Tile.GOLD_MINE_1:
                            if (serverStuff.myHero.id != 1)
                            {
                                Pos minePos = new Pos();

                                minePos.x = xPos;
                                minePos.y = yPos;

                                mines.Add(minePos);

                                break;
                            }
                            else
                            {
                                break;
                            }
                        case Tile.GOLD_MINE_2:
                            if (serverStuff.myHero.id != 2)
                            {
                                Pos minePos = new Pos();

                                minePos.x = xPos;
                                minePos.y = yPos;

                                mines.Add(minePos);

                                break;
                            }
                            else
                            {
                                break;
                            }

                        case Tile.GOLD_MINE_3:
                            if (serverStuff.myHero.id != 3)
                            {
                                Pos minePos = new Pos();

                                minePos.x = xPos;
                                minePos.y = yPos;

                                mines.Add(minePos);

                                break;
                            }
                            else
                            {
                                break;
                            }
                        case Tile.GOLD_MINE_4:
                            if (serverStuff.myHero.id != 4)
                            {
                                Pos minePos = new Pos();

                                minePos.x = xPos;
                                minePos.y = yPos;

                                mines.Add(minePos);

                                break;
                            }
                            else
                            {
                                break;
                            }
                        case Tile.GOLD_MINE_NEUTRAL:

                            Pos mineNPos = new Pos();

                            mineNPos.x = xPos;
                            mineNPos.y = yPos;

                            mines.Add(mineNPos);
                            break;
                    }
                }
            }

            return mines;
        }


        public List<Pos> sortList(List<Pos> list)
        {
            Pos tempPos = new Pos();

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    double distanceToI = Math.Abs(list[i].x - serverStuff.myHero.pos.x) + Math.Abs(list[i].y - serverStuff.myHero.pos.y);
                    double distanceToJ = Math.Abs(list[j].x - serverStuff.myHero.pos.x) + Math.Abs(list[j].y - serverStuff.myHero.pos.y);

                    if (i != j && (distanceToI < distanceToJ))
                    {
                        tempPos.x = list[i].x;
                        tempPos.y = list[i].y;

                        list[i].x = list[j].x;
                        list[i].y = list[j].y;

                        list[j].x = tempPos.x;
                        list[j].y = tempPos.y;
                    }
                }
            }

            return list;
        }


        public void sortByMineCount()
        {
            mineCounter.Clear();

            for (int i = 0; i < 4; i++)
            {
                mineCounter.Add(serverStuff.heroes[i].mineCount);
            }

            mineCounter.Sort();
            mineCounter.Reverse();
        }

        public void sortByLife()
        {
            lifeCounter.Clear();

            for (int i = 0; i < 4; i++)
            {
                lifeCounter.Add(serverStuff.heroes[i].life);
            }

            lifeCounter.Sort();
            lifeCounter.Reverse();
        }


        public double distanceTo(Pos point)
        {
            return Math.Abs(point.x - serverStuff.myHero.pos.x) + Math.Abs(point.y - serverStuff.myHero.pos.y);
        }


        public Pos getNearestEnemy()
        {
            //Console.Out.WriteLine("getNearestEnemy: x=" + enemies[0].x + ", y=" + enemies[0].y + ".");
//             Pos t = new Pos();
//             t.x = enemies[0].y;
//             t.y = enemies[0].x;
            return enemies[0];
        }

        public Pos getNearestMine()
        {
            //Console.Out.WriteLine("getNearestMine:i x=" + mines[0].x + ", y=" + mines[0].y + ".");
//             Pos t = new Pos();
//             t.x = mines[0].y;
//             t.y = mines[0].x;
            return mines[0];
        }

        public Pos getNearestTavern()
        {
            //Console.Out.WriteLine("getNearestTavern: x=" + taverns[0].x + ", y=" + taverns[0].y + ".");
//             Pos t = new Pos();
//             t.x = taverns[0].y;
//             t.y = taverns[0].x;
            return taverns[0];
        }

        public String getDirection(List<List<Pos>> path)
        {
            if (path[0].Count == 0)
            {
                return "Stay";
            }
            else
            {
                //                 int deltaX = path[0].x - serverStuff.myHero.pos.x;
                //                 int deltaY = path[0].y - serverStuff.myHero.pos.y;
                int deltaX = path[0][0].x - serverStuff.myHero.pos.x;
                int deltaY = path[0][0].y - serverStuff.myHero.pos.y;
                if (deltaX == -1 && deltaY == 0)
                {
                    return "North";
                }
                else if (deltaX == 1 && deltaY == 0)
                {
                    return "South";
                }
                else if (deltaX == 0 && deltaY == -1)
                {
                    return "West";
                }
                else if (deltaX == 0 && deltaY == 1)
                {
                    return "East";
                }
            }
            return "Stay";
        }
    }
}
