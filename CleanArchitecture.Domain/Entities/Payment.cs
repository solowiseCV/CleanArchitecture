namespace CleanArchitecture.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // concurrency token for optimistic locking; EF will automatically check this on updates
        public byte[]? RowVersion { get; set; }
    }
}
