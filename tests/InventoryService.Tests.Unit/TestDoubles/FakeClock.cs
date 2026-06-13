using InventoryService.Application.Common.Interfaces;
using System;

namespace InventoryService.Tests.Unit.TestDoubles;

internal sealed class FakeClock(DateTime utcNow) : IClock
{
    public DateTime UtcNow { get; } = utcNow;
}