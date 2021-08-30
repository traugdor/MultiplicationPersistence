using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MultiplicationPersistence
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            Console.Write("Enter starting point: ");
            string sstart = Console.ReadLine().Trim();
            Console.Write("Please input the number of digits to search: ");
            string smax = Console.ReadLine().Trim();
            Console.WriteLine();

            long coreCount = Environment.ProcessorCount;

            long[] arr = new long[coreCount];
            Task[] tasks = new Task[coreCount];
            long.TryParse(smax, out long longmax);
            long.TryParse(sstart, out long longstart);
            longmax += longstart;
            long pos = 0;
            long maxsteps = 0;
            Console.WriteLine("Starting Stopwatch.");
            sw.Start();
            Console.WriteLine("Calculating " + longmax + " digits on " + coreCount + " threads. . .");

            long chunkStart;
            long chunkEnd = longstart;
            long chunkSize = (longmax - longstart) / arr.Length;
            for (long startIndex = 0; startIndex < coreCount; startIndex ++)
            {
                long sindex = startIndex;
                long cstart;
                long cend;
                //calculate start and end of chunk
                chunkStart = chunkEnd;
                chunkEnd += chunkSize;
                if (startIndex == coreCount - 1)
                    chunkEnd = longmax;

                cstart = chunkStart;
                cend = chunkEnd;
                //Thread t = new Thread(() => DoChunk(arr, sIndex, chunkStart, chunkEnd));
                //t.Start();
                tasks[startIndex] = Task.Run(() => DoChunk(arr, (int)sindex, cstart, cend));
            }

            Task.WaitAll(tasks);

            foreach(long tpos in arr)
            {
                long steps = Per(tpos.ToString());
                if (steps > maxsteps)
                {
                    maxsteps = steps;
                    pos = tpos;
                }
            }

            Console.WriteLine("Done. Stopping Stopwatch.");
            sw.Stop();
            Console.WriteLine("Done. Max steps is " + maxsteps + " at " + pos);
            Console.WriteLine("Program took " + sw.Elapsed.ToString("G") +" to finish.");
            Per(pos.ToString(), true);
            Console.Write("Press ANY KEY to continue. . .");
            Console.ReadKey();
        }
        static void DoChunk(long [] arr, int index, long start, long finish)
        {
            Console.WriteLine("Thread " + index + " started.");
            long maxSteps = 0;
            long pos = 0;
            for(long i = start; i < finish; i++)
            {
                long steps = Per(i.ToString());
                if (steps > maxSteps)
                {
                    maxSteps = steps;
                    pos = i;
                }
            }
            arr[index] = pos;
            Console.WriteLine("Thread " + index + " completed and calculated " + pos);
        }

        static long Per(string n, bool output = false, int steps = 0)
        {
            if (n.Length == 1)
            {
                if (output)
                    Console.WriteLine("DONE\n\n" + steps + " steps taken.");
                return (long)steps;
            }
            if (steps == 0 
                &&(n.Contains("0") 
                    || n.Contains("5"))
                )
            {
                return 0;
            }

            long result = 1;
            char[] digits = n.ToCharArray();
            foreach (char i in digits)
            {
                result *= (int)char.GetNumericValue(i);
            }
            if (output)
            {
                Console.WriteLine(result);
                Console.WriteLine(" ↓");
            }
            return Per(result.ToString(), output, steps + 1);
        }

    }
}
