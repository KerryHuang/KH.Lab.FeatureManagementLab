# ASP.NET Feature Management 使用說明與教學

`.NET` 的 `AddFeatureManagement` 是由 Microsoft 提供的功能管理 (Feature Management) 庫，用於啟用或禁用應用程式中的功能。這個庫支持動態開啟或關閉功能，尤其在微服務架構或 DevOps 流程中非常實用。通過 `AddFeatureManagement`，您可以集中管理功能標誌 (Feature Flags) 並在應用程式中輕鬆地檢查和控制功能的啟用狀態。

### 步驟 1：安裝 Feature Management 套件

首先，在您的 .NET 應用程式中安裝 `Microsoft.FeatureManagement.AspNetCore` 套件：

```bash
dotnet add package Microsoft.FeatureManagement.AspNetCore
```

### 步驟 2：在 `Program.cs` 中添加 Feature Management

要在應用中啟用 Feature Management，請在 `Program.cs` 中進行配置：

```csharp
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// 設置 Feature Management
builder.Services.AddFeatureManagement();

builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

在上面的代碼中，`AddFeatureManagement()` 方法會將 Feature Management 添加到服務容器，並允許應用在執行時檢查功能標誌的狀態。

### 步驟 3：定義功能標誌 (Feature Flags)

在 `appsettings.json` 中定義功能標誌。您可以將標誌設置為 `true` 或 `false`，以控制功能的啟用狀態。

#### appsettings.json 示例

```json
{
  "FeatureManagement": {
    "NewFeature": true,
    "ExperimentalFeature": false
  }
}
```

這段配置會定義兩個功能標誌 `NewFeature` 和 `ExperimentalFeature`，其中 `NewFeature` 是啟用狀態，`ExperimentalFeature` 是禁用狀態。

### 步驟 4：在代碼中使用功能標誌

一旦定義了功能標誌，您就可以在代碼中檢查這些標誌的狀態，以控制功能的啟用或禁用。

#### 4.1. 在控制器中使用功能標誌

在控制器或其他服務中，注入 `IFeatureManager` 來檢查功能標誌的狀態：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

public class HomeController : Controller
{
    private readonly IFeatureManager _featureManager;

    public HomeController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task<IActionResult> Index()
    {
        if (await _featureManager.IsEnabledAsync("NewFeature"))
        {
            ViewData["Message"] = "New Feature is enabled!";
        }
        else
        {
            ViewData["Message"] = "New Feature is disabled.";
        }

        return View();
    }
}
```

在這段代碼中，`IsEnabledAsync("NewFeature")` 會檢查 `NewFeature` 是否啟用，並根據結果決定顯示不同的訊息。

#### 4.2. 使用特性 (Attribute) 控制功能訪問

您還可以使用 `FeatureGate` 特性來保護控制器或操作方法，當功能標誌被禁用時，請求將返回 `404 Not Found`：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

[FeatureGate("NewFeature")]
public class NewFeatureController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

這段代碼使用 `FeatureGate("NewFeature")`，當 `NewFeature` 標誌被禁用時，所有請求都會自動被阻止並返回 `404`。

### 步驟 5：使用條件 Feature Filters

`Feature Filters` 提供了基於條件啟用或禁用功能標誌的能力，例如基於使用者角色、百分比或自定義條件。

#### 在 `appsettings.json` 中配置條件過濾器

下面的示例展示了如何設置基於百分比的 Feature Filter：

```json
{
  "FeatureManagement": {
    "BetaFeature": {
      "EnabledFor": [
        {
          "Name": "Percentage",
          "Parameters": {
            "Value": 50
          }
        }
      ]
    }
  }
}
```

在這裡，`BetaFeature` 標誌會以 50% 的概率被啟用。這意味著每次請求有一半的機率會啟用此功能。

#### 使用條件 Feature Filters

