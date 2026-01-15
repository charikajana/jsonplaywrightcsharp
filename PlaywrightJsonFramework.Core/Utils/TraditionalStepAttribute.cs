namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Custom attribute to mark methods as traditional fallback steps
/// Use this instead of Reqnroll's [Given/When/Then] to avoid ambiguity errors
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TraditionalStepAttribute : Attribute
{
    public string Regex { get; }

    public TraditionalStepAttribute(string regex)
    {
        Regex = regex;
    }
}
