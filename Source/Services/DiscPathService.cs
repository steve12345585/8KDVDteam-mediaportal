using System;
using MediaPortal.Common;
using MediaPortal.Common.Logging;

namespace EightKDVD.Services
{
  /// <summary>
  /// Service to share disc path between components
  /// Singleton service registered with MediaPortal
  /// </summary>
  public class DiscPathService
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();
    private static DiscPathService _instance;
    private string _currentDiscPath;
    private readonly object _lockObject = new object();

    private DiscPathService()
    {
      Logger.Debug("8KDVD Player: DiscPathService created");
    }

    public static DiscPathService Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (typeof(DiscPathService))
          {
            if (_instance == null)
            {
              _instance = new DiscPathService();
            }
          }
        }
        return _instance;
      }
    }

    /// <summary>
    /// Gets the current disc path
    /// </summary>
    public string CurrentDiscPath
    {
      get
      {
        lock (_lockObject)
        {
          return _currentDiscPath;
        }
      }
    }

    /// <summary>
    /// Sets the current disc path
    /// </summary>
    public void SetDiscPath(string discPath)
    {
      lock (_lockObject)
      {
        if (_currentDiscPath != discPath)
        {
          _currentDiscPath = discPath;
          Logger.Info($"8KDVD Player: Disc path set to: {discPath}");
          OnDiscPathChanged?.Invoke(discPath);
        }
      }
    }

    /// <summary>
    /// Clears the current disc path
    /// </summary>
    public void ClearDiscPath()
    {
      lock (_lockObject)
      {
        if (_currentDiscPath != null)
        {
          Logger.Info("8KDVD Player: Disc path cleared");
          _currentDiscPath = null;
          OnDiscPathChanged?.Invoke(null);
        }
      }
    }

    /// <summary>
    /// Event fired when disc path changes
    /// </summary>
    public event Action<string> OnDiscPathChanged;
  }
}
