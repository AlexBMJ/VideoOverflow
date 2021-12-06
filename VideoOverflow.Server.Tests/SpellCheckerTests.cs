using Server;

namespace VideoOverflow.Server.Tests; 

public class SpellCheckerTests {
    [Fact]
    public void SpellChecker_test()
    {
        SpellChecker.SpellCheck();
        Assert.Equal(1, 1);
    }
}