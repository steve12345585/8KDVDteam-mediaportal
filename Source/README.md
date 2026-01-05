# 8KDVD Player - Source Code

This folder will contain the C# source code for the 8KDVD Player plugin.

## Planned Structure

```
Source/
├── Core/
│   ├── DiscDetector.cs          # Detect 8KDVD discs
│   ├── CertificateValidator.cs  # C-Logic verification
│   └── BootSequence.cs          # Boot sequence handler
├── Models/
│   ├── EightKDVDWorkflowModel.cs    # Main workflow model
│   └── WebViewHelperModel.cs        # WebView2 integration
├── Player/
│   ├── EightKDVDPlayerBuilder.cs    # Player builder
│   ├── EightKDVDPlayer.cs           # Main player class
│   ├── VP9Decoder.cs                # VP9 video decoder
│   └── OpusDecoder.cs               # Opus audio decoder
├── WebView/
│   ├── WebViewControl.cs            # WebView2 control wrapper
│   └── JavaScriptBridge.cs          # JavaScript bridge
├── Configuration/
│   ├── DefaultQuality.cs            # Default quality setting
│   └── AutoSwitch.cs                # Auto-switch setting
└── PluginStateTracker.cs            # Disc detection tracker
```

## Key Classes to Implement

### 1. PluginStateTracker
- Monitor disc drives
- Detect 8KDVD disc insertion
- Auto-launch plugin

### 2. WebViewHelperModel
- Embed WebView2 control
- Load `weblauncher.html` from disc
- Handle JavaScript calls
- Bridge to MediaPortal functions

### 3. EightKDVDPlayerBuilder
- Register player builder
- Create 8KDVD player instances
- Handle `.EVO` file playback

### 4. EightKDVDPlayer
- VP9/Opus codec support
- Quality switching
- Chapter navigation
- Subtitle overlay

## References

- **OnlineVideos** - WebView2 integration
- **VideoPlayers** - Player registration
- **RemovableMediaManager** - Disc detection
