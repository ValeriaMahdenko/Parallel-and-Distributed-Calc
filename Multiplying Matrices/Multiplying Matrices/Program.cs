using System;
using System.Diagnostics;
using System.Threading;

namespace Multiplying_Matrices
{
    class Program
    {
        static Random randNum = new Random();

        static void NullMatrix(int[,] a, int row, int col)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    a[i, j] = 0;
                }
            }
        }

        static void RandomMatrix(int[,] a, int row, int col)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    a[i, j] = randNum.Next(0, 100);
                }
            }
        }
        
        static void Multiplying(int[,] first, int[,] second, int[,] res, int col, int from, int to)
        {
            for (int i = from; i < to; ++i)
            {
                for (int j = 0; j < col; ++j)
                {
                    for(int k =0; k < col; ++k)
                        res[i, j] += first[i, k] * second[k, j];   
                }
            }
        }
        static void Threads(int[,] first, int[,] second, int[,] res, int row, int col, int count)
        {
            Thread[] threads = new Thread[count];
            int step = row / count;
            int from = 0;
            int to = step;
            for (int i = 0; i < count; i++)
            {
                int f = from;
                int t = to;
                if (i == count - 1 && row % count != 0)
                {
                    threads[i] = new Thread(() => Multiplying(first, second, res, col, f, row));
                    break;
                }
                threads[i] = new Thread(() => Multiplying(first, second, res, col, f, t));

                from += step;
                to += step;
            }
            for (int i = 0; i < count; ++i)
            {
                threads[i].Start();
            }
        }
        static void Time(TimeSpan t, int k)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                t.Hours, t.Minutes, t.Seconds,
                t.Milliseconds / 10);
            Console.WriteLine("RunTime (" + k + "): " + elapsedTime);
        }


        static void Main(string[] args)
        {
            const int row = 1000;
            const int col = 1000;
            int[,] firstMatrix = new int[row, col];
            int[,] secondMatrix = new int[row, col];
            int[,] resultMatrix = new int[row, col];

            NullMatrix(resultMatrix, row, col);
            RandomMatrix(firstMatrix, row, col);
            RandomMatrix(secondMatrix, row, col);
            //for (int i = 0; i < row; i++)
            //{
            //    for (int j = 0; j < col; j++)
            //    {
            //        Console.Write("{0} ", firstMatrix[i, j]);
            //    }
            //    Console.WriteLine();
            //}
            //for (int i = 0; i < row; i++)
            //{
            //    for (int j = 0; j < col; j++)
            //    {
            //        Console.Write("{0} ", secondMatrix[i, j]);
            //    }
            //    Console.WriteLine();
            //}
            Stopwatch stopwatch = new Stopwatch();

            //1 thread
            stopwatch.Start();
            Multiplying(firstMatrix, secondMatrix, resultMatrix, col, 0, row);
            stopwatch.Stop();
            TimeSpan t1 = stopwatch.Elapsed;
            Time(t1, 1);
            //for (int i = 0; i < row; i++)
            //{
            //    for (int j = 0; j < col; j++)
            //    {
            //        Console.Write("{0} ", resultMatrix[i, j]);
            //    }
            //    Console.WriteLine();
            //}
            //Console.WriteLine("Hello!!!");

            //NullMatrix(resultMatrix, row, col);
            //stopwatch.Restart();
            //Threads(firstMatrix, secondMatrix, resultMatrix, row, col, 3);
            //stopwatch.Stop();
            //for (int L = 0; L < row; L++)
            //{ 
            //    for (int j = 0; j < col; j++)
            //    {
            //        Console.Write("{0} ", resultMatrix[L, j]);
            //    }
            //    Console.WriteLine();
            //}
            //TimeSpan t2 = stopwatch.Elapsed;
            //Time(t2, 3);

            //10 - 100
            for (int i = 10; i < 101; i += 10)
            {
                NullMatrix(resultMatrix, row, col);
                stopwatch.Restart();
                Threads(firstMatrix, secondMatrix, resultMatrix, row, col, i);
                stopwatch.Stop();
                //for (int L = 0; L < row; L++)
                //{ 
                //    for (int j = 0; j < col; j++)
                //    {
                //        Console.Write("{0} ", resultMatrix[L, j]);
                //    }
                //    Console.WriteLine();
                //}
                TimeSpan t2 = stopwatch.Elapsed;
                Time(t2, i);
            }
        }
    }
}
