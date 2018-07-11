using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HaarCascadeDeme
{
    class MemoryInfo
    {
//         [StructLayout( LayoutKind.Sequential)]
//         public struct MEMORYSTATUS
//         {
//             public UInt32 dwLength;
//             public UInt32 dwMemoryLoad;
//             public UInt32 dwTotalPhys;
//             public UInt32 dwAvailPhys;
//             public UInt32 dwTotalPageFile;
//             public UInt32 dwAvailPageFile;
//             public UInt32 dwTotalVirtual;
//             public UInt32 dwAvailVirtual; 
//         }


//         [StructLayout(LayoutKind.Sequential)]
//         public struct PERFORMANCE_INFORMATION
//         {
//             public UInt32 cb;
//             public UInt32 CommitTotal;
//             public UInt32 CommitLimit;
//             public UInt32 CommitPeak;
//             public UInt32 PhysicalTotal;
//             public UInt32 PhysicalAvailable;
//             public UInt32 SystemCache;
//             public UInt32 KernelTotal;
//             public UInt32 KernelPaged;
//             public UInt32 KernelNonpaged;
//             public UInt32 PageSize;
//             public UInt32 HandleCount;
//             public UInt32 ProcessCount;
//             public UInt32 ThreadCount;
//         }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUSEX
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt64 ullTotalPhys;
            public UInt64 ullAvailPhys;
            public UInt64 ullTotalPageFile;
            public UInt64 ullAvailPageFile;
            public UInt64 ullTotalVirtual;
            public UInt64 ullAvailVirtual;
            public UInt64 ullAvailExtendedVirtual;
        }

//         [DllImport("kernel32.dll ")]
//         private static extern void GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
// 
//         [DllImport("Psapi.dll ")]
//         private static extern bool GetPerformanceInfo(ref PERFORMANCE_INFORMATION pPerformanceInformation, int cb);
       
        [DllImport("kernel32.dll ")]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        public static double GetFreePhysicalMemory()
        {
            MEMORYSTATUSEX memInfoEx = new MEMORYSTATUSEX();
            memInfoEx.dwLength = (UInt32)Marshal.SizeOf(memInfoEx);
            bool succeedEx = GlobalMemoryStatusEx(ref memInfoEx);

            double result = succeedEx ? (double)memInfoEx.ullAvailPhys : -1;
            return result;
        }

        public static string MemoryConsumeTest()
        {
            StringBuilder sb = new StringBuilder(200);

            MEMORYSTATUSEX memInfoEx = new MEMORYSTATUSEX();
            memInfoEx.dwLength = (UInt32)Marshal.SizeOf(memInfoEx);
            bool succeedEx = GlobalMemoryStatusEx(ref memInfoEx);
            long totalMb2 = Convert.ToInt64(memInfoEx.ullTotalPhys.ToString()) / 1024 / 1024;
            long avaliableMb2 = Convert.ToInt64(memInfoEx.ullAvailPhys.ToString()) / 1024 / 1024;

            sb.AppendLine(succeedEx.ToString());
            sb.AppendLine("物理内存共有" + totalMb2 + " MB");
            sb.AppendLine("可使用的物理内存有" + avaliableMb2 + " MB");

            const int DIV = 1024;
            sb.AppendLine(string.Format("There is  {0} percent of memory in use.\n",
                    memInfoEx.dwMemoryLoad));
            sb.AppendLine(string.Format("There are {0} total Kbytes of physical memory.\n",
                    memInfoEx.ullTotalPhys / DIV));
            sb.AppendLine(string.Format("There are {0} free Kbytes of physical memory.\n",
                    memInfoEx.ullAvailPhys / DIV));
            sb.AppendLine(string.Format("There are {0} total Kbytes of paging file.\n",
                    memInfoEx.ullTotalPageFile / DIV));
            sb.AppendLine(string.Format("There are {0} free Kbytes of paging file.\n",
                    memInfoEx.ullAvailPageFile / DIV));
            sb.AppendLine(string.Format("There are {0} total Kbytes of virtual memory.\n",
                    memInfoEx.ullTotalVirtual / DIV));
            sb.AppendLine(string.Format("There are {0} free Kbytes of virtual memory.\n",
                    memInfoEx.ullAvailVirtual / DIV));

            // Show the amount of extended memory available.

            sb.AppendLine(string.Format("There are {0} free Kbytes of extended memory.\n",
                    memInfoEx.ullAvailExtendedVirtual / DIV));

            return sb.ToString();
        }
    }
}
