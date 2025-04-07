namespace LMCM_BE.DTOs.DashboardDtos
{
    public class ChartDataDto
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Data { get; set; } = new();
    }

}
