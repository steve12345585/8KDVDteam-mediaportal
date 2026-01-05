using MediaPortal.Common.Settings;

namespace EightKDVD.Configuration
{
  /// <summary>
  /// Configuration setting for default playback quality
  /// </summary>
  public class DefaultQuality
  {
    [Setting(SettingScope.User, "1080p")]
    public string Quality { get; set; } = "1080p"; // Options: "8K", "4K", "1080p"
  }
}
