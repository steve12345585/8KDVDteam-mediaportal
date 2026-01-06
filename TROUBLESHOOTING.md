# Troubleshooting - Build Issues

## ❌ "Failed to restore" Error

### Problem
```
Failed to restore EightKDVDPlayer.csproj
```

### Common Causes

1. **MediaPortal2Path not set** - Project can't find MediaPortal 2 DLLs
2. **MediaPortal 2 not installed** - DLLs don't exist
3. **Wrong MediaPortal 2 path** - Path doesn't match installation

---

## ✅ Solution 1: Set MediaPortal2Path Environment Variable

### Option A: Set for Current Session (Temporary)
```powershell
$env:MediaPortal2Path = "C:\Program Files (x86)\Team MediaPortal\MP2-Client"
```

### Option B: Set Permanently (Recommended)
1. Open **System Properties** → **Environment Variables**
2. Under **User variables**, click **New**
3. Variable name: `MediaPortal2Path`
4. Variable value: `C:\Program Files (x86)\Team MediaPortal\MP2-Client`
5. Click **OK**
6. **Restart Visual Studio**

### Option C: Edit Project File Directly
If environment variable doesn't work, edit `EightKDVDPlayer.csproj`:

Replace:
```xml
<HintPath>$(MediaPortal2Path)\MediaPortal.Common.dll</HintPath>
```

With:
```xml
<HintPath>C:\Program Files (x86)\Team MediaPortal\MP2-Client\MediaPortal.Common.dll</HintPath>
```

Do this for all MediaPortal references.

---

## ✅ Solution 2: Verify MediaPortal 2 Installation

Check if MediaPortal 2 is installed:

```powershell
Test-Path "C:\Program Files (x86)\Team MediaPortal\MP2-Client\MediaPortal.Common.dll"
```

If this returns `False`, MediaPortal 2 is not installed or in a different location.

**Find MediaPortal 2 installation:**
- Check: `C:\Program Files (x86)\Team MediaPortal\MP2-Client\`
- Or: `C:\Program Files\Team MediaPortal\MP2-Client\`
- Or search for: `MediaPortal.Common.dll`

---

## ✅ Solution 3: Update Project File with Correct Path

If MediaPortal 2 is in a different location:

1. Find where `MediaPortal.Common.dll` is located
2. Edit `EightKDVDPlayer.csproj`
3. Replace all `$(MediaPortal2Path)` with the actual path

Example:
```xml
<Reference Include="MediaPortal.Common">
  <HintPath>C:\Your\Actual\Path\MP2-Client\MediaPortal.Common.dll</HintPath>
</Reference>
```

---

## ✅ Solution 4: Restore NuGet Packages Manually

In Visual Studio:
1. Right-click solution → **Restore NuGet Packages**
2. Or: **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Run: `Update-Package -reinstall`

---

## 🔍 Check What's Wrong

### Step 1: Check Environment Variable
```powershell
$env:MediaPortal2Path
```
Should show the MediaPortal 2 path.

### Step 2: Check if DLLs Exist
```powershell
Test-Path "$env:MediaPortal2Path\MediaPortal.Common.dll"
```
Should return `True`.

### Step 3: Check Visual Studio Output
- Open **View** → **Output**
- Select **Package Manager** from dropdown
- Look for specific error messages

---

## 📝 Quick Fix Script

Run this in PowerShell (as Administrator if needed):

```powershell
# Set MediaPortal2Path
$mp2Path = "C:\Program Files (x86)\Team MediaPortal\MP2-Client"
if (Test-Path $mp2Path) {
    [System.Environment]::SetEnvironmentVariable("MediaPortal2Path", $mp2Path, "User")
    Write-Host "MediaPortal2Path set to: $mp2Path"
    Write-Host "Please restart Visual Studio for changes to take effect"
} else {
    Write-Host "MediaPortal 2 not found at: $mp2Path"
    Write-Host "Please update the path in the script"
}
```

---

## 🎯 Most Likely Fix

**90% of the time, it's the MediaPortal2Path environment variable.**

1. Set `MediaPortal2Path` environment variable
2. Restart Visual Studio
3. Try restoring packages again

---

## Still Having Issues?

Check:
- ✅ MediaPortal 2 is actually installed
- ✅ Path is correct (no typos)
- ✅ Visual Studio has been restarted after setting environment variable
- ✅ You have read permissions to MediaPortal 2 folder

---

**Need more help?** Check the specific error message in Visual Studio's Output window for more details.
