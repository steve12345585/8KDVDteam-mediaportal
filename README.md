# 8KDVD Player Plugin for MediaPortal 2

**Technology:** WebView2 (Chromium Edge)  
**Base:** OnlineVideos plugin patterns  
**Purpose:** Play 8KDVD discs with HTML menu support

---

## Project Structure

```
8KDVDPlayer/
├── plugin.xml                    # Plugin registration
├── EightKDVDPlayer.dll          # Main plugin assembly (to be built)
├── Source/                      # Source code (C#)
│   ├── Core/                    # Core functionality
│   │   ├── DiscDetector.cs
│   │   ├── CertificateValidator.cs
│   │   └── BootSequence.cs
│   ├── Models/                  # Workflow models
│   │   ├── EightKDVDWorkflowModel.cs
│   │   └── WebViewHelperModel.cs
│   ├── Player/                   # Player integration
│   │   ├── EightKDVDPlayerBuilder.cs
│   │   ├── EightKDVDPlayer.cs
│   │   ├── VP9Decoder.cs
│   │   └── OpusDecoder.cs
│   ├── WebView/                 # WebView2 integration
│   │   ├── WebViewControl.cs
│   │   └── JavaScriptBridge.cs
│   └── Configuration/           # Settings
│       ├── DefaultQuality.cs
│       └── AutoSwitch.cs
├── Skin/
│   └── default/
│       ├── screens/
│       │   ├── EightKDVD-main.xaml      # Main menu (with WebView2)
│       │   └── EightKDVD-player.xaml    # Player screen
│       ├── workflow/
│       │   └── EightKDVD-actions.xml    # Navigation
│       └── images/                      # Images
└── Language/
    └── strings_en.xml                   # Localization
```

---

## Key Features

1. **WebView2 HTML Menu** - Renders `weblauncher.html` from disc
2. **Disc Detection** - Auto-detects 8KDVD discs
3. **Certificate Verification** - C-Logic security check
4. **VP9/Opus Playback** - High-quality video/audio
5. **Subtitle Support** - SRT file parsing and overlay
6. **Quality Switching** - Adaptive quality based on hardware

---

## Dependencies

- MediaPortal 2
- Microsoft WebView2 Runtime
- FFMpegLib (for VP9/Opus if needed)

---

## Development Status

🚧 **In Development**

- [x] Plugin structure created
- [ ] Source code implementation
- [ ] WebView2 integration
- [ ] Disc detection
- [ ] Player integration
- [ ] Testing

---

## Based On

- **OnlineVideos Plugin** - WebView2 integration patterns
- **VideoPlayers Plugin** - Player registration patterns
- **RemovableMediaManager** - Disc detection patterns
- **BDHandler Plugin** - Disc player patterns

---

## License

GPL
