using MediaPortal.Common.Settings;

namespace EightKDVD.Configuration
{
  /// <summary>
  /// Configuration setting for auto-play on disc insertion
  /// </summary>
  public class AutoPlay
  {
    [Setting(SettingScope.User, true)]
    public bool Enabled { get; set; } = true;
  }
}
