using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testBDO
{
    class runGame
    {
        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(long hProcess, long lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(int hProcess, long lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool DebugActiveProcess(int dwProcessId);

        public static string exeName = "BlackDesert64.exe";

        public static async Task LaunchGameAsync(string username, string password, string ServerIP)
        {
            await OpenGameExeAsync(exeName, username, password);
            //await StartProcessManipulationAsync(exeName, ServerIP);
            await StartProcessManipulationAsync(exeName, ServerIP, 1);
        }

        public static async Task OpenGameExeAsync(string exeFile, string username, string password)
        {
            string argument = string.Format("{0},{1}", username, password);

            Process game = new Process();
            game.StartInfo.FileName = exeFile;
            game.StartInfo.Arguments = argument;
            game.Start();
        }

        public static void WriteProcessMemory(long adress, byte[] processBytes, int processHandle)
        {
            WriteProcessMemory(processHandle, adress, processBytes, processBytes.Length, 0);
        }

        public int GetProcessId(string proc)
        {
            return Process.GetProcessesByName(proc)[0].Id;
        }

        public static byte[] ReadProcessMemory(long adress, int processSize, int processHandle)
        {
            byte[] buffer = new byte[processSize];
            ReadProcessMemory(processHandle, adress, buffer, processSize, 0);
            return buffer;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }

        public static async Task StartProcessManipulationAsync(string processName, string ServerIP, int type)
        {
            await Task.Delay(1500);
            //Thread.Sleep(1500);

            Process[] processesByName = Process.GetProcessesByName(processName); // Get the actual process.
            int processHandle = OpenProcess(0x1F0FFF, false, processesByName[0].Id); // Open the process for data stream manipulation.
            IntPtr baseAddress = processesByName[0].MainModule.BaseAddress; // Set the data stream manipulation to start at the base address of the process.
            byte[] serverIPtranslation = Encoding.ASCII.GetBytes(ServerIP); // Translate server IPv4 address


            switch (type)
            {
                case 0:
                        WriteProcessMemory(baseAddress.ToInt64() + 0x0A29306, new byte[] { 0x90, 0x90 }, processHandle); // Crypto fix by Matt
                        WriteProcessMemory(baseAddress.ToInt64() + 0x07A5B0A, new byte[] { 0x90, 0x90 }, processHandle); // XC Fix 1 by Matt
                        WriteProcessMemory(baseAddress.ToInt64() + 0x07A5BF0, new byte[] { 0xEB }, processHandle); // XC Fix 2 by Matt
                        WriteProcessMemory(baseAddress.ToInt64() + 0x2B41A38, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, processHandle); // Wipe IP fix by r00tz
                        WriteProcessMemory(baseAddress.ToInt64() + 0x2B41A38, serverIPtranslation, processHandle); // Server IP fix by Matt
                    break;
                case 1:
                        WriteProcessMemory(baseAddress.ToInt64() + 0x7A8E64, new byte[] { 0x90, 0x90 }, processHandle);
                        WriteProcessMemory(baseAddress.ToInt64() + 0x7A9334, new byte[] { 0x90, 0x90 }, processHandle);
                        WriteProcessMemory(baseAddress.ToInt64() + 0x7A9804, new byte[] { 0x90, 0x90 }, processHandle);
                    break;
                default:
                   
                    break;
            }


        }

    }
}
