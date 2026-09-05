namespace YourProject.Dtos
{
    public class DashboardRequest
    {
        public double SaldoTotal { get; set; }
        public double InvestimentoTotal { get; set; }
    }

    public class DashboardResponse
    {
        public int Id { get; set; }
        public double SaldoTotal { get; set; }
        public double InvestimentoTotal { get; set; }
    }
}
