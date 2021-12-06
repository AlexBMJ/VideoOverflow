using System;
using System.IO;

namespace Server; 

public class SpellChecker {
    public static void SpellCheck() {
        //create object
        int initialCapacity = 82765;
        int maxEditDistanceDictionary = 2; //maximum edit distance per dictionary precalculation
        var symSpell = new SymSpell(initialCapacity, maxEditDistanceDictionary);

        //load dictionary
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
        string dictionaryPath = Path.Combine(baseDirectory, @"Resources/frequency_dictionary_en_82_765.txt");
        Console.WriteLine(dictionaryPath);

        if (!symSpell.LoadDictionary(dictionaryPath, 0, 1) || !symSpell.LoadBigramDictionary(dictionaryPath, 0, 2)) {
            Console.WriteLine("File not found!");
            //press any key to exit program
            return;
        }

        //lookup suggestions for multi-word input strings (supports compound splitting & merging)
        string inputTerm = "run c++ in docler of a terninal";
        int maxEditDistanceLookup = 2; //max edit distance per lookup (per single word, not per whole input string)
        var suggestions = symSpell.LookupCompound(inputTerm, maxEditDistanceLookup);

        //display suggestions, edit distance and term frequency
        foreach (var suggestion in suggestions) {
            Console.WriteLine(suggestion.term + " " + suggestion.distance.ToString() + " " +
                              suggestion.count.ToString("N0"));
        }
    }
}