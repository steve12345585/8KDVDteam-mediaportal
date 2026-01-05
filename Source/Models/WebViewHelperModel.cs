using System;
using System.IO;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.Common.MediaManagement;
using EightKDVD.Services;
using EightKDVD.Core;

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
        
        // Parse startTime from parameters (could be in seconds or timecode format)
        // For now, assume it's a time in seconds or timecode string
        string discPath = DiscPathService.Instance.CurrentDiscPath;
        if (string.IsNullOrEmpty(discPath))
        {
          Logger.Warn("8KDVD Player: No disc path available for playback");
          return;
        }

        // Get stream folder path
        var discDetector = new DiscDetector();
        string streamFolder = discDetector.GetStreamFolderPath(discPath);
        
        if (string.IsNullOrEmpty(streamFolder))
        {
          Logger.Warn("8KDVD Player: Stream folder not found");
          return;
        }

        // Find video file (typically .EVO files in STREAM folder)
        // For now, get first .EVO file (in real implementation, would parse playlist)
        var videoFiles = Directory.GetFiles(streamFolder, "*.EVO", SearchOption.TopDirectoryOnly);
        if (videoFiles.Length == 0)
        {
          Logger.Warn("8KDVD Player: No video files found in stream folder");
          return;
        }

        string videoFile = videoFiles[0]; // Use first file for now
        
        // TODO: Parse startTime and seek to that position
        // TODO: Create MediaItem and start playback via IPlayerManager
        Logger.Info($"8KDVD Player: Would play: {videoFile} from time: {parameters}");
        
        // This would typically use IPlayerManager to start playback
        // var playerManager = ServiceRegistration.Get<IPlayerManager>();
        // playerManager.PlayMediaItem(mediaItem);
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error handling play movie", ex);
      }
    }

    private void HandleChangeQuality(string parameters)
    {
      Logger.Info($"8KDVD Player: Change quality requested: {parameters}");
      // TODO: Implement quality switching
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
