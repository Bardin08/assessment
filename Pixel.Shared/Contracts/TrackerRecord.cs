﻿namespace Pixel.Shared.Contracts;

public record TrackerRecord(
    DateTimeOffset Timestamp,
    string? UserAgent,
    string? Referer,
    string IpAddress);