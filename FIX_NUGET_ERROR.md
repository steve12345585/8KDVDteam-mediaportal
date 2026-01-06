# Fix: NU1101 - Unable to find Microsoft.Web.WebView2.WinForms

## ❌ Error
```
NU1101: Unable to find package Microsoft.Web.WebView2.WinForms
```

## ✅ Solutions

### Solution 1: Check NuGet Package Sources

1. **In Visual Studio:**
   - Go to **Tools** → **Options**
   - Navigate to **NuGet Package Manager** → **Package Sources**
   - Make sure **nuget.org** is enabled and checked
   - If missing, add it:
     - Click **+** button
     - Name: `nuget.org`
     - Source: `https://api.nuget.org/v3/index.json`
     - Click **OK**

2. **Restore Packages:**
   - Right-click solution → **Restore NuGet Packages**

---

### Solution 2: Update Package Name

The package might need to be updated. Try:

1. **Open Package Manager Console:**
   - **Tools** → **NuGet Package Manager** → **Package Manager Console**

2. **Install/Update Package:**
   ```powershell
   Install-Package Microsoft.Web.WebView2.WinForms -Version 1.0.2045.28
   ```

   Or try the latest version:
   ```powershell
   Install-Package Microsoft.Web.WebView2.WinForms
   ```

---

### Solution 3: Use Package Manager UI

1. **Right-click project** → **Manage NuGet Packages**
2. **Browse** tab
3. **Search for:** `Microsoft.Web.WebView2.WinForms`
4. **Install** the package

---

### Solution 4: Check Internet Connection

- Make sure you're connected to the internet
- NuGet needs to download packages from nuget.org

---

### Solution 5: Clear NuGet Cache

In Package Manager Console:
```powershell
dotnet nuget locals all --clear
```

Or manually:
1. **Tools** → **Options** → **NuGet Package Manager** → **General**
2. Click **Clear All NuGet Cache(s)**

---

### Solution 6: Update Package Version

The version `1.0.2045.28` might be outdated. Try:

1. Edit `EightKDVDPlayer.csproj`
2. Change:
   ```xml
   <PackageReference Include="Microsoft.Web.WebView2.WinForms" Version="1.0.2045.28" />
   ```
   
   To (latest version):
   ```xml
   <PackageReference Include="Microsoft.Web.WebView2.WinForms" Version="1.0.2822.45" />
   ```

   Or remove version to get latest:
   ```xml
   <PackageReference Include="Microsoft.Web.WebView2.WinForms" />
   ```

---

## 🔍 Verify NuGet Sources

Check if nuget.org is available:

In Package Manager Console:
```powershell
Get-PackageSource
```

Should show `nuget.org` in the list.

---

## 🎯 Most Likely Fix

**90% of the time, it's missing nuget.org source:**

1. **Tools** → **Options** → **NuGet Package Manager** → **Package Sources**
2. **Enable nuget.org** (check the checkbox)
3. **Restore NuGet Packages**

---

## ✅ After Fixing

1. Restore packages should succeed
2. Error should disappear
3. Project should build

---

**Try Solution 1 first - it's the most common fix!**
