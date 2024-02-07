﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniSportUAQ_API.Data;

#nullable disable

namespace UniSportUAQ_API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240207030026_testing")]
    partial class testing
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Expediente")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUsers");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Student", b =>
                {
                    b.HasBaseType("UniSportUAQ_API.Data.Models.ApplicationUser");

                    b.Property<int>("CurrentCourse")
                        .HasColumnType("int");

                    b.Property<string>("FinishedCoursesJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Group")
                        .HasColumnType("int");

                    b.Property<bool>("IsInOfficialGroup")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSuscribed")
                        .HasColumnType("bit");

                    b.Property<int>("Semester")
                        .HasColumnType("int");

                    b.Property<string>("StudyPlan")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SuscribedDateTime")
                        .HasColumnType("datetime2");

                    b.ToTable("Students", (string)null);
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Student", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.ApplicationUser", null)
                        .WithOne()
                        .HasForeignKey("UniSportUAQ_API.Data.Student", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
