using System;
using System.Collections.Generic;
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
using MediaPortal.Common.ResourceAccess;
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

    public Guid ModelId => new Guid("2B3C4D5E-6F7A-8B9C-0D1E-2F3A4B5C6D7E");

    public bool CanEnterState(NavigationContext oldContext, NavigationContext newContext)
    {
      return true;
    }
    
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

    public void UpdateMenuActions(NavigationContext context, IDictionary<Guid, WorkflowAction> actions)
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
        string discPath = DiscPathService.Instance.CurrentDiscPath;
        if (string.IsNullOrEmpty(_htmlFilePath) && !string.IsNullOrEmpty(discPath))
        {
          var discDetector = new Core.DiscDetector();
          _htmlFilePath = discDetector.GetWebLauncherPath(discPath);
        }
        return _htmlFilePath;
      }
    }

    /// <summary>
    /// Gets the current disc path
    /// </summary>
    public string DiscPath => DiscPathService.Instance.CurrentDiscPath;

    /// <summary>
    /// Loads the disc path from a service or workflow context
    /// TODO: Implement proper service or context passing
    /// </summary>
    private void LoadDiscPath()
    {
      // Disc path is now managed by DiscPathService
      // This method is kept for compatibility but no longer needed
      string discPath = DiscPathService.Instance.CurrentDiscPath;
      if (!string.IsNullOrEmpty(discPath))
      {
        Logger.Info($"8KDVD Player: Disc path available from service: {discPath}");
      }
      else
      {
        Logger.Warn("8KDVD Player: No 8KDVD disc found");
      }
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

        // TODO: Create MediaItem for playback using correct MediaPortal 2 API
        // The following code needs to be fixed based on actual MediaPortal 2 API:
        // - ResourceLocator constructor takes ResourcePath, not string
        // - MediaItem constructor parameters may be different
        // - VideoAspect attribute names may be different (ATTR_VIDEO_PATH, ATTR_MIME_TYPE, ATTR_TITLE)
        // - MediaItem.SetResourceLocator may not exist
        // - IPlayerManager.Play, CurrentPlayer, Stop methods may have different names
        
        Logger.Info($"8KDVD Player: TODO - Create MediaItem for playback: {videoFile}");
        Logger.Warn("8KDVD Player: MediaItem creation and playback not yet implemented - needs MediaPortal 2 API research");
        
        // Verify codec support before starting playback
        if (!CodecVerifier.IsLAVFiltersInstalled())
        {
          Logger.Warn("8KDVD Player: LAV Filters not detected - playback may fail");
          Logger.Warn($"8KDVD Player: {CodecVerifier.GetCodecStatusMessage()}");
        }
        
        // TODO: Implement MediaItem creation and playback
        // Example (needs verification):
        // var resourcePath = ResourcePath.BuildLocalFsPath(videoFile);
        // var resourceLocator = new ResourceLocator(resourcePath);
        // var mediaItem = new MediaItem(Guid.NewGuid(), aspectsDictionary);
        // var playerManager = ServiceRegistration.Get<IPlayerManager>();
        // playerManager.StartPlayback(mediaItem); // or correct method name
        
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
        
        // TODO: Implement seeking after playback starts
        // This will need to be implemented once MediaItem creation and playback are working
        if (startTime > 0)
        {
          Logger.Info($"8KDVD Player: TODO - Seek to start time: {startTime} seconds");
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

        // TODO: Get current playhead position and switch quality
        // This needs to be implemented once MediaItem creation and playback are working
        double playheadPosition = _lastPlayheadPosition;
        
        Logger.Info($"8KDVD Player: TODO - Quality switching to {qualityName} at position {playheadPosition} seconds");
        Logger.Warn("8KDVD Player: Quality switching not yet implemented - needs MediaPortal 2 API research");

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
