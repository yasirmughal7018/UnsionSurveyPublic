using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UnionSurvey.Model;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace UnionSurvey.Data.Models;

public partial class SurveyUnionContext : IdentityDbContext<IdentityUser>//, IDataProtectionKeyContext
{
    private readonly IConfiguration _configuration;
    public SurveyUnionContext(IConfiguration configuration) => _configuration = configuration;

    public SurveyUnionContext(DbContextOptions<SurveyUnionContext> options, IConfiguration configuration)
        : base(options) => _configuration = configuration;


    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<ChatCommunication> ChatCommunications { get; set; }

    //public virtual DbSet<Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey> DataProtectionKeys { get; set; }


    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<ScheduledJob> ScheduledJobs { get; set; }

    public virtual DbSet<ScheduledJobLog> ScheduledJobLogs { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }

    public virtual DbSet<SurveyUnionConfig> SurveyUnionConfigs { get; set; }

    public virtual DbSet<SurveyUserMapping> SurveyUserMappings { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionInternal> TransactionInternals { get; set; }

    public virtual DbSet<TransactionLog> TransactionLogs { get; set; }

    public virtual DbSet<TransactionWithdrawSignature> TransactionWithdrawSignatures { get; set; }

    public virtual DbSet<UserFinancial> UserFinancials { get; set; }

    public virtual DbSet<UserFinancialComission> UserFinancialComissions { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }


    //Store Proceudres
    public virtual DbSet<SP_UserHierarchy> SP_UserHierarchyWithStats { get; set; }

    public virtual DbSet<SP_AdminDashboardStat> SP_AdminDashboardStats { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string,
    //you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration
    //- see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings,
    //see https://go.microsoft.com/fwlink/?LinkId=723263.
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserName).HasName("PK_User");

            entity.HasIndex(e => e.UserCode, "IX_UserCode").IsUnique();

            entity.HasIndex(e => e.UserName, "IX_UserName");

            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Avatar).HasMaxLength(200);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .HasComment("it is AspNetUser id");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.ReferalCode)
                .HasMaxLength(10)
                .HasDefaultValue("YSADMIN786");
            entity.Property(e => e.UserCode).HasMaxLength(10);

            entity.HasOne(d => d.UserNameNavigation).WithOne(p => p.AppUser)
                .HasForeignKey<AppUser>(d => d.UserName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AppUsers_UserFinancial");
        });

        modelBuilder.Entity<ChatCommunication>(entity =>
        {
            entity.Property(e => e.AgentName).HasMaxLength(50);
            entity.Property(e => e.TimeStamp).HasPrecision(2);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.AgentNameNavigation).WithMany(p => p.ChatCommunicationAgentNameNavigations)
                .HasForeignKey(d => d.AgentName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatCommunications_AppUsers1");

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.ChatCommunicationUserNameNavigations)
                .HasForeignKey(d => d.UserName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatCommunications_AppUsers");
        });

        modelBuilder.Entity<DataProtectionKey>(entity =>
        {
            entity.Property(e => e.Id)
                .HasMaxLength(40)
                .HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK_SurveyQuestions");

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.Option1).HasMaxLength(1000);
            entity.Property(e => e.Option2).HasMaxLength(1000);
            entity.Property(e => e.Option3).HasMaxLength(1000);
            entity.Property(e => e.Option4).HasMaxLength(1000);
            entity.Property(e => e.Question1)
                .HasMaxLength(2000)
                .HasColumnName("Question");
        });

        modelBuilder.Entity<ScheduledJob>(entity =>
        {
            entity.Property(e => e.JobName).HasMaxLength(50);
            entity.Property(e => e.NextJobDate).HasPrecision(0);
            entity.Property(e => e.SchedualTime).HasPrecision(0);
        });

        modelBuilder.Entity<ScheduledJobLog>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ScheduledJobDate).HasPrecision(0);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.ScheduledJob).WithMany(p => p.ScheduledJobLogs)
                .HasForeignKey(d => d.ScheduledJobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScheduledJobLogs_ScheduledJobs");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.ToTable("Survey");

            entity.HasIndex(e => e.Title, "IX_Survey").IsUnique();

            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.DomainUrl).HasMaxLength(100);
            entity.Property(e => e.LogoName).HasMaxLength(50);
            entity.Property(e => e.LogoPath).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.PathUniqueCode).HasMaxLength(10);
            entity.Property(e => e.SurveyAmount).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<SurveyQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SurveyQuestionMappings");

            entity.ToTable("SurveyQuestion");

            entity.HasOne(d => d.Question).WithMany(p => p.SurveyQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyQuestionMappings_SurveyQuestions");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyQuestions)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyQuestionMappings_Survey");
        });

        modelBuilder.Entity<SurveyUnionConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigName).HasName("PK_Configurations");

            entity.ToTable("SurveyUnionConfig");

            entity.Property(e => e.ConfigName).HasMaxLength(50);
            entity.Property(e => e.ConfigVal).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<SurveyUserMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SurveyUserMapping");

            entity.Property(e => e.CompletedDate).HasPrecision(0);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyUserMappings)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUserMapping_Survey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("numeric(18, 0)");
            entity.Property(e => e.CryptoReceiverAddress).HasMaxLength(50);
            entity.Property(e => e.CryptoSenderAddress).HasMaxLength(50);
            entity.Property(e => e.CryptoTransactionId).HasMaxLength(100);
            entity.Property(e => e.CryptoTransactionStatus).HasMaxLength(20);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate).HasPrecision(0);
            entity.Property(e => e.RecordStatus)
                .HasMaxLength(3)
                .HasComment("Record Status will be either IN or OUT");
            entity.Property(e => e.TransAccountName).HasMaxLength(50);
            entity.Property(e => e.TransactionStatus)
                .HasMaxLength(20)
                .HasComment("It will be either In Progress, Pending, Completed");

            entity.HasOne(d => d.Survey).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK_Transactions_Survey");
        });

        modelBuilder.Entity<TransactionInternal>(entity =>
        {
            entity.HasKey(e => e.TransactionId);

            entity.ToTable("TransactionInternal");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.ComissionPercentage).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.FromAvailableBalance).HasColumnType("decimal(10, 3)");
            entity.Property(e => e.FromUserName).HasMaxLength(50);
            entity.Property(e => e.RecordStatus).HasMaxLength(3);
            entity.Property(e => e.ToUserName).HasMaxLength(50);
            entity.Property(e => e.TransactionTime).HasPrecision(0);
            entity.Property(e => e.TransactionType).HasMaxLength(50);

            entity.HasOne(d => d.FromUserNameNavigation).WithMany(p => p.TransactionInternalFromUserNameNavigations)
                .HasForeignKey(d => d.FromUserName)
                .HasConstraintName("FK_TransactionInternal_AppUsers");

            entity.HasOne(d => d.Survey).WithMany(p => p.TransactionInternals)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionInternal_Survey");

            entity.HasOne(d => d.ToUserNameNavigation).WithMany(p => p.TransactionInternalToUserNameNavigations)
                .HasForeignKey(d => d.ToUserName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionInternal_AppUsers1");
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.Property(e => e.LogBy).HasMaxLength(50);
            entity.Property(e => e.TransactionStatus).HasMaxLength(20);

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionLogs_Transactions");
        });

        modelBuilder.Entity<TransactionWithdrawSignature>(entity =>
        {
            entity.ToTable("TransactionWithdrawSignature");

            entity.Property(e => e.Signature).HasMaxLength(250);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.Transaction).WithMany(p => p.TransactionWithdrawSignatures)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionWithdrawSignature_Transactions");
        });

        modelBuilder.Entity<UserFinancial>(entity =>
        {
            entity.HasKey(e => e.UserName);

            entity.ToTable("UserFinancial");

            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.LastDepositDate).HasPrecision(0);
            entity.Property(e => e.LastSurveyDate).HasPrecision(0);
            entity.Property(e => e.LastWithdrawDate).HasPrecision(0);
            entity.Property(e => e.TeamComission).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.TodayIncome).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.UserBalance).HasColumnType("decimal(18, 6)");
        });

        modelBuilder.Entity<UserFinancialComission>(entity =>
        {
            entity.ToTable("UserFinancialComission");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ComissionFrom).HasMaxLength(50);
            entity.Property(e => e.Percentage).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSess__3214EC0780E42ED4");

            entity.Property(e => e.ExpirationTime).HasColumnType("datetime");
            entity.Property(e => e.LastActivityTime).HasColumnType("datetime");
            entity.Property(e => e.LoginTime).HasColumnType("datetime");
            entity.Property(e => e.SessionId).HasMaxLength(450);
            entity.Property(e => e.UserName).HasMaxLength(50);

            entity.HasOne(d => d.UserNameNavigation).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSessions_AppUsers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
