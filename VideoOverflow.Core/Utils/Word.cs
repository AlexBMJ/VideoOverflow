using System.Text.RegularExpressions;

namespace VideoOverflow.Core.Utils;

public static class Word
{
    /// <summary>
    /// Separates words on upper case letters
    /// </summary>
    /// <param name="str">String to separate</param>
    /// <returns>Separated string</returns>
    public static string SeparateWords(string str) 
        => string.Join(' ', Regex.Split(str, @"(?<!^)(?=[A-Z])"));
}