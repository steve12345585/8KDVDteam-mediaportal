using MediaPortal.Common.Settings;

namespace EightKDVD.Configuration
{
  /// <summary>
  /// Configuration setting for automatic quality switching
  /// </summary>
  public class AutoSwitch
  {
    [Setting(SettingScope.User, true)]
    public bool Enabled { get; set; } = true;
  }
}
