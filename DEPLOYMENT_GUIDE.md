# 8KDVD Player Plugin - Deployment Guide

## 📦 Building and Installing the Plugin

### Prerequisites

1. **Visual Studio 2019 or later**
   - .NET Framework 4.8 support
   - C# development tools

2. **MediaPortal 2 Installation**
   - MediaPortal 2 must be installed on your system
   - Note the installation path (typically `C:\Program Files (x86)\Team MediaPortal\MediaPortal 2`)

3. **LAV Filters** (for VP9/Opus playback)
   - Download from: https://github.com/Nevcairiel/LAVFilters/releases
   - Install LAV Splitter, LAV Video Decoder, LAV Audio Decoder

4. **WebView2 Runtime** (for HTML menu)
   - Usually pre-installed on Windows 10/11
   - If not, download from: https://developer.microsoft.com/microsoft-edge/webview2/

---

## 🔧 Step 1: Build the Plugin

### Option A: Visual Studio

1. **Open the Project**
   ```
   Open: 8KDVDPlayer/EightKDVDPlayer.csproj in Visual Studio
   ```

2. **Set MediaPortal 2 Path**
   - The project file should reference MediaPortal 2 assemblies
   - If needed, update the MediaPortal2Path environment variable or edit `.csproj` file
   - Default MediaPortal 2 path: `C:\Program Files (x86)\Team MediaPortal\MediaPortal 2`

3. **Restore NuGet Packages**
   - Right-click solution → "Restore NuGet Packages"
   - This will download WebView2 packages

4. **Build the Solution**
   - Press `Ctrl+Shift+B` or Build → Build Solution
   - Check for errors in the Output window
   - Build output will be in `bin\Debug\` or `bin\Release\`

### Option B: Command Line (MSBuild)

```powershell
cd "8KDVDPlayer"
msbuild EightKDVDPlayer.csproj /p:Configuration=Release
```

---

## 📁 Step 2: Locate Build Output

After building, you'll find:

```
8KDVDPlayer/
└── bin/
    └── Release/  (or Debug/)
        ├── EightKDVDPlayer.dll
        ├── plugin.xml
        ├── Skin/
        │   └── default/
        │       └── screens/
        │           └── EightKDVD-main.xaml
        └── Language/
            └── strings_en.xml
```

**Key files needed:**
- `EightKDVDPlayer.dll` - The main plugin assembly
- `plugin.xml` - Plugin registration file
- `Skin/` folder - XAML skin files
- `Language/` folder - Language strings

---

## 📂 Step 3: Install to MediaPortal 2

### MediaPortal 2 Plugin Directory

MediaPortal 2 **client** plugins are installed in:
```
C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\
```

**Note:** Since this is a client plugin (with UI, WebView2, menu), it goes in **MP2-Client**, not MP2-Server.

Alternative locations (if using portable installation):
- `C:\ProgramData\Team MediaPortal\MediaPortal 2\Plugins\`
- `%LOCALAPPDATA%\Team MediaPortal\MediaPortal 2\Plugins\`

### Installation Steps

1. **Create Plugin Directory**
   ```
   Create folder: C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\8KDVDPlayer\
   ```
   
   **Note:** You may need administrator privileges to create folders in Program Files.

2. **Copy Plugin Files**
   Copy the following from your build output:
   ```
   From: 8KDVDPlayer\bin\Release\
   To:   C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\8KDVDPlayer\
   
   Files to copy:
   - EightKDVDPlayer.dll
   - plugin.xml
   - Skin/ (entire folder)
   - Language/ (entire folder)
   ```

3. **Verify Structure**
   Your plugin directory should look like:
   ```
   C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\8KDVDPlayer\
   ├── EightKDVDPlayer.dll
   ├── plugin.xml
   ├── Skin/
   │   └── default/
   │       └── screens/
   │           └── EightKDVD-main.xaml
   └── Language/
       └── strings_en.xml
   ```

---

## 🚀 Step 4: Start MediaPortal 2

1. **Launch MediaPortal 2**
   - Start MediaPortal 2 client
   - The plugin should be automatically detected and loaded

2. **Check Plugin Status**
   - Go to: Settings → Plugins
   - Look for "8KDVD Player" in the plugin list
   - Verify it's enabled

3. **View Logs** (if issues)
   - Logs location: `%LOCALAPPDATA%\Team MediaPortal\MediaPortal 2\Log\
   - Look for `EightKDVDPlayer.log` or check main log files

