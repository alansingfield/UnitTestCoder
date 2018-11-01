namespace UnitTestCoder.Core.Coder
{
    public interface IObjectLiteralCoder
    {
        string Code(object arg, string lvalue);
    }
}