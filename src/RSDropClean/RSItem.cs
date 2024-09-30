namespace RSDropClean;

internal class RSItem
{
  public ushort Id { get; private set; }
  public string Name { get; private set; } = string.Empty;

  public RSItem(ushort id, string name)
  {
    Id = id;
    Name = name;
  }
}
