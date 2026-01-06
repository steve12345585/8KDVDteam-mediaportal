# Next Steps - 8KDVD Player Plugin

## 🎯 Current Status

✅ **Foundation Complete:**
- Plugin structure and registration
- Disc detection and certificate validation
- WebView2 control created
- JavaScript bridge skeleton
- Player registration

## 🔨 Critical Next Steps (Priority Order)

### 1. **Connect WebViewPanel to WebViewHelperModel** ⚠️ HIGH PRIORITY
**Why:** JavaScript calls from HTML won't work until this is connected

**What to do:**
- Wire up WebViewPanel's JavaScript bridge to WebViewHelperModel
- Connect events properly in XAML or code-behind
- Test that JavaScript calls reach the model

**Files to modify:**
- `Source/WebView/WebViewPanel.cs` - Connect bridge to model
- `Skin/default/screens/EightKDVD-main.xaml` - Wire up model binding

---

### 2. **Complete Player Integration** ⚠️ HIGH PRIORITY
**Why:** playMovie() calls won't actually play video until this works

**What to do:**
- Use `IPlayerManager` to start playback
- Create `MediaItem` from .EVO video files
- Handle startTime seeking
- Integrate with MediaPortal's player framework

**Files to modify:**
- `Source/Models/WebViewHelperModel.cs` - Complete HandlePlayMovie()
- Study VideoPlayers plugin for IPlayerManager usage

---

### 3. **Test Basic Functionality** ⚠️ HIGH PRIORITY
**Why:** Need to verify what works before adding more features

**What to test:**
- Plugin loads in MediaPortal 2
- Disc detection works
- HTML menu loads in WebView2
- JavaScript bridge receives calls
- Basic navigation works

**Test with:** Elephants Dream disc

---

### 4. **VP9/Opus Codec Support** ⚠️ MEDIUM PRIORITY
**Why:** Video won't play without codec support

**What to do:**
- Research if MediaPortal supports VP9/Opus natively
- If not, integrate FFMpegLib or external decoders
- Test playback with actual .EVO files

**Files to modify:**
- `Source/Player/EightKDVDPlayer.cs`
- May need to create decoder wrappers

---

### 5. **Quality Switching** ⚠️ MEDIUM PRIORITY
**Why:** Adaptive quality is a key feature

**What to do:**
- Detect hardware capabilities
- Switch between 8K/4K/1080p
- Handle quality changes during playback

**Files to create:**
- `Source/Player/QualityManager.cs`

---

### 6. **Subtitle System** ⚠️ MEDIUM PRIORITY
**Why:** Subtitles are missing from HTML version

**What to do:**
- Parse SRT files (UTF-8)
- Render as overlay
- Synchronize with playback

**Files to create:**
- `Source/Subtitles/SubtitleManager.cs`
- `Source/Subtitles/SRTParser.cs`

---

## 🚀 Recommended Action Plan

### Week 1: Core Functionality
1. **Day 1-2:** Connect WebViewPanel to model
2. **Day 3-4:** Complete player integration
3. **Day 5:** Test with Elephants Dream disc

### Week 2: Codec Support
1. **Day 1-2:** Research VP9/Opus support
2. **Day 3-4:** Implement codec integration
3. **Day 5:** Test playback

### Week 3: Advanced Features
1. **Day 1-2:** Quality switching
2. **Day 3-4:** Subtitle system
3. **Day 5:** Testing and bug fixes

---

## 📝 Immediate Next Action

**Start with:** Connect WebViewPanel to WebViewHelperModel

This is the critical path - without this, nothing else will work!
