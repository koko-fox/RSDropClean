using System.Diagnostics;
using Timer = System.Threading.Timer;

namespace RSDropClean;

internal class DropCleaner
{
  private IntPtr hProcess;
  private IntPtr fieldItemsAddr;
  private int cleanInterval;
  private Timer? timer;
  private const int MaxFieldItems = 1024;
  private DropCleanConfig config;

  public DropCleaner(Process targetProcess, DropCleanConfig config, int cleanInterval)
  {
    this.cleanInterval = cleanInterval;
    this.config = config;

    hProcess = MemoryEditor.OpenProcess(
        MemoryEditor.PROCESS_VM_READ | MemoryEditor.PROCESS_VM_WRITE | MemoryEditor.PROCESS_VM_OPERATION,
        false,
        targetProcess.Id);

    if (hProcess == IntPtr.Zero)
      return;

    if (targetProcess.MainModule == null)
      return;

    var baseAddress = targetProcess.MainModule.BaseAddress;
    var offsetToPointer = 0x010A5A48;
    var fieldItemsPtrAddress = IntPtr.Add(baseAddress, offsetToPointer);
    var pointerBuffer = new byte[IntPtr.Size];

    if (MemoryEditor.ReadProcessMemory(hProcess, fieldItemsPtrAddress, pointerBuffer, pointerBuffer.Length, out int bytesRead))
    {
      fieldItemsAddr = (IntPtr)BitConverter.ToUInt32(pointerBuffer, 0);
    }
  }

  public void Start()
  {
    if (timer != null)
      return;

    timer = new Timer((state) =>
    {
      CleanFieldItems();
    }, null, 0, cleanInterval);
  }

  public void Stop()
  {
    if (timer != null)
    {
      timer.Dispose();
      timer = null;
    }
  }

  public void Close()
  {
    if (hProcess != IntPtr.Zero)
    {
      MemoryEditor.CloseHandle(hProcess);
      hProcess = IntPtr.Zero;
    }
  }

  private void CleanFieldItems()
  {
    for (int i = 0; i < MaxFieldItems; i++)
    {
      var itemBaseAddr = fieldItemsAddr + (i - 1) * 0x24 + 0x02;
      var itemIdBuffer = new byte[sizeof(ushort)];
      if (!MemoryEditor.ReadProcessMemory(hProcess, itemBaseAddr, itemIdBuffer, itemIdBuffer.Length, out var bytesRead))
        continue;

      var itemId = BitConverter.ToUInt16(itemIdBuffer, 0);

      if (itemId == 0xffff)
        continue;

      if (!config.IsRemoveMarkdItem(itemId))
        continue;

      var itemPositionXAddr = fieldItemsAddr + (i - 1) * 0x24 + 0x0A;
      var itemPositionYAddr = fieldItemsAddr + (i - 1) * 0x24 + 0x0E;
      var moveDestinationBuffer = new byte[sizeof(ushort)];
      MemoryEditor.WriteProcessMemory(hProcess, itemPositionXAddr, moveDestinationBuffer, moveDestinationBuffer.Length, out var bytesWrite);
      MemoryEditor.WriteProcessMemory(hProcess, itemPositionYAddr, moveDestinationBuffer, moveDestinationBuffer.Length, out bytesWrite);
    }
  }
}
