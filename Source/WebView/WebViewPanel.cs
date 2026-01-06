using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Web.WebView2.Wpf;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.UI.Presentation.Models;
using EightKDVD.Services;
using EightKDVD.Core;
using EightKDVD.WebView;
using EightKDVD.Models;

namespace EightKDVD.Controls
{
  /// <summary>
  /// Custom control that embeds WebView2 for rendering HTML menu from disc
  /// Based on OnlineVideos OverlayPanel pattern
  /// </summary>
  public class WebViewPanel : UserControl
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();
    private WebView2 _webView;
    private Grid _container;
    private bool _isInitialized = false;
    private JavaScriptBridge _bridge;
    private WebViewHelperModel _model;
    
    /// <summary>
    /// Dependency property for the model
    /// </summary>
    public static readonly DependencyProperty ModelProperty =
      DependencyProperty.Register(
        "Model",
        typeof(WebViewHelperModel),
        typeof(WebViewPanel),
        new PropertyMetadata(null, OnModelChanged));

    public WebViewHelperModel Model
    {
      get { return (WebViewHelperModel)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }

    private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var panel = (WebViewPanel)d;
      panel._model = (WebViewHelperModel)e.NewValue;
      if (panel._model != null && panel._bridge == null)
      {
        panel._bridge = new JavaScriptBridge(panel._model);
        Logger.Debug("8KDVD Player: JavaScript bridge connected to model");
      }
    }

    public WebViewPanel()
    {
      InitializeComponent();
      Loaded += WebViewPanel_Loaded;
      Unloaded += WebViewPanel_Unloaded;
    }

    private void InitializeComponent()
    {
      _container = new Grid();
      Content = _container;
    }

    private async void WebViewPanel_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        Logger.Debug("8KDVD Player: WebViewPanel loaded, initializing WebView2");

        // Create WebView2 control
        _webView = new WebView2();
        _webView.NavigationCompleted += WebView_NavigationCompleted;
        _webView.WebMessageReceived += WebView_WebMessageReceived;
        _webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;

        _container.Children.Add(_webView);

        // Initialize WebView2
        await _webView.EnsureCoreWebView2Async();

