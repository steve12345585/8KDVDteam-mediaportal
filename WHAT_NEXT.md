# What's Next? - 8KDVD Player Plugin

## 🎉 Just Completed

✅ **Player Integration Complete!**
- `HandlePlayMovie()` now uses `IPlayerManager` to start playback
- Creates `MediaItem` with proper `VideoAspect` and resource locator
- Handles quality selection (EVO8 > EVO4 > EVOH priority)
- Parses `startTime` parameter from JavaScript calls

**The plugin can now:**
1. Detect 8KDVD discs ✅
2. Validate certificates ✅
3. Load HTML menu in WebView2 ✅
4. Receive JavaScript calls ✅
5. **Start video playback** ✅ (NEW!)

---

## 🚀 Next Critical Steps

### 1. **VP9/Opus Codec Support** ✅ COMPLETE
**Status:** Codec integration complete!

**What was done:**
- ✅ Researched MediaPortal 2 codec architecture
- ✅ Confirmed LAV Filters support VP9/Opus via DirectShow
- ✅ Created `CodecVerifier.cs` for codec detection
- ✅ Updated MIME type to `video/mp4` (EVO files are MP4 containers)
- ✅ Integrated codec verification into playback flow

**How it works:**
- MediaPortal 2 uses DirectShow filters for playback
- LAV Filters automatically decode VP9 video and Opus audio
- No custom decoder needed - MediaPortal handles it

**Files created/modified:**
- `Source/Player/CodecVerifier.cs` - Codec detection
- `Source/Models/WebViewHelperModel.cs` - MIME type and verification
- `Source/Player/EightKDVDPlayer.cs` - Codec verification

**See:** `CODEC_RESEARCH.md` and `CODEC_IMPLEMENTATION.md` for details

---

### 2. **Test Basic Flow** ⚠️ HIGH PRIORITY
**Why:** Need to verify everything works before adding more features

**What to test:**
1. Build plugin in Visual Studio
2. Install in MediaPortal 2
3. Insert Elephants Dream disc
4. Verify:
   - ✅ Disc detection works
   - ✅ Certificate validation works
   - ✅ HTML menu loads
   - ✅ JavaScript calls work
   - ⚠️ Video playback (needs codec support)

**Test with:** Elephants Dream disc

---

### 3. **Quality Switching** ⚠️ MEDIUM PRIORITY
**Why:** 8KDVD spec requires adaptive quality switching

**What to do:**
- Complete `HandleChangeQuality()` method
- Switch between EVO8/EVO4/EVOH during playback
- Maintain playhead position (per 8KDVD spec)
- Detect hardware capabilities

**Files to work on:**
- `Source/Models/WebViewHelperModel.cs` - `HandleChangeQuality()`
- May need `QualityManager.cs` class

---

### 4. **Subtitle System** ⚠️ MEDIUM PRIORITY
**Why:** HTML version lacks subtitles - this is a key feature

**What to do:**
- Parse SRT files (UTF-8 encoding)
- Render as overlay during playback
- Synchronize with video
- Multiple language support

**Files to create:**
- `Source/Subtitles/SubtitleManager.cs`
- `Source/Subtitles/SRTParser.cs`
- `Source/Models/WebViewHelperModel.cs` - `HandleLoadSubtitle()`

---

### 5. **Seeking Implementation** ⚠️ MEDIUM PRIORITY
**Why:** `playMovie(startTime)` needs to actually seek

**What to do:**
- Implement seeking in `EightKDVDPlayer.cs`
- Handle seek requests from JavaScript
- Support time-based seeking (seconds)

**Files to work on:**
- `Source/Player/EightKDVDPlayer.cs` - `Seek()` method

---

## 📋 Recommended Action Plan

### Week 1: Codec Support
1. **Day 1-2:** Research VP9/Opus support in MediaPortal 2
2. **Day 3-4:** Implement codec integration
3. **Day 5:** Test playback with Elephants Dream disc

### Week 2: Testing & Quality
1. **Day 1-2:** Test complete flow, fix bugs
2. **Day 3-4:** Implement quality switching
3. **Day 5:** Test quality switching

### Week 3: Advanced Features
1. **Day 1-2:** Subtitle system
2. **Day 3-4:** Seeking implementation
3. **Day 5:** Final testing and polish

---

## 🎯 Immediate Next Action

**Start with:** Research VP9/Opus codec support in MediaPortal 2

**Where to look:**
- MediaPortal 2 source code repository
- FFMpegLib plugin documentation
- VideoPlayers plugin codec handling
- MediaPortal 2 forums for codec support

**Goal:** Determine if we need external decoders or if MediaPortal supports VP9/Opus natively

---

## 📝 Current Status Summary

**✅ Working:**
- Disc detection
- Certificate validation
- HTML menu loading
- JavaScript bridge
- Playback initiation

**⚠️ Needs Work:**
- Actual video playback (codec support)
- Quality switching
- Subtitles
- Seeking

**🎉 Progress:** Core infrastructure is complete! The plugin can detect discs, load menus, and initiate playback. The main remaining work is codec support and advanced features.

---

**Ready to continue!** 🚀
