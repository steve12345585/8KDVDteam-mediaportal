# Quick Install Guide - 8KDVD Player Plugin

## 🎯 Installation Path

**For MediaPortal 2 Client Plugin:**
```
C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\8KDVDPlayer\
```

---

## 📋 Quick Steps

### 1. Build the Plugin
```powershell
cd "8KDVDPlayer"
msbuild EightKDVDPlayer.csproj /p:Configuration=Release
```

### 2. Copy Files to Plugin Directory

**From:** `8KDVDPlayer\bin\Release\`  
**To:** `C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\8KDVDPlayer\`

**Files to copy:**
- ✅ `EightKDVDPlayer.dll`
- ✅ `plugin.xml`
- ✅ `Skin/` folder (entire folder)
- ✅ `Language/` folder (entire folder)

### 3. Start MediaPortal 2

The plugin will be automatically detected and loaded.

---

## ⚠️ Important Notes

1. **Administrator Rights:** You may need admin privileges to copy files to Program Files
2. **LAV Filters:** Install LAV Filters for VP9/Opus playback support
3. **WebView2:** Usually pre-installed on Windows 10/11

---

## ✅ Verify Installation

1. Start MediaPortal 2
2. Go to: **Settings → Plugins**
3. Look for: **"8KDVD Player"**
4. Verify it's enabled

---

## 🧪 Test

Insert an 8KDVD disc - the plugin should auto-detect and launch the menu!

---

**That's it!** 🚀
