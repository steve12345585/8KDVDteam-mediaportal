# Fix: "Failed to restore" Error

## ✅ Quick Fix Applied

I've set the `MediaPortal2Path` environment variable for you.

**Path set to:** `C:\Program Files (x86)\Team MediaPortal\MP2-Client`

---

## 🔄 Next Steps

### 1. **Restart Visual Studio**
   - Close Visual Studio completely
   - Reopen the project
   - The environment variable needs a restart to take effect

### 2. **Restore NuGet Packages Again**
   - Right-click solution → **Restore NuGet Packages**
   - Or: **Tools** → **NuGet Package Manager** → **Restore NuGet Packages**

### 3. **Verify It Works**
   - Check the Output window
   - Should see: "Restore succeeded" or similar

---

## 🔍 If It Still Fails

### Check Environment Variable
In PowerShell:
```powershell
[System.Environment]::GetEnvironmentVariable('MediaPortal2Path', 'User')
```

Should show: `C:\Program Files (x86)\Team MediaPortal\MP2-Client`

### Verify DLLs Exist
```powershell
Test-Path "C:\Program Files (x86)\Team MediaPortal\MP2-Client\MediaPortal.Common.dll"
```

Should return: `True`

---

## 📝 Alternative: Edit Project File Directly

If environment variable still doesn't work, you can hardcode the path in the `.csproj` file:

Replace all instances of:
```xml
<HintPath>$(MediaPortal2Path)\MediaPortal.Common.dll</HintPath>
```

With:
```xml
<HintPath>C:\Program Files (x86)\Team MediaPortal\MP2-Client\MediaPortal.Common.dll</HintPath>
```

Do this for:
- MediaPortal.Common
- MediaPortal.UI
- MediaPortal.Utilities
- MediaPortal.UI.SkinEngine

---

## ✅ Most Likely Solution

**Just restart Visual Studio** - that's usually all it takes after setting the environment variable!

---

**Status:** Environment variable set. Restart Visual Studio and try again! 🚀
