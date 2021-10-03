using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gaussian_method
{
    class Program
    {
        const int size = 60;

        static Random randNum = new Random();

        static void Time(TimeSpan t, int k)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                t.Hours, t.Minutes, t.Seconds,
                t.Milliseconds / 10);
            Console.WriteLine("RunTime (" + k + "): \t" + elapsedTime);
        }

        static void Random(double[,] a, double[] b, int size)
        {
            for (int i = 0; i < size; i++)
            {
                b[i] = randNum.Next(0, 10);
                for (int j = 0; j < size; j++)
                {
                    a[i, j] = randNum.Next(0, 10);
                }
            }
        }

        static void NullVector(double[] a, int size)
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

        static void multiplyMatrixAndVec(double[,] matrix, double[] vec, double[] result, int size, int fromRow, int toRow)
        {
            for (int i = fromRow; i < toRow; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    result[i] += matrix[i, j] * vec[j];
                }
            }
        }

        static void multiplyingMatrices(double[,] firstMatrix, double[,] secondMatrix, double[,] resMatrix, int size, int fromRow, int toRow)
        {
            for (int i = fromRow; i < toRow; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    for (int k = 0; k < size; ++k)
                    {
                        resMatrix[i, j] += firstMatrix[i, k] * secondMatrix[k, j];
                    }
                }
            }
        }

        static void createThreadsForMatrixAndVecMult(double[,] matrixT, double[] vectorB, double[] vectorRes, int size, int threadsCount)
        {
            Thread[] threadsArray = new Thread[threadsCount];
            int from = 0;
            int threadStep = size / threadsCount;
            int to = threadStep;
            for (int i = 0; i < threadsCount; ++i)
            {
                int f = from;
                int t = to;
                threadsArray[i] = new Thread(() => multiplyMatrixAndVec(matrixT, vectorB, vectorRes, size, f, t));
                from += threadStep;
                to += threadStep;
            }
            for (int i = 0; i < threadsCount; ++i)
            {
                threadsArray[i].Start();
            }
            for (int i = 0; i < threadsCount; ++i)
            {
                threadsArray[i].Join();
            }
        }

        static void createThreadsForMatricesMult(double[,] firstMatrix, double[,] secondMatrix, double[,] resultMatrix, int size, int threadsCount)
        {
            Thread[] threadsArray = new Thread[threadsCount];
            int fromRow = 0;
            int threadStep = size / threadsCount;
            int toRow = threadStep;
            for (int i = 0; i < threadsCount; ++i)
            {
                int f = fromRow;
                int t = toRow;
                threadsArray[i] = new Thread(() => multiplyingMatrices(firstMatrix, secondMatrix, resultMatrix, size, f, t));
                fromRow += threadStep;
                toRow += threadStep;
            }
            for (int i = 0; i < threadsCount; ++i)
            {
                threadsArray[i].Start();
            }
            for (int i = 0; i < threadsCount; ++i)
            {
                threadsArray[i].Join();
            }
        }

        static void directCourse(double[,] matrixA, double[] vectorB, int size, int from, int to, int threadsCount)
        {
            for (int k = from; k < to; ++k)
            {
                double[,] matrixT = new double[size, size];
                for (int i = 0; i < size; ++i)
                {
                    matrixT[i, i] = 1;
                }
                matrixT[k, k] = 1 / matrixA[k, k];
                for (int i = k + 1; i < size; ++i)
                {
                    matrixT[i, k] = -matrixA[i, k] / matrixA[k, k];
                }
                createThreadsForMatricesMult(matrixT, matrixA, matrixA, size, threadsCount);
                createThreadsForMatrixAndVecMult(matrixT, vectorB, vectorB, size, threadsCount);
            }
        }

        static void reverseCourse(double[,] matrixA, double[] vectorB, int size, int from, int to, int threadsCount)
        {
            for (int k = from; k >= to; --k)
            {
                double[,] matrixV = new double[size, size];
                for (int i = 0; i < size; ++i)
                {
                    matrixV[i, i] = 1;
                }
                for (int i = 0; i < k; ++i)
                {
                    matrixV[i, k] = -matrixA[i, k];
                }
                createThreadsForMatricesMult(matrixV, matrixA, matrixA, size, threadsCount);
                createThreadsForMatrixAndVecMult(matrixV, vectorB, vectorB, size, threadsCount);
            }

        }

        static void gaussianAlgorithm(double[,] matrixA, double[] vectorB, double[] result, int size, int threadsCount = 1)
        {
            Thread[] threadsArray = new Thread[2 * threadsCount];

            //прямий хід
            int from = 0;
            int threadStep = size / threadsCount;
            int to = threadStep;
            for (int i = 0; i < threadsCount; ++i)
            {
                int f = from;
                int t = to;
                threadsArray[i] = new Thread(() => directCourse(matrixA, vectorB, size, f, t, threadsCount));
                from += threadStep;
                to += threadStep;
            }

            //обернений хід
            from = size - 1;
            to = from - threadStep;
            for (int i = threadsCount; i < 2 * threadsCount; ++i)
            {
                int f = from;
                int t = to;
                threadsArray[i] = new Thread(() => reverseCourse(matrixA, vectorB, size, f, t, threadsCount));
                from -= threadStep;
                to -= threadStep;
            }
            for (int i = 0; i < 2 * threadsCount; ++i)
            {
                threadsArray[i].Start();
            }
            for (int i = 0; i < 2 * threadsCount; ++i)
            {
                threadsArray[i].Join();
            }
            for (int i = 0; i < size; ++i)
            {
                result[i] = vectorB[i];
            }
        }

        static void Main(string[] args)
        {

            double[,] A = new double[size, size];
            double[] B = new double[size];
            double[] Result = new double[size];

            Random(A, B, size);
            NullVector(Result, size);
            Stopwatch stopwatch = new Stopwatch();

            //1 thread
            stopwatch.Start();
            gaussianAlgorithm(A, B, Result, size);
            stopwatch.Stop();
            TimeSpan t1 = stopwatch.Elapsed;
            Time(t1, 1);
            Console.WriteLine();

            //2 - 10
            for (int i = 2; i < 11; i += 1)
            {
                NullVector(Result, size);
                stopwatch.Restart();
                gaussianAlgorithm(A, B, Result, size, i);
                stopwatch.Stop();

                TimeSpan t2 = stopwatch.Elapsed;
                Time(t2, i);
                Acceleration(t1, t2, i);
            }
        }
    }
}
