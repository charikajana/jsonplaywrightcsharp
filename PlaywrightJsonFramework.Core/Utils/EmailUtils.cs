using System.Net;
using System.Net.Mail;
using PlaywrightJsonFramework.Core.Config;

namespace PlaywrightJsonFramework.Core.Utils;

/// <summary>
/// Utility to send email notifications for CI/CD runs
/// </summary>
public static class EmailUtils
{
    private const string COMPONENT = "EMAIL UTILS";

    public static async Task SendTestSummary(string subject, string body, bool isHtml = true)
    {
        if (!ExecutionConfig.EnableEmail)
        {
            Logger.Info("Email notifications are disabled (ENABLE_EMAIL=false). Skipping.", COMPONENT);
            return;
        }

        if (string.IsNullOrEmpty(ExecutionConfig.SmtpHost) || string.IsNullOrEmpty(ExecutionConfig.RecipientEmail))
        {
            Logger.Error("SMTP Host or Recipient Email is missing. Cannot send notification.", COMPONENT);
            return;
        }

        try
        {
            Logger.Info($"Preparing to send email to: {ExecutionConfig.RecipientEmail}", COMPONENT);

            using var client = new SmtpClient(ExecutionConfig.SmtpHost, ExecutionConfig.SmtpPort)
            {
                Credentials = new NetworkCredential(ExecutionConfig.SmtpUser, ExecutionConfig.SmtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(ExecutionConfig.SmtpUser, "Playwright Automation Framework"),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(ExecutionConfig.RecipientEmail);

            await client.SendMailAsync(mailMessage);
            Logger.Success("Email notification sent successfully!", COMPONENT);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to send email: {ex.Message}", COMPONENT);
        }
    }

    /// <summary>
    /// Generates a professional HTML report summary for CI/CD
    /// </summary>
    public static string GenerateHtmlReport(int passed, int failed, string env, string browser, string buildNumber, string resultsUrl)
    {
        var color = failed > 0 ? "#e74c3c" : "#2ecc71";
        var status = failed > 0 ? "FAILED" : "PASSED";
        var date = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
        
        var resultsRow = string.IsNullOrEmpty(resultsUrl) ? "" : $@"
            <tr>
                <td style='padding: 10px; border: 1px solid #ddd;'>Results Link</td>
                <td style='padding: 10px; border: 1px solid #ddd;'><a href='{resultsUrl}' style='color: #3498db; text-decoration: none;'>View Detailed Report</a></td>
            </tr>";

        return $@"
        <html>
        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
            <div style='max-width: 600px; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                <h2 style='color: #2c3e50;'>🚀 Test Execution Report</h2>
                <hr style='border: 0; border-top: 1px solid #eee;' />
                <p><strong>Status:</strong> <span style='color: {color}; font-weight: bold;'>{status}</span></p>
                
                <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                    <tr style='background: #f8f9fa;'>
                        <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Metric</th>
                        <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Value</th>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border: 1px solid #ddd;'>Date</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{date}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border: 1px solid #ddd;'>Build Number</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'><b>{buildNumber}</b></td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border: 1px solid #ddd;'>Environment</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{env.ToUpper()}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border: 1px solid #ddd;'>Browser</td>
                        <td style='padding: 10px; border: 1px solid #ddd;'>{browser.ToUpper()}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border: 1px solid #ddd;'>Tests Passed</td>
                        <td style='padding: 10px; border: 1px solid #ddd; color: #27ae60; font-weight: bold;'>{passed}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; border: 1px solid #ddd;'>Tests Failed</td>
                        <td style='padding: 10px; border: 1px solid #ddd; color: #e74c3c; font-weight: bold;'>{failed}</td>
                    </tr>
                    {resultsRow}
                </table>
                <p style='margin-top: 20px; font-size: 12px; color: #7f8c8d;'>
                    This is an automated message from your Playwright Hybrid Framework CI/CD pipeline.
                </p>
            </div>
        </body>
        </html>";
    }
}
