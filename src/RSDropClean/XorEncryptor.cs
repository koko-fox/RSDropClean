namespace RSDropClean;

internal class XorEncryptor(byte[] key)
{
  private readonly byte[] key = key;

  public byte[] ApplyXor(byte[] data)
  {
    byte[] result = new byte[data.Length];
    for (int i = 0; i < data.Length; i++)
    {
      result[i] = (byte)(data[i] ^ key[i % key.Length]);
    }
    return result;
  }
}
