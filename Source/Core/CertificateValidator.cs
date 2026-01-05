using System;
using System.IO;
using System.Text.RegularExpressions;
using MediaPortal.Common;
using MediaPortal.Common.Logging;

namespace EightKDVD.Core
{
  /// <summary>
  /// Validates 8KDVD certificate and license (C-Logic security)
  /// </summary>
  public class CertificateValidator
  {
    private static readonly ILogger Logger = ServiceRegistration.Get<ILogger>();

    /// <summary>
    /// Validates the certificate and license for an 8KDVD disc
    /// </summary>
    /// <param name="discPath">Path to the disc root</param>
    /// <returns>True if certificate is valid and license is present</returns>
    public bool ValidateCertificate(string discPath)
    {
      try
      {
        string certPath = Path.Combine(discPath, "CERTIFICATE", "Certificate.html");
        if (!File.Exists(certPath))
        {
          Logger.Warn($"8KDVD Player: Certificate not found at {certPath}");
          return false;
        }

        // Level 1: Check for license line in Certificate.html
        bool hasLicense = CheckLicenseLine(certPath);
        if (!hasLicense)
        {
          Logger.Warn("8KDVD Player: License not found in certificate");
          return false;
        }

        // Level 2: Check for LICENCEINFO folder and files
        bool hasLicenseInfo = CheckLicenseInfo(discPath);
        if (!hasLicenseInfo)
        {
          Logger.Warn("8KDVD Player: License info validation failed");
          return false;
        }

        Logger.Info("8KDVD Player: Certificate validation successful");
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("8KDVD Player: Error validating certificate", ex);
        return false;
      }
    }

    /// <summary>
    /// Level 1: Checks for license line in Certificate.html
    /// Expected format: <p><strong>Licence:</strong> Yes</p>
    /// </summary>
    private bool CheckLicenseLine(string certPath)
    {
      try
      {
        string content = File.ReadAllText(certPath);
        
        // Look for license pattern: <p><strong>Licence:</strong> Yes</p>
        // Case-insensitive, flexible whitespace
        Regex licensePattern = new Regex(
          @"<p>\s*<strong>\s*Licence:\s*</strong>\s*Yes\s*</p>",
          RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace
        );

        bool hasLicense = licensePattern.IsMatch(content);
        
        if (hasLicense)
        {
          Logger.Debug("8KDVD Player: License line found in certificate");
        }
        else
        {
          Logger.Debug("8KDVD Player: License line not found in certificate");
        }

        return hasLicense;
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error checking license line in {certPath}", ex);
        return false;
      }
    }

    /// <summary>
    /// Level 2: Checks for LICENCEINFO folder and LICENCEINFO.xml file
    /// Also validates that the dummy file is 0 KB
    /// </summary>
    private bool CheckLicenseInfo(string discPath)
    {
      try
      {
        string licenseInfoFolder = Path.Combine(discPath, "LICENCEINFO");
        if (!Directory.Exists(licenseInfoFolder))
        {
          Logger.Debug("8KDVD Player: LICENCEINFO folder not found");
          return false;
        }

        string licenseInfoFile = Path.Combine(licenseInfoFolder, "LICENCEINFO.xml");
        if (!File.Exists(licenseInfoFile))
        {
          Logger.Debug("8KDVD Player: LICENCEINFO.xml not found");
          return false;
        }

        // Check that the file is 0 KB (dummy file)
        FileInfo fileInfo = new FileInfo(licenseInfoFile);
        if (fileInfo.Length != 0)
        {
          Logger.Debug($"8KDVD Player: LICENCEINFO.xml is not 0 KB (size: {fileInfo.Length})");
          return false;
        }

        Logger.Debug("8KDVD Player: License info validation passed");
        return true;
      }
      catch (Exception ex)
      {
        Logger.Warn($"8KDVD Player: Error checking license info in {discPath}", ex);
        return false;
      }
    }
  }
}
