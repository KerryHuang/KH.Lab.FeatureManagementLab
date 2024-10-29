using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace KH.Lab.FeatureManagementLab.Controllers;

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

    // 使用 FeatureGate 限制端點訪問
    [FeatureGate("NewFeature")]
    [HttpGet("new-feature-endpoint")]
    public IActionResult NewFeatureEndpoint()
    {
        return Ok("New Feature Endpoint is accessible!");
    }
}
