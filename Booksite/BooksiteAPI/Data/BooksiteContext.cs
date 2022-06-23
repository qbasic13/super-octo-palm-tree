using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BooksiteAPI.Data
{
    public partial class BooksiteContext : DbContext
    {
        public BooksiteContext()
        {

        }

        public BooksiteContext(DbContextOptions<BooksiteContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<M2mOrdersBook> M2mOrdersBooks { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<RefreshSession> RefreshSessions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserType> UserTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.BIsbn)
                    .HasName("PK_b");

                entity.ToTable("books");

                entity.HasIndex(e => e.BGenre, "IXFK_b_g");

                entity.HasIndex(e => e.BTitle, "IX_b_title");

                entity.Property(e => e.BIsbn)
                    .HasMaxLength(20)
                    .HasColumnName("b_isbn");

                entity.Property(e => e.BAuthor)
                    .HasMaxLength(150)
                    .HasColumnName("b_author");

                entity.Property(e => e.BCoverFile)
                    .HasMaxLength(150)
                    .HasColumnName("b_cover_file");

                entity.Property(e => e.BGenre).HasColumnName("b_genre");

                entity.Property(e => e.BPrice)
                    .HasColumnType("money")
                    .HasColumnName("b_price");

                entity.Property(e => e.BPublishYear).HasColumnName("b_publish_year");

                entity.Property(e => e.BQuantity).HasColumnName("b_quantity");

                entity.Property(e => e.BTitle)
                    .HasMaxLength(150)
                    .HasColumnName("b_title");

                entity.HasOne(d => d.BGenreNavigation)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.BGenre)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_b_g");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.GId)
                    .HasName("PK_g");

                entity.ToTable("genres");

                entity.HasIndex(e => e.GName, "UNQ_g_name")
                    .IsUnique();

                entity.Property(e => e.GId).HasColumnName("g_id");

                entity.Property(e => e.GName)
                    .HasMaxLength(50)
                    .HasColumnName("g_name");
            });

            modelBuilder.Entity<M2mOrdersBook>(entity =>
            {
                entity.HasKey(e => new { e.M2mobOId, e.M2mobBIsbn })
                    .HasName("PK_m2mob");

                entity.ToTable("m2m_orders_books");

                entity.HasIndex(e => e.M2mobBIsbn, "IXFK_m2mob_b");

                entity.HasIndex(e => e.M2mobOId, "IXFK_m2mob_o");

                entity.Property(e => e.M2mobOId).HasColumnName("m2mob_o_id");

                entity.Property(e => e.M2mobBIsbn)
                    .HasMaxLength(20)
                    .HasColumnName("m2mob_b_isbn");

                entity.Property(e => e.M2mobPrice)
                    .HasColumnType("money")
                    .HasColumnName("m2mob_price");

                entity.HasOne(d => d.M2mobBIsbnNavigation)
                    .WithMany(p => p.M2mOrdersBooks)
                    .HasForeignKey(d => d.M2mobBIsbn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_m2mob_b");

                entity.HasOne(d => d.M2mobO)
                    .WithMany(p => p.M2mOrdersBooks)
                    .HasForeignKey(d => d.M2mobOId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_m2mob_o");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OId)
                    .HasName("PK_o");

                entity.ToTable("orders");

                entity.HasIndex(e => e.OStatus, "IXFK_o_os");

                entity.HasIndex(e => e.OCreator, "IXFK_o_u");

                entity.Property(e => e.OId).HasColumnName("o_id");

                entity.Property(e => e.OCompletionDt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("o_completion_dt");

                entity.Property(e => e.OCreationDt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("o_creation_dt");

                entity.Property(e => e.OCreator).HasColumnName("o_creator");

                entity.Property(e => e.OStatus).HasColumnName("o_status");

                entity.Property(e => e.OTotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("o_total_price");

                entity.HasOne(d => d.OCreatorNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OCreator)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_o_u");

                entity.HasOne(d => d.OStatusNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_o_os");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.OsId)
                    .HasName("PK_os");

                entity.ToTable("order_statuses");

                entity.HasIndex(e => e.OsName, "UNQ_os_name")
                    .IsUnique();

                entity.Property(e => e.OsId)
                    .ValueGeneratedNever()
                    .HasColumnName("os_id");

                entity.Property(e => e.OsName)
                    .HasMaxLength(50)
                    .HasColumnName("os_name");
            });

            modelBuilder.Entity<RefreshSession>(entity =>
            {
                entity.HasKey(e => e.RsId)
                    .HasName("PK_rs");

                entity.ToTable("refresh_sessions");

                entity.HasIndex(e => e.RsUserId, "IXFK_rs_u");

                entity.Property(e => e.RsId).HasColumnName("rs_id");

                entity.Property(e => e.RsCreatedAt).HasColumnName("rs_created_at");

                entity.Property(e => e.RsExpiresIn).HasColumnName("rs_expires_in");

                entity.Property(e => e.RsFingerprint)
                    .HasMaxLength(150)
                    .HasColumnName("rs_fingerprint");

                entity.Property(e => e.RsRefreshToken).HasColumnName("rs_refresh_token");

                entity.Property(e => e.RsUserId).HasColumnName("rs_user_id");

                entity.HasOne(d => d.RsUser)
                    .WithMany(p => p.RefreshSessions)
                    .HasForeignKey(d => d.RsUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_rs_u");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PK_u");

                entity.ToTable("users");

                entity.HasIndex(e => e.UEmail, "UNQ_u_email")
                    .IsUnique();

                entity.Property(e => e.UId).HasColumnName("u_id");

                entity.Property(e => e.UEmail)
                    .HasMaxLength(150)
                    .HasColumnName("u_email");

                entity.Property(e => e.UFirstName)
                    .HasMaxLength(50)
                    .HasColumnName("u_first_name");

                entity.Property(e => e.ULastName)
                    .HasMaxLength(50)
                    .HasColumnName("u_last_name");

                entity.Property(e => e.UMiddleName)
                    .HasMaxLength(50)
                    .HasColumnName("u_middle_name");

                entity.Property(e => e.UPassword)
                    .HasMaxLength(128)
                    .HasColumnName("u_password")
                    .IsFixedLength();

                entity.Property(e => e.UPhone)
                    .HasMaxLength(16)
                    .HasColumnName("u_phone");

                entity.Property(e => e.UProfileFile)
                    .HasMaxLength(150)
                    .HasColumnName("u_profile_file");

                entity.Property(e => e.URegisterDt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("u_register_dt");

                entity.HasMany(d => d.M2muutUts)
                    .WithMany(p => p.M2muutUs)
                    .UsingEntity<Dictionary<string, object>>(
                        "M2mUsersUserType",
                        l => l.HasOne<UserType>().WithMany().HasForeignKey("M2muutUtId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_m2muut_ut"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("M2muutUId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_m2m_uut_u"),
                        j =>
                        {
                            j.HasKey("M2muutUId", "M2muutUtId").HasName("PK_m2muut");

                            j.ToTable("m2m_users_user_types");

                            j.HasIndex(new[] { "M2muutUId" }, "IXFK_m2muut_u");

                            j.HasIndex(new[] { "M2muutUtId" }, "IXFK_m2muut_ut");

                            j.IndexerProperty<int>("M2muutUId").HasColumnName("m2muut_u_id");

                            j.IndexerProperty<int>("M2muutUtId").HasColumnName("m2muut_ut_id");
                        });
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.HasKey(e => e.UtId)
                    .HasName("PK_ut");

                entity.ToTable("user_types");

                entity.HasIndex(e => e.UtName, "UNQ_ut_name")
                    .IsUnique();

                entity.Property(e => e.UtId)
                    .ValueGeneratedNever()
                    .HasColumnName("ut_id");

                entity.Property(e => e.UtName)
                    .HasMaxLength(50)
                    .HasColumnName("ut_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
