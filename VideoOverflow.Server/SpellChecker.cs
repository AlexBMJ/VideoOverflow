global using System.IO;
using System.Runtime.CompilerServices;

namespace Server; 

public class SpellChecker {
    private readonly SymSpell _symSpell;
    private readonly int _maxEditDistanceDictionary;

    public SpellChecker(int maxEditDistance = 2, string dictionary = "frequency_dictionary.txt") {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string dictionaryPath = Path.Combine(baseDirectory, $@"Resources/{dictionary}");
        if (!File.Exists(dictionaryPath)) {
            throw new FileNotFoundException($"Failed to load dictionary: {dictionaryPath}");
        }

        var lineCount = File.ReadLines(dictionaryPath).Count();

        int initialCapacity = lineCount;
        _maxEditDistanceDictionary = maxEditDistance; //maximum edit distance per dictionary precalculation
        _symSpell = new SymSpell(initialCapacity, _maxEditDistanceDictionary);

        _symSpell.LoadDictionary(dictionaryPath, 0, 1);
        _symSpell.LoadBigramDictionary(dictionaryPath, 0, 1);
    }

    public string SpellCheck(string inputText) {
        var suggestions = _symSpell.LookupCompound(inputText, _maxEditDistanceDictionary);
        return suggestions.Count > 0 ? suggestions.First().term : inputText;
    }
}