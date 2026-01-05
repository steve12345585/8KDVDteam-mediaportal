using System;
using System.IO;
using MediaPortal.Common;
using MediaPortal.Common.Logging;

namespace EightKDVD.Core
{
  /// <summary>
  /// Detects 8KDVD discs by checking for required files and folder structure
  /// </summary>
  public class DiscDetector
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();

    /// <summary>
    /// Checks if the specified path contains a valid 8KDVD disc
    /// </summary>
    /// <param name="discPath">Path to the disc root (e.g., "D:\")</param>
    /// <returns>True if 8KDVD disc is detected</returns>
    public bool Is8KDVDDisc(string discPath)
    {
      try
      {
        if (string.IsNullOrEmpty(discPath) || !Directory.Exists(discPath))
          return false;

        Logger.Debug($"8KDVD Player: Checking disc structure at {discPath}");

        // Check for required 8KDVD structure:
        // 1. CERTIFICATE/Certificate.html
        // 2. 8KDVD_TS/ADV_OBJ/weblauncher.html
        // 3. 8KDVD_TS/STREAM/ folder

        bool hasCertificate = File.Exists(Path.Combine(discPath, "CERTIFICATE", "Certificate.html"));
        bool hasWebLauncher = File.Exists(Path.Combine(discPath, "8KDVD_TS", "ADV_OBJ", "weblauncher.html"));
        bool hasStreamFolder = Directory.Exists(Path.Combine(discPath, "8KDVD_TS", "STREAM"));

        bool is8KDVD = hasCertificate && hasWebLauncher && hasStreamFolder;

        if (is8KDVD)
        {
          Logger.Info($"8KDVD Player: Valid 8KDVD disc detected at {discPath}");
        }
        else
        {
          Logger.Debug($"8KDVD Player: Disc at {discPath} is not a valid 8KDVD disc");
        }

        return is8KDVD;
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error detecting disc at {discPath}", ex);
        return false;
      }
    }

    /// <summary>
    /// Gets the path to the weblauncher.html file on the disc
    /// </summary>
    /// <param name="discPath">Path to the disc root</param>
    /// <returns>Full path to weblauncher.html, or null if not found</returns>
    public string GetWebLauncherPath(string discPath)
    {
      try
      {
        string launcherPath = Path.Combine(discPath, "8KDVD_TS", "ADV_OBJ", "weblauncher.html");
        if (File.Exists(launcherPath))
        {
          return launcherPath;
        }
        return null;
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error getting web launcher path for {discPath}", ex);
        return null;
      }
    }

    /// <summary>
    /// Gets the path to the certificate file on the disc
    /// </summary>
    /// <param name="discPath">Path to the disc root</param>
    /// <returns>Full path to Certificate.html, or null if not found</returns>
    public string GetCertificatePath(string discPath)
    {
      try
      {
        string certPath = Path.Combine(discPath, "CERTIFICATE", "Certificate.html");
        if (File.Exists(certPath))
        {
          return certPath;
        }
        return null;
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error getting certificate path for {discPath}", ex);
        return null;
      }
    }

    /// <summary>
    /// Gets the path to the STREAM folder on the disc
    /// </summary>
    /// <param name="discPath">Path to the disc root</param>
    /// <returns>Full path to STREAM folder, or null if not found</returns>
    public string GetStreamFolderPath(string discPath)
    {
      try
      {
        string streamPath = Path.Combine(discPath, "8KDVD_TS", "STREAM");
        if (Directory.Exists(streamPath))
        {
          return streamPath;
        }
        return null;
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error getting stream folder path for {discPath}", ex);
        return null;
      }
    }
  }
}
