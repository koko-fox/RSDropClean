namespace RSDropClean;

internal class DropCleanConfig
{
  private List<bool> dropCleanTable = [];
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
    using var reader = new BinaryReader(File.Open(ConfigFilePath, FileMode.OpenOrCreate));

    while (reader.BaseStream.Position != reader.BaseStream.Length)
    {
      dropCleanTable.Add(reader.ReadBoolean());
    }
  }

  public void Save()
  {
    using var writer = new BinaryWriter(File.Open(ConfigFilePath, FileMode.Create));

    foreach (bool value in dropCleanTable)
    {
      writer.Write(value);
    }
  }
}
