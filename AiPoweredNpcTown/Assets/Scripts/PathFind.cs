//pathfinding reference copyright,source - Nisse Engström
//modified by - Daniel Ogunrinde
//https://stackoverflow.com/questions/2138642/how-to-implement-an-a-algorithm



using System.Collections.Generic;
using System;
using System.Linq;

public class PathFind
{

    public static void Main()
    {
        char[][] matrix = new char[][] { new char[] {'-', '_', '-', '-', 'X'},
                                         new char[] {'-', 'X', 'X', '-', '-'},
                                         new char[] {'-', '-', '-', 'X', '-'},
                                         new char[] {'X', '-', 'X', '-', '-'},
                                         new char[] {'-', '-', '-', '-', 'X'}};
        Console.WriteLine(matrix[0].Length);
        Stack<int[]> coords = unitTest_AStar(matrix, new int[] { 0, 1 }, new int[] { 3, 3 });
        Console.WriteLine("-------------------");
        while (coords.Count > 0)
        {
            int[] nodes = coords.Pop();
            Console.WriteLine(nodes[0] + " " + nodes[1]);
        }
    }

    public static Stack<int[]> unitTest_AStar(char[][] matrix, int[] start, int[] destination)
    {

        //looking for shortest path from 'S' at (0,1) to 'E' at (3,3)
        //obstacles marked by 'X'
        int fromX = start[0], fromY = start[1], toX = destination[0], toY = destination[1];
        matrixNode endNode = AStar(matrix, fromX, fromY, toX, toY);

        //looping through the Parent nodes until we get to the start node
        Stack<matrixNode> path = new Stack<matrixNode>();

        while (endNode.x != fromX || endNode.y != fromY)
        {
            path.Push(endNode);
            endNode = endNode.parent;
        }
        path.Push(endNode);

        Console.WriteLine("The shortest path from  " +
                          "(" + fromX + "," + fromY + ")  to " +
                          "(" + toX + "," + toY + ")  is:  \n");

        Stack<int[]> coords = new Stack<int[]>();
        for (int i = path.Count - 1; i >= 0; i--)
        {
            matrixNode node = path.ElementAt(i);
            coords.Push(new int[] { node.x, node.y });
        }
        /*
        while (path.Count > 0)
        {
            matrixNode node = path.Pop();
            Console.WriteLine("(" + node.x + "," + node.y + ")");
        }
        */
        return coords;

    }

    public class matrixNode
    {
        public int fr = 0, to = 0, sum = 0;
        public int x, y;
        public matrixNode parent;
    }

    public static matrixNode AStar(char[][] matrix, int fromX, int fromY, int toX, int toY)
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // in this version an element in a matrix can move left/up/right/down in one step, two steps for a diagonal move.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //the keys for greens and reds are x.ToString() + y.ToString() of the matrixNode 
        Dictionary<string, matrixNode> greens = new Dictionary<string, matrixNode>(); //open 
        Dictionary<string, matrixNode> reds = new Dictionary<string, matrixNode>(); //closed 

        matrixNode startNode = new matrixNode { x = fromX, y = fromY };
        string key = startNode.x.ToString() + startNode.x.ToString();
        greens.Add(key, startNode);

        Func<KeyValuePair<string, matrixNode>> smallestGreen = () =>
        {
            KeyValuePair<string, matrixNode> smallest = greens.ElementAt(0);

            foreach (KeyValuePair<string, matrixNode> item in greens)
            {
                if (item.Value.sum < smallest.Value.sum)
                    smallest = item;
                else if (item.Value.sum == smallest.Value.sum
                        && item.Value.to < smallest.Value.to)
                    smallest = item;
            }

            return smallest;
        };


        //add these values to current node's x and y values to get the left/up/right/bottom neighbors
        List<KeyValuePair<int, int>> fourNeighbors = new List<KeyValuePair<int, int>>()
                                            { new KeyValuePair<int, int>(-1,0),
                                              new KeyValuePair<int, int>(0,1),
                                              new KeyValuePair<int, int>(1, 0),
                                              new KeyValuePair<int, int>(0,-1) };

        int maxX = matrix.GetLength(0);
        if (maxX == 0)
            return null;
        int maxY = matrix[0].Length;

        while (true)
        {
            if (greens.Count == 0)
                return null;

            KeyValuePair<string, matrixNode> current = smallestGreen();
            if (current.Value.x == toX && current.Value.y == toY)
                return current.Value;

            greens.Remove(current.Key);
            reds.Add(current.Key, current.Value);

            foreach (KeyValuePair<int, int> plusXY in fourNeighbors)
            {
                int nbrX = current.Value.x + plusXY.Key;
                int nbrY = current.Value.y + plusXY.Value;
                string nbrKey = nbrX.ToString() + nbrY.ToString();
                if (nbrX < 0 || nbrY < 0 || nbrX >= maxX || nbrY >= maxY
                    || matrix[nbrX][nbrY] == 'X' //obstacles marked by 'X'
                    || reds.ContainsKey(nbrKey))
                    continue;

                if (greens.ContainsKey(nbrKey))
                {
                    matrixNode curNbr = greens[nbrKey];
                    int from = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                    if (from < curNbr.fr)
                    {
                        curNbr.fr = from;
                        curNbr.sum = curNbr.fr + curNbr.to;
                        curNbr.parent = current.Value;
                    }
                }
                else
                {
                    matrixNode curNbr = new matrixNode { x = nbrX, y = nbrY };
                    curNbr.fr = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                    curNbr.to = Math.Abs(nbrX - toX) + Math.Abs(nbrY - toY);
                    curNbr.sum = curNbr.fr + curNbr.to;
                    curNbr.parent = current.Value;
                    greens.Add(nbrKey, curNbr);
                }
            }
        }
    }
}
