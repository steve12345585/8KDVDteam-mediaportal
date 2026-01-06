# Visual Studio Requirements - 8KDVD Player Plugin

## 🎯 Recommended Visual Studio Version

### **Visual Studio 2019 or Visual Studio 2022**

Both will work, but **Visual Studio 2022** is recommended for the latest features and support.

---

## 📋 Requirements Breakdown

### Project Requirements:
- **.NET Framework 4.8** - Target framework
- **C#** - Programming language
- **NuGet Package Manager** - For WebView2 packages
- **MSBuild** - For building the project

### Visual Studio 2019:
- ✅ **Community Edition** (free) - Fully sufficient
- ✅ Supports .NET Framework 4.8
- ✅ Includes NuGet Package Manager
- ✅ Includes MSBuild
- ✅ Works with SDK-style projects

### Visual Studio 2022:
- ✅ **Community Edition** (free) - Recommended
- ✅ Latest version with best support
- ✅ Better performance
- ✅ Supports .NET Framework 4.8
- ✅ Includes NuGet Package Manager
- ✅ Includes MSBuild

---

## 🔧 Required Workloads/Components

### Minimum Required:
1. **.NET desktop development** workload
   - Includes .NET Framework 4.8 support
   - Includes C# compiler
   - Includes MSBuild

### Optional but Recommended:
2. **NuGet Package Manager** (usually included)
3. **Git for Windows** (if using Git)

---

## 📥 Download Links

### Visual Studio 2022 Community (Recommended):
- **Download:** https://visualstudio.microsoft.com/downloads/
- **Select:** "Community 2022" (free)
- **During install, select:** ".NET desktop development" workload

### Visual Studio 2019 Community (Alternative):
- **Download:** https://visualstudio.microsoft.com/vs/older-downloads/
- **Select:** "Community 2019" (free)
- **During install, select:** ".NET desktop development" workload

---

## ✅ Verification

After installing Visual Studio:

1. **Open the project:**
   ```
   Open: 8KDVDPlayer/EightKDVDPlayer.csproj
   ```

2. **Check if it loads:**
   - Project should open without errors
   - References should resolve (may need to set MediaPortal2Path)

3. **Restore NuGet packages:**
   - Right-click solution → "Restore NuGet Packages"
   - Should download WebView2 packages

4. **Try building:**
   - Press `Ctrl+Shift+B`
   - Should build successfully (after setting MediaPortal2Path)

---

## 🎯 Quick Answer

**Use: Visual Studio 2022 Community Edition**

- ✅ Free
- ✅ Latest version
- ✅ Fully supports .NET Framework 4.8
- ✅ Best compatibility

**Install with:** ".NET desktop development" workload

---

## 📝 Alternative: Visual Studio Code

**Note:** VS Code can work but is **not recommended** for this project because:
- ❌ Limited .NET Framework 4.8 support
- ❌ No built-in MSBuild integration
- ❌ More complex setup required

**Stick with Visual Studio 2019/2022 for best experience.**

---

## 🔗 Next Steps

After installing Visual Studio:

1. Open the project
2. Set MediaPortal2Path (see DEPLOYMENT_GUIDE.md)
3. Restore NuGet packages
4. Build the solution
5. Deploy to MediaPortal 2

---

**Ready to install?** Download Visual Studio 2022 Community Edition! 🚀
