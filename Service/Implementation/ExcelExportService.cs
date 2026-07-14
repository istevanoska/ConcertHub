using ClosedXML.Excel;
using Domain.Dto;
using Service.Interface;

namespace Service.Implementation;

public class ExcelExportService : IExcelExportService
{
    public byte[] BuildRevenueReport(List<RevenueReportDto> report)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Revenue");

        var headers = new[] { "Concert", "Venue", "Date", "Tickets Sold", "Capacity", "Occupancy %", "Revenue (EUR)", "Sold Out" };
        for (var i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
        }

        var row = 2;
        foreach (var item in report)
        {
            ws.Cell(row, 1).Value = item.Title;
            ws.Cell(row, 2).Value = item.VenueName;
            ws.Cell(row, 3).Value = item.StartTime;
            ws.Cell(row, 3).Style.DateFormat.Format = "yyyy-mm-dd hh:mm";
            ws.Cell(row, 4).Value = item.TicketsSold;
            ws.Cell(row, 5).Value = item.VenueCapacity;
            ws.Cell(row, 6).Value = Math.Round(item.OccupancyRate * 100, 1);
            ws.Cell(row, 7).Value = item.TotalRevenue;
            ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 8).Value = item.IsSoldOut ? "YES" : "";
            row++;
        }

        ws.Cell(row, 1).Value = "TOTAL";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 4).Value = report.Sum(r => r.TicketsSold);
        ws.Cell(row, 7).Value = report.Sum(r => r.TotalRevenue);
        ws.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
        ws.Row(row).Style.Font.Bold = true;

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
