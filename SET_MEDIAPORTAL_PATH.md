# How to Set MediaPortal2Path Environment Variable

## ✅ Method 1: Already Set (Check First)

I've already set it for you! But let's verify:

### Check if it's set:
```powershell
[System.Environment]::GetEnvironmentVariable('MediaPortal2Path', 'User')
```

Should show: `C:\Program Files (x86)\Team MediaPortal\MP2-Client`

---

## 🔧 Method 2: Set Manually via Windows GUI

### Step-by-Step:

1. **Open Environment Variables**
   - Press `Win + R`
   - Type: `sysdm.cpl` and press Enter
   - Click **"Advanced"** tab
   - Click **"Environment Variables"** button

2. **Add User Variable**
   - Under **"User variables"** section, click **"New..."**
   - Variable name: `MediaPortal2Path`
   - Variable value: `C:\Program Files (x86)\Team MediaPortal\MP2-Client`
   - Click **"OK"**

3. **Verify**
   - You should see `MediaPortal2Path` in the User variables list
   - Click **"OK"** to close all dialogs

4. **Restart Visual Studio**
   - Close Visual Studio completely
   - Reopen it
   - Environment variables require a restart

---

## 🔧 Method 3: Set via PowerShell (Quick)

Run this in PowerShell:

```powershell
[System.Environment]::SetEnvironmentVariable("MediaPortal2Path", "C:\Program Files (x86)\Team MediaPortal\MP2-Client", "User")
```

Then **restart Visual Studio**.

---

## 🔍 Verify It's Set

### In PowerShell:
```powershell
[System.Environment]::GetEnvironmentVariable('MediaPortal2Path', 'User')
```

### In Command Prompt:
```cmd
echo %MediaPortal2Path%
```
(Note: This only works in new command prompt windows after setting it)

---

## ⚠️ Important Notes

1. **User vs System Variable**
   - Set as **User variable** (recommended) - only affects your account
   - Or **System variable** - affects all users (requires admin)

2. **Restart Required**
   - Visual Studio must be **completely closed and reopened**
   - Just closing the project isn't enough

3. **Path Must Be Exact**
   - No trailing backslash
   - Case doesn't matter, but spelling does
   - Must point to where `MediaPortal.Common.dll` exists

---

## ✅ Quick Verification Script

Run this to check everything:

```powershell
$path = [System.Environment]::GetEnvironmentVariable('MediaPortal2Path', 'User')
if ($path) {
    Write-Host "✅ MediaPortal2Path is set to: $path"
    if (Test-Path "$path\MediaPortal.Common.dll") {
        Write-Host "✅ MediaPortal.Common.dll found!"
    } else {
        Write-Host "❌ MediaPortal.Common.dll NOT found at that path"
    }
} else {
    Write-Host "❌ MediaPortal2Path is NOT set"
    Write-Host "Run: [System.Environment]::SetEnvironmentVariable('MediaPortal2Path', 'C:\Program Files (x86)\Team MediaPortal\MP2-Client', 'User')"
}
```

---

## 🎯 After Setting

1. ✅ Set the environment variable (done above)
2. ✅ **Restart Visual Studio** (important!)
3. ✅ Open your project
4. ✅ Right-click solution → **Restore NuGet Packages**
5. ✅ Should work now!

---

**Need help?** The path should be exactly: `C:\Program Files (x86)\Team MediaPortal\MP2-Client`
