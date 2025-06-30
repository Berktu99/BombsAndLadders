using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;

public static class Helpers 
{
    public static float mapping(float newFrom, float newTo, float oldFrom, float oldTo, float value)
    {
        if (value <= oldFrom)
            return newFrom;
        else if (value >= oldTo)
            return newTo;
        return (newTo - newFrom) * ((value - oldFrom) / (oldTo - oldFrom)) + newFrom;
    } 

    public static float performantDistance(Vector2 v1, Vector2 v2)
    {
        return Mathf.Pow((v1.x- v2.x), 2) + Mathf.Pow((v1.y- v2.y), 2);
    }

    public static float performantDistance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2) + Mathf.Pow((v1.z - v2.z) , 2);
    }

    public static bool performantMagnitudeCompare(Vector2 vector, float compareTo)
    {
        return Mathf.Pow((vector.x), 2) + Mathf.Pow((vector.y), 2) > Mathf.Pow(compareTo, 2);
    }

    public static Vector3 randomV3(this Vector3 v3)
    {
        Vector3 v = new Vector3(
            Random.Range(-1f, 1f),
             Random.Range(-1f, 1f),
              Random.Range(-1f, 1f));
        return v.normalized;
    }


    public static T[] FindComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool forceActive = false) where T : Component
    {
        if (parent == null) { throw new System.ArgumentNullException(); }
        if (string.IsNullOrEmpty(tag) == true) { throw new System.ArgumentNullException(); }
        List<T> list = new List<T>(parent.GetComponentsInChildren<T>(forceActive));
        if (list.Count == 0) { return null; }

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].CompareTag(tag) == false)
            {
                list.RemoveAt(i);
            }
        }
        return list.ToArray();
    }

    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            foreach (Transform transform in tr)
            {

            }
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }

    public static T FindChildWithTagBreadthFirst<T>(this GameObject aParent, string aTag) where T : Component
    {
        // Queue is First In First Out (FIFO)
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent.transform);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.CompareTag(aTag))
                return c.GetComponent<T>();
            foreach (Transform t in c)
                queue.Enqueue(t);
        }

        return null;
    }

    public static Transform FindChildWithNameBreadthFirst(this Transform aParent, string aName)
    {
        // Queue is First In First Out (FIFO)
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static Transform FindChildWithNameDepthFirst(this Transform aParent, string aName)
    {
        foreach (Transform child in aParent)
        {
            if (child.name == aName)
                return child;
            var result = child.FindChildWithNameDepthFirst(aName);
            if (result != null)
                return result;
        }
        return null;
    }
}
public static class SerializeHelper
{
    public static string serializeToXML<T>(this T toSerialize)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();

        xml.Serialize(writer, toSerialize);

        return writer.ToString();
    }

    public static T deserializeFromXML<T>(this string toDeSerialize)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(toDeSerialize);

        return (T)xml.Deserialize(reader);
    }

    public static void serializeToJson<T>(this T toSerialize, string path)
    {
        string potion = JsonUtility.ToJson(toSerialize);
        System.IO.File.WriteAllText(path, potion);
    }

    public static T deserializeFromJson<T>(this string toDeSerialize, string path)
    {
        T potion = JsonUtility.FromJson<T>(toDeSerialize);
        System.IO.File.ReadAllText(path);

        return potion;
    }

    public static void serializeToJsonSafe<T>(this T toSerialize, string path)
    {
        string json = JsonUtility.ToJson(toSerialize);
        string encrypted = Encryption.EncryptAES(json);
        System.IO.File.WriteAllText(path, encrypted);
    }

    public static T deserializeFromJsonSafe<T>(this string path)
    {
        string decryptedJson = Encryption.DecryptAES(File.ReadAllText(path));
        T t = JsonUtility.FromJson<T>(decryptedJson);
        return t;
    }
}

public static class Encryption
{
    static byte[] ivBytes = new byte[16]; // Generate the iv randomly and send it along with the data, to later parse out
    static byte[] keyBytes = new byte[16]; // Generate the key using a deterministic algorithm rather than storing here as a variable

    static void GenerateIVBytes()
    {
        System.Random rnd = new System.Random();
        rnd.NextBytes(ivBytes);
    }

    //const string nameOfGame = "The Game of Life";
    private static string idleAnimationHash = "9ai!,a^1gi?*_$1NSwz";
    static void GenerateKeyBytes()
    {
        int sum = 0;
        foreach (char curChar in idleAnimationHash)
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
        byte[] inputBuffer = System.Text.Encoding.Unicode.GetBytes(data);
        byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

        string ivString = System.Text.Encoding.Unicode.GetString(ivBytes);
        string encryptedString = System.Convert.ToBase64String(outputBuffer);

        return ivString + encryptedString;
    }

    public static string DecryptAES(this string text)
    {
        GenerateIVBytes();
        GenerateKeyBytes();

        int endOfIVBytes = ivBytes.Length / 2;  // Half length because unicode characters are 64-bit width

        string ivString = text.Substring(0, endOfIVBytes);
        byte[] extractedivBytes = System.Text.Encoding.Unicode.GetBytes(ivString);

        string encryptedString = text.Substring(endOfIVBytes);

        SymmetricAlgorithm algorithm = Aes.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes, extractedivBytes);
        byte[] inputBuffer = System.Convert.FromBase64String(encryptedString);
        byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

        string decryptedString = System.Text.Encoding.Unicode.GetString(outputBuffer);

        return decryptedString;
    }
}

public static class ShuffleExtension
{
    //shuffle arrays:
    public static void Shuffle<T>(this T[] array, int shuffleAccuracy)
    {
        for (int i = 0; i < shuffleAccuracy; i++)
        {
            int randomIndex = Random.Range(0, array.Length);

            T temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }
    //shuffle lists:
    public static void Shuffle<T>(this List<T> list, int shuffleAccuracy)
    {
        for (int i = 0; i < shuffleAccuracy; i++)
        {
            int randomIndex = Random.Range(0, list.Count);

            T temp = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = temp;
        }
    }
}

public static class RectTransformExtensions
{

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void changeRectTransform(this RectTransform rt, float originalTop, float originalBottom, float originalLeft, float originalRight)
    {
        rt.SetTop(originalTop);
        rt.SetBottom(originalBottom);
        rt.SetLeft(originalLeft);
        rt.SetRight(originalRight);
    }    
}
