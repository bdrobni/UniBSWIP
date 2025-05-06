using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace dipwebapp.Models
{
    public partial class diplomskidbContext : DbContext
    {
        public diplomskidbContext()
        {
        }

        public diplomskidbContext(DbContextOptions<diplomskidbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Appuser> Appuser { get; set; }
        public virtual DbSet<Authoredobj> Authoredobj { get; set; }
        public virtual DbSet<Fileassociations> Fileassociations { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Tagassociations> Tagassociations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-JTKM7IQ6;Database=diplomskidb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appuser>(entity =>
            {
                entity.ToTable("appuser");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.Pass)
                    .IsRequired()
                    .HasColumnName("pass")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);

                entity.Property(e => e.Userrole)
                    .IsRequired()
                    .HasColumnName("userrole")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Authoredobj>(entity =>
            {
                entity.ToTable("authoredobj");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Authorid).HasColumnName("authorid");

                entity.Property(e => e.Createddate)
                    .HasColumnName("createddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Filetype)
                    .IsRequired()
                    .HasColumnName("filetype")
                    .HasMaxLength(25);

                entity.Property(e => e.Objcontent)
                    .IsRequired()
                    .HasColumnName("objcontent");

                entity.Property(e => e.Objdescription)
                    .HasColumnName("objdescription")
                    .HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Authoredobj)
                    .HasForeignKey(d => d.Authorid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__authoredo__autho__398D8EEE");
            });

            modelBuilder.Entity<Fileassociations>(entity =>
            {
                entity.HasKey(e => new { e.Parentfileid, e.Associatedid })
                    .HasName("PK__fileasso__35E70E2F558D0A34");

                entity.ToTable("fileassociations");

                entity.Property(e => e.Parentfileid).HasColumnName("parentfileid");

                entity.Property(e => e.Associatedid).HasColumnName("associatedid");

                entity.HasOne(d => d.Associated)
                    .WithMany(p => p.FileassociationsAssociated)
                    .HasForeignKey(d => d.Associatedid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__fileassoc__assoc__4316F928");

                entity.HasOne(d => d.Parentfile)
                    .WithMany(p => p.FileassociationsParentfile)
                    .HasForeignKey(d => d.Parentfileid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__fileassoc__paren__4222D4EF");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tag");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Tagdescription)
                    .IsRequired()
                    .HasColumnName("tagdescription")
                    .HasMaxLength(250);

                entity.Property(e => e.Tagname)
                    .IsRequired()
                    .HasColumnName("tagname")
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<Tagassociations>(entity =>
            {
                entity.HasKey(e => new { e.Tagid, e.Objectid })
                    .HasName("PK__tagassoc__65DF2BAC8316A07D");

                entity.ToTable("tagassociations");

                entity.Property(e => e.Tagid).HasColumnName("tagid");

                entity.Property(e => e.Objectid).HasColumnName("objectid");

                entity.HasOne(d => d.Object)
                    .WithMany(p => p.Tagassociations)
                    .HasForeignKey(d => d.Objectid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tagassoci__objec__3F466844");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.Tagassociations)
                    .HasForeignKey(d => d.Tagid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tagassoci__tagid__3E52440B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
