using System;
using OpenCL.Net;
using System.Diagnostics;

namespace CUDA
{
    class CudaProgram
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

        static void NullMatrix(int[,] a, int count)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    a[i, j] = 0;
                }
            }
        }

        static void RandomMatrix(int[,] a, int count)
        {
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    a[i, j] = randNum.Next(0, 100);
                }
            }
        }

        static void Multiplying(int[,] first, int[,] second, int[,] res, int size)
        {
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    for (int k = 0; k < size; ++k)
                        res[i, j] += first[i, k] * second[k, j];
                }
            }
        }

        static void Time(TimeSpan t, int i = 0)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                t.Hours, t.Minutes, t.Seconds,
                t.Milliseconds / 10);
            if (i > 0)
                Console.WriteLine("RunTime (" + i + "): " + elapsedTime);
            else
                Console.WriteLine("RunTime OpenCL: " + elapsedTime);
        }


        static void Main(string[] args)
        {
            Console.Write("Input count: ");
            int count = Convert.ToInt32(Console.ReadLine());

            int[,] firstMatrix = new int[count, count];
            int[,] secondMatrix = new int[count, count];
            int[,] resultMatrix = new int[count, count];

            NullMatrix(resultMatrix, count);
            RandomMatrix(firstMatrix, count);
            RandomMatrix(secondMatrix, count);

            Stopwatch stopwatch = new Stopwatch();

            //1 thread
            stopwatch.Start();
            Multiplying(firstMatrix, secondMatrix, resultMatrix, count);
            stopwatch.Stop();
            TimeSpan t1 = stopwatch.Elapsed;
            Time(t1, 1);

            stopwatch.Restart();
            Event event0; ErrorCode err;
            Platform[] platforms = Cl.GetPlatformIDs(out err);
            Device[] devices = Cl.GetDeviceIDs(platforms[0], DeviceType.Gpu, out err);
            Device device = devices[0]; 
            Context context = Cl.CreateContext(null, 1, devices, null, IntPtr.Zero, out err);
            CommandQueue cmdQueue = Cl.CreateCommandQueue(context, device, CommandQueueProperties.None, out err);

            string programSource = @"
                __kernel void Multi(__global int * inputA,__global int * inputB, __global int * output,int n) 
                { 
                    int i = get_global_id(0);
                    int j = get_global_id(1);
                    if (i < n && j < n)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            output[i * n + j] += inputA[i * n + k] * inputB[k * n + j];
                            
                        }
                        
                    }      
                };";
            Program program = Cl.CreateProgramWithSource(context, 1, new[] { programSource }, null, out err);
            Cl.BuildProgram(program, 0, null, string.Empty, null, IntPtr.Zero);  
            Kernel kernel = Cl.CreateKernel(program, "Multi", out err);

            Mem memInputA = (Mem)Cl.CreateBuffer(context, MemFlags.ReadOnly, sizeof(int) * count * count, out err);
            Mem memInputB = (Mem)Cl.CreateBuffer(context, MemFlags.ReadOnly, sizeof(int) * count * count, out err);
            Mem memoutput = (Mem)Cl.CreateBuffer(context, MemFlags.WriteOnly, sizeof(int) * count * count, out err);
            int[] a = new int[count * count];
            int[] b = new int[count * count];
            int[] c = new int[count * count];

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    a[i * count + j] = firstMatrix[i, j];
                    b[i * count + j] = secondMatrix[i, j];
                    c[i * count + j] = 0;
                }
            }

            Cl.EnqueueWriteBuffer(cmdQueue, (IMem)memInputA, Bool.True, IntPtr.Zero, new IntPtr(sizeof(int) * count * count), a, 0, null, out event0);
            Cl.EnqueueWriteBuffer(cmdQueue, (IMem)memInputB, Bool.True, IntPtr.Zero, new IntPtr(sizeof(int) * count * count), b, 0, null, out event0);
            Cl.EnqueueWriteBuffer(cmdQueue, (IMem)memoutput, Bool.True, IntPtr.Zero, new IntPtr(sizeof(int) * count * count), c, 0, null, out event0);

            IntPtr[] localWorkSize = new IntPtr[2], globalWorkSize = new IntPtr[2];
            globalWorkSize[0] = new IntPtr(count * 4);
            globalWorkSize[1] = new IntPtr(count * 4);
            localWorkSize[0] = new IntPtr(sizeof(int));
            localWorkSize[1] = new IntPtr(sizeof(int));

            Cl.SetKernelArg(kernel, 0, new IntPtr(4), memInputA);
            Cl.SetKernelArg(kernel, 1, new IntPtr(4), memInputB);
            Cl.SetKernelArg(kernel, 2, new IntPtr(4), memoutput);
            Cl.SetKernelArg(kernel, 3, new IntPtr(4), count);
            var eroior = Cl.EnqueueNDRangeKernel(cmdQueue, kernel, 2, null, globalWorkSize, localWorkSize, 0, null, out event0);
            Cl.Finish(cmdQueue);

            Cl.EnqueueReadBuffer(cmdQueue, (IMem)memoutput, Bool.True, IntPtr.Zero, new IntPtr(count * count * sizeof(int)), c, 0, null, out event0);
           
            stopwatch.Stop();
            TimeSpan t2 = stopwatch.Elapsed;
            Time(t2, 0);

        }
    }
}
