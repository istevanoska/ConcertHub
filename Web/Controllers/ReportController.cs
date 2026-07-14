using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IConcertService _concertService;
    private readonly IExcelExportService _excelExportService;
    private readonly IEtlSyncService _etlSyncService;
    private readonly IWebHostEnvironment _env;

    public ReportController(
        IConcertService concertService,
        IExcelExportService excelExportService,
        IEtlSyncService etlSyncService,
        IWebHostEnvironment env)
    {
        _concertService = concertService;
        _excelExportService = excelExportService;
        _etlSyncService = etlSyncService;
        _env = env;
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueAsync()
        => Ok(await _concertService.GetRevenueReportAsync());

    [HttpGet("revenue/excel")]
    public async Task<IActionResult> GetRevenueExcelAsync()
    {
        var report = await _concertService.GetRevenueReportAsync();
        var bytes = _excelExportService.BuildRevenueReport(report);
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"revenue-report-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    [HttpPost("etl/run")]
    public async Task<IActionResult> RunEtlAsync()
    {
        await _etlSyncService.SyncAllAsync();
        return Ok(new { message = "ETL sync completed." });
    }

    [HttpGet("emails")]
    public IActionResult GetSentEmails()
    {
        var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var dir = Path.Combine(root, "outbox");

        if (!Directory.Exists(dir))
            return Ok(Array.Empty<object>());

        var emails = new DirectoryInfo(dir)
            .GetFiles("*.html")
            .OrderByDescending(f => f.Name)
            .Take(10)
            .Select(f =>
            {
                var content = System.IO.File.ReadAllText(f.FullName);
                var to = string.Empty;
                var subject = string.Empty;
                var body = content;

                if (content.StartsWith("<!--"))
                {
                    var end = content.IndexOf("-->", StringComparison.Ordinal);
                    if (end > 0)
                    {
                        var meta = content.Substring(4, end - 4);
                        body = content[(end + 3)..].Trim();
                        foreach (var part in meta.Split('|'))
                        {
                            var kv = part.Split(':', 2);
                            if (kv.Length != 2) continue;
                            if (kv[0].Trim() == "To") to = kv[1].Trim();
                            if (kv[0].Trim() == "Subject") subject = kv[1].Trim();
                        }
                    }
                }

                return new { file = f.Name, to, subject, body };
            });

        return Ok(emails);
    }
}
