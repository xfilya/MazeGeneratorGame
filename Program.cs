using System.Numerics;


namespace Program2
{
    public class Map
    {
        Vector2[] directions = new Vector2[4]
        {
            new Vector2(0,1),
            new Vector2(0,-1),
            new Vector2(1,0),
            new Vector2(-1,0)
        };

        public Dictionary<Vector2, Wall> wallCords = new Dictionary<Vector2, Wall>();
        public Dictionary<Vector2, Wall> mazeWallCords = new Dictionary<Vector2, Wall>();
        public List<Vector2> checkedSpots = new List<Vector2>();
        public List<Vector2> spotsToCheck = new List<Vector2>();
        public List<Vector2> pathCords = new List<Vector2>();
        private int roadSizeX = 16;
        private int roadSizeY = 16;
        public int RoadSizeX => roadSizeX;
        public int RoadSizeY => roadSizeY;
        private Vector2 mapCords;
        public Vector2 MapCords => mapCords;


        public Map()
        {
            Random random = new Random();
            roadSizeX = random.Next(7, 15);
            roadSizeY = random.Next(7, 15);
            int randX = random.Next((-roadSizeX / 2) + 1, roadSizeX / 2);
            int randY = random.Next(-roadSizeY / 2 + 1, roadSizeY / 2);

            Vector2 startingSpot = new Vector2(randX, randY);
            spotsToCheck.Add(startingSpot);

            for (mapCords.Y = (roadSizeY / 2); mapCords.Y >= (0 - roadSizeY / 2); mapCords.Y--)
            {
                for (mapCords.X = (0 - roadSizeX / 2); mapCords.X <= (roadSizeX / 2); mapCords.X++)
                {

                    if (mapCords.X == -roadSizeX / 2 || mapCords.X == roadSizeX / 2 || mapCords.Y == -roadSizeY / 2 || mapCords.Y == roadSizeY / 2)
                    {
                        Wall wall = new Wall();
                        wall.IsBoundary = true;
                        wallCords.Add(mapCords, wall);
                        continue;
                    }
                    else
                    {
                        Wall wall = new Wall();
                        wallCords.Add(mapCords, wall);
                    }

                }
            }

            MazeGenerator();
        }

        public void UpdateMap(Player player, Enemy enemy)
        {
            Console.Clear();


            for (mapCords.Y = (roadSizeY / 2); mapCords.Y >= (0 - roadSizeY / 2); mapCords.Y--)
            {
                for (mapCords.X = (0 - roadSizeX / 2); mapCords.X <= (roadSizeX / 2); mapCords.X++)
                {
                    if (wallCords.ContainsKey(mapCords))
                    {
                        Wall wall = wallCords[mapCords];
                        if (wall.HP > 0)
                        {
                            Console.Write(wall.WallIcon[wall.HP - 1]);
                            continue;
                        }
                        else
                            wallCords.Remove(mapCords);
                    }

                    if (mapCords.Y == enemy.EnemyPos.Y && mapCords.X == enemy.EnemyPos.X)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(enemy.EnemyIcon);
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    if (mapCords.Y == player.PlayerPos.Y && mapCords.X == player.PlayerPos.X)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(player.PlayerIcon);
                        Console.ForegroundColor = ConsoleColor.White;
                        continue;
                    }
                    Console.Write(" ");

                }
                Console.WriteLine("");

            }

        }

        private void MazeGenerator()
        {
            Random random = new Random();

            /* Console.WriteLine(startingSpot);
             Console.ReadLine();*/
            while (spotsToCheck.Count > 0)
            {
                // Console.WriteLine("Проверка");
                Vector2 checkWall = spotsToCheck[random.Next(spotsToCheck.Count)];
                int neighbouringPathsAmount = 0;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 neighbourCheck = checkWall;
                    neighbourCheck += directions[i];
                    if (wallCords.ContainsKey(neighbourCheck) == false)
                    {
                        neighbouringPathsAmount++;
                    }

                }

                if (neighbouringPathsAmount < 2)
                {
                    wallCords.Remove(checkWall);
                    pathCords.Add(checkWall);

                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 neighbourCheck = checkWall;
                        neighbourCheck += directions[i];

                        if (checkedSpots.Contains(neighbourCheck) == false && !wallCords[neighbourCheck].IsBoundary)
                        {
                            spotsToCheck.Add(neighbourCheck);
                        }
                    }

                }

