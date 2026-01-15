# Copilot Instructions: Hybrid Framework JSON Generation

This document provides strict instructions for generating "Strong JSON" locator files. These files are the lifeblood of the Hybrid Playwright Framework's No-Code engine.

## 📂 1. Step Pre-Processing Rules

### A. Normalization (The `_param_` rule)
- **Format**: All lowercase, spaces replaced by underscores.
- **Param Strategy**: Replace specific data values (names, quoted strings, numbers) with `_param_`.
- **Example**: `User enters "admin" and "password123"` -> `user_enters_param_and_param.json`

### B. File Location
- **Path**: Always save to `PlaywrightJsonFramework.Tests/LocatorRepository/`.

---

## 🏗️ 2. The "Strong" JSON Schema (Mandatory Root Fields)

Every JSON file must have this top-level metadata:

```json
{
  "stepFileName": "normalized_name_param",
  "gherkinStep": "The original full Gherkin step text",
  "normalizedStep": "normalized_name_param",
  "stepType": "Given/When/Then",
  "stepNumber": 1,
  "status": "passed",
  "actions": [ ... ],
  "metadata": {
    "createdDate": "2026-01-15T12:00:00",
    "totalActions": 1
  }
}
```

---

## 🧬 3. The 17-Attribute Element DNA

For every element in the `actions` array, populate these 17 attributes. **Never use empty strings `""`**; use `null` if the attribute is missing on the element.

| ID | Attribute | Mandatory Format |
| :--- | :--- | :--- |
| 1-3 | `type`, `id`, `name` | Core HTML attributes |
| 4-6 | `selector`, `cssSelector`, `xpath` | `xpath` MUST be a **Relative XPath** |
| 7-9 | `text`, `placeholder`, `dataTest` | Content & QA-specific attributes |
| 10-13 | `ariaLabel`, `role`, `title`, `alt` | Accessibility & UX attributes |
| 14-17 | `className`, `value`, `href`, `src` | Styling & Functional attributes |

### DNA Fingerprinting
Every element must also include a `fingerprint` object:
- `attributes`: Object containing key-value pairs like `{ "type": "submit", "role": "button" }`.
- `context`: Object including `nearbyText` (nearest label/text), `parentTag`, and `heading`.

---

## 🛠️ 4. Action Type Mapping

| Intent | ActionType | Required Data |
| :--- | :--- | :--- |
| Go to Page | `NAVIGATE` | `value`: The full URL |
| Click | `CLICK` | `element` block |
| Enter Text | `TYPE` | `value`: `___RUNTIME_PARAMETER___`, `element` block |
| Clear Field | `CLEAR` | `element` block |
| Tab to Popup | `CLICK_AND_SWITCH` | `element` block (The link/button) |
| Multi-intent | (Sequence) | Action 1: `CLEAR`, Action 2: `TYPE` |
| Verification | `VERIFY_TEXT` | `value`: `___RUNTIME_PARAMETER___`, `element` block |
| Visibility | `VERIFY_ELEMENT` | `element` block |
| Gone/Hidden | `VERIFY_NOT_VISIBLE`| `element` block |
| Stability | `WAIT_STABLE` | No element required |

---

## 🚫 5. Critical Prohibitions (THE "DON'Ts")

1.  **NO Guesswork**: Do not guess locators. If you don't have access to the live DOM, ask for a screenshot or DOM snippet.
2.  **NO Absolute XPaths**: Use relative XPaths anchored to IDs or Unique labels (e.g., `//form[@id='login']//input[@name='user']`).
3.  **NO Empty Strings**: Use `null` for missing attributes.
4.  **NO Duplication**: Check if the normalized step file already exists before creating a new one.

---

## 💡 Pro-Tip for Parameterization
When the Gherkin step says: `User verifies title is "Login Success"`:
- `normalizedStep`: `user_verifies_title_is_param`
- `actionType`: `VERIFY_TEXT`
- `value`: `___RUNTIME_PARAMETER___`
- `element`: Capture metadata for the Heading/Title element.
