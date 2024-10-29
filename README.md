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

