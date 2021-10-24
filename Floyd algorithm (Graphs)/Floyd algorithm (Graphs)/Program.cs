using System;
using System.Threading;
using System.Diagnostics;


namespace Floyd_algorithm__Graphs_
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

        static void NullMatrix(int[,] a, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    a[i, j] = 0;
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
                        if (a[i, j] > a[i, k] + a[k, j])
                            a[i, j] = a[i, k] + a[k, j];

                    }
                }
            }
        }

        static void Threads(int[,] a, int size, int count=1)
        {
            Thread[] threads = new Thread[count];
            int step = size / count;
            int from = 0;
            int to = step;
            for (int i = 0; i < count; i++)
            {
                int f = from;
                int t = to;
                if (i == count - 1 && size% count != 0)
                {
                    threads[i] = new Thread(() => Floyd(a, size, f, size));
                    break;
                }
                threads[i] = new Thread(() => Floyd(a, size, f, t));

                from += step;
                to += step;
            }
            for (int i = 0; i < count; ++i)
            {
                threads[i].Start();
            }
        }

        static void Main(string[] args)
        {
            const int size = 1000;
            int[,] Matrix = new int[size, size];
            int[,] Result = new int[size, size];

            RandomMatrix(Matrix, size);
            Stopwatch stopwatch = new Stopwatch();

            //1 thread
            Result = (int[,])Matrix.Clone();
            stopwatch.Start();
            Floyd(Result, size, 0, size);
            stopwatch.Stop();

            TimeSpan t1 = stopwatch.Elapsed;
            Time(t1, 1);

            //10 - 100
            for (int i = 2; i < 21; i += 2)
            {
                Result = (int[,])Matrix.Clone();
                stopwatch.Restart();
                Threads(Result, size, i);
                stopwatch.Stop();

                TimeSpan t2 = stopwatch.Elapsed;
                Time(t2, i);
                Acceleration(t1, t2, i);
            }
        }
    }
}
