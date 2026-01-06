# MediaPortal 2 API Fixes Needed

## Critical Issues

The code uses incorrect MediaPortal 2 APIs. These need to be fixed based on the actual MediaPortal 2 API documentation.

## Issues to Fix

1. **ResourceLocator Constructor**
   - Error: `cannot convert from 'string' to 'ResourcePath'`
   - Fix: Use `ResourcePath` instead of string
   - Example: `var resourcePath = ResourcePath.BuildLocalFsPath(videoFile);`

2. **MediaItem Constructor**
   - Error: Wrong constructor parameters
   - Fix: Check MediaPortal 2 API for correct constructor
   - May need: `new MediaItem(Guid.NewGuid(), aspectsDictionary)` or similar

3. **VideoAspect Attributes**
   - Error: `ATTR_VIDEO_PATH`, `ATTR_MIME_TYPE`, `ATTR_TITLE` don't exist
   - Fix: Use correct attribute names from VideoAspect
   - May need: `VideoAspect.ATTR_PATH`, `VideoAspect.ATTR_MIME`, etc.

4. **MediaItem.SetResourceLocator**
   - Error: Method doesn't exist
   - Fix: Resource locator may need to be set during construction or via different method

5. **IPlayerManager API**
   - Error: `Play`, `CurrentPlayer`, `Stop` don't exist
   - Fix: Check correct IPlayerManager interface methods
   - May need: `StartPlayback()`, `GetCurrentPlayer()`, etc.

6. **PlayerState Enum**
   - Error: `PlayerState.Paused` doesn't exist
   - Fix: Use correct enum value (may be `PlayerState.Paused` or different name)

7. **IResourceProviderManager**
   - Error: Type doesn't exist
   - Fix: May not be needed, or use different service

8. **System.Text.Json**
   - Error: Namespace doesn't exist in .NET Framework 4.8
   - Fix: Use `Newtonsoft.Json` or `System.Web.Script.Serialization`

9. **_currentDiscPath**
   - Error: Variable doesn't exist
   - Fix: Use `DiscPathService.Instance.CurrentDiscPath` instead

## Temporary Solution

For now, I'll comment out problematic code and add TODO comments indicating what needs to be fixed based on actual MediaPortal 2 API documentation.