                checkedSpots.Add(checkWall);
                spotsToCheck.Remove(checkWall);

            }


        }




    }

    public class Wall
    {
        private char[] wallIcon;
        public char[] WallIcon => wallIcon;
        private Vector2 wallPos;
        private int hp;
        private bool isBoundary;
        public int HP
        {
            get { return hp; }
            set { hp = value; }
        }
        public Vector2 WallPos
        {
            get { return wallPos; }
            set { wallPos = value; }
        }
        public bool IsBoundary
        {
            get { return isBoundary; }
            set { isBoundary = value; }
        }

        public Wall()
        {
            hp = 3;
            wallPos = new Vector2();
            wallIcon = new char[3] { '▒', '▓', '█' };
        }
        public void Damage()
        {
            if (isBoundary == false)
            {
                hp--;
            }
        }
    }

    public class Player
    {

        public ConsoleKeyInfo key;
        private int moveCounter;
        private Vector2 playerPos;
        private string playerIcon = "☺";

        public int MoveCounter => moveCounter;
        public Vector2 PlayerPos => playerPos;
        public string PlayerIcon => playerIcon;
        public Player(Map map)
        {
            Random random = new Random();

            playerPos.X = map.pathCords[random.Next(map.pathCords.Count)].X;
            playerPos.Y = map.pathCords[random.Next(map.pathCords.Count)].Y;


            moveCounter = 0;
        }


        public void Move(ref Map map)
        {


            Vector2 nextPos = playerPos;
            key = Console.ReadKey();

            if (key.Key == ConsoleKey.D)
            {
                nextPos.X++;
            }
            if (key.Key == ConsoleKey.A)
            {
                nextPos.X--;
            }
            if (key.Key == ConsoleKey.S)
            {
                nextPos.Y--;
            }
            if (key.Key == ConsoleKey.W)
            {
                nextPos.Y++;
            }
            if (map.wallCords.ContainsKey(nextPos) == false)
            {
                playerPos = nextPos;
            }
            else
            {
                Wall wall = map.wallCords[nextPos];
                wall.Damage();
            }




            moveCounter++;

            // ======= DEBUG =======
            /* Console.WriteLine($"Move counter {MoveCounter}");
            Console.WriteLine($"key {key.Key}");
            Console.WriteLine($"enemyX {Enemy.enemyPosX}, enemyY {Enemy.enemyPosY}");
            Console.WriteLine($"playerX {playerPosX}, playerY {playerPosY}"); */




        }
    }

    public class Enemy
    {
        private Vector2 enemyPos;

        private char enemyIcon = '☻';

        public Vector2 EnemyPos => enemyPos;
        public char EnemyIcon => enemyIcon;

        public Enemy(Map map, Player player)
        {

            Random random = new Random();
            enemyPos.X = map.pathCords[random.Next(map.pathCords.Count)].X;
            enemyPos.Y = map.pathCords[random.Next(map.pathCords.Count)].X;


        }
        public void Move(Player player, Map map)
        {

            if (player.MoveCounter % 2 == 0 && player.MoveCounter > 1 && GameManager.IsGameOver(this, player) == false)
            {
                Vector2 nextPos = enemyPos;
                if (player.PlayerPos.X > enemyPos.X)
                {
                    nextPos.X++;
                }
                if (player.PlayerPos.X < enemyPos.X)
                {
                    nextPos.X--;
                }
                if (player.PlayerPos.Y < enemyPos.Y)
                {
                    nextPos.Y--;
                }
                if (player.PlayerPos.Y > enemyPos.Y)
                {
                    nextPos.Y++;
                }
                if (map.wallCords.ContainsKey(nextPos) == false)
                {
                    enemyPos = nextPos;
                }
                else
                {
                    Wall wall = map.wallCords[nextPos];
                    wall.Damage();
                }

            }



        }


    }

    public class GameManager
    {

        static void Main(string[] args)
        {
            Map map = new Map();
            Player player = new Player(map);
            Enemy enemy = new Enemy(map, player);
            Wall wall = new Wall();

            map.UpdateMap(player, enemy);

            while (IsGameOver(enemy, player) == false)
            {
                player.Move(ref map);
                enemy.Move(player, map);
                map.UpdateMap(player, enemy);
                IsGameOver(enemy, player);

            }

        }


        public static bool IsGameOver(Enemy enemy, Player player)
        {
            if (enemy.EnemyPos.X == player.PlayerPos.X && enemy.EnemyPos.Y == player.PlayerPos.Y)
            {
                Console.Clear();
                Console.WriteLine("===========\n Game Over \n===========");
                return true;
            }
            else
                return false;
        }
    }


}