---

## 🧪 Step 5: Test the Plugin

### Test with Elephants Dream Disc

1. **Insert 8KDVD Disc**
   - Insert the Elephants Dream disc (or any 8KDVD disc)
   - The plugin should auto-detect it

2. **Verify Detection**
   - Check MediaPortal logs for: "8KDVD disc detected"
   - Plugin should auto-launch the menu

3. **Test Features**
   - ✅ HTML menu loads in WebView2
   - ✅ JavaScript calls work
   - ✅ Video playback starts
   - ✅ Quality switching works
   - ✅ Playhead position preserved

### Manual Testing

If auto-detection doesn't work:
1. Navigate to: Plugins → 8KDVD Player
2. Manually launch the plugin
3. It should detect the disc and load the menu

---

## 🔍 Troubleshooting

### Plugin Not Loading

**Check:**
- Plugin DLL is in correct location
- `plugin.xml` is present and valid
- MediaPortal 2 logs for errors
- All dependencies are available (MediaPortal.Common, etc.)

**Common Issues:**
- Missing MediaPortal 2 assemblies → Check .csproj references
- Plugin not in correct folder → Verify path
- XML errors in plugin.xml → Validate XML syntax

### Codec Issues

**If VP9/Opus playback fails:**
- Verify LAV Filters are installed
- Check MediaPortal 2 codec settings
- Verify LAV Filters are set as default decoders
- Check logs for codec-related errors

### WebView2 Issues

**If HTML menu doesn't load:**
- Verify WebView2 Runtime is installed
- Check Windows version (WebView2 requires Windows 10/11)
- Check logs for WebView2 initialization errors

### Disc Detection Issues

**If disc not detected:**
- Verify disc structure matches 8KDVD spec
- Check for `CERTIFICATE/Certificate.html`
- Check for `8KDVD_TS/ADV_OBJ/weblauncher.html`
- Check logs for detection errors

---

## 📝 Quick Installation Script

You can create a PowerShell script to automate installation:

```powershell
# Build the plugin
cd "8KDVDPlayer"
msbuild EightKDVDPlayer.csproj /p:Configuration=Release

# Create plugin directory (requires admin privileges)
$pluginDir = "C:\Program Files (x86)\Team MediaPortal\MP2-Client\Plugins\8KDVDPlayer"
New-Item -ItemType Directory -Force -Path $pluginDir

# Copy files
Copy-Item "bin\Release\*.dll" -Destination $pluginDir
Copy-Item "bin\Release\plugin.xml" -Destination $pluginDir
Copy-Item "bin\Release\Skin" -Destination $pluginDir -Recurse
Copy-Item "bin\Release\Language" -Destination $pluginDir -Recurse

Write-Host "Plugin installed to: $pluginDir"
```

---

## ✅ Verification Checklist

After installation, verify:

- [ ] Plugin appears in MediaPortal 2 plugin list
- [ ] Plugin is enabled
- [ ] No errors in MediaPortal 2 logs
- [ ] Disc detection works
- [ ] HTML menu loads
- [ ] Video playback works (if LAV Filters installed)
- [ ] Quality switching works
- [ ] JavaScript bridge works

---

## 🔄 Updating the Plugin

To update an existing installation:

1. **Stop MediaPortal 2** (if running)
2. **Replace files** in plugin directory:
   - Copy new `EightKDVDPlayer.dll`
   - Update `plugin.xml` if changed
   - Update `Skin/` and `Language/` folders if changed
3. **Restart MediaPortal 2**
4. **Verify** plugin loads correctly

---

## 📚 Additional Resources

- **MediaPortal 2 Plugin Development:** https://www.team-mediaportal.com/
- **LAV Filters:** https://github.com/Nevcairiel/LAVFilters
- **WebView2:** https://developer.microsoft.com/microsoft-edge/webview2/

---

## 🎯 Next Steps

After successful installation:

1. **Test with actual 8KDVD disc**
2. **Verify all features work**
3. **Report any issues**
4. **Enjoy 8KDVD playback!** 🎬

---

**Status:** Ready for deployment! 🚀
