using System.Security.Cryptography;

namespace Shared;

public static class Encryption
{
    public static byte[] EncryptRsa(this byte[] message, byte[] publicKey)
    {
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);
        return rsa.Encrypt(message, RSAEncryptionPadding.Pkcs1);
    }
    
    public static byte[] DecryptRsa(this byte[] message, RSA rsa)
    {
        return rsa.Decrypt(message[..256], RSAEncryptionPadding.Pkcs1);
    }

    public static byte[] PadToLength(this byte[] message, int length)
    {
        var source = new byte[length];
        message.CopyTo(source, 0);
        RandomNumberGenerator.Fill(source.AsSpan(message.Length));
        return source;
    }
    
    public static byte[] ApplyXorCipher(this byte[] source, byte[] key)
    {
        if (source.Length != key.Length)
            throw new ArgumentOutOfRangeException(nameof(key), "Key length must be equal to the size of the source");

        return source
            .Zip(key)
            .Select(tuple => (byte)(tuple.First ^ tuple.Second))
            .ToArray();
    }
}