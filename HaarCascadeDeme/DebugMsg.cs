using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace HaarCascadeDeme
{
    class DebugMsg
    {
        public static bool Debug = false;
        static BackgroundWorker worker;
        static public Stopwatch stopwatch;

        public static void Init(BackgroundWorker w)
        {
            worker = w;
            Debug = true;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

//         public delegate void ShowMessageDelegate(string msg,int lineBack);
//         public static event ShowMessageDelegate MessageAdded = null;
        public static void AddMessage(string msg,int lineBack)
        {
//             if (MessageAdded != null)
//                 MessageAdded(msg,lineBack);
            if(Debug)
                worker.ReportProgress(1,new object[]{msg,lineBack});
        }

    }
}
