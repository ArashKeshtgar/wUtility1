using System.Security.Cryptography;
using System.Text;

namespace SchemaSyncApi;

// Real, keyed AES-256-GCM encryption for stored connection strings — the
// security upgrade over the original WPF app's Connections.xaml.cs, which
// used AES with a key hardcoded directly in source (CryptographyUtils, key
// "452654645"). The key here comes from an environment variable, never from
// source; if it's missing, a random one is generated for this process only,
// which is fine for local dev but means anything encrypted before a restart
// becomes unreadable — that's a deliberate, logged limitation, not a bug.
public class ConnectionVault
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public ConnectionVault(ILogger<ConnectionVault> logger)
    {
        var envKey = Environment.GetEnvironmentVariable("CONNECTION_VAULT_KEY");
        if (!string.IsNullOrEmpty(envKey))
        {
            _key = Convert.FromBase64String(envKey);
        }
        else
        {
            _key = RandomNumberGenerator.GetBytes(32);
            logger.LogWarning(
                "CONNECTION_VAULT_KEY is not set — using a random per-process key. " +
                "Stored connections will become undecryptable after this process restarts. " +
                "Set CONNECTION_VAULT_KEY (base64, 32 bytes) for a real deployment.");
        }
    }

    public string Encrypt(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSize];

        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var payload = new byte[NonceSize + ciphertext.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
        Buffer.BlockCopy(ciphertext, 0, payload, NonceSize, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, payload, NonceSize + ciphertext.Length, TagSize);

        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string encoded)
    {
        var payload = Convert.FromBase64String(encoded);
        var nonce = payload[..NonceSize];
        var tag = payload[^TagSize..];
        var ciphertext = payload[NonceSize..^TagSize];
        var plaintextBytes = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(_key, TagSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}
