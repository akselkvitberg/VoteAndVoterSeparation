using System.Security.Cryptography;

namespace Shared;

public static class Encryption
{
    public static byte[] EncryptWithRsaPublicKey(this byte[] message, byte[] publicKey)
    {
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.Encrypt(message, RSAEncryptionPadding.Pkcs1);
    }
    
    public static byte[] DecryptWithRsa(this byte[] message, RSA rsa)
    {
        return rsa.Decrypt(message, RSAEncryptionPadding.Pkcs1);
    }

    public static byte[] ApplyXorCipher(this byte[] source, byte[] key)
    {
        if (source.Length >= key.Length)
            throw new ArgumentOutOfRangeException(nameof(key), "Key length must be equal to or greater the size of the source");

        return source
            .Zip(key, (v, k) => (byte)(v ^ k))
            .ToArray();
    }
}

public static class ShuffleExtensions
{
    public static IList<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var list = new List<T>();
        foreach (var item in source)
        {
            var i = Random.Shared.Next(list.Count + 1);
            if (i == list.Count)
            {
                list.Add(item);
            }
            else
            {
                var temp = list[i];
                list[i] = item;
                list.Add(temp);
            }
        }
        return list;
    }
}