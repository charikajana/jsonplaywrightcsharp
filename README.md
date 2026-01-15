# Hybrid JSON-Driven & Traditional Playwright Excellence (C#)

![Playwright](https://img.shields.io/badge/Playwright-1.57.0-green)
![.NET](https://img.shields.io/badge/.NET-10.0-blue)
![Reqnroll](https://img.shields.io/badge/Reqnroll-2.2.1-orange)
![Allure](https://img.shields.io/badge/Allure-2.13.1-blue)

## 🚀 Framework Architecture
This is a **High-Performance Hybrid Framework** designed for enterprise-grade automation. It provides a "JSON-First, Code-Second" approach, merging the simplicity of No-Code with the power of professional C# development.

### 🎭 Two Pillars of Execution
1.  **JSON Engine (Primary)**: Execute steps without writing a single line of C#. Just define a JSON file matching your Gherkin step text in the `LocatorRepository`.
2.  **Traditional Fallback (Pro)**: When complex logic is needed, the framework automatically falls back to traditional C# step definitions using the `[TraditionalStep]` attribute.

---

## 🛠️ Centralized Command Center: `WebActions`
At the heart of the framework is the **`WebActions`** utility. Every interaction—whether triggered by a JSON file or a C# step—flows through this centralized class.
- **SmartWait Integration**: Automatically waits for elements to be stable, visible, and interactive.
- **Unified Logging**: Consistent, color-coded logging for every action.
- **Auto-Allure Screenshots**: Manual and failure screenshots automatically attach to Allure reports.

---

## 📂 Project Structure
```
PlaywrightJsonFramework/
├── PlaywrightJsonFramework.Core/          # Central Logic & Utilities
│   ├── Utils/
│   │   ├── WebActions.cs                  # The Action Engine
│   │   ├── SmartWait.cs                   # Intelligent Waiting logic
│   │   └── Logger.cs                      # Professional Console/File Logging
│   ├── Executor/
│   │   ├── JsonEnhancedExecutor.cs        # Orchestrates JSON Steps
│   │   └── Interaction/Verification Handlers
│
└── PlaywrightJsonFramework.Tests/         # Test Project
    ├── Features/                          # Gherkin Files
    ├── LocatorRepository/                 # JSON Step Files
    ├── StepDefinitions/
    │   ├── UniversalStepDefinition.cs     # The Hybrid Dispatcher
    │   ├── BaseStepDefinition.cs          # Powerful Base for C# Steps
    │   └── TraditionalStepDefinitions.cs  # Your custom C# code
```

---

## 📖 Action Toolkit: Sample Usage
The framework supports a massive suite of actions. Here is how to use them in both **JSON** and **Traditional C#**.

### 1. Navigation & Basic Interaction
| Action | JSON Type | C# Method (`BaseStepDefinition`) |
| :--- | :--- | :--- |
| **Navigate** | `NAVIGATE` | `await Navigate("url");` |
| **Click** | `CLICK` | `await Click("#selector", "Login Button");` |
| **Type** | `TYPE` | `await Type("#input", "Value");` |
| **Clear** | `CLEAR` | `await Clear("#input");` |

### 2. Advanced Interactions
| Action | JSON Type | C# Method |
| :--- | :--- | :--- |
| **Drag & Drop** | `DRAG_AND_DROP` | `await DragAndDrop("#source", "#target");` |
| **Scroll To** | `SCROLL_TO` | `await ScrollTo("#footer");` |
| **Right Click** | `RIGHT_CLICK` | `await RightClick(".menu");` |
| **Hover** | `HOVER` | `await Hover(".tooltip-trigger");` |
| **Upload File** | `UPLOAD_FILE` | `await UploadFile("#input", "C:\\file.pdf");` |

### 3. Verification & Validation
| Action | JSON Type | C# Method |
| :--- | :--- | :--- |
| **Verify Text** | `VERIFY_TEXT` | `await VerifyText("#header", "Welcome");` |
| **Verify Visibility**| `VERIFY_ELEMENT` | `await VerifyVisible(".success-msg");` |
| **Verify Attribute**| `VERIFY_ATTRIBUTE`| `await VerifyAttribute("#img", "src", "logo.png");` |
| **Verify CSS** | `VERIFY_CSS` | `await VerifyCss("#btn", "color", "rgb(0,0,0)");` |

### 4. Pro-Level Utilities
| Action | JSON Type | C# Method |
| :--- | :--- | :--- |
| **JS Evaluate** | `JS_EVALUATE` | `await JsEvaluate("window.scrollTo(0,0)");` |
| **Screenshot** | `SCREENSHOT` | `await TakeScreenshot("Manual_Step");` |
| **Switch Window** | *(Auto in JSON)* | `await SwitchToWindow("Google");` |
| **Wait for Popup** | *(Auto in JSON)* | `await WaitForPopup(async () => { ... });` |
| **Network Idle** | `WAIT_STABLE` | `await WaitForNetworkIdle();` |

---

## 📝 Writing a Traditional Step
If you need to write custom C# code, simply inherit from `BaseStepDefinition` and use the `[TraditionalStep]` attribute.

```csharp
[Binding]
public class MySteps : BaseStepDefinition
{
    public MySteps(ScenarioContext context) : base(context) { }

    [TraditionalStep(@"User performs advanced logic")]
    public async Task WhenUserPerformsAdvancedLogic()
    {
        await Navigate("https://example.com");
        await HandleDialog("accept"); // Auto-handle next popup
        
        // Wait for a new tab
        var newTab = await WaitForNewWindow(async () => {
            await Click("#open-tab-btn");
        });
        
        await TakeScreenshot("New_Tab_Opened");
        await CloseWindow();
    }
}
```

---

## 📊 Professional Reporting (Allure)
The framework is fully integrated with **Allure Report 3**:
- **Dynamic Env Info**: Automatically captures OS, Browser, Environment, and Timeouts.
- **Fail-Safe Artifacts**: Automatically attaches **Screenshots** and **Full Video Recordings** on test failure.
- **Detailed Logs**: Every interaction (Click, Type, Wait) is logged with high-precision timestamps.

To view reports:
```powershell
dotnet test
allure serve allure-results
```

---

## 🌟 Why Use This Framework?
- ✅ **90% No-Code**: Most engineers never need to touch C#.
- ✅ **Professional Debugging**: Rich, color-coded logging.
- ✅ **Extreme Stability**: Centralized `SmartWait` logic kills flakiness.
- ✅ **Easy Evolution**: Start with JSON, add C# only when necessary.

**Made with ❤️ for High-Speed QA Teams**

