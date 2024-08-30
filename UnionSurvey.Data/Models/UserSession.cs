using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class UserSession
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string SessionId { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime? LastActivityTime { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public virtual AppUser UserNameNavigation { get; set; } = null!;
}
