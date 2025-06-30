using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Xml;

[System.Serializable]
public struct WeaponInfo
{
    public string weaponID;
    public int durability;
}

public class SerializationAndEncryption : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] bool serialize;
    [SerializeField] bool usingXML;
    [SerializeField] bool encrypt;

    void Start()
    {
        WeaponInfo createdWeaponInfo = new WeaponInfo();
        createdWeaponInfo.weaponID = "Dirty Knife";
        createdWeaponInfo.durability = 5;

        text.text = createdWeaponInfo.ToString();
        Debug.Log("Weapon ID: " + createdWeaponInfo.weaponID);

        //////////////////////////////////////////////////////////////
        // Let's first serialize and encrypt....
        //////////////////////////////////////////////////////////////

        if (serialize)
        {
            if (usingXML)
                text.text = Utils.SerializeXML<WeaponInfo>(createdWeaponInfo);
            else
                text.text = JsonUtility.ToJson(createdWeaponInfo);

            string serialized = text.text;
            Debug.Log("Serialized: " + serialized);
        }

        if (encrypt)
        {
            text.text = Utils.EncryptAES(text.text);

            string encrypted = text.text;
            Debug.Log("Encrypted: " + encrypted);
        }

        //////////////////////////////////////////////////////////////
        // Now let's de-serialize and de-encrypt....
        //////////////////////////////////////////////////////////////

        string stringData = text.text;
        if (encrypt)
        {
            stringData = Utils.DecryptAES(stringData);
            Debug.Log("Decrypted: " + stringData);
        }

        WeaponInfo derivedWeaponInfo = new WeaponInfo();
        if (serialize)
        {
            if (usingXML)
                derivedWeaponInfo = Utils.DeserializeXML<WeaponInfo>(stringData);
            else
                derivedWeaponInfo = JsonUtility.FromJson<WeaponInfo>(stringData);

            Debug.Log("Deserialized: " + derivedWeaponInfo.weaponID);
        }
    }
}

public static class Utils
{
    public static string SerializeXML<T>(System.Object inputData)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (var sww = new StringWriter())
        {
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                serializer.Serialize(writer, inputData);
                return sww.ToString();
            }
        }
    }

    public static T DeserializeXML<T>(string data)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (var sww = new StringReader(data))
        {
            using (XmlReader reader = XmlReader.Create(sww))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }

    static byte[] ivBytes = new byte[16]; // Generate the iv randomly and send it along with the data, to later parse out
    static byte[] keyBytes = new byte[16]; // Generate the key using a deterministic algorithm rather than storing here as a variable

    static void GenerateIVBytes()
    {
        System.Random rnd = new System.Random();
        rnd.NextBytes(ivBytes);
    }

    const string nameOfGame = "The Game of Life";
    static void GenerateKeyBytes()
    {
        int sum = 0;
        foreach (char curChar in nameOfGame)
            sum += curChar;

        System.Random rnd = new System.Random(sum);
        rnd.NextBytes(keyBytes);
    }

    public static string EncryptAES(string data)
    {
        GenerateIVBytes();
        GenerateKeyBytes();

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateEncryptor(keyBytes, ivBytes);
        byte[] inputBuffer = Encoding.Unicode.GetBytes(data);
        byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

        string ivString = Encoding.Unicode.GetString(ivBytes);
        string encryptedString = Convert.ToBase64String(outputBuffer);

        return ivString + encryptedString;
    }

    public static string DecryptAES(this string text)
    {
        GenerateIVBytes();
        GenerateKeyBytes();

        int endOfIVBytes = ivBytes.Length / 2;  // Half length because unicode characters are 64-bit width

        string ivString = text.Substring(0, endOfIVBytes);
        byte[] extractedivBytes = Encoding.Unicode.GetBytes(ivString);

        string encryptedString = text.Substring(endOfIVBytes);

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes, extractedivBytes);
        byte[] inputBuffer = Convert.FromBase64String(encryptedString);
        byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

        string decryptedString = Encoding.Unicode.GetString(outputBuffer);

        return decryptedString;
    }
}
