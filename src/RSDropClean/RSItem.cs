namespace RSDropClean;

internal class RSItem
{
  public ushort Id { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public ushort Category { get; private set; }

  public RSItem(ushort id, string name, ushort category)
  {
    Id = id;
    Name = name;
    Category = category;
  }
}
