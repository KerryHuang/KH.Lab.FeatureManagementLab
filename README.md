# ASP.NET Feature Management �ϥλ����P�о�

`.NET` �� `AddFeatureManagement` �O�� Microsoft ���Ѫ��\��޲z (Feature Management) �w�A�Ω�ҥΩθT�����ε{�������\��C�o�Ӯw����ʺA�}�ҩ������\��A�ר�b�L�A�Ȭ[�c�� DevOps �y�{���D�`��ΡC�q�L `AddFeatureManagement`�A�z�i�H�����޲z�\��лx (Feature Flags) �æb���ε{�������P�a�ˬd�M����\�઺�ҥΪ��A�C

### �B�J 1�G�w�� Feature Management �M��

�����A�b�z�� .NET ���ε{�����w�� `Microsoft.FeatureManagement.AspNetCore` �M��G

```bash
dotnet add package Microsoft.FeatureManagement.AspNetCore
```

### �B�J 2�G�b `Program.cs` ���K�[ Feature Management

�n�b���Τ��ҥ� Feature Management�A�Цb `Program.cs` ���i��t�m�G

```csharp
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// �]�m Feature Management
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

�b�W�����N�X���A`AddFeatureManagement()` ��k�|�N Feature Management �K�[��A�Ȯe���A�ä��\���Φb������ˬd�\��лx�����A�C

### �B�J 3�G�w�q�\��лx (Feature Flags)

�b `appsettings.json` ���w�q�\��лx�C�z�i�H�N�лx�]�m�� `true` �� `false`�A�H����\�઺�ҥΪ��A�C

#### appsettings.json �ܨ�

```json
{
  "FeatureManagement": {
    "NewFeature": true,
    "ExperimentalFeature": false
  }
}
```

�o�q�t�m�|�w�q��ӥ\��лx `NewFeature` �M `ExperimentalFeature`�A�䤤 `NewFeature` �O�ҥΪ��A�A`ExperimentalFeature` �O�T�Ϊ��A�C

### �B�J 4�G�b�N�X���ϥΥ\��лx

�@���w�q�F�\��лx�A�z�N�i�H�b�N�X���ˬd�o�Ǽлx�����A�A�H����\�઺�ҥΩθT�ΡC

#### 4.1. �b������ϥΥ\��лx

�b����Ψ�L�A�Ȥ��A�`�J `IFeatureManager` ���ˬd�\��лx�����A�G

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

�b�o�q�N�X���A`IsEnabledAsync("NewFeature")` �|�ˬd `NewFeature` �O�_�ҥΡA�îھڵ��G�M�w��ܤ��P���T���C

#### 4.2. �ϥίS�� (Attribute) ����\��X��

�z�٥i�H�ϥ� `FeatureGate` �S�ʨӫO�@����ξާ@��k�A��\��лx�Q�T�ήɡA�ШD�N��^ `404 Not Found`�G

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

�o�q�N�X�ϥ� `FeatureGate("NewFeature")`�A�� `NewFeature` �лx�Q�T�ήɡA�Ҧ��ШD���|�۰ʳQ����ê�^ `404`�C

### �B�J 5�G�ϥα��� Feature Filters

`Feature Filters` ���ѤF������ҥΩθT�Υ\��лx����O�A�Ҧp���ϥΪ̨���B�ʤ���Φ۩w�q����C

#### �b `appsettings.json` ���t�m����L�o��

�U�����ܨҮi�ܤF�p��]�m���ʤ��� Feature Filter�G

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

�b�o�̡A`BetaFeature` �лx�|�H 50% �����v�Q�ҥΡC�o�N���ۨC���ШD���@�b�����v�|�ҥΦ��\��C

#### �ϥα��� Feature Filters

�n�ϥ� Feature Filter�A�z�ݭn�b `Program.cs` ���K�[�������L�o������G

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<PercentageFilter>();
```

### �B�J 6�G�۩w�q Feature Filter

�p�G�z�ݭn���۩w�q�޿�ӱ���\��лx�A�h�i�H�Ыئ۩w�q Feature Filter�C

#### �Ыئ۩w�q Feature Filter

�H�U�O�@�Ӱ��ϥΪ̨��⪺�۩w�q Feature Filter�G

```csharp
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

public class RoleFeatureFilter : IFeatureFilter
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        // �۩w�q�޿�P�_�A�o�̥i�H��󨤦�Ψ�L����
        bool isAdmin = // �۩w�q�P�_�޿�...
        return Task.FromResult(isAdmin);
    }
}
```

#### ���U�۩w�q Feature Filter

�b `Program.cs` �����U�۩w�q�L�o���G

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<RoleFeatureFilter>();
```

### �ϥ��`��

1. **�w�q�\��лx**�G�b `appsettings.json` ���w�q�\��лx�C
2. **�t�m Feature Management**�G�b `Program.cs` ���ϥ� `AddFeatureManagement` �K�[����C
3. **�ˬd�\��лx**�G�b�N�X���ϥ� `IFeatureManager` �� `FeatureGate` �S�ʨ��ˬd�M����\�઺�ҥΡC
4. **�ϥ� Feature Filters**�G������]�m Feature Filters�]�p�ʤ���Φ۩w�q����^�A����\�઺�ʺA�ҥΡC

�o�ˡA�z�N�i�H�F���a�޲z���Τ����\��лx�A�îھڱ���ʺA����\��ҥΡC


---

##  ASP.NET Core Web API ���Τ���{

�H�U�O�ϥ� `AddFeatureManagement` �b ASP.NET Core Web API ���Τ���{�\��лx (Feature Flags) ���d�ҡA�]�A�򥻥\��лx���]�m�B�ˬd�M�ϥα��� Feature Filter�C

