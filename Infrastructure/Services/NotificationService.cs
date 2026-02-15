using Application.Contracts.Notifications;

namespace Infrastructure.Services;

public class NotificationService
    (IOptions<EmailTemplateOptions> templateOptions,
    IOptions<AppUrlSettings> appSettings,
    ITemplateRenderer templateRenderer,
    IEmailSender emailSender,
    IUnitOfWork unitOfWork,
    IJobManager jobManager) : INotificationService
{
    private readonly EmailTemplateOptions _templateOptions = templateOptions.Value;
    private readonly ITemplateRenderer _templateRenderer = templateRenderer;
    private readonly string _clientBaseUrl = appSettings.Value.ClientUrl;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJobManager _jobManager = jobManager;

    private const string _projectName = "Rahiq";

    public async Task SendConfirmationLinkAsync(User user, string code, int expiryTimeInHours = 24)
    {
        var link = $"{_clientBaseUrl}/auth/emailConfirmation?userId={user.Id}&code={code}";

        var placeholders = BuildPlaceholders(user.FirstName, new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.ActionUrl, link },
            { EmailTemplateOptions.Placeholders.ExpiryTime, expiryTimeInHours.ToString() }
        });

        await EnqueueEmailAsync(user.Email!, $"{_projectName} email confirmation", EmailTemplateOptions.TemplatesNames.EmailConfirmationLink, placeholders);
    }

    public async Task SendResetPasswordAsync(User user, string code, int expiryTimeInHours = 24)
    {
        var link = $"{_clientBaseUrl}/auth/forgetPassword?email={user.Email}&code={code}";

        var placeholders = BuildPlaceholders(user.FirstName, new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.ActionUrl, link },
            { EmailTemplateOptions.Placeholders.ExpiryTime, expiryTimeInHours.ToString() }
        });

        await EnqueueEmailAsync(user.Email!, $"{_projectName} reset password", EmailTemplateOptions.TemplatesNames.ResetPassword, placeholders);
    }

    public async Task SendChangeEmailNotificationAsync(User user, string oldEmail, DateTime changeDate)
    {
        var placeholders = BuildPlaceholders(user.FirstName, new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.NewEmail, user.Email! },
            { EmailTemplateOptions.Placeholders.ChangeDate, changeDate.ToString("f") }
        });

        await EnqueueEmailAsync(oldEmail, $"{_projectName} email change notification", EmailTemplateOptions.TemplatesNames.ChangeEmailNotification, placeholders);
    }

    public async Task SendQuantityWarningAsync()
    {
        var bundles = await _unitOfWork.Bundles
            .FindAllProjectionAsync<BundleQuantityWarning>
            (
                x => x.QuantityAvailable == 0 && x.EndAt > DateOnly.FromDateTime(DateTime.UtcNow),
                [$"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}"]
            );

        if (!bundles.Any()) return;

        var tableHtml = BuildBundleWarningTable(bundles);

        var placeholders = BuildPlaceholders(_projectName + "'s Manager", new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.ReportDate, DateTime.UtcNow.ToString("MMMM dd, yyyy") },
            { EmailTemplateOptions.Placeholders.TotalCount, bundles.Count().ToString() },
            { EmailTemplateOptions.Placeholders.ItemsTable, tableHtml }
        });

        await EnqueueEmailAsync(_templateOptions.SupportEmail, $"{_projectName} Bundle Quantity Warning", EmailTemplateOptions.TemplatesNames.BundleQuantityWarning, placeholders);
    }

    public async Task SendPendingOrderListAsync()
    {
        var today = DateTime.UtcNow.Date;
        var orders = await _unitOfWork.Orders
            .FindAllProjectionAsync<OrderReportInfo>
            (
                x => x.Status == OrderStatus.Pending && x.Date.Date == today
            );

        if (!orders.Any()) return;

        var tableHtml = BuildOrderTable(orders);

        var placeholders = BuildPlaceholders(_projectName + "'s Manager", new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.ReportDate, DateTime.UtcNow.ToString("MMMM dd, yyyy") },
            { EmailTemplateOptions.Placeholders.TotalCount, orders.Count().ToString() },
            { EmailTemplateOptions.Placeholders.ItemsTable, tableHtml }
        });

        await EnqueueEmailAsync(_templateOptions.SupportEmail, $"{_projectName} Daily Pending Orders Report", EmailTemplateOptions.TemplatesNames.PendingOrderList, placeholders);
    }

    public async Task SendCanceledOrderListAsync()
    {
        var today = DateTime.UtcNow.Date;
        var orders = await _unitOfWork.Orders
            .FindAllProjectionAsync<OrderReportInfo>
            (
                x => x.Status == OrderStatus.Canceled && x.Date.Date == today
            );

        if (!orders.Any()) return;

        var tableHtml = BuildOrderTable(orders);

        var placeholders = BuildPlaceholders(_projectName + "'s Manager", new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.ReportDate, DateTime.UtcNow.ToString("MMMM dd, yyyy") },
            { EmailTemplateOptions.Placeholders.TotalCount, orders.Count().ToString() },
            { EmailTemplateOptions.Placeholders.ItemsTable, tableHtml }
        });

        await EnqueueEmailAsync(_templateOptions.SupportEmail, $"{_projectName} Daily Canceled Orders Report", EmailTemplateOptions.TemplatesNames.CanceledOrderList, placeholders);
    }

    private async Task EnqueueEmailAsync(string email, string subject, string templateName, Dictionary<string, string> placeholders)
    {
        var body = await _templateRenderer.RenderAsync(templateName, placeholders);
        _jobManager.Enqueue(() => _emailSender.SendAsync(email, subject, body));
    }

    private Dictionary<string, string> BuildPlaceholders(string userName, Dictionary<string, string>? additionalPlaceholders = null)
    {
        var dictionary = new Dictionary<string, string>
        {
            { EmailTemplateOptions.Placeholders.TitleName, _templateOptions.TitleName },
            { EmailTemplateOptions.Placeholders.TeamName, _templateOptions.TeamName },
            { EmailTemplateOptions.Placeholders.Address, _templateOptions.Address },
            { EmailTemplateOptions.Placeholders.City, _templateOptions.City },
            { EmailTemplateOptions.Placeholders.Country, _templateOptions.Country },
            { EmailTemplateOptions.Placeholders.SupportEmail, _templateOptions.SupportEmail},
            { EmailTemplateOptions.Placeholders.UserName, userName }
        };

        if (additionalPlaceholders != null && additionalPlaceholders.Count > 0)
            foreach (var item in additionalPlaceholders)
                dictionary.Add(item.Key, item.Value);

        return dictionary;
    }

    private static string BuildBundleWarningTable(IEnumerable<BundleQuantityWarning> bundles)
    {
        var sb = new StringBuilder();
        foreach (var bundle in bundles)
        {
            var products = string.Join(", ", bundle.BundleItems.Select(x => x.ItemName));
            sb.Append($@"
                <tr>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #2c3e50;"">{bundle.BundleId}</td>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #2c3e50; font-weight: 500;"">{bundle.BundleName}</td>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #666; font-size: 13px;"">{products}</td>
                </tr>");
        }
        return sb.ToString();
    }

    private static string BuildOrderTable(IEnumerable<OrderReportInfo> orders)
    {
        var sb = new StringBuilder();
        foreach (var order in orders)
        {
            sb.Append($@"
                <tr>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #2c3e50; font-weight: 500;"">{order.OrderId}</td>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #666; font-size: 12px;"">{order.CustomerId}</td>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #2c3e50;"">{order.OrderDate:MMM dd, yyyy HH:mm}</td>
                    <td style=""padding: 12px 15px; border-bottom: 1px solid #eee; color: #2c3e50; font-weight: 600;"">${order.Total:N2}</td>
                </tr>");
        }
        return sb.ToString();
    }
}
