# Compilation Errors - Fix Guide

## ✅ Fixed Issues

### 1. Missing Using Directives
- ✅ Added `System.Collections.Generic` for `IDictionary<,>`
- ✅ Added `MediaPortal.Common.PluginManager` for `IPluginStateTracker` and `IPluginRuntime`

### 2. Interface Implementations
- ✅ Added `ModelId` property to `IWorkflowModel` implementations
- ✅ Added `CanEnterState()` method to `IWorkflowModel` implementations
- ✅ Fixed `UpdateMenuActions()` method name (was `UpdateMenuItems`)
- ✅ Added `Name`, `State`, `MediaItemTitle` properties to `IPlayer`
- ✅ Fixed `IPlayerBuilder.GetPlayer()` method name (was `CreatePlayer`)

### 3. Type Fixes
- ✅ Changed `PluginRuntime` to `IPluginRuntime`
- ✅ Changed namespace from `MediaPortal.UI.Presentation.Players` to `MediaPortal.UI.Players` for `IPlayer`

### 4. WPF References
- ✅ Added `PresentationFramework`, `PresentationCore`, `WindowsBase`, `System.Xaml` references

### 5. Removed Invalid Namespaces
- ✅ Removed `MediaPortal.Common.MediaManagement.MediaQueries` (doesn't exist)
- ✅ Removed `MediaPortal.Common.ResourceAccess.ResourceProviders` (doesn't exist)

---

## ⚠️ Remaining Issues to Check

### PlayerState Enum
The `PlayerState` enum should be in `MediaPortal.UI.Players` namespace. If it's not found, you may need to:

1. Check if it exists in MediaPortal 2 DLLs
2. Or define it locally if missing

### WebView2.Wpf Namespace
The `Microsoft.Web.WebView2.Wpf` namespace comes from the `Microsoft.Web.WebView2` package. Make sure:
- NuGet package is restored
- Package is properly referenced

---

## 🔍 Verification Steps

1. **Build the project** - Check if errors are resolved
2. **Check Output window** - Look for specific error messages
3. **Verify references** - Make sure all DLLs are found

---

## 📝 If Errors Persist

### Check MediaPortal 2 DLLs
Make sure these DLLs exist at `$(MediaPortal2Path)`:
- `MediaPortal.Common.dll`
- `MediaPortal.UI.dll`
- `MediaPortal.Utilities.dll`
- `MediaPortal.UI.SkinEngine.dll`

### Check NuGet Package
Make sure `Microsoft.Web.WebView2` package is restored:
- Right-click solution → Restore NuGet Packages
- Check `packages` folder or `obj` folder for restored packages

---

**Most errors should be fixed now!** Try building again. 🚀
