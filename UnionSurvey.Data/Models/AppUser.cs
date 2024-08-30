using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class AppUser
{
    /// <summary>
    /// it is AspNetUser id
    /// </summary>
    public string Id { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public bool TermsAndCondition { get; set; }

    public bool IsAdmin { get; set; }

    public bool? IsAgent { get; set; }

    public bool? AgentStatus { get; set; }

    public bool IsActive { get; set; }

    public bool IsDelete { get; set; }

    public string UserCode { get; set; } = null!;

    public string? ReferalCode { get; set; }

    public string? Gender { get; set; }

    public DateTime? BrithDate { get; set; }

    public string? Avatar { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? Address { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<ChatCommunication> ChatCommunicationAgentNameNavigations { get; set; } = new List<ChatCommunication>();

    public virtual ICollection<ChatCommunication> ChatCommunicationUserNameNavigations { get; set; } = new List<ChatCommunication>();

    public virtual ICollection<TransactionInternal> TransactionInternalFromUserNameNavigations { get; set; } = new List<TransactionInternal>();

    public virtual ICollection<TransactionInternal> TransactionInternalToUserNameNavigations { get; set; } = new List<TransactionInternal>();

    public virtual UserFinancial UserNameNavigation { get; set; } = null!;

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
}
