using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PhucNH.Commons.Extensions;

/// <summary>
/// Extension for hash a object data.
/// After research about performance hash algorithm, my personal knowlege is:
/// Speed hash on CPU 32bit : SHA1 < MD5 < SHA256 < SHA384 < SHA512.
/// Speed hash on CPU 64bit : SHA1 < MD5 < SHA384 < SHA512 < SHA256.
/// </summary>
public static class HashExtension
{
    /// <summary>
    /// Create a hash algorithm by hash type.
    /// </summary>
    /// <param name="hashType">Hash type choosing.</param>
    /// <returns>Alrotithm.</returns>
    public static async Task<HashAlgorithm> CreateHashAsync(
            this HashAlgorithmName hashType)
    {
        return await Task.Run(() =>
        {
            if (hashType == HashAlgorithmName.SHA1)
            {
                return (HashAlgorithm)SHA1.Create();
            }
            if (hashType == HashAlgorithmName.SHA256)
            {
                return (HashAlgorithm)SHA256.Create();
            }
            if (hashType == HashAlgorithmName.SHA384)
            {
                return (HashAlgorithm)SHA384.Create();
            }
            if (hashType == HashAlgorithmName.SHA512)
            {
                return (HashAlgorithm)SHA512.Create();
            }
            return (HashAlgorithm)MD5.Create();
        });
    }

    /// <summary>
    /// Hash a object.
    /// </summary>
    /// <param name="dataObject">Object data.</param>
    /// <param name="hashType">Hash type.</param>
    /// <typeparam name="TData">Data type of object.</typeparam>
    /// <returns>Hash result.</returns>
    public static async Task<string> HashAsync<TData>(
        this TData dataObject,
        HashAlgorithmName hashType)
    {
        string result = string.Empty;
        if (dataObject == null)
        {
            throw new ArgumentNullException(
                nameof(HashAsync),
                $"Please check argument [dataObject], value is empty.");
        }
        try
        {
            using (var sha = await hashType.CreateHashAsync())
            {
                if (sha == null)
                {
                    return result;
                }
                string data = JsonSerializer.Serialize(dataObject);
                var dataBytes = Encoding.Unicode.GetBytes(data);
                byte[] hashValue = sha.ComputeHash(dataBytes);
                result = Convert.ToBase64String(hashValue);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result ?? string.Empty;
    }

    /// <summary>
    /// Create a hash mac algorithm by hash type.
    /// </summary>
    /// <param name="hashType">Hash type choosing.</param>
    /// <param name="key">Key value of hash.</param>
    /// <returns>Alrotithm.</returns>
    public static async Task<HMAC> CreateHashMacAsync(
            this HashAlgorithmName hashType,
            byte[] key)
    {
        return await Task.Run(() =>
        {
            if (hashType == HashAlgorithmName.SHA1)
            {
                return (HMAC)(new HMACSHA1(key));
            }
            if (hashType == HashAlgorithmName.SHA256)
            {
                return (HMAC)(new HMACSHA256(key));
            }
            if (hashType == HashAlgorithmName.SHA384)
            {
                return (HMAC)(new HMACSHA384(key));
            }
            if (hashType == HashAlgorithmName.SHA512)
            {
                return (HMAC)(new HMACSHA512(key));
            }
            return (HMAC)(new HMACMD5(key));
        });
    }

    /// <summary>
    /// Hash mac a object.
    /// </summary>
    /// <param name="dataObject">Object data.</param>
    /// <param name="hashType">Hash type.</param>
    /// <param name="key">Key for hash.</param>
    /// <typeparam name="TData">Data type of object.</typeparam>
    /// <returns>Hash mac result.</returns>
    public static async Task<string> HashMacAsync<TData>(
        this TData dataObject,
        string key,
        HashAlgorithmName hashType)
    {
        string result = string.Empty;
        if (dataObject == null || !string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(
                nameof(HashMacAsync),
                $"Please check argument [dataObject] or [key], value is empty.");
        }
        try
        {
            var keyBytes = Encoding.Unicode.GetBytes(key);
            using (var sha = await hashType.CreateHashMacAsync(keyBytes))
            {
                if (sha == null)
                {
                    return result;
                }
                string data = JsonSerializer.Serialize(dataObject);
                var dataBytes = Encoding.Unicode.GetBytes(data);
                byte[] hashValue = sha.ComputeHash(dataBytes);
                result = Convert.ToBase64String(hashValue);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return result ?? string.Empty;
    }
}