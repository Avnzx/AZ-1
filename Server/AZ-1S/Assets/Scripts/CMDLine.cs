using System;
using System.Threading;
using Godot;

public class CMDLine {

  CMDLine() {
    cmdthread = new Thread(ProcessCMDLine);
    // cmdthread.IsBackground = true;
    cmdthread.Priority = ThreadPriority.Lowest;
    cmdthread.Start();
  }

  private static void ProcessCMDLine() {
    var th = Thread.CurrentThread;

    var cmdline = OS.ReadStringFromStdIn();
    GD.Print(cmdline);

    ProcessCMDLine();
  }

  Thread? cmdthread;

}