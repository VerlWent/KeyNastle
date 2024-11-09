using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API1.Models;

public partial class KeynastleContext : DbContext
{
    public KeynastleContext()
    {
    }

    public KeynastleContext(DbContextOptions<KeynastleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<GenreT> GenreTs { get; set; }

    public virtual DbSet<KeyStatusT> KeyStatusTs { get; set; }

    public virtual DbSet<KeyStorageT> KeyStorageTs { get; set; }

    public virtual DbSet<PreviewT> PreviewTs { get; set; }

    public virtual DbSet<ProductGenreT> ProductGenreTs { get; set; }

    public virtual DbSet<ProductKeyT> ProductKeyTs { get; set; }

    public virtual DbSet<ProductPreviewT> ProductPreviewTs { get; set; }

    public virtual DbSet<ProductT> ProductTs { get; set; }

    public virtual DbSet<RoleT> RoleTs { get; set; }

    public virtual DbSet<UserBusketT> UserBusketTs { get; set; }

    public virtual DbSet<UserProductT> UserProductTs { get; set; }

    public virtual DbSet<UserPurchaseT> UserPurchaseTs { get; set; }

    public virtual DbSet<UserT> UserTs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=chloe700A;database=keynastle", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<GenreT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("genre_t");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.GenreName).HasMaxLength(45);
        });

        modelBuilder.Entity<KeyStatusT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("key-status_t");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.KeyStatus).HasMaxLength(45);
        });

        modelBuilder.Entity<KeyStorageT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("key-storage_t");

            entity.HasIndex(e => e.StatusId, "Key-Storage_Status_Id_FK_idx");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).HasMaxLength(25);
            entity.Property(e => e.StatusId).HasColumnName("Status_Id");

            entity.HasOne(d => d.Status).WithMany(p => p.KeyStorageTs)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Key-Storage_Status_Id_FK");
        });

        modelBuilder.Entity<PreviewT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("preview_t");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PreviewContent).HasMaxLength(256);
        });

        modelBuilder.Entity<ProductGenreT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product/genre_t");

            entity.HasIndex(e => e.GenreId, "Product/Genre_Genre_Id_FK_idx");

            entity.HasIndex(e => e.ProductId, "Product/Genre_Product_ID_FK_idx");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.GenreId).HasColumnName("Genre_Id");
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");

            entity.HasOne(d => d.Genre).WithMany(p => p.ProductGenreTs)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product/Genre_Genre_Id_FK");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductGenreTs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product/Genre_Product_ID_FK");
        });

        modelBuilder.Entity<ProductKeyT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product/key_t");

            entity.HasIndex(e => e.KeyId, "Key_Id_FK_idx");

            entity.HasIndex(e => e.ProductId, "Product_Id_FK_idx");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.KeyId).HasColumnName("Key_Id");
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");

            entity.HasOne(d => d.Key).WithMany(p => p.ProductKeyTs)
                .HasForeignKey(d => d.KeyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Key_Id_FK");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductKeyTs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product_Id_FK");
        });

        modelBuilder.Entity<ProductPreviewT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product/preview_t");

            entity.HasIndex(e => e.PreviewId, "Product/Preview_Preview_Id_FK_idx");

            entity.HasIndex(e => e.ProductId, "Product/Preview_Product_Id_FK_idx");

            entity.HasIndex(e => e.UserLogin, "Product/Preview_User_Login_FK_idx");

            entity.Property(e => e.PreviewId).HasColumnName("Preview_Id");
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(20)
                .HasColumnName("User_Login");

            entity.HasOne(d => d.Preview).WithMany(p => p.ProductPreviewTs)
                .HasForeignKey(d => d.PreviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product/Preview_Preview_Id_FK");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPreviewTs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product/Preview_Product_Id_FK");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.ProductPreviewTs)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Product/Preview_User_Login_FK");
        });

        modelBuilder.Entity<ProductT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("product_t");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasColumnType("mediumtext");
            entity.Property(e => e.Image).HasMaxLength(45);
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        modelBuilder.Entity<RoleT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("role_t");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RoleName).HasMaxLength(45);
        });

        modelBuilder.Entity<UserBusketT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user/busket_t");

            entity.HasIndex(e => e.ProductId, "UserBusket/Product/Id_FK_idx");

            entity.HasIndex(e => e.UserLogin, "UserBusket/User/Login_FK_idx");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ProductId).HasColumnName("Product_Id");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(20)
                .HasColumnName("User_Login");

            entity.HasOne(d => d.Product).WithMany(p => p.UserBusketTs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserBusket/Product/Id_FK");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.UserBusketTs)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserBusket/User/Login_FK");
        });

        modelBuilder.Entity<UserProductT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user/product_t");

            entity.HasIndex(e => e.ProductId, "UserProduct/Product/Id_FK_idx");

            entity.HasIndex(e => e.UserLogin, "UserProduct/User/Login_FK_idx");

            entity.Property(e => e.ProductId).HasColumnName("Product_Id");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(20)
                .HasColumnName("User_Login");

            entity.HasOne(d => d.Product).WithMany(p => p.UserProductTs)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserProduct/Product/Id_FK");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.UserProductTs)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserProduct/User/Login_FK");
        });

        modelBuilder.Entity<UserPurchaseT>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user/purchase_t");

            entity.HasIndex(e => e.ProductKeyId, "UserPurchase/ProductKey/Id_FK_idx");

            entity.HasIndex(e => e.UserLogin, "UserPurchase/User/ID_FK_idx");

            entity.Property(e => e.ProductKeyId).HasColumnName("ProductKey_Id");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(20)
                .HasColumnName("User_Login");

            entity.HasOne(d => d.ProductKey).WithMany(p => p.UserPurchaseTs)
                .HasForeignKey(d => d.ProductKeyId)
                .HasConstraintName("UserPurchase/ProductKey/ID_FK");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.UserPurchaseTs)
                .HasForeignKey(d => d.UserLogin)
                .HasConstraintName("UserPurchase/User/ID_FK");
        });

        modelBuilder.Entity<UserT>(entity =>
        {
            entity.HasKey(e => e.Login).HasName("PRIMARY");

            entity.ToTable("user_t");

            entity.HasIndex(e => e.RoleId, "User/Role/Id_FK_idx");

            entity.Property(e => e.Login).HasMaxLength(20);
            entity.Property(e => e.Certificate).HasMaxLength(45);
            entity.Property(e => e.Email).HasMaxLength(45);
            entity.Property(e => e.NickName).HasMaxLength(45);
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Salt).HasMaxLength(256);

            entity.HasOne(d => d.Role).WithMany(p => p.UserTs)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User/Role/Id_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
