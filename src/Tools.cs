using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenDatabaseAPI;

public class Tools
{
    
    /// <summary>
    /// Combines the provided strings on the given delimiter.
    /// </summary>
    /// <param name="strings"> Strings to combine. </param>
    /// <param name="delimiter"> Delimiter to combine on. </param>
    /// <returns> Combined string. </returns>
    public static string CombineStrings(string[] strings, string delimiter)
    {
        StringBuilder stringBuilder = new StringBuilder();

        for (int x = 0; x < strings.Length - 1; x++)
        {
            stringBuilder.Append(strings[x]);
            stringBuilder.Append(delimiter);
        }

        stringBuilder.Append(strings[strings.Length - 1]);    
        
        return stringBuilder.ToString();
    }

    public static T[] EliminateDuplicates<T>(T[] array)
    {
        Dictionary<T, T> objectMap = new Dictionary<T, T>();

        List<T> uniqueValues = new List<T>();
        
        for (int x = 0; x < array.Length; x++)
            if (!objectMap.ContainsKey(array[x]))
            {
                uniqueValues.Add(array[x]);
                objectMap.Add(array[x], array[x]);
            }
        
        return uniqueValues.ToArray();
    } 
    
} 
