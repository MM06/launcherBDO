using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace testBDO
{
    class runGame
    {
        public static string exeName = "BlackDesert64.exe";
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(long hProcess, long lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(int hProcess, long lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten);

        public async static void LaunchGame(string username, string password, string ServerIP)
        {
            
            await OpenGameExe(exeName, username, password);
            await StartProcessManipulation(exeName, ServerIP);
        }

        public async static Task OpenGameExe(string exeFile, string username, string password)
        {
            Process.Start(exeFile, (username + "," + password) ?? "");
        }

        public static void WriteProcessMemory(long adress, byte[] processBytes, int processHandle)
        {
            WriteProcessMemory(processHandle, adress, processBytes, processBytes.Length, 0);
        }

        public async static Task StartProcessManipulation(string processName, string ServerIP)
        {
            await Task.Delay(1500); // Wait for the client to start.

            // Start process manipulation
            //int processId = GetProcessId(processName); // Get the process ID.
            Process[] processesByName = Process.GetProcessesByName(processName); // Get the actual process.
            //Console.WriteLine(processesByName[0].Id);
            int processHandle = OpenProcess(0x1F0FFF, false, processesByName[0].Id); // Open the process for data stream manipulation.
            IntPtr baseAddress = processesByName[0].MainModule.BaseAddress; // Set the data stream manipulation to start at the base address of the process.
            // Start server IP initialisation
            byte[] serverIPtranslation = Encoding.ASCII.GetBytes(ServerIP); // Translate server IPv4 address
            // Start data stream manipulation
            WriteProcessMemory(baseAddress.ToInt64() + 0x0A29306, new byte[] { 0x90, 0x90 }, processHandle); // Crypto fix by Matt
            WriteProcessMemory(baseAddress.ToInt64() + 0x07A5B0A, new byte[] { 0x90, 0x90 }, processHandle); // XC Fix 1 by Matt
            WriteProcessMemory(baseAddress.ToInt64() + 0x07A5BF0, new byte[] { 0xEB }, processHandle); // XC Fix 2 by Matt
            WriteProcessMemory(baseAddress.ToInt64() + 0x2B41A38, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, processHandle); // Wipe IP fix by r00tz
            WriteProcessMemory(baseAddress.ToInt64() + 0x2B41A38, serverIPtranslation, processHandle); // Server IP fix by Matt
        }
    }
}
