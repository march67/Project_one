using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Presentation.DTOs.ReponseDTOs;

namespace Presentation.Controllers
{
    [Route("health")]
    [ApiController]
    public sealed class HealthController(HealthCheckService healthCheckService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<HealthResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<HealthResponseDto>(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<HealthResponseDto>> Get(CancellationToken cancellationToken)
        {
            var healthReport = await healthCheckService.CheckHealthAsync(cancellationToken);
            var healthResponse = new HealthResponseDto(healthReport.Status.ToString());

            return healthReport.Status == HealthStatus.Unhealthy
                ? StatusCode(StatusCodes.Status503ServiceUnavailable, healthResponse)
                : Ok(healthResponse);
        }
    }
}
