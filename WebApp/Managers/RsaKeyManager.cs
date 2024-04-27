using System.Security.Cryptography;

namespace WebApp.Managers;

public static class KeyManager
{
    public static RSA RsaKey()
    {
        var key = RSA.Create();

        if (File.Exists("key"))
        {
            key.ImportRSAPrivateKey(File.ReadAllBytes("key"), out _);
        }
        else
        {
            var privateKey = key.ExportRSAPrivateKey();
            File.WriteAllBytes("key", privateKey);
        }

        return key;
    }

    public static RSA RsaPublicKey()
    {
        var key = RSA.Create();

        var publicKey = key.ExportRSAPublicKey();

        key.ImportRSAPublicKey(publicKey, out _);

        return key;
    }
}