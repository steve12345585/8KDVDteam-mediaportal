using System;
using System.IO;
using System.Linq;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PluginManager;
using MediaPortal.UI.Presentation.Workflow;
using EightKDVD.Services;
using EightKDVD.Core;

namespace EightKDVD
{
  /// <summary>
  /// Plugin state tracker for detecting 8KDVD disc insertion and auto-launching
  /// Based on RemovableMediaManager pattern
  /// </summary>
  public class PluginStateTracker : IPluginStateTracker
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();
    private System.IO.FileSystemWatcher _discWatcher;
    private readonly DiscDetector _discDetector = new DiscDetector();
    private readonly CertificateValidator _certValidator = new CertificateValidator();

    public void Activated(PluginRuntime pluginRuntime)
    {
      Logger.Info("8KDVD Player: Plugin activated, starting disc detection");
      StartDiscMonitoring();
    }

    public void Deactivated(PluginRuntime pluginRuntime)
    {
      Logger.Info("8KDVD Player: Plugin deactivated, stopping disc detection");
      StopDiscMonitoring();
    }

    public void Continue()
    {
      Logger.Info("8KDVD Player: Plugin continued");
      StartDiscMonitoring();
    }

    public void Pause()
    {
      Logger.Info("8KDVD Player: Plugin paused");
      StopDiscMonitoring();
    }

    public void Shutdown()
    {
      Logger.Info("8KDVD Player: Plugin shutting down");
      StopDiscMonitoring();
    }

    public bool RequestEnd()
    {
      Logger.Info("8KDVD Player: Request end");
      StopDiscMonitoring();
      return true; // Allow plugin to end
    }

    public void Stop()
    {
      Logger.Info("8KDVD Player: Stop requested");
      StopDiscMonitoring();
    }

    private void StartDiscMonitoring()
    {
      try
      {
        // Monitor all drive letters for disc insertion
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.CDRom))
        {
          CheckDrive(drive);
        }

        // Set up file system watcher for disc insertion
        // Note: This is a simplified version - may need enhancement
        Logger.Info("8KDVD Player: Disc monitoring started");
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error starting disc monitoring", ex);
      }
    }

    private void StopDiscMonitoring()
    {
      try
      {
        if (_discWatcher != null)
        {
          _discWatcher.Dispose();
          _discWatcher = null;
        }
        Logger.Info("8KDVD Player: Disc monitoring stopped");
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error stopping disc monitoring", ex);
      }
    }

    private void CheckDrive(DriveInfo drive)
    {
      try
      {
        if (!drive.IsReady)
          return;

        string drivePath = drive.RootDirectory.FullName;
        Logger.Debug($"8KDVD Player: Checking drive {drivePath}");

        if (_discDetector.Is8KDVDDisc(drivePath))
        {
          Logger.Info($"8KDVD Player: 8KDVD disc detected on {drivePath}");
          
          // Validate certificate before launching
          if (_certValidator.ValidateCertificate(drivePath))
          {
            // Store disc path in service
            DiscPathService.Instance.SetDiscPath(drivePath);
            Launch8KDVDMenu(drivePath);
          }
          else
          {
            Logger.Warn($"8KDVD Player: Certificate validation failed for disc at {drivePath}");
            // TODO: Show error message to user
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error checking drive {drive.RootDirectory.FullName}", ex);
      }
    }

    private void Launch8KDVDMenu(string discPath)
    {
      try
      {
        // Navigate to 8KDVD main menu workflow state
        var workflowManager = ServiceRegistration.Get<IWorkflowManager>();
        if (workflowManager != null)
        {
          // Store disc path for use by the workflow model
          // This would typically be done via a service or shared state
          Logger.Info($"8KDVD Player: Launching menu for disc at {discPath}");
          
          // Navigate to main menu state
          // State ID: 3C4D5E6F-7A8B-9C0D-1E2F-3A4B5C6D7E8F (from plugin.xml)
          var stateId = new Guid("3C4D5E6F-7A8B-9C0D-1E2F-3A4B5C6D7E8F");
          workflowManager.NavigatePush(stateId);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error launching menu", ex);
      }
    }
  }
}
