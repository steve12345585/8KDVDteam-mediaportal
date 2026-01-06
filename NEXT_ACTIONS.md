# Next Actions - 8KDVD Player Plugin

## 🎯 Current Status

**✅ Completed:**
- Core plugin structure
- Disc detection & certificate validation
- WebView2 HTML menu integration
- JavaScript bridge
- Player integration with IPlayerManager
- **VP9/Opus codec support** (just completed!)

**⚠️ Remaining:**
- Quality switching
- Seeking implementation
- Subtitle system
- Testing

---

## 🚀 Recommended Next Steps (Priority Order)

### 1. **Quality Switching** ⚠️ HIGH PRIORITY
**Why:** Core 8KDVD spec requirement - users need to switch between 8K/4K/1080p

**Current Status:**
- `HandleChangeQuality()` method exists but is empty
- Quality selection logic exists in `HandlePlayMovie()` (EVO8 > EVO4 > EVOH priority)
- Need to implement switching during playback

**What to implement:**
- Complete `HandleChangeQuality()` method
- Switch between EVO8/EVO4/EVOH files
- Maintain playhead position (per 8KDVD spec)
- Update current video file based on quality selection

**Files to work on:**
- `Source/Models/WebViewHelperModel.cs` - `HandleChangeQuality()` method
- May need to track current quality state

**Estimated effort:** Medium (2-3 hours)

---

### 2. **Seeking Implementation** ⚠️ MEDIUM PRIORITY
**Why:** `playMovie(startTime)` needs to actually seek to the specified time

**Current Status:**
- `Seek()` method exists in `EightKDVDPlayer.cs` but is empty
- `HandlePlayMovie()` parses startTime but doesn't use it yet

**What to implement:**
- Implement seeking in `EightKDVDPlayer.cs`
- Seek after playback starts
- Handle time-based seeking (seconds to milliseconds conversion)

**Files to work on:**
- `Source/Player/EightKDVDPlayer.cs` - `Seek()` method
- `Source/Models/WebViewHelperModel.cs` - Use startTime after playback starts

**Estimated effort:** Medium (2-3 hours)

---

### 3. **Subtitle System** ⚠️ MEDIUM PRIORITY
**Why:** HTML version lacks subtitles - this is a key differentiator

**Current Status:**
- `HandleLoadSubtitle()` method exists but is empty
- No subtitle parsing or rendering yet

**What to implement:**
- SRT file parser (UTF-8 encoding)
- Subtitle overlay renderer
- Synchronization with playback
- Multiple language support

**Files to create:**
- `Source/Subtitles/SubtitleManager.cs`
- `Source/Subtitles/SRTParser.cs`
- `Source/Models/WebViewHelperModel.cs` - `HandleLoadSubtitle()` method

**Estimated effort:** High (4-6 hours)

---

### 4. **Testing & Bug Fixes** ⚠️ ONGOING
**Why:** Need to verify everything works with actual discs

**What to test:**
1. Build plugin in Visual Studio
2. Install in MediaPortal 2
3. Test with Elephants Dream disc
4. Verify all features work
5. Fix any bugs found

**Test checklist:**
- [ ] Disc detection
- [ ] Certificate validation
- [ ] HTML menu loading
- [ ] JavaScript calls
- [ ] Video playback (VP9/Opus)
- [ ] Quality switching
- [ ] Seeking
- [ ] Subtitles (when implemented)

---

## 💡 My Recommendation

**Start with: Quality Switching**

**Why:**
1. It's a core 8KDVD spec requirement
2. The method is already stubbed out
3. Relatively straightforward to implement
4. Makes the plugin more functional

**Then:**
- Seeking (completes the playback control)
- Subtitles (adds key missing feature)
- Testing (verify everything works)

---

## 📝 Implementation Plan for Quality Switching

### Step 1: Track Current Quality
- Add quality state to `WebViewHelperModel`
- Store current EVO file path
- Store current playhead position

### Step 2: Implement Quality Switch
- Find new EVO file based on quality selection
- Get current playhead position
- Stop current playback
- Start new playback at same position

### Step 3: Handle JavaScript Calls
- Parse quality parameter ("8K", "4K", "1080p")
- Map to EVO file extensions (EVO8, EVO4, EVOH)
- Call quality switch logic

---

## 🎯 Immediate Next Action

**Implement Quality Switching** - This will make the plugin significantly more functional and complete a core 8KDVD spec requirement.

**Ready to start?** Let me know and I'll implement it! 🚀
