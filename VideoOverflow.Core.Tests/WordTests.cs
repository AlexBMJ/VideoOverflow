using VideoOverflow.Core.Utils;
using Xunit;

namespace VideoOverflow.Core.Tests;

public class WordTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("thisisatest")]
    [InlineData("bruhlmaoxdhaha")]
    [InlineData("softwareengineering")]
    public void SeparateWords_GivenLowercaseString_ReturnsOriginalString(string str) {
        // Arrange & Act
        var separated = Word.SeparateWords(str);
        // Assert
        Assert.Equal(str, separated);
    }
    
    [Theory]
    [InlineData("SoftwareEngineering", "Software Engineering")]
    [InlineData("ThisIsATest", "This Is A Test")]
    [InlineData("AAA", "A A A")]
    public void SeparateWords_GivenPascalCaseWords_ReturnsWordsSeparatedBySpace(string str, string expected) {
        // Arrange & Act
        var separated = Word.SeparateWords(str);
        // Assert
        Assert.Equal(expected, separated);
    }
}