using System;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using EightKDVD.Models;

namespace EightKDVD.WebView
{
  /// <summary>
  /// Bridge between WebView2 JavaScript calls and MediaPortal functions
  /// </summary>
  public class JavaScriptBridge
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();
    private readonly WebViewHelperModel _model;

    public JavaScriptBridge(WebViewHelperModel model)
    {
      _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    /// <summary>
    /// Handles JavaScript function calls from WebView2
    /// </summary>
    public void HandleCall(string functionName, object parameters)
    {
      try
      {
        Logger.Debug($"8KDVD Player: JavaScript bridge handling: {functionName}({parameters})");
        _model.HandleJavaScriptCall(functionName, parameters?.ToString() ?? "");
      }
      catch (Exception ex)
      {
        Logger.Error($"8KDVD Player: Error in JavaScript bridge for {functionName}", ex);
      }
    }
  }
}
