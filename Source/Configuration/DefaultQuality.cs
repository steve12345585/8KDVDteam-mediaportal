using MediaPortal.Common.Configuration.ConfigurationClasses;
using MediaPortal.Common.Settings;

namespace EightKDVD.Configuration
{
  /// <summary>
  /// Settings class - stores the actual configuration value
  /// </summary>
  public class EightKDVDSettings
  {
    [Setting(SettingScope.User, "1080p")]
    public string DefaultQuality { get; set; } = "1080p";
    
    [Setting(SettingScope.User, false)]
    public bool AutoSwitchQuality { get; set; } = false;
    
    [Setting(SettingScope.User, false)]
    public bool AutoPlayDisc { get; set; } = false;
  }

  /// <summary>
  /// Configuration class - provides UI editor for default quality
  /// TODO: Fix - Select class not found. Need to find correct base class or namespace.
  /// Temporarily disabled to allow project to build.
  /// </summary>
  public class DefaultQuality
  {
    // TODO: Implement once correct base class is found
    // Should inherit from Select or similar configuration editor class
  }
}
