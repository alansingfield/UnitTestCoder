# UnitTestCoder
Automatic generation of C# code for unit tests. Uses the excellent [Shouldly](https://github.com/shouldly/shouldly) library.

[![Build status](https://didsburysoftware.visualstudio.com/UnitTestCoder/_apis/build/status/UnitTestCoder-ci)](https://didsburysoftware.visualstudio.com/UnitTestCoder/_build/latest?definitionId=10)

https://www.nuget.org/packages/UnitTestCoder.Shouldly/

## What is it?

This module:
- Writes C# code for Shouldly tests by reading your test output
- Snapshots your test output into a C# literal, creating "canned" data for input into your next test.

## Why use UnitTestCoder?

- You can achieve 100% test coverage as it automatically tests every property of your object.
- It is far quicker to read/review a list of assertions than write them.
- By snapshotting the data from one test as the input to the next, you keep your tests independent.

## Generating a Shouldly test

Create a test like this:

``` C#
[TestMethod]
public void ReadmeExample()
{
    var result = new
    {
        X = 1 + 2,
        Y = "Hello " + "World"
    };

    ShouldlyTest.Gen(result, nameof(result));
}
```

Run the test, then copy the text from the Output window:

![ReadmeExample1.1](https://raw.githubusercontent.com/alansingfield/UnitTestCoder/master/img/ReadmeExample1.1.png)
![ReadmeExample1.2](https://raw.githubusercontent.com/alansingfield/UnitTestCoder/master/img/ReadmeExample1.2.png)
``` C#
{
    result.X.ShouldBe(3);
    result.Y.ShouldBe("Hello World");
}
```

Paste this back into your unit test, and comment out the ShouldlyTest.Gen call:

``` C#
[TestMethod]
public void ReadmeExample()
{
    var result = new
    {
        X = 1 + 2,
        Y = "Hello " + "World"
    };

    // ShouldlyTest.Gen(result, nameof(result));

    {
        result.X.ShouldBe(3);
        result.Y.ShouldBe("Hello World");
    }
}
```

Voila, you have a passing unit test with 100% coverage!

Obviously with two fields this is not worth the effort, but consider what happens if you have 40 fields or a complex, nested object model. UnitTestCoder will recursively scan your full object tree and generate test lines for every value.

## Object snapshot

UnitTestCoder will write the C# code required to instantiate an object, based on its runtime value when running the test.

This is particularly useful for where you want to pass the output of one test to the input of another. 

``` c#
public class MyItem
{
    public int Number { get; set; }
    public string Text { get; set; }
}

private MyItem create(int n) => new MyItem() { Number = n, Text = n.ToString() };

[TestMethod]
public void ObjectLiteralExample1()
{
    var item = create(20);

    ObjectLiteral.Gen(item, nameof(item));
}
```

![ObjectLiteralExample1](https://raw.githubusercontent.com/alansingfield/UnitTestCoder/master/img/ObjectLiteralExample1.png)
![ObjectLiteralExample2](https://raw.githubusercontent.com/alansingfield/UnitTestCoder/master/img/ObjectLiteralExample2.png)


Now we can take the output from the first test and hard-code it into the second test.

``` c#
private MyItem process(MyItem x) => new MyItem() { Number = x.Number + 1, Text = x.Text + "X" };

[TestMethod]
public void ObjectLiteralExample2()
{
    var item = new MyItem()
    {
        Number = 20,
        Text = "20",
    };

    var result = process(item);

    ShouldlyTest.Gen(result, nameof(result));

    {
        result.Number.ShouldBe(21);
        result.Text.ShouldBe("20X");
    }
}

```

The tests are now isolated, you can change the create() method tested in Example1 and it won't cause Example2 to fail.
