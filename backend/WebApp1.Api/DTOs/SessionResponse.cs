using WebApp1.Api.Models;

namespace WebApp1.Api.DTOs;

public class SessionResponse
{
    public int Id { get; set; }
    public DateTimeOffset FirstLogin { get; set; }
    public DateTimeOffset LastLogin { get; set; }
    public string? DeviceInfo { get; set; }
    public string? Ip { get; set; }
    public bool IsCurrentSession { get; set; }
    public SessionState State { get; set; }
}