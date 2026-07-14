using Domain.Dto;

namespace Service.Interface;

public interface IExcelExportService
{

    byte[] BuildRevenueReport(List<RevenueReportDto> report);
}
