# VP9/Opus Codec Support Research - MediaPortal 2

## 🔍 Research Summary

### Key Finding: **VP9 and Opus ARE Supported via LAV Filters**

MediaPortal 2 uses **DirectShow** for media playback, which means it can support any codec installed on the Windows system. By default, MediaPortal 2 uses **LAV Filters**, which **DO support VP9 and Opus codecs**.

---

## 📋 MediaPortal 2 Codec Architecture

### 1. **DirectShow-Based Playback**
- MediaPortal 2 uses DirectShow filters for video/audio decoding
- Supports any codec that has DirectShow filters installed
- Default: LAV Filters (LAV Splitter, LAV Video Decoder, LAV Audio Decoder)

### 2. **Native Codec Support (VideoPlayers Plugin)**
From `Plugins/VideoPlayers/plugin.xml`, MediaPortal 2 has configuration for:
- **MPEG2** (VideoMPEG2Codec)
- **MPEG4** (VideoMPEG4Codec)
- **AVC/H.264** (VideoAVCCodec)
- **HEVC/H.265** (VideoHEVCCodec)

**Note:** VP9 and Opus are NOT explicitly listed in the VideoPlayers plugin configuration, meaning they rely on external DirectShow filters (LAV Filters).

### 3. **FFMpegLib Plugin**
- Purpose: Provides FFMpeg functionality for **metadata extractors**
- **NOT used for playback** - only for extracting metadata
- Plugin ID: `{8B61C331-DBE8-4E56-8A1D-11C32D8C850D}`

---

## ✅ VP9/Opus Support via LAV Filters

### LAV Filters Support:
- ✅ **VP9 Video** - Supported in LAV Video Decoder
- ✅ **Opus Audio** - Supported in LAV Audio Decoder
- ✅ **MP4 Container** - Supported in LAV Splitter (MPEG-4 Part 14)

### Requirements:
1. **LAV Filters must be installed** on the system
2. **LAV Filters must be configured** as default decoders in MediaPortal 2
3. **MediaPortal 2 must use DirectShow** for playback (default behavior)

---

## 🎯 Implementation Strategy

### Option 1: Use DirectShow with LAV Filters (Recommended)
**Pros:**
- ✅ No additional code needed - MediaPortal 2 handles it automatically
- ✅ Works if LAV Filters are installed
- ✅ Hardware acceleration support (if GPU supports VP9)

**Cons:**
- ⚠️ Requires LAV Filters to be installed
- ⚠️ May not work if user has different codec pack installed

**Implementation:**
- Use MediaPortal's standard `IPlayerManager` (already implemented)
- Set MIME type to `video/mp4` or `video/8kdvd`
- MediaPortal will use DirectShow filters automatically
- LAV Filters will handle VP9/Opus decoding

### Option 2: Direct FFMpeg Integration (Advanced)
**Pros:**
- ✅ Full control over decoding
- ✅ No dependency on external codec packs
- ✅ Can optimize for 8K playback

**Cons:**
- ❌ Much more complex implementation
- ❌ Need to handle rendering, audio output, etc.
- ❌ Significant development time

**Implementation:**
- Use FFMpeg.AutoGen or similar library
- Create custom decoder wrapper
- Integrate with MediaPortal's rendering system

---

## 📝 Recommended Approach

### **Use DirectShow with LAV Filters (Option 1)**

**Why:**
1. MediaPortal 2 already supports this architecture
2. LAV Filters are widely used and well-maintained
3. Hardware acceleration support for VP9
4. Minimal code changes needed

**What to do:**
1. **Verify LAV Filters support:**
   - Check if LAV Filters are installed
   - Verify VP9/Opus support in LAV Filters version

2. **Use standard MediaPortal playback:**
   - Already implemented via `IPlayerManager.Play()`
   - MediaPortal will use DirectShow filters automatically
   - LAV Filters will decode VP9/Opus if installed

3. **Handle fallback:**
   - Check if playback fails
   - Show error message if codec not available
   - Provide instructions for installing LAV Filters

4. **Optional: Direct FFMpeg integration:**
   - Only if DirectShow approach doesn't work
   - Or if we need more control over decoding

---

## 🔧 Implementation Details

### Current Implementation Status:
✅ **Playback initiation** - `HandlePlayMovie()` uses `IPlayerManager`
✅ **MediaItem creation** - Proper `VideoAspect` and resource locator
✅ **MIME type** - Set to `video/8kdvd` (custom, will need to map to DirectShow)

### What Needs to be Done:

1. **Verify MIME type handling:**
   - MediaPortal may need `video/mp4` instead of `video/8kdvd`
   - Or register custom MIME type mapping

2. **Test with LAV Filters:**
   - Install LAV Filters
   - Test VP9/Opus playback
   - Verify hardware acceleration

3. **Error handling:**
   - Detect if codec is not available
   - Provide user-friendly error messages
   - Guide users to install LAV Filters

4. **Optional enhancements:**
   - Detect hardware capabilities
   - Enable/disable hardware acceleration
   - Quality switching based on performance

---

## 📚 References

- **MediaPortal 2 VideoPlayers Plugin:** `Plugins/VideoPlayers/plugin.xml`
- **LAV Filters:** https://github.com/Nevcairiel/LAVFilters
- **DirectShow:** Windows DirectShow API
- **8KDVD Spec:** VP9 video, Opus audio, MP4 container

---

## ✅ Conclusion

**VP9 and Opus ARE supported in MediaPortal 2** via LAV Filters and DirectShow. The current implementation using `IPlayerManager` should work if LAV Filters are installed. No major code changes needed - just ensure proper MIME type handling and add error detection.

**Next Steps:**
1. Test current implementation with LAV Filters installed
2. Verify MIME type mapping works correctly
3. Add error handling for missing codecs
4. Consider direct FFMpeg integration only if needed

---

**Status:** Research complete. Ready for implementation testing! 🚀
