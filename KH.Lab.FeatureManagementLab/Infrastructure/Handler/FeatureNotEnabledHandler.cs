using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement.Mvc;
using System.Text.Json;

namespace KH.Lab.FeatureManagementLab.Infrastructure.Handler;

// 創建自定義的 FeatureNotEnabledHandler
public class FeatureNotEnabledHandler : IDisabledFeaturesHandler
{
    public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
    {
        var response = context.HttpContext.Response;
        response.StatusCode = StatusCodes.Status403Forbidden;  // 可改為所需的狀態碼
        response.ContentType = "application/json";

        var errorResponse = new
        {
            Status = response.StatusCode,
            Message = "This feature is currently disabled.",
            Feature = context.ActionDescriptor.DisplayName // 可以包含更多上下文信息
        };

        // 將自定義錯誤訊息轉為 JSON
        var json = JsonSerializer.Serialize(errorResponse);

        return response.WriteAsync(json);
    }
}