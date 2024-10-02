namespace RSDropClean;

internal class DropCleanConfig
{
  private List<bool> dropCleanTable = new(Enumerable.Repeat(false, 0xffff));
  private const string ConfigFilePath = "./dc.dat";

  public bool IsRemoveMarkdItem(int id)
  {
    if (id < 0 || id >= dropCleanTable.Count)
      return false;

    return dropCleanTable[id];
  }

  public void SetWillBeRemoved(int id, bool willBeRemoved)
  {
    if (id < 0 || id >= dropCleanTable.Count)
      return;

    dropCleanTable[id] = willBeRemoved;
    Save();
  }

  public void Load()
  {
    if (!File.Exists(ConfigFilePath))
      return;

    try
    {
      using var reader = new BinaryReader(File.Open(ConfigFilePath, FileMode.Open));

      int index = 0;
      while (reader.BaseStream.Position != reader.BaseStream.Length)
      {
        dropCleanTable[index] = reader.ReadBoolean();
        index++;
      }
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message);
    }
  }

  public void Save()
  {
    try
    {
      using var writer = new BinaryWriter(File.Open(ConfigFilePath, FileMode.OpenOrCreate));

      foreach (bool value in dropCleanTable)
      {
        writer.Write(value);
      }
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message);
    }
  }
}