        // Load HTML from disc
        LoadHtmlFromDisc();
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error initializing WebView2", ex);
      }
    }

    private void WebViewPanel_Unloaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (_webView != null)
        {
          _webView.NavigationCompleted -= WebView_NavigationCompleted;
          _webView.WebMessageReceived -= WebView_WebMessageReceived;
          _webView.CoreWebView2InitializationCompleted -= WebView_CoreWebView2InitializationCompleted;
          _webView.Dispose();
          _webView = null;
        }
        Logger.Debug("8KDVD Player: WebViewPanel unloaded");
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error unloading WebViewPanel", ex);
      }
    }

    private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
    {
      if (e.IsSuccess)
      {
        Logger.Info("8KDVD Player: WebView2 Core initialized successfully");
        _isInitialized = true;

        // Set up JavaScript bridge
        SetupJavaScriptBridge();
      }
      else
      {
        Logger.Error($"8KDVD Player: WebView2 Core initialization failed: {e.InitializationException}");
      }
    }

    private void SetupJavaScriptBridge()
    {
      try
      {
        // Inject JavaScript to bridge MediaPortal functions
        string bridgeScript = @"
          window.MediaPortal = {
            playMovie: function(startTime) {
              window.chrome.webview.postMessage(JSON.stringify({
                function: 'playMovie',
                parameters: startTime
              }));
            },
            changeQuality: function(quality) {
              window.chrome.webview.postMessage(JSON.stringify({
                function: 'changeQuality',
                parameters: quality
              }));
            },
            loadSubtitle: function(lang) {
              window.chrome.webview.postMessage(JSON.stringify({
                function: 'loadSubtitle',
                parameters: lang
              }));
            },
            backToMainMenu: function() {
              window.chrome.webview.postMessage(JSON.stringify({
                function: 'backToMainMenu',
                parameters: null
              }));
            }
          };

          // Override existing functions in weblauncher.html if they exist
          if (typeof playMovie === 'function') {
            var originalPlayMovie = playMovie;
            playMovie = function(startTime) {
              window.MediaPortal.playMovie(startTime);
              originalPlayMovie(startTime);
            };
          }
        ";

        _webView.CoreWebView2.AddWebResourceRequestedFilter("*", Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All);
        _webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(bridgeScript);
        
        Logger.Info("8KDVD Player: JavaScript bridge setup complete");
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error setting up JavaScript bridge", ex);
      }
    }

    private void WebView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
    {
      try
      {
        string message = e.TryGetWebMessageAsString();
        Logger.Debug($"8KDVD Player: JavaScript message received: {message}");

        // Parse JSON message
        // Parse JSON message manually (System.Text.Json not available in .NET Framework 4.8)
        // Simple parsing for "Function":"name","Parameters":"value" format
        JavaScriptMessage messageObj = null;
        try
        {
          // Simple JSON parsing - extract function and parameters
          int funcStart = message.IndexOf("\"Function\":\"") + 12;
          int funcEnd = message.IndexOf("\"", funcStart);
          int paramStart = message.IndexOf("\"Parameters\":\"") + 14;
          int paramEnd = message.IndexOf("\"", paramStart);
          
          if (funcStart > 11 && funcEnd > funcStart && paramStart > 13 && paramEnd > paramStart)
          {
            messageObj = new JavaScriptMessage
            {
              Function = message.Substring(funcStart, funcEnd - funcStart),
              Parameters = message.Substring(paramStart, paramEnd - paramStart)
            };
          }
        }
        catch
        {
          // If parsing fails, try to extract function name from simple format
          messageObj = new JavaScriptMessage { Function = message, Parameters = null };
        }
        if (messageObj != null)
        {
          // Forward to JavaScript bridge (which connects to model)
          if (_bridge != null)
          {
            _bridge.HandleCall(messageObj.Function, messageObj.Parameters);
          }
          else if (_model != null)
          {
            // Fallback: call model directly if bridge not initialized
            _model.HandleJavaScriptCall(messageObj.Function, messageObj.Parameters?.ToString() ?? "");
          }
          // Also fire event for external handlers
          OnJavaScriptCall?.Invoke(messageObj.Function, messageObj.Parameters?.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error handling JavaScript message", ex);
      }
    }

    private void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
      if (e.IsSuccess)
      {
        Logger.Info($"8KDVD Player: Navigation completed: {_webView.Source}");
      }
      else
      {
        Logger.Warn($"8KDVD Player: Navigation failed: {e.WebErrorStatus}");
      }
    }

    private void LoadHtmlFromDisc()
    {
      try
      {
        string discPath = DiscPathService.Instance.CurrentDiscPath;
        if (string.IsNullOrEmpty(discPath))
        {
          Logger.Warn("8KDVD Player: No disc path available, cannot load HTML");
          return;
        }

        var discDetector = new DiscDetector();
        string htmlPath = discDetector.GetWebLauncherPath(discPath);

        if (string.IsNullOrEmpty(htmlPath) || !File.Exists(htmlPath))
        {
          Logger.Warn($"8KDVD Player: HTML file not found: {htmlPath}");
          return;
        }

        // Convert to file:// URI
        string fileUri = new Uri(htmlPath).ToString();
        Logger.Info($"8KDVD Player: Loading HTML from: {fileUri}");

        if (_webView != null && _isInitialized)
        {
          _webView.Source = new Uri(fileUri);
        }
        else
        {
          // Wait for initialization
          _webView.CoreWebView2InitializationCompleted += (s, e) =>
          {
            if (e.IsSuccess)
            {
              _webView.Source = new Uri(fileUri);
            }
          };
        }
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error loading HTML from disc", ex);
      }
    }

    /// <summary>
    /// Event fired when JavaScript calls are received
    /// </summary>
    public event Action<string, object> OnJavaScriptCall;

    /// <summary>
    /// JavaScript message structure
    /// </summary>
    private class JavaScriptMessage
    {
      public string Function { get; set; }
      public object Parameters { get; set; }
    }
  }
}
