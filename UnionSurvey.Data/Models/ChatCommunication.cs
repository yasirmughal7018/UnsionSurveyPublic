using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class ChatCommunication
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string AgentName { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsFromAgent { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual AppUser AgentNameNavigation { get; set; } = null!;

    public virtual AppUser UserNameNavigation { get; set; } = null!;
}
