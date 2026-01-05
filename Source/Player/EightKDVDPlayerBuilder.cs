using System;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Players;

namespace EightKDVD.Player
{
  /// <summary>
  /// Player builder for 8KDVD discs
  /// Based on VideoPlayers plugin pattern
  /// </summary>
  public class EightKDVDPlayerBuilder : IPlayerBuilder
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();

    public IPlayer CreatePlayer(MediaItem mediaItem)
    {
      try
      {
        Logger.Debug($"8KDVD Player: Creating player for media item: {mediaItem?.Title}");

        // Check if this is an 8KDVD media item
        if (Is8KDVDMediaItem(mediaItem))
        {
          Logger.Info("8KDVD Player: Creating 8KDVD player instance");
          return new EightKDVDPlayer(mediaItem);
        }

        return null;
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error creating player", ex);
        return null;
      }
    }

    private bool Is8KDVDMediaItem(MediaItem mediaItem)
    {
      if (mediaItem == null)
        return false;

      // Check if media item has 8KDVD mime type or .EVO extension
      // This would typically be set by a metadata extractor or resource provider
      var resourceAccessor = mediaItem.GetResourceLocator();
      if (resourceAccessor != null)
      {
        string path = resourceAccessor.NativeResourcePath?.ToString() ?? "";
        if (path.EndsWith(".EVO", StringComparison.OrdinalIgnoreCase))
        {
          return true;
        }
      }

      return false;
    }
  }
}
