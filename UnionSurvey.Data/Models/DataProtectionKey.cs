using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class DataProtectionKey
{
    public string Id { get; set; } = null!;

    public string? FriendlyName { get; set; }

    public string Xml { get; set; } = null!;
}
