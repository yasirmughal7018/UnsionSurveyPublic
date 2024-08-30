using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class Survey
{
    public int SurveyId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? DomainUrl { get; set; }

    public string? PathUniqueCode { get; set; }

    public string? LogoPath { get; set; }

    public string? LogoName { get; set; }

    public decimal? SurveyAmount { get; set; }

    public int UsersForSurveyActive { get; set; }

    public bool IsDelete { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<SurveyQuestion> SurveyQuestions { get; set; } = new List<SurveyQuestion>();

    public virtual ICollection<SurveyUserMapping> SurveyUserMappings { get; set; } = new List<SurveyUserMapping>();

    public virtual ICollection<TransactionInternal> TransactionInternals { get; set; } = new List<TransactionInternal>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
