using Microsoft.AspNetCore.Mvc;
using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;
using System;

namespace MonitoringService.Services;

public class SystemEventLogService : ISystemEventLogService
{
    private readonly ISystemEventLogRepository _repo;

    public SystemEventLogService(ISystemEventLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SystemEventLogEntryDto>> GetRecentEventsAsync(int minutes)
    {
        var since = DateTime.UtcNow.AddMinutes(-minutes);
        return await _repo.GetEventsSinceAsync(since);
    }

    public async Task<List<SystemEventLogEntryDto>> FilterEventsAsync(string? type, string? path, int minutes)
    {
        var since = DateTime.UtcNow.AddMinutes(-minutes);
        var events = await _repo.GetEventsSinceAsync(since);

        if (!string.IsNullOrEmpty(type))
            events = events.Where(e => e.EventType == type).ToList();

        if (!string.IsNullOrEmpty(path))
            events = events.Where(e => e.Path != null && e.Path.Contains(path)).ToList();

        return events;
    }
}