要使用 Feature Filter，您需要在 `Program.cs` 中添加相應的過濾器支持：

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<PercentageFilter>();
```

### 步驟 6：自定義 Feature Filter

如果您需要基於自定義邏輯來控制功能標誌，則可以創建自定義 Feature Filter。

#### 創建自定義 Feature Filter

以下是一個基於使用者角色的自定義 Feature Filter：

```csharp
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

public class RoleFeatureFilter : IFeatureFilter
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        // 自定義邏輯判斷，這裡可以基於角色或其他條件
        bool isAdmin = // 自定義判斷邏輯...
        return Task.FromResult(isAdmin);
    }
}
```

#### 註冊自定義 Feature Filter

在 `Program.cs` 中註冊自定義過濾器：

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<RoleFeatureFilter>();
```

### 使用總結

1. **定義功能標誌**：在 `appsettings.json` 中定義功能標誌。
2. **配置 Feature Management**：在 `Program.cs` 中使用 `AddFeatureManagement` 添加支持。
3. **檢查功能標誌**：在代碼中使用 `IFeatureManager` 或 `FeatureGate` 特性來檢查和控制功能的啟用。
4. **使用 Feature Filters**：基於條件設置 Feature Filters（如百分比或自定義條件），控制功能的動態啟用。

這樣，您就可以靈活地管理應用中的功能標誌，並根據條件動態控制功能啟用。


---

##  ASP.NET Core Web API 應用中實現

以下是使用 `AddFeatureManagement` 在 ASP.NET Core Web API 應用中實現功能標誌 (Feature Flags) 的範例，包括基本功能標誌的設置、檢查和使用條件 Feature Filter。

### 步驟 1：安裝 Feature Management 套件

首先，在您的 Web API 項目中安裝 `Microsoft.FeatureManagement.AspNetCore` 套件：

```bash
dotnet add package Microsoft.FeatureManagement.AspNetCore
```

### 步驟 2：在 `Program.cs` 中配置 Feature Management

要在 Web API 中啟用 Feature Management，請在 `Program.cs` 中配置 `AddFeatureManagement()`。

```csharp
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// 配置 Feature Management
builder.Services.AddFeatureManagement();

// 配置控制器
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

在上面的代碼中，`AddFeatureManagement()` 方法會將 Feature Management 添加到服務容器中，使應用可以在運行時檢查功能標誌的狀態。

### 步驟 3：定義功能標誌

在 `appsettings.json` 中定義功能標誌，設定啟用或禁用的狀態。

#### appsettings.json 範例

```json
{
  "FeatureManagement": {
    "NewFeature": true,
    "ExperimentalFeature": false
  }
}
```

在這裡定義了兩個功能標誌 `NewFeature` 和 `ExperimentalFeature`，分別設定為 `true` 和 `false`。

### 步驟 4：在 API 控制器中使用功能標誌

接下來，我們在控制器中使用 `IFeatureManager` 檢查功能標誌的狀態，並根據標誌狀態執行不同的邏輯。

#### 使用 `IFeatureManager` 檢查功能標誌

創建一個控制器，並注入 `IFeatureManager` 來控制功能標誌：

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeatureController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpGet("check-new-feature")]
    public async Task<IActionResult> CheckNewFeature()
    {
        if (await _featureManager.IsEnabledAsync("NewFeature"))
        {
            return Ok("New Feature is enabled!");
        }
        else
        {
            return Ok("New Feature is disabled.");
        }
    }
}
```

這段代碼會根據 `NewFeature` 功能標誌的狀態返回不同的響應。

### 步驟 5：使用 `FeatureGate` 屬性控制 API 端點訪問

您可以使用 `FeatureGate` 屬性來限制 API 端點的訪問。如果功能標誌被禁用，該端點會返回 `404 Not Found`。

```csharp
[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeatureController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    // 使用 FeatureGate 限制端點訪問
    [FeatureGate("NewFeature")]
    [HttpGet("new-feature-endpoint")]
    public IActionResult NewFeatureEndpoint()
    {
        return Ok("New Feature Endpoint is accessible!");
    }
}
```

