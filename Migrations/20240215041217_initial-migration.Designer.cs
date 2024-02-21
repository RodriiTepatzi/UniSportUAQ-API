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
    [Migration("20240215041217_initial-migration")]
    partial class initialmigration
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

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Attendance", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("AttendanceClass")
                        .HasColumnType("bit");

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Course", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("InstructorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("InstructorId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.CourseClass", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Day")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Hour")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quota")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseClasses");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Inscription", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Accredit")
                        .HasColumnType("bit");

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateInscription")
                        .HasColumnType("datetime2");

                    b.Property<bool>("InInfo")
                        .HasColumnType("bit");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Inscriptions");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Admin", b =>
                {
                    b.HasBaseType("UniSportUAQ_API.Data.Models.ApplicationUser");

                    b.ToTable("Admins", (string)null);
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Instructor", b =>
                {
                    b.HasBaseType("UniSportUAQ_API.Data.Models.ApplicationUser");

                    b.ToTable("Instructors", (string)null);
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Student", b =>
                {
                    b.HasBaseType("UniSportUAQ_API.Data.Models.ApplicationUser");

                    b.Property<string>("CurrentCourse")
                        .HasColumnType("nvarchar(450)");

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
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("SuscribedDateTime")
                        .HasColumnType("datetime2");

                    b.HasIndex("CurrentCourse");

                    b.ToTable("Students", (string)null);
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Attendance", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UniSportUAQ_API.Data.Models.Student", "Student")
                        .WithMany("Attendances")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Course", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.Instructor", "Instructor")
                        .WithMany()
                        .HasForeignKey("InstructorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.CourseClass", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Inscription", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.CourseClass", "CourseClass")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UniSportUAQ_API.Data.Models.Student", "Student")
                        .WithMany("Inscriptions")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CourseClass");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Admin", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.ApplicationUser", null)
                        .WithOne()
                        .HasForeignKey("UniSportUAQ_API.Data.Models.Admin", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Instructor", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.ApplicationUser", null)
                        .WithOne()
                        .HasForeignKey("UniSportUAQ_API.Data.Models.Instructor", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Student", b =>
                {
                    b.HasOne("UniSportUAQ_API.Data.Models.CourseClass", "CourseClass")
                        .WithMany()
                        .HasForeignKey("CurrentCourse");

                    b.HasOne("UniSportUAQ_API.Data.Models.ApplicationUser", null)
                        .WithOne()
                        .HasForeignKey("UniSportUAQ_API.Data.Models.Student", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CourseClass");
                });

            modelBuilder.Entity("UniSportUAQ_API.Data.Models.Student", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("Inscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
