using System;
using System.IO;
using Microsoft.Win32;
using MediaPortal.Common;
using MediaPortal.Common.Logging;

namespace EightKDVD.Player
{
  /// <summary>
  /// Verifies that VP9/Opus codec support is available via LAV Filters
  /// </summary>
  public static class CodecVerifier
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();
    private static bool? _lavFiltersInstalled = null;

    /// <summary>
    /// Checks if LAV Filters are installed on the system
    /// </summary>
    public static bool IsLAVFiltersInstalled()
    {
      if (_lavFiltersInstalled.HasValue)
        return _lavFiltersInstalled.Value;

      try
      {
        // Check for LAV Filters in registry
        // LAV Filters typically register under DirectShow filters
        using (var key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{EE30215D-164F-4A92-A4EB-9D4C13390F9F}"))
        {
          // This is the CLSID for LAV Video Decoder
          if (key != null)
          {
            _lavFiltersInstalled = true;
            Logger.Info("8KDVD Player: LAV Filters detected - VP9/Opus support available");
            return true;
          }
        }

        // Alternative check: Look for LAV Splitter
        using (var key = Registry.ClassesRoot.OpenSubKey(@"CLSID\{B98D13E7-55DB-4385-A33D-09FD1BA26338}"))
        {
          if (key != null)
          {
            _lavFiltersInstalled = true;
            Logger.Info("8KDVD Player: LAV Filters detected via Splitter - VP9/Opus support available");
            return true;
          }
        }

        _lavFiltersInstalled = false;
        Logger.Warn("8KDVD Player: LAV Filters not detected - VP9/Opus playback may not work");
        return false;
      }
      catch (Exception ex)
      {
        Logger.Warn("8KDVD Player: Error checking for LAV Filters", ex);
        _lavFiltersInstalled = false;
        return false;
      }
    }

    /// <summary>
    /// Verifies codec support and returns a user-friendly message
    /// </summary>
    public static string GetCodecStatusMessage()
    {
      if (IsLAVFiltersInstalled())
      {
        return "VP9/Opus codec support available via LAV Filters";
      }
      else
      {
        return "LAV Filters not detected. Please install LAV Filters for VP9/Opus playback support.\n" +
               "Download from: https://github.com/Nevcairiel/LAVFilters/releases";
      }
    }

    /// <summary>
    /// Resets the cached installation status (useful for testing)
    /// </summary>
    public static void ResetCache()
    {
      _lavFiltersInstalled = null;
    }
  }
}