在這裡，`FeatureGate("NewFeature")` 屬性將 `NewFeature` 端點限制在 `NewFeature` 標誌啟用時才能訪問。當 `NewFeature` 標誌被禁用時，這個端點將返回 `404`。

### 步驟 6：添加條件 Feature Filters

`Feature Filters` 允許您基於條件控制功能的啟用。以下是基於百分比的 Feature Filter 設置範例。

#### appsettings.json 配置

可以在 `appsettings.json` 中設定基於百分比的 Feature Filter：

```json
{
  "FeatureManagement": {
    "BetaFeature": {
      "EnabledFor": [
        {
          "Name": "Percentage",
          "Parameters": {
            "Value": 50
          }
        }
      ]
    }
  }
}
```

在這裡，`BetaFeature` 標誌會以 50% 的概率被啟用，這意味著每次請求有一半的機率會啟用該功能。

#### 在 `Program.cs` 中添加 Feature Filter 支持

您需要在 `Program.cs` 中將 `PercentageFilter` 添加到 Feature Management 中：

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<Microsoft.FeatureManagement.FeatureFilters.PercentageFilter>();
```

### 步驟 7：自定義 Feature Filter（可選）

如果您需要根據自定義邏輯控制功能標誌，可以創建自定義的 Feature Filter，例如基於使用者角色。

#### 自定義 Feature Filter 類別

創建一個基於角色的自定義 Feature Filter：

```csharp
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

public class RoleFeatureFilter : IFeatureFilter
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        // 自定義邏輯判斷，例如根據角色控制啟用
        bool isAdmin = /* 自定義邏輯 */;
        return Task.FromResult(isAdmin);
    }
}
```

#### 註冊自定義 Feature Filter

在 `Program.cs` 中註冊自定義 Feature Filter：

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<RoleFeatureFilter>();
```

### 使用總結

1. **設置功能標誌**：在 `appsettings.json` 中定義功能標誌。
2. **配置 Feature Management**：在 `Program.cs` 中添加 `AddFeatureManagement`。
3. **檢查功能標誌狀態**：在 Web API 控制器中使用 `IFeatureManager` 或 `FeatureGate` 屬性來檢查並控制功能啟用。
4. **使用 Feature Filters**：通過 `PercentageFilter` 或自定義的 `RoleFeatureFilter` 來基於條件控制功能標誌的啟用。

這樣，您可以在 ASP.NET Core Web API 中靈活地管理和使用功能標誌，並根據條件動態控制 API 端點的啟用。

---

## 在 ASP.NET Core Web API 中結合 **Azure App Configuration** 的 Feature Management

在 ASP.NET Core Web API 中結合 **Azure App Configuration** 的 Feature Management，可以集中管理功能標誌並實現應用內的動態功能控制。Azure App Configuration 讓您可以直接管理功能標誌，而不需要修改 `appsettings.json`。以下是一個整合 Azure Feature Management 和 .NET Web API 的完整示例。

### 步驟 1：在 Azure 上配置 App Configuration 和 Feature Flags

