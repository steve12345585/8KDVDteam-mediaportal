# 8KDVD Player - Implementation Notes

## What's Implemented

### ✅ Core Infrastructure
1. **Plugin Registration** - Complete plugin.xml with all registrations
2. **Disc Detection** - `DiscDetector` class detects 8KDVD disc structure
3. **Certificate Validation** - `CertificateValidator` implements C-Logic security
4. **Auto-Launch** - `PluginStateTracker` monitors discs and auto-launches

### ✅ Services
1. **DiscPathService** - Singleton service to share disc path between components

### ✅ Models
1. **WorkflowModel** - Main workflow model for navigation
2. **WebViewHelperModel** - WebView2 integration model with JavaScript bridge

### ✅ WebView2 Integration
1. **WebViewPanel** - Custom control that embeds WebView2
2. **JavaScript Bridge** - Handles JavaScript calls from HTML menu
3. **HTML Loading** - Loads `weblauncher.html` from disc

### ✅ Player
1. **PlayerBuilder** - Registers 8KDVD player with MediaPortal
2. **Player Class** - Skeleton implementation (needs VP9/Opus integration)

### ✅ Configuration
1. **DefaultQuality** - Settings for default playback quality
2. **AutoSwitch** - Settings for automatic quality switching
3. **AutoPlay** - Settings for auto-play on disc insertion

---

## What Needs Implementation

### 🔨 WebViewPanel Model Connection
**Priority: HIGH**

Connect WebViewPanel to WebViewHelperModel:
- Set up JavaScript bridge connection
- Wire up events properly
- Test JavaScript calls

**Location:** `Source/WebView/WebViewPanel.cs` and `Skin/default/screens/EightKDVD-main.xaml`

---

### 🔨 Player Integration
**Priority: HIGH**

Complete player implementation:
- Use IPlayerManager to start playback
- Create MediaItem from video file
- Handle startTime seeking
- Integrate with MediaPortal player framework

**Location:** `Source/Models/WebViewHelperModel.cs` - `HandlePlayMovie` method

---

### 🔨 VP9/Opus Codec Integration
**Priority: HIGH**

Implement actual playback:
- VP9 video decoder integration
- Opus audio decoder integration
- Use FFMpegLib or native decoders
- Integrate with MediaPortal player framework

**Location:** `Source/Player/EightKDVDPlayer.cs`

---

### 🔨 Quality Switching
**Priority: MEDIUM**

Implement adaptive quality:
- Detect hardware capabilities
- Switch between 8K/4K/1080p
- Handle quality changes during playback

**Location:** `Source/Player/QualityManager.cs` (to be created)

---

### 🔨 Subtitle System
**Priority: MEDIUM**

Implement subtitle support:
- Parse SRT files (UTF-8)
- Render as overlay
- Synchronize with playback
- Multiple language support

**Location:** `Source/Subtitles/` (to be created)

---

## Recent Additions

### ✅ DiscPathService
- Singleton service to share disc path
- Used by PluginStateTracker, WebViewHelperModel, and WebViewPanel
- Event-based notifications when disc path changes

### ✅ WebViewPanel Control
- Custom WPF control embedding WebView2
- Loads HTML from disc automatically
- JavaScript bridge setup
- Handles JavaScript messages

### ✅ JavaScript Bridge
- Bridges JavaScript calls to WebViewHelperModel
- Handles playMovie, changeQuality, loadSubtitle, backToMainMenu

### ✅ Enhanced WebViewHelperModel
- Uses DiscPathService for disc path
- Improved HandlePlayMovie implementation
- Navigate back to main menu implemented

---

## Next Steps

1. **Connect WebViewPanel to Model**
   - Wire up JavaScript bridge to WebViewHelperModel
   - Test JavaScript calls work

2. **Complete Player Integration**
   - Use IPlayerManager to start playback
   - Create MediaItem from .EVO files
   - Handle startTime seeking

3. **Test HTML Rendering**
   - Test with Elephants Dream disc
   - Verify weblauncher.html loads
   - Test menu navigation

4. **Implement Codec Support**
   - VP9/Opus decoder integration
   - Test playback

---

## Testing Checklist

- [ ] Plugin loads in MediaPortal 2
- [ ] Disc detection works
- [ ] Certificate validation works
- [ ] HTML loads in WebView2
- [ ] JavaScript bridge receives calls
- [ ] playMovie() triggers player
- [ ] Navigation works
- [ ] Video playback works

---

## Known Issues / TODOs

1. **WebViewPanel Model Connection** - Need to properly connect bridge to model
2. **Player Integration** - Need to use IPlayerManager properly
3. **Codec Support** - Need to determine if MediaPortal supports VP9/Opus natively
4. **Error Handling** - Add more comprehensive error handling
5. **Logging** - Enhance logging throughout

---

## References

- **OnlineVideos Plugin** - WebView2 integration
- **VideoPlayers Plugin** - Player registration
- **RemovableMediaManager** - Disc detection
- **BDHandler Plugin** - Disc player patterns

---

**Status:** WebView2 integration complete, ready for player integration! 🚀