### �B�J 1�G�w�� Feature Management �M��

�����A�b�z�� Web API ���ؤ��w�� `Microsoft.FeatureManagement.AspNetCore` �M��G

```bash
dotnet add package Microsoft.FeatureManagement.AspNetCore
```

### �B�J 2�G�b `Program.cs` ���t�m Feature Management

�n�b Web API ���ҥ� Feature Management�A�Цb `Program.cs` ���t�m `AddFeatureManagement()`�C

```csharp
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// �t�m Feature Management
builder.Services.AddFeatureManagement();

// �t�m���
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

�b�W�����N�X���A`AddFeatureManagement()` ��k�|�N Feature Management �K�[��A�Ȯe�����A�����Υi�H�b�B����ˬd�\��лx�����A�C

### �B�J 3�G�w�q�\��лx

�b `appsettings.json` ���w�q�\��лx�A�]�w�ҥΩθT�Ϊ����A�C

#### appsettings.json �d��

```json
{
  "FeatureManagement": {
    "NewFeature": true,
    "ExperimentalFeature": false
  }
}
```

�b�o�̩w�q�F��ӥ\��лx `NewFeature` �M `ExperimentalFeature`�A���O�]�w�� `true` �M `false`�C

### �B�J 4�G�b API ������ϥΥ\��лx

���U�ӡA�ڭ̦b������ϥ� `IFeatureManager` �ˬd�\��лx�����A�A�îھڼлx���A���椣�P���޿�C

#### �ϥ� `IFeatureManager` �ˬd�\��лx

�Ыؤ@�ӱ���A�ê`�J `IFeatureManager` �ӱ���\��лx�G

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

�o�q�N�X�|�ھ� `NewFeature` �\��лx�����A��^���P���T���C

### �B�J 5�G�ϥ� `FeatureGate` �ݩʱ��� API ���I�X��

�z�i�H�ϥ� `FeatureGate` �ݩʨӭ��� API ���I���X�ݡC�p�G�\��лx�Q�T�ΡA�Ӻ��I�|��^ `404 Not Found`�C

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

    // �ϥ� FeatureGate ������I�X��
    [FeatureGate("NewFeature")]
    [HttpGet("new-feature-endpoint")]
    public IActionResult NewFeatureEndpoint()
    {
        return Ok("New Feature Endpoint is accessible!");
    }
}
```

�b�o�̡A`FeatureGate("NewFeature")` �ݩʱN `NewFeature` ���I����b `NewFeature` �лx�ҥήɤ~��X�ݡC�� `NewFeature` �лx�Q�T�ήɡA�o�Ӻ��I�N��^ `404`�C

### �B�J 6�G�K�[���� Feature Filters

`Feature Filters` ���\�z�����󱱨�\�઺�ҥΡC�H�U�O���ʤ��� Feature Filter �]�m�d�ҡC

#### appsettings.json �t�m

�i�H�b `appsettings.json` ���]�w���ʤ��� Feature Filter�G

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

�b�o�̡A`BetaFeature` �лx�|�H 50% �����v�Q�ҥΡA�o�N���ۨC���ШD���@�b�����v�|�ҥθӥ\��C

#### �b `Program.cs` ���K�[ Feature Filter ���

�z�ݭn�b `Program.cs` ���N `PercentageFilter` �K�[�� Feature Management ���G

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<Microsoft.FeatureManagement.FeatureFilters.PercentageFilter>();
```

### �B�J 7�G�۩w�q Feature Filter�]�i��^

�p�G�z�ݭn�ھڦ۩w�q�޿豱��\��лx�A�i�H�Ыئ۩w�q�� Feature Filter�A�Ҧp���ϥΪ̨���C

#### �۩w�q Feature Filter ���O

�Ыؤ@�Ӱ�󨤦⪺�۩w�q Feature Filter�G

```csharp
using Microsoft.FeatureManagement;
using System.Threading.Tasks;

public class RoleFeatureFilter : IFeatureFilter
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        // �۩w�q�޿�P�_�A�Ҧp�ھڨ��ⱱ��ҥ�
        bool isAdmin = /* �۩w�q�޿� */;
        return Task.FromResult(isAdmin);
    }
}
```

#### ���U�۩w�q Feature Filter

�b `Program.cs` �����U�۩w�q Feature Filter�G

```csharp
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<RoleFeatureFilter>();
```

### �ϥ��`��

1. **�]�m�\��лx**�G�b `appsettings.json` ���w�q�\��лx�C
2. **�t�m Feature Management**�G�b `Program.cs` ���K�[ `AddFeatureManagement`�C
3. **�ˬd�\��лx���A**�G�b Web API ������ϥ� `IFeatureManager` �� `FeatureGate` �ݩʨ��ˬd�ñ���\��ҥΡC
4. **�ϥ� Feature Filters**�G�q�L `PercentageFilter` �Φ۩w�q�� `RoleFeatureFilter` �Ӱ����󱱨�\��лx���ҥΡC

�o�ˡA�z�i�H�b ASP.NET Core Web API ���F���a�޲z�M�ϥΥ\��лx�A�îھڱ���ʺA���� API ���I���ҥΡC

---