1. **創建 Azure App Configuration 資源**
   - 登錄到 [Azure 入口網站](https://portal.azure.com)。
   - 搜索 `App Configuration`，點擊「**Create**」來創建資源。
   - 填寫必要資訊，並創建 App Configuration 資源。
2. **配置 Feature Flags**
   - 在 App Configuration 資源中，選擇「**Feature Manager**」。
   - 點擊「**+ Add**」，為每個功能標誌添加標識（如 `NewFeature`）。
   - 可以設置每個功能標誌的啟用狀態以及其他條件（如標籤、百分比等）。
3. **取得連接字符串**
   - 在 Azure App Configuration 資源中，選擇「**Access keys**」。
   - 複製連接字符串，稍後需要在應用程式中使用它。

### 步驟 2：在 Web API 中安裝必要的 NuGet 套件

您需要安裝 `Microsoft.Azure.AppConfiguration.AspNetCore` 和 `Microsoft.FeatureManagement.AspNetCore` 兩個套件來實現 Azure App Configuration 與 Feature Management。

```bash
dotnet add package Microsoft.Azure.AppConfiguration.AspNetCore
dotnet add package Microsoft.FeatureManagement.AspNetCore
```

### 步驟 3：配置 `Program.cs` 使用 Azure App Configuration 和 Feature Management

在 `Program.cs` 中，添加 Azure App Configuration 連接字符串並設置 Feature Management。確保 Feature Management 可以動態刷新。

```csharp
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

// 從環境變數或密碼管理器獲取 Azure App Configuration 連接字符串
string appConfigConnectionString = builder.Configuration["AppConfig:ConnectionString"];

// 添加 Azure App Configuration 並啟用動態配置刷新
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(appConfigConnectionString)
           .UseFeatureFlags(featureOptions =>
           {
               featureOptions.CacheExpirationInterval = TimeSpan.FromSeconds(30); // 每 30 秒刷新一次功能標誌狀態
           });
});

// 配置 Feature Management
builder.Services.AddFeatureManagement();

// 添加控制器
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 步驟 4：在 Web API 控制器中使用功能標誌

您可以通過 `IFeatureManager` 來檢查功能標誌的狀態，並根據不同狀態控制 API 的響應。

#### 在 API 控制器中檢查功能標誌

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeatureController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpGet("check-new-feature")]
    public async Task<IActionResult> CheckNewFeature()
    {
        if (await _featureManager.IsEnabledAsync("NewFeature"))
        {
            return Ok("New Feature is enabled!");
        }
        else
        {
            return Ok("New Feature is disabled.");
        }
    }
}
```

在這段代碼中，`IsEnabledAsync("NewFeature")` 會從 Azure App Configuration 檢查 `NewFeature` 的狀態。根據狀態，API 返回不同的響應。

#### 使用 `FeatureGate` 屬性控制 API 端點訪問

可以使用 `FeatureGate` 屬性直接控制端點的訪問。當功能標誌被禁用時，端點將返回 `404 Not Found`。

```csharp
[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeatureController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    [FeatureGate("NewFeature")]
    [HttpGet("new-feature-endpoint")]
    public IActionResult NewFeatureEndpoint()
    {
        return Ok("New Feature Endpoint is accessible!");
    }
}
```

當 `NewFeature` 功能標誌被禁用時，對 `new-feature-endpoint` 的訪問將返回 `404`。

### 步驟 5：使用條件 Feature Filters（例如百分比）

您可以在 Azure App Configuration 中設置基於條件的功能標誌過濾器，例如百分比控制。在 Azure App Configuration 內，為功能標誌（如 `BetaFeature`）設置一個百分比過濾器：

1. 回到 Azure App Configuration 的「**Feature Manager**」。
2. 點擊 `BetaFeature`，然後添加 `Percentage` 過濾器。
3. 設置「Parameters」中的「Value」為 50（即 50% 的請求會啟用此功能）。

#### 在 `Program.cs` 中註冊 PercentageFilter 支持

在 `Program.cs` 中添加 `PercentageFilter` 支持，以便應用可以正確處理這一過濾器：

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<Microsoft.FeatureManagement.FeatureFilters.PercentageFilter>();
```

這樣設置後，應用會根據設定的百分比控制功能標誌的啟用。

### 步驟 6：測試與調試

1. **啟動應用程式**，然後訪問 `/api/feature/check-new-feature` 或 `/api/feature/new-feature-endpoint`。
2. **在 Azure App Configuration 中更新功能標誌狀態**，並觀察 Web API 中的變化。
3. **使用條件過濾器**進行測試，比如修改百分比並觀察 API 響應。

### 使用總結

1. **Azure App Configuration 配置功能標誌**：在 Azure App Configuration 中設置功能標誌及過濾器。
2. **整合 .NET Web API**：在 `Program.cs` 中配置 Azure App Configuration 和 Feature Management。
3. **檢查功能標誌狀態**：在 API 控制器中使用 `IFeatureManager` 或 `FeatureGate` 屬性檢查和控制功能。
4. **使用條件過濾器**：根據條件（如百分比）動態控制功能標誌。

這樣，您就能在 Azure 中集中管理功能標誌，並將其整合到 .NET Web API 應用中，以實現更靈活的功能控制。

---

## 自定義回傳的錯誤格式與訊息

`FeatureNotEnabledHandler` 用於自定義當功能標誌 (Feature Flag) 被禁用時的錯誤響應格式。在 ASP.NET Core Web API 中，您可以通過實現 `IMvcFeatureNotEnabledHandler` 接口來定義自定義的錯誤處理邏輯。這樣，當使用 `FeatureGate` 屬性控制的功能標誌被禁用時，就可以返回自定義的錯誤訊息和狀態碼。

以下是如何使用 `FeatureNotEnabledHandler` 自定義錯誤響應的完整教程。

### 步驟 1：創建自定義的 `FeatureNotEnabledHandler`

首先，創建一個類來實現 `IDisabledFeaturesHandler`，並在其中定義當功能被禁用時應返回的錯誤訊息和格式。

#### 自定義 FeatureNotEnabledHandler 類別

```csharp
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.FeatureManagement.Mvc;
using System.Text.Json;

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
```

在此代碼中，我們自定義了 `HandleFeatureNotEnabledAsync` 方法，以返回 `403 Forbidden` 狀態碼和自定義 JSON 格式的錯誤訊息。您可以根據需求調整狀態碼和錯誤內容。

### 步驟 2：註冊自定義的 `FeatureNotEnabledHandler`

在 `Program.cs` 中註冊自定義的 `FeatureNotEnabledHandler`，以便當功能標誌被禁用時，ASP.NET Core Web API 使用我們自定義的處理邏輯。

#### 在 `Program.cs` 中添加註冊

```csharp
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 添加 Feature Management 和自定義的 FeatureNotEnabledHandler
builder.Services.AddFeatureManagement();
builder.Services.AddSingleton<IDisabledFeaturesHandler, FeatureNotEnabledHandler>();

// 添加控制器
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

在上面的代碼中，`AddSingleton<IMvcFeatureNotEnabledHandler, CustomFeatureNotEnabledHandler>()` 註冊了自定義的 `FeatureNotEnabledHandler`，這樣當使用 `FeatureGate` 檢查到功能標誌被禁用時，就會調用自定義的錯誤處理邏輯。

### 步驟 3：在控制器中使用 `FeatureGate` 屬性

在控制器的 API 端點上應用 `FeatureGate` 屬性來控制訪問。當功能標誌被禁用時，應用程序將返回自定義的錯誤響應。

#### API 控制器範例

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    [FeatureGate("NewFeature")]
    [HttpGet("new-feature-endpoint")]
    public IActionResult NewFeatureEndpoint()
    {
        return Ok("New Feature Endpoint is accessible!");
    }
}
```

這段代碼使用 `FeatureGate("NewFeature")` 來控制 `/api/feature/new-feature-endpoint` 的訪問。當 `NewFeature` 被禁用時，`FeatureNotEnabledHandler` 會攔截請求並返回自定義的錯誤響應。

### 驗證效果

當 `NewFeature` 標誌被禁用時，訪問 `/api/feature/new-feature-endpoint` 將返回自定義的錯誤訊息：

```json
{
    "Status": 403,
    "Message": "This feature is currently disabled.",
    "Feature": "new-feature-endpoint"
}
```

### 調整錯誤響應內容

您可以根據需要在 `CustomFeatureNotEnabledHandler` 中自定義錯誤訊息的格式和內容，例如添加更多的上下文信息（如用戶身份、請求 ID 等），或更改狀態碼。

這樣，您就能使用 `FeatureNotEnabledHandler` 自定義 `FeatureGate` 屬性控制的功能標誌被禁用時的錯誤響應格式，從而提升用戶體驗並便於進行錯誤排查。