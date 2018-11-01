# UnitTestCoder
Automatic generation of C# code for unit tests

## What is it?

This module writes Shouldly unit tests for you.

Create a test like this:

``` C#
[TestMethod]
public void Test1()
{
    var result = new {
        X = 1 + 2,
        Y = "Hello " + "World"
    };
    
    UnitTestCoder.Gen(result, nameof(result));   
}
```

Run the test, then copy the text from the Output window:

``` C#
result.X.ShouldBe(3);
result.Y.ShouldBe("Hello World");
```

Paste this back into your unit test, and comment out the UnitTestCoder call:

``` C#
[TestMethod]
public void Test1()
{
    var result = new {
        X = 1 + 2,
        Y = "Hello " + "World"
    };
    
    //UnitTestCoder.Gen(result, nameof(result));  
    
    {
        result.X.ShouldBe(3);
        result.Y.ShouldBe("Hello World");
    }
}
```

Voila, you have a passing unit test with 100% coverage!

Obviously with two fields this is not worth the effort, but consider what happens if you have 40 fields or a complex, nested object model. UnitTestCoder will recursively scan your full object tree and generate test lines for every value.

