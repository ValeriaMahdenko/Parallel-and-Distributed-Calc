using System;
using System.Threading;
using System.Diagnostics;


namespace Floyd_algorithm__Graphs_
{
    class Program
    {
        static Random randNum = new Random();
        static int INF = 100000;

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

        public static void ShowRes(double[] X, int Size)
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
                    int num = randNum.Next(1, 200);
                    if (i == j)
                    {
                        a[i, j] = 0;
                    }
                    else
                    {
                        a[i, j] = (num % 38 != 0 || num % 10 != 0) ? num : INF;

                    }
                }
            }
        }

        static void MatrixCopy(int[,] a, int [,]b, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    a[i, j] = b[i, j];
                }
            }
        }

        static void Acceleration(TimeSpan t1, TimeSpan t2, int count)
        {
            float accel = t1.Ticks / (float)t2.Ticks;
            float effect = accel / count;
            Console.WriteLine("Acceleration: \t" + accel + "\nEffectiveness: \t" + effect + "\n");
        }

        static void Floyd(int[,] a, int size, int from, int to)
        {
            for (int k = from; k < to; ++k)
            {
                for (int i = 0; i < size; ++i)
                {
                    for (int j = 0; j < size; ++j)
                    {
                        a[i, j] = Math.Min(a[i, j], a[i, k] + a[k, j]);
                    }
                }
            }
        }

        static void FloydThreads(int [,]a, int size, int count)
        {
            if (count > size)
                count = size;

            Thread[] threads = new Thread[count];
            int step = size / count;
            int from = 0;
            int to = step;
            for (int i = 0; i < count; i++)
            {
                int f = from;
                int t = to;
                if (i == count - 1 && size % count != 0)
                {
                    threads[i] = new Thread(() => Floyd(a, size, f, size));
                    break;
                }
                threads[i] = new Thread(() => { Floyd(a, size, f, t); });
                from += step;
                to += step;
            }

            for (int i = 0; i < count; ++i)
            {
                threads[i].Start();
            }
            for (int i = 0; i < count; ++i)
            {
                threads[i].Join();
            }
            
        }

        static void Main(string[] args)
        {
            const int size = 1000;
            int[,] Matrix = new int[size, size];
            int[,] Result = new int[size, size];

            RandomMatrix(Matrix, size);
            MatrixCopy(Result, Matrix, size);
            Stopwatch stopwatch = new Stopwatch();

            //1 thread
            stopwatch.Start();
            FloydThreads(Matrix, size, 1);
            stopwatch.Stop();
            TimeSpan t1 = stopwatch.Elapsed;
            Time(t1, 1);

            //10 - 100
            for (int i = 2; i < 17; i += 2)
            {
                MatrixCopy(Matrix, Result, size);
                stopwatch.Restart();
                FloydThreads(Matrix, size, i);
                stopwatch.Stop();

                TimeSpan t2 = stopwatch.Elapsed;
                Time(t2, i);
                Acceleration(t1, t2, i);
            }
        }
    }
}
