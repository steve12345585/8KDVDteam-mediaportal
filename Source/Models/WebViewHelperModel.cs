using System;
using System.IO;
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.MediaManagement.MediaQueries;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.Common.ResourceAccess.ResourceProviders;
using MediaPortal.Common.Services.ResourceAccess;
using EightKDVD.Services;
using EightKDVD.Core;
using EightKDVD.Player;

namespace EightKDVD.Models
{
  /// <summary>
  /// Model for WebView2 integration - handles HTML menu rendering
  /// Based on OnlineVideos WebViewPlayerHelperModel pattern
  /// </summary>
  public class WebViewHelperModel : IWorkflowModel
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();
    private string _htmlFilePath;
    
    // Quality switching state
    private string _currentQuality; // "8K", "4K", "1080p"
    private string _currentVideoFile; // Current EVO file path
    private double _lastPlayheadPosition; // Last known playhead position in seconds

    public void EnterModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      Logger.Debug("8KDVD Player: WebViewHelperModel entered");
      
      // Get disc path from service
      string discPath = DiscPathService.Instance.CurrentDiscPath;
      if (!string.IsNullOrEmpty(discPath))
      {
        Logger.Info($"8KDVD Player: Using disc path from service: {discPath}");
      }
      else
      {
        Logger.Warn("8KDVD Player: No disc path available in service");
      }
    }

    public void ExitModelContext(NavigationContext oldContext, NavigationContext newContext)
    {
      Logger.Debug("8KDVD Player: WebViewHelperModel exited");
      _htmlFilePath = null;
    }

    public void ChangeModelContext(NavigationContext oldContext, NavigationContext newContext, bool push)
    {
      // Handle context changes if needed
    }

    public void Deactivate(NavigationContext oldContext, NavigationContext newContext)
    {
      // Cleanup if needed
    }

    public void Reactivate(NavigationContext oldContext, NavigationContext newContext)
    {
      // Reactivate if needed
    }

    public void UpdateMenuItems(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
    {
      // Update menu items if needed
    }

    public ScreenUpdateMode UpdateScreen(NavigationContext context, ref string screen)
    {
      return ScreenUpdateMode.AutoWorkflowManager;
    }

    /// <summary>
    /// Gets the path to the HTML file to load in WebView2
    /// </summary>
    public string HtmlFilePath
    {
      get
      {
        if (string.IsNullOrEmpty(_htmlFilePath) && !string.IsNullOrEmpty(_currentDiscPath))
        {
          var discDetector = new Core.DiscDetector();
          _htmlFilePath = discDetector.GetWebLauncherPath(_currentDiscPath);
        }
        return _htmlFilePath;
      }
    }

    /// <summary>
    /// Gets the current disc path
    /// </summary>
    public string DiscPath => _currentDiscPath;

    /// <summary>
    /// Loads the disc path from a service or workflow context
    /// TODO: Implement proper service or context passing
    /// </summary>
    private void LoadDiscPath()
    {
      // This is a placeholder - in real implementation, this would:
      // 1. Get disc path from a service (e.g., DiscPathService)
      // 2. Or get from workflow context
      // 3. Or get from PluginStateTracker
      
      // For now, try to detect any 8KDVD disc
      var discDetector = new Core.DiscDetector();
      foreach (var drive in System.IO.DriveInfo.GetDrives())
      {
        if (drive.DriveType == System.IO.DriveType.CDRom && drive.IsReady)
        {
          if (discDetector.Is8KDVDDisc(drive.RootDirectory.FullName))
          {
            _currentDiscPath = drive.RootDirectory.FullName;
            Logger.Info($"8KDVD Player: Loaded disc path: {_currentDiscPath}");
            return;
          }
        }
      }

      Logger.Warn("8KDVD Player: No 8KDVD disc found");
    }

    /// <summary>
    /// Handles JavaScript calls from the HTML menu
    /// </summary>
    public void HandleJavaScriptCall(string functionName, string parameters)
    {
      Logger.Debug($"8KDVD Player: JavaScript call: {functionName}({parameters})");

      switch (functionName.ToLower())
      {
        case "playmovie":
          HandlePlayMovie(parameters);
          break;
        case "changequality":
          HandleChangeQuality(parameters);
          break;
        case "loadsubtitle":
          HandleLoadSubtitle(parameters);
          break;
        case "backtomainmenu":
          HandleBackToMainMenu();
          break;
        default:
          Logger.Warn($"8KDVD Player: Unknown JavaScript function: {functionName}");
          break;
      }
    }

    private void HandlePlayMovie(string parameters)
    {
      try
      {
        Logger.Info($"8KDVD Player: Play movie requested with parameters: {parameters}");
        
        // Parse startTime from parameters (8KDVD spec: playMovie(startTime) where startTime is in seconds)
        double startTime = 0;
        if (!string.IsNullOrEmpty(parameters))
        {
          if (double.TryParse(parameters, out double parsedTime))
          {
            startTime = parsedTime;
          }
          else
          {
            Logger.Warn($"8KDVD Player: Could not parse start time '{parameters}', using 0");
          }
        }

        string discPath = DiscPathService.Instance.CurrentDiscPath;
        if (string.IsNullOrEmpty(discPath))
        {
          Logger.Warn("8KDVD Player: No disc path available for playback");
          return;
        }

        // Get stream folder path (8KDVD_TS/STREAM/)
        string streamFolder = Path.Combine(discPath, "8KDVD_TS", "STREAM");
        if (!Directory.Exists(streamFolder))
        {
          Logger.Warn($"8KDVD Player: Stream folder not found: {streamFolder}");
          return;
        }

        // Find video file - prioritize quality: EVO8 > EVO4 > EVOH (per 8KDVD spec)
        // According to spec: .EVO8 (8K), .EVO4 (4K), .EVOH (1080p)
        string videoFile = null;
        string[] qualityExtensions = { "*.EVO8", "*.EVO4", "*.EVOH", "*.EVO" };
        
        foreach (string pattern in qualityExtensions)
        {
          var files = Directory.GetFiles(streamFolder, pattern, SearchOption.TopDirectoryOnly);
          if (files.Length > 0)
          {
            videoFile = files[0]; // Use first matching file
            Logger.Info($"8KDVD Player: Found video file: {videoFile} (quality: {pattern})");
            break;
          }
        }

        if (string.IsNullOrEmpty(videoFile) || !File.Exists(videoFile))
        {
          Logger.Warn("8KDVD Player: No video files found in stream folder");
          return;
        }

        // Create MediaItem for playback
        // According to 8KDVD spec: .EVO files are MP4 containers with VP9/Opus
        var resourceProviderManager = ServiceRegistration.Get<IResourceProviderManager>();
        if (resourceProviderManager == null)
        {
          Logger.Error("8KDVD Player: IResourceProviderManager service not available");
          return;
        }

        // Create resource locator from file path
        // Use LocalFsResourceProvider for local file system access
        // Format: "local:///" + full path
        string resourcePath = "local:///" + videoFile.Replace("\\", "/");
        var resourceLocator = new ResourceLocator(resourcePath);
        
        // Create MediaItem with video aspect
        // Note: Use "video/mp4" MIME type since .EVO files are MP4 containers
        // MediaPortal 2 will use DirectShow filters (LAV Filters) for VP9/Opus decoding
        var mediaItem = new MediaItem(Guid.NewGuid(), new[] { VideoAspect.Metadata });
        var videoAspect = mediaItem.Aspects.Get<VideoAspect>();
        videoAspect.SetAttribute(VideoAspect.ATTR_VIDEO_PATH, videoFile);
        videoAspect.SetAttribute(VideoAspect.ATTR_MIME_TYPE, "video/mp4"); // MP4 container - LAV Filters will decode VP9/Opus
        videoAspect.SetAttribute(VideoAspect.ATTR_TITLE, Path.GetFileNameWithoutExtension(videoFile));
        
        // Set resource locator
        mediaItem.SetResourceLocator(resourceLocator);

        Logger.Info($"8KDVD Player: Created MediaItem for playback: {videoFile}");

        // Verify codec support before starting playback
        if (!CodecVerifier.IsLAVFiltersInstalled())
        {
          Logger.Warn("8KDVD Player: LAV Filters not detected - playback may fail");
          Logger.Warn($"8KDVD Player: {CodecVerifier.GetCodecStatusMessage()}");
          // Continue anyway - user might have other codec pack installed
        }

        // Get IPlayerManager and start playback
        var playerManager = ServiceRegistration.Get<IPlayerManager>();
        if (playerManager == null)
        {
          Logger.Error("8KDVD Player: IPlayerManager service not available");
          return;
        }

        // Start playback
        // MediaPortal 2 will use DirectShow filters (LAV Filters) for VP9/Opus decoding
        // The .EVO file is an MP4 container, so MediaPortal will use standard MP4 playback
        // LAV Filters will automatically decode VP9 video and Opus audio streams
        playerManager.Play(mediaItem);
        
        // Update state for quality switching
        _currentVideoFile = videoFile;
        _lastPlayheadPosition = startTime;
        
        // Determine current quality from file extension
        string extension = Path.GetExtension(videoFile).ToUpper();
        switch (extension)
        {
          case ".EVO8":
            _currentQuality = "8K";
            break;
          case ".EVO4":
            _currentQuality = "4K";
            break;
          case ".EVOH":
            _currentQuality = "1080p";
            break;
          case ".3D4":
            _currentQuality = "3D 4K";
            break;
          default:
            _currentQuality = "Unknown";
            break;
        }
        
        // Seek to start time if specified (8KDVD spec: playMovie(startTime) seeks to that position)
        if (startTime > 0)
        {
          Logger.Info($"8KDVD Player: Seeking to start time: {startTime} seconds");
          // Wait a moment for playback to start, then seek
          System.Threading.Thread.Sleep(500);
          try
          {
            var player = playerManager.CurrentPlayer;
            if (player != null)
            {
              long seekTimeMs = (long)(startTime * 1000);
              player.Seek(seekTimeMs);
              Logger.Info($"8KDVD Player: Seeked to {startTime} seconds ({seekTimeMs}ms)");
            }
          }
          catch (Exception ex)
          {
            Logger.Warn("8KDVD Player: Could not seek to start time", ex);
          }
        }

        Logger.Info($"8KDVD Player: Playback initiated for: {videoFile} (Quality: {_currentQuality})");
        Logger.Info("8KDVD Player: MediaPortal will use DirectShow filters (LAV Filters) for VP9/Opus decoding");
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error handling play movie", ex);
      }
    }

    private void HandleChangeQuality(string parameters)
    {
      try
      {
        Logger.Info($"8KDVD Player: Change quality requested: {parameters}");
        
        // Parse quality parameter (8KDVD spec: "8K", "4K", "1080p", or "3D4")
        string requestedQuality = parameters?.Trim().ToUpper();
        if (string.IsNullOrEmpty(requestedQuality))
        {
          Logger.Warn("8KDVD Player: Quality parameter is empty");
          return;
        }

        // Map quality string to file extension pattern
        string filePattern = null;
        string qualityName = null;
        
        switch (requestedQuality)
        {
          case "8K":
            filePattern = "*.EVO8";
            qualityName = "8K";
            break;
          case "4K":
            filePattern = "*.EVO4";
            qualityName = "4K";
            break;
          case "1080P":
          case "1080":
          case "HD":
            filePattern = "*.EVOH";
            qualityName = "1080p";
            break;
          case "3D4":
          case "3D":
            filePattern = "*.3D4";
            qualityName = "3D 4K";
            break;
          default:
            Logger.Warn($"8KDVD Player: Unknown quality parameter: {requestedQuality}");
            return;
        }

        // Check if quality is already active
        if (_currentQuality == qualityName)
        {
          Logger.Info($"8KDVD Player: Quality {qualityName} is already active");
          return;
        }

        Logger.Info($"8KDVD Player: Switching to quality: {qualityName} (pattern: {filePattern})");

        // Get disc path
        string discPath = DiscPathService.Instance.CurrentDiscPath;
        if (string.IsNullOrEmpty(discPath))
        {
          Logger.Warn("8KDVD Player: No disc path available for quality switching");
          return;
        }

        // Get stream folder path
        string streamFolder = Path.Combine(discPath, "8KDVD_TS", "STREAM");
        if (!Directory.Exists(streamFolder))
        {
          Logger.Warn($"8KDVD Player: Stream folder not found: {streamFolder}");
          return;
        }

        // Find video file with requested quality
        var videoFiles = Directory.GetFiles(streamFolder, filePattern, SearchOption.TopDirectoryOnly);
        if (videoFiles.Length == 0)
        {
          Logger.Warn($"8KDVD Player: No {qualityName} video file found (pattern: {filePattern})");
          return;
        }

        string newVideoFile = videoFiles[0]; // Use first matching file
        Logger.Info($"8KDVD Player: Found {qualityName} video file: {newVideoFile}");

        // Get current playhead position (8KDVD spec: maintain playhead during quality switch)
        double playheadPosition = _lastPlayheadPosition;
        
        var playerManager = ServiceRegistration.Get<IPlayerManager>();
        if (playerManager != null)
        {
          try
          {
            // Try to get current player and position
            var currentPlayer = playerManager.CurrentPlayer;
            if (currentPlayer != null && currentPlayer.IsPlaying)
            {
              // Get current time in milliseconds, convert to seconds
              long currentTimeMs = currentPlayer.CurrentTime;
              playheadPosition = currentTimeMs / 1000.0;
              Logger.Info($"8KDVD Player: Current playhead position: {playheadPosition} seconds");
            }
          }
          catch (Exception ex)
          {
            Logger.Warn("8KDVD Player: Could not get current playhead position, using last known position", ex);
          }
        }

        // Stop current playback if playing
        if (playerManager != null && playerManager.CurrentPlayer != null && playerManager.CurrentPlayer.IsPlaying)
        {
          Logger.Info("8KDVD Player: Stopping current playback for quality switch");
          playerManager.Stop();
          // Give it a moment to stop
          System.Threading.Thread.Sleep(100);
        }

        // Create MediaItem for new quality file
        var resourceProviderManager = ServiceRegistration.Get<IResourceProviderManager>();
        if (resourceProviderManager == null)
        {
          Logger.Error("8KDVD Player: IResourceProviderManager service not available");
          return;
        }

        // Create resource locator
        string resourcePath = "local:///" + newVideoFile.Replace("\\", "/");
        var resourceLocator = new ResourceLocator(resourcePath);

        // Create MediaItem with video aspect
        var mediaItem = new MediaItem(Guid.NewGuid(), new[] { VideoAspect.Metadata });
        var videoAspect = mediaItem.Aspects.Get<VideoAspect>();
        videoAspect.SetAttribute(VideoAspect.ATTR_VIDEO_PATH, newVideoFile);
        videoAspect.SetAttribute(VideoAspect.ATTR_MIME_TYPE, "video/mp4"); // MP4 container
        videoAspect.SetAttribute(VideoAspect.ATTR_TITLE, Path.GetFileNameWithoutExtension(newVideoFile));
        mediaItem.SetResourceLocator(resourceLocator);

        Logger.Info($"8KDVD Player: Created MediaItem for {qualityName} quality: {newVideoFile}");

        // Start playback at saved playhead position
        if (playerManager == null)
        {
          Logger.Error("8KDVD Player: IPlayerManager service not available");
          return;
        }

        playerManager.Play(mediaItem);

        // Seek to playhead position (8KDVD spec: maintain position during quality switch)
        if (playheadPosition > 0)
        {
          Logger.Info($"8KDVD Player: Seeking to saved playhead position: {playheadPosition} seconds");
          // Note: Seeking will be implemented in the player class
          // For now, we'll attempt to seek via IPlayerManager if available
          try
          {
            // Wait a moment for playback to start
            System.Threading.Thread.Sleep(500);
            
            var player = playerManager.CurrentPlayer;
            if (player != null)
            {
              long seekTimeMs = (long)(playheadPosition * 1000);
              player.Seek(seekTimeMs);
              Logger.Info($"8KDVD Player: Seeked to {playheadPosition} seconds ({seekTimeMs}ms)");
            }
          }
          catch (Exception ex)
          {
            Logger.Warn("8KDVD Player: Could not seek to playhead position", ex);
          }
        }

        // Update state
        _currentQuality = qualityName;
        _currentVideoFile = newVideoFile;
        _lastPlayheadPosition = playheadPosition;

        Logger.Info($"8KDVD Player: Quality switched to {qualityName} at position {playheadPosition} seconds");
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error handling quality change", ex);
      }
    }

    private void HandleLoadSubtitle(string parameters)
    {
      Logger.Info($"8KDVD Player: Load subtitle requested: {parameters}");
      // TODO: Implement subtitle loading
    }

    private void HandleBackToMainMenu()
    {
      try
      {
        Logger.Info("8KDVD Player: Back to main menu requested");
        
        // Navigate back to main menu workflow state
        var workflowManager = ServiceRegistration.Get<IWorkflowManager>();
        if (workflowManager != null)
        {
          var mainMenuStateId = new Guid("3C4D5E6F-7A8B-9C0D-1E2F-3A4B5C6D7E8F");
          workflowManager.NavigatePush(mainMenuStateId);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error navigating to main menu", ex);
      }
    }
  }
}
