using Microsoft.EntityFrameworkCore;

namespace Crypto_Trivia.Services;

public sealed class RewardClaim
{
    public string GameId { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public int Score { get; set; }
    public string ChainId { get; set; } = string.Empty;
    public string TokenAddress { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? TransactionHash { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public sealed class RewardClaimDbContext : DbContext
{
    public RewardClaimDbContext(DbContextOptions<RewardClaimDbContext> options) : base(options) { }

    public DbSet<RewardClaim> Claims => Set<RewardClaim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RewardClaim>().HasKey(claim => claim.GameId);
        modelBuilder.Entity<RewardClaim>().Property(claim => claim.GameId).HasMaxLength(100);
        modelBuilder.Entity<RewardClaim>().HasIndex(claim => claim.WalletAddress);
    }
}
