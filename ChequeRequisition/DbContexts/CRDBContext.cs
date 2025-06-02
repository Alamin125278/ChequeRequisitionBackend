using System;
using System.Collections.Generic;
using ChequeRequisiontService.Models.CRDB;
using Microsoft.EntityFrameworkCore;

namespace ChequeRequisiontService.DbContexts;

public partial class CRDBContext : DbContext
{
    public CRDBContext()
    {
    }

    public CRDBContext(DbContextOptions<CRDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Challan> Challans { get; set; }

    public virtual DbSet<ChallanDetail> ChallanDetails { get; set; }

    public virtual DbSet<ChequeBookRequisition> ChequeBookRequisitions { get; set; }

    public virtual DbSet<FtpImport> FtpImports { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<SetSerialNumber> SetSerialNumbers { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<StatusHistory> StatusHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserMenuPermission> UserMenuPermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserRoleDefaultMenuPermission> UserRoleDefaultMenuPermissions { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=ChequeBookRe;User Id=sa;Password=alamin1252;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Trusted_Connection=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC070EC56C5A");

            entity.Property(e => e.ActionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Details).HasColumnType("text");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RequestIp)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLogs__UserI__797309D9");
        });

        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banks__3214EC07F507CD98");

            entity.Property(e => e.BankAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BankCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BankEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BankName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RoutingNumber)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BankCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Bank_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BankUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Banks_UpdatedBy");

            entity.HasOne(d => d.Vendor).WithMany(p => p.Banks)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Banks_VendorId");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Branches__3214EC077E51D051");

            entity.Property(e => e.BranchAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BranchCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BranchPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RoutingNo)
                .HasMaxLength(9)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Bank).WithMany(p => p.Branches)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK_Branches_Bank");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BranchCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Branches_CreatedBy");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BranchUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Branches_UpdatedBy");
        });

        modelBuilder.Entity<Challan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Challans__3214EC079EDF1E5A");

            entity.Property(e => e.ChallanNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChallanCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Challans__Create__5EBF139D");

            entity.HasOne(d => d.ReceivingBranchNavigation).WithMany(p => p.Challans)
                .HasForeignKey(d => d.ReceivingBranch)
                .HasConstraintName("FK__Challans__Receiv__5DCAEF64");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ChallanUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Challans__Update__5FB337D6");
        });

        modelBuilder.Entity<ChallanDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChallanD__3214EC0782D36941");

            entity.HasOne(d => d.Challan).WithMany(p => p.ChallanDetails)
                .HasForeignKey(d => d.ChallanId)
                .HasConstraintName("FK__ChallanDe__Chall__6383C8BA");

            entity.HasOne(d => d.RequisitionItem).WithMany(p => p.ChallanDetails)
                .HasForeignKey(d => d.RequisitionItemId)
                .HasConstraintName("FK__ChallanDe__Requi__628FA481");
        });

        modelBuilder.Entity<ChequeBookRequisition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChequeBo__3214EC07B77056B1");

            entity.Property(e => e.AccountName)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.ChequePrefix)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ChequeType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CusAddress).HasColumnType("text");
            entity.Property(e => e.MicrNo)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.Series)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Bank).WithMany(p => p.ChequeBookRequisitions)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK__ChequeBoo__BankI__5165187F");

            entity.HasOne(d => d.Branch).WithMany(p => p.ChequeBookRequisitionBranches)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__ChequeBoo__Branc__52593CB8");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ChequeBookRequisitionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__ChequeBoo__Creat__5629CD9C");

            entity.HasOne(d => d.ReceivingBranch).WithMany(p => p.ChequeBookRequisitionReceivingBranches)
                .HasForeignKey(d => d.ReceivingBranchId)
                .HasConstraintName("FK__ChequeBoo__recei__5441852A");

            entity.HasOne(d => d.RequestedByNavigation).WithMany(p => p.ChequeBookRequisitionRequestedByNavigations)
                .HasForeignKey(d => d.RequestedBy)
                .HasConstraintName("FK__ChequeBoo__Reque__534D60F1");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.ChequeBookRequisitions)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK__ChequeBoo__Statu__5535A963");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ChequeBookRequisitionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__ChequeBoo__Updat__571DF1D5");
        });

        modelBuilder.Entity<FtpImport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ftp_impo__3213E83F1658B506");

            entity.ToTable("ftp_imports");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BankId).HasColumnName("bank_id");
            entity.Property(e => e.ClearedAt)
                .HasColumnType("datetime")
                .HasColumnName("cleared_at");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Filename)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("filename");
            entity.Property(e => e.ImportedAt)
                .HasColumnType("datetime")
                .HasColumnName("imported_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Bank).WithMany(p => p.FtpImports)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK__ftp_impor__bank___76969D2E");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menus__3214EC0796C7CF49");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Icon)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MenuName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Path)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0795B6F66C");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ReadAt).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__7C4F7684");
        });

        modelBuilder.Entity<SetSerialNumber>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SetSeria__3214EC07B37BF37A");

            entity.Property(e => e.ChequeType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Series)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Bank).WithMany(p => p.SetSerialNumbers)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK__SetSerial__BankI__71D1E811");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SetSerialNumberCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SetSerial__Creat__72C60C4A");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.SetSerialNumberUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__SetSerial__Updat__73BA3083");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Statuses__3214EC0752ACCD56");

            entity.HasIndex(e => e.StatusName, "UQ__Statuses__05E7698AD59545B1").IsUnique();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.StatusName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusHi__3214EC07E5371FF7");

            entity.Property(e => e.ChangedAt).HasColumnType("datetime");
            entity.Property(e => e.ModuleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Remarks).HasColumnType("text");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.StatusHistories)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__StatusHis__Chang__5AEE82B9");

            entity.HasOne(d => d.Status).WithMany(p => p.StatusHistories)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__StatusHis__Statu__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07D99599D4");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Bank).WithMany(p => p.Users)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK_Users_Bank");

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Users_Branch");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Users_CreatedBy");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("FK_Users_UserRoles");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_Users_UpdatedBy");

            entity.HasOne(d => d.Vendor).WithMany(p => p.Users)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_Users_Vendors");
        });

        modelBuilder.Entity<UserMenuPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserMenu__3214EC07DD463418");

            entity.ToTable("UserMenuPermission");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserMenuPermissionCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserMenuPermission_Users1");

            entity.HasOne(d => d.Menu).WithMany(p => p.UserMenuPermissions)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserMenuP__MenuI__6EF57B66");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.UserMenuPermissionUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK_UserMenuPermission_Users");

            entity.HasOne(d => d.User).WithMany(p => p.UserMenuPermissionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserMenuP__UserI__6E01572D");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07502C3C27");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<UserRoleDefaultMenuPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC075FFB7CE4");

            entity.ToTable("UserRoleDefaultMenuPermission");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Menu).WithMany(p => p.UserRoleDefaultMenuPermissions)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK__UserRoleD__MenuI__6B24EA82");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoleDefaultMenuPermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__UserRoleD__RoleI__6A30C649");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vendors__3214EC07AD3B344F");

            entity.HasIndex(e => e.Email, "UQ__Vendors__A9D105344F1A9AE1").IsUnique();

            entity.Property(e => e.Address).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.VendorName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
