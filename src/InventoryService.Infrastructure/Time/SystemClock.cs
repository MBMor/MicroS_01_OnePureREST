using InventoryService.Application.Common.Interfaces;

namespace InventoryService.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}