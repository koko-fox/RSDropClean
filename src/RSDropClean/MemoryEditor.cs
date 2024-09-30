using System.Runtime.InteropServices;

namespace RSDropClean;

internal class MemoryEditor
{
  [DllImport("kernel32.dll")]
  public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern bool CloseHandle(IntPtr hObject);

  public const int PROCESS_VM_READ = 0x0010;
  public const int PROCESS_VM_WRITE = 0x0020;
  public const int PROCESS_VM_OPERATION = 0x0008;
}
