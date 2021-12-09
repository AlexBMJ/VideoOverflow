using System.Collections.Generic;
using System.IO;

namespace VideoOverflow.Server.Tests; 

public class SpellCheckerTests {
    [Theory]
    [InlineData("I'm spell checking this string with stuff","i'm spell checking this string with stuff")]
    [InlineData("gonna meke some spleling misakesnow", "gonna make some spelling mistakes now")]
    [InlineData("howto write dockercomposse in yaml", "how to write docker compose in yaml")]
    [InlineData("Declar varible in C#", "declare variable in c#")]
    public void SpellChecker_given_misspelled_string_return_properly_spelled_suggestion(string input, string output) {
        var spellChecker = new SpellChecker();
        string spellchecked = spellChecker.SpellCheck(input);
        Assert.Equal(spellchecked, output);
    }
    
    [Fact]
    public void SpellChecker_given_invalid_dict_throw_exception() {
        Assert.Throws<FileNotFoundException>(() => new SpellChecker(2, "wrong_file.txt"));
    }
}