using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gaussian_method
{
    class Program
    {
        static Random randNum = new Random();

        static void RandomMatrix(int[,] a, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    a[i, j] = randNum.Next(0, 100);
                }
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
            const int size = 1000;
            int[,] A = new int[size, size];
            int[] B = new int[size];
            int[] Result = new int[size];

            RandomMatrix(A, size);
            for(int i=0; i<size; i++)
            {
                B[i] = randNum.Next(0, 100);
                Result[i] = 0;
            }

            Stopwatch stopwatch = new Stopwatch();
        }
    }
}
