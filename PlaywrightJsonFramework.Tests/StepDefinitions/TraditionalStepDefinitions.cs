using PlaywrightJsonFramework.Core.Utils;
using Reqnroll;

namespace PlaywrightJsonFramework.Tests.StepDefinitions;

/// <summary>
/// Practical implementation of traditional steps using the BaseStepDefinition
/// </summary>
public class TraditionalStepDefinitions : BaseStepDefinition
{
    public TraditionalStepDefinitions(ScenarioContext context) : base(context)
    {
    }

    [TraditionalStep(@"User clicks login button")]
    public async Task WhenUserClicksLoginButton()
    {
        // Now you can use the pre-defined functions from Base class
        await Click("button[type='submit']", "Login Button");
    }
}
