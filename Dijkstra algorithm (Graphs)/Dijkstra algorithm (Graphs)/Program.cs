using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Dijkstra_algorithm__Graphs_
{
    class Program
    {
        static Random randNum = new Random();
        public static void ShowMatrix(int[,] A, int Size)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Console.Write("{0:0.##} ", A[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void ShowRes(int[] X, int Size)
        {
            Console.WriteLine("res:");
            for (int i = 0; i < Size; i++)
            {
                Console.Write("{0:0.##}  ", X[i]);
            }
            Console.WriteLine();
        }

        static void Time(TimeSpan t, int k)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                t.Hours, t.Minutes, t.Seconds,
                t.Milliseconds / 10);
            Console.WriteLine("RunTime (" + k + "): \t" + elapsedTime);
        }

        static void RandomMatrix(int[,] a, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == j)
                    {
                        a[i, j] = 0;
                    }
                    else
                    {
                        a[i, j] = randNum.Next(0, 100);
                    }
                }
            }
        }
        static void NullVector(int[] a, int size)
        {
            for (int i = 0; i < size; i++)
            {
                a[i] = 0;
            }
        }

        static void Acceleration(TimeSpan t1, TimeSpan t2, int count)
        {
            float accel = t1.Ticks / (float)t2.Ticks;
            float effect = accel / count;
            Console.WriteLine("Acceleration: \t" + accel + "\nEffectiveness: \t" + effect + "\n");
        }

        static int FindMin(int[] ways, bool[] visited, int size)
        {
            int min = int.MaxValue;
            int min_index = -1;

            for (int i = 0; i < size; i++)
            {
                if (visited[i] == false && ways[i] <= min)
                {
                    min = ways[i];
                    min_index = i;
                }
            }
            return min_index;
        }

        static void Dijkstra(int[,] matrix, int[] ways, bool[] visited, int size, int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                int pos = FindMin(ways, visited, size);
                visited[pos] = true;

                for (int j = 0; j < size; j++)
                {
                    if (!visited[j] && matrix[pos, j] != 0 && ways[pos] != int.MaxValue && ways[pos] + matrix[pos, j] < ways[j])
                    {
                        ways[j] = ways[pos] + matrix[pos, j];
                    }
                }
            }
        }

        static void DijkstraThreads(int[,] matrix, int[] ways, int size, int count=1)
        {
            bool[] visited = new bool[size];
            for (int i = 0; i < size; i++)
            {
                ways[i] = int.MaxValue;
                visited[i] = false;
            }
            ways[0] = 0;
            if (count > size)
                count = size;

            int step = size / count;
            Thread[] threads = new Thread[count];

            for (int t = 0; t < count; t++)
            {
                int from = t * step;
                int to = 0;

                if (t + 1 == count)
                    to = size;
                else
                    to = from + step;
                threads[t] = new Thread(() => { Dijkstra(matrix, ways, visited, size, from, to); });
            }

            for (int j = 0; j < count; ++j)
                threads[j].Start();
            for (int j = 0; j < count; ++j)
                threads[j].Join();
        }

        static void Main(string[] args)
        {
            const int size = 10000;
            int[,] Matrix = new int[size, size];
            int[] Result = new int[size];

            NullVector(Result, size);
            RandomMatrix(Matrix, size);

            Stopwatch stopwatch = new Stopwatch();

            //1 thread
            stopwatch.Start();
            DijkstraThreads(Matrix, Result, size);
            stopwatch.Stop();
            TimeSpan t1 = stopwatch.Elapsed;
            Time(t1, 1);

            //10 - 100
            for (int i = 2; i < 17; i += 2)
            {
                NullVector(Result, size);
                stopwatch.Restart();
                DijkstraThreads(Matrix, Result, size, i);
                stopwatch.Stop();

                TimeSpan t2 = stopwatch.Elapsed;
                Time(t2, i);
                Acceleration(t1, t2, i);
            }
        }
    }
}
