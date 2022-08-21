using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Binaron.Serializer;

namespace StrausTech.CommonLib;

public static class ExtensionMethods
{
    /// <summary>
    /// Converts Object to Double
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double ToDouble(this string text)
    {
        double.TryParse(text, out double result);
        return result;
    }

    /// <summary>
    /// Converts Object to Double
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int ToInt(this string text)
    {
        int.TryParse(text, out int result);
        return result;
    }

    public static string PadRightEx(this string str, int padding, char paddingChar = ' ')
    {
        Encoding coding = Encoding.UTF8;
        int dcount = 0;
        foreach (char ch in str.ToCharArray())
        {
            var t = coding.GetByteCount(ch.ToString());
            
            if (coding.GetByteCount(ch.ToString()) >= 2)
                dcount++;
        }
        string w = str.PadRight(padding - dcount, paddingChar);
        return w;
    }

    public static bool IsNullOrEmpty(this string? text)
    {
        return string.IsNullOrEmpty(text);
    }

    public static string SerializeToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public static T? DeserializeToObject<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public static T? FromByteArray<T>(this byte[] byteArray)
    {
        if (byteArray == null)
            return default(T);

        using (var stream = new MemoryStream(byteArray))
        {
            return BinaronConvert.Deserialize<T>(stream);
        }
    }

    public static byte[] ToByteArray<T>(this T obj)
    {
        if (obj == null)
            return new byte[0];

        using (var stream = new MemoryStream())
        {
            BinaronConvert.Serialize(obj, stream);
            return stream.ToArray();
        }
    }

    public static bool HasValue<T>(this object? obj)
    {
        return obj != null && obj != default;
    }
}