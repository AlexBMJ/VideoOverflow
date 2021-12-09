global using System.IO;
namespace Server; 

public class SpellChecker {
    private readonly SymSpell _symSpell;
    private readonly int _maxEditDistanceDictionary;
    public SpellChecker(int maxEditDistance=2, string dictionary="frequency_dictionary.txt") {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
        string dictionaryPath = Path.Combine(baseDirectory, $@"Resources/{dictionary}");
        var lineCount = File.ReadLines(dictionaryPath).Count();
        
        int initialCapacity = lineCount;
        _maxEditDistanceDictionary = maxEditDistance; //maximum edit distance per dictionary precalculation
        _symSpell = new SymSpell(initialCapacity, _maxEditDistanceDictionary);

        if (!_symSpell.LoadDictionary(dictionaryPath, 0, 1) || !_symSpell.LoadBigramDictionary(dictionaryPath, 0, 2)) {
            throw new FileNotFoundException($"Failed to load dictionary: {dictionaryPath}");
        }
    }
    public string SpellCheck(string inputText) {
        var suggestions = _symSpell.LookupCompound(inputText, _maxEditDistanceDictionary);
        return suggestions.Count > 0 ? suggestions.First().term : inputText;
    }
}