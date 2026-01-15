using Microsoft.Playwright;
using PlaywrightJsonFramework.Core.Data;
using PlaywrightJsonFramework.Core.Utils;

namespace PlaywrightJsonFramework.Core.Healing;

/// <summary>
/// AI-powered element recovery using fingerprint DNA
/// Implements 4 healing strategies in cascading order
/// </summary>
public static class SelfHealingEngine
{
    private const string COMPONENT = "HEALING";

    /// <summary>
    /// Attempt to heal element using fingerprint DNA
    /// Returns healed locator and new selector if successful
    /// </summary>
    public static async Task<HealedResult?> AttemptHealing(
        IPage page,
        ElementLocators locators)
    {
        if (locators.Fingerprint == null)
        {
            Logger.Error("No fingerprint data available for healing", COMPONENT);
            return null;
        }

        Logger.NewLine();
        Logger.Separator('=');
        Logger.Info("INITIATING SELF-HEALING ENGINE", COMPONENT);
        Logger.Separator('=');

        // Strategy 1: Label Healing
        var labelResult = await TryLabelHeal(page, locators.Fingerprint);
        if (labelResult != null)
        {
            Logger.Success("HEALED using Label Strategy", COMPONENT);
            return labelResult;
        }

        // Strategy 2: Semantic Healing
        var semanticResult = await TrySemanticHeal(page, locators.Fingerprint);
        if (semanticResult != null)
        {
            Logger.Success("HEALED using Semantic Strategy", COMPONENT);
            return semanticResult;
        }

        // Strategy 3: Proximity Healing
        var proximityResult = await TryProximityHeal(page, locators.Fingerprint);
        if (proximityResult != null)
        {
            Logger.Success("HEALED using Proximity Strategy", COMPONENT);
            return proximityResult;
        }

        // Strategy 4: Fuzzy Attribute Healing
        var fuzzyResult = await TryFuzzyAttributeHeal(page, locators.Fingerprint);
        if (fuzzyResult != null)
        {
            Logger.Success("HEALED using Fuzzy Attribute Strategy", COMPONENT);
            return fuzzyResult;
        }

        Logger.Error("All healing strategies failed", COMPONENT);
        Logger.Separator('=');
        Logger.NewLine();

        return null;
    }

    /// <summary>
    /// Strategy 1: Use getByLabel with nearbyText
    /// </summary>
    private static async Task<HealedResult?> TryLabelHeal(IPage page, FingerprintData fingerprint)
    {
        try
        {
            var nearbyText = fingerprint.Context?.NearbyText;
            if (string.IsNullOrWhiteSpace(nearbyText))
                return null;

            Logger.Info($"Trying Label Heal with: '{nearbyText}'", COMPONENT);

            var locator = page.GetByLabel(nearbyText);
            var count = await locator.CountAsync();

            if (count > 0)
            {
                var selector = $"label:has-text('{nearbyText}')";
                return new HealedResult(locator.First, selector);
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Label Heal failed: {ex.Message}", COMPONENT);
        }

        return null;
    }

    /// <summary>
    /// Strategy 2: Use getByRole with ariaLabel
    /// </summary>
    private static async Task<HealedResult?> TrySemanticHeal(IPage page, FingerprintData fingerprint)
    {
        try
        {
            var role = fingerprint.Attributes?.Role;
            var ariaLabel = fingerprint.Attributes?.AriaLabel;

            if (string.IsNullOrWhiteSpace(role))
                return null;

            Logger.Info($"Trying Semantic Heal with role: '{role}', ariaLabel: '{ariaLabel}'", COMPONENT);

            ILocator locator;
            string selector;

            if (!string.IsNullOrWhiteSpace(ariaLabel))
            {
                locator = page.GetByRole(ParseAriaRole(role), new PageGetByRoleOptions
                {
                    Name = ariaLabel
                });
                selector = $"role={role}[name='{ariaLabel}']";
            }
            else
            {
                locator = page.GetByRole(ParseAriaRole(role));
                selector = $"role={role}";
            }

            var count = await locator.CountAsync();
            if (count > 0)
            {
                return new HealedResult(locator.First, selector);
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Semantic Heal failed: {ex.Message}", COMPONENT);
        }

        return null;
    }

    /// <summary>
    /// Strategy 3: Use :near() relative selector with nearbyText
    /// </summary>
    private static async Task<HealedResult?> TryProximityHeal(IPage page, FingerprintData fingerprint)
    {
        try
        {
            var nearbyText = fingerprint.Context?.NearbyText;
            var elementType = fingerprint.Attributes?.Type;

            if (string.IsNullOrWhiteSpace(nearbyText) || string.IsNullOrWhiteSpace(elementType))
                return null;

            Logger.Info($"Trying Proximity Heal near text: '{nearbyText}'", COMPONENT);

            // Try to find element near the text
            var escapedText = nearbyText.Replace("'", "\\'");
            var selector = $"{elementType}:near(:text('{escapedText}'))";

            var locator = page.Locator(selector);
            var count = await locator.CountAsync();

            if (count > 0)
            {
                return new HealedResult(locator.First, selector);
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Proximity Heal failed: {ex.Message}", COMPONENT);
        }

        return null;
    }

    /// <summary>
    /// Strategy 4: Match by unique class name
    /// </summary>
    private static async Task<HealedResult?> TryFuzzyAttributeHeal(IPage page, FingerprintData fingerprint)
    {
        try
        {
            var classList = fingerprint.Attributes?.ClassList;
            if (string.IsNullOrWhiteSpace(classList))
                return null;

            // Split classes and try each one
            var classes = classList.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var className in classes)
            {
                Logger.Info($"Trying Fuzzy Attribute Heal with class: '{className}'", COMPONENT);

                var selector = $".{className}";
                var locator = page.Locator(selector);
                var count = await locator.CountAsync();

                // Only use if unique or limited matches
                if (count > 0 && count <= 3)
                {
                    return new HealedResult(locator.First, selector);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Fuzzy Attribute Heal failed: {ex.Message}", COMPONENT);
        }

        return null;
    }

    /// <summary>
    /// Parse string to AriaRole enum
    /// </summary>
    private static AriaRole ParseAriaRole(string role)
    {
        return role.ToLowerInvariant() switch
        {
            "button" => AriaRole.Button,
            "textbox" => AriaRole.Textbox,
            "link" => AriaRole.Link,
            "heading" => AriaRole.Heading,
            "checkbox" => AriaRole.Checkbox,
            "radio" => AriaRole.Radio,
            "combobox" => AriaRole.Combobox,
            "listbox" => AriaRole.Listbox,
            "option" => AriaRole.Option,
            "img" => AriaRole.Img,
            _ => AriaRole.Button  // Default fallback
        };
    }
}

/// <summary>
/// Result of a successful healing operation
/// </summary>
public class HealedResult
{
    public ILocator Locator { get; }
    public string Selector { get; }

    public HealedResult(ILocator locator, string selector)
    {
        Locator = locator;
        Selector = selector;
    }
}
