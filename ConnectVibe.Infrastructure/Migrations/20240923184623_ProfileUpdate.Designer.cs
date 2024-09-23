﻿// <auto-generated />
using System;
using ConnectVibe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    [DbContext(typeof(ConnectVibeDbContext))]
    [Migration("20240923184623_ProfileUpdate")]
    partial class ProfileUpdate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ConnectVibe.Domain.Entities.OTP", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("OTPs");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.AddressComponent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("LocationResultId")
                        .HasColumnType("int");

                    b.Property<string>("LongName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Types")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LocationResultId");

                    b.ToTable("AddressComponent");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Ethnicity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EthnicityType")
                        .HasColumnType("int");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Ethnicity");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Gender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GenderType")
                        .HasColumnType("int");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Gender");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Geometry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("LocationType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Geometry");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.HaveKids", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("HaveKidsType")
                        .HasColumnType("int");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("HaveKids");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<double>("Lat")
                        .HasColumnType("float");

                    b.Property<double>("Lng")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.LocationDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId")
                        .IsUnique();

                    b.ToTable("LocationDetails");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.LocationResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FormattedAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GeometryId")
                        .HasColumnType("int");

                    b.Property<int?>("LocationDetailId")
                        .HasColumnType("int");

                    b.Property<string>("PlaceId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Types")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GeometryId");

                    b.HasIndex("LocationDetailId");

                    b.ToTable("LocationResult");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Locationn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Lat")
                        .HasColumnType("float");

                    b.Property<double>("Lng")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Locationn");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.ProfilePhoto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Caption")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserProfileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserProfileId");

                    b.ToTable("ProfilePhoto");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Religion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<int>("ReligionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Religion");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.SexualOrientation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<int>("SexualOrientationType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SexualOrientation");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AllowNotifications")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<int>("EducationStatusId")
                        .HasColumnType("int");

                    b.Property<int>("EthnicityId")
                        .HasColumnType("int");

                    b.Property<int>("GenderId")
                        .HasColumnType("int");

                    b.Property<int?>("HaveKidsId")
                        .HasColumnType("int");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<int>("OccupationId")
                        .HasColumnType("int");

                    b.Property<string>("Passions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfileTypes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReligionId")
                        .HasColumnType("int");

                    b.Property<int?>("SexualOrientationId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("WantKidsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EducationStatusId");

                    b.HasIndex("EthnicityId");

                    b.HasIndex("GenderId");

                    b.HasIndex("HaveKidsId");

                    b.HasIndex("LocationId");

                    b.HasIndex("OccupationId");

                    b.HasIndex("ReligionId");

                    b.HasIndex("SexualOrientationId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.HasIndex("WantKidsId");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.WantKids", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<int>("WantKidsType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("WantKids");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEmailValidated")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Fliq.Domain.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("Environment")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Method")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Provider")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Fliq.Domain.Entities.Profile.EducationStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EducationLevel")
                        .HasColumnType("int");

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("EducationStatus");
                });

            modelBuilder.Entity("Fliq.Domain.Entities.Profile.Occupation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsVisible")
                        .HasColumnType("bit");

                    b.Property<string>("OccupationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Occupation");
                });

            modelBuilder.Entity("Fliq.Domain.Entities.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("Environment")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Provider")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.OTP", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.AddressComponent", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.Profile.LocationResult", null)
                        .WithMany("AddressComponents")
                        .HasForeignKey("LocationResultId");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Geometry", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Locationn", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.LocationDetail", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Location", "Location")
                        .WithOne("LocationDetail")
                        .HasForeignKey("ConnectVibe.Domain.Entities.Profile.LocationDetail", "LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.LocationResult", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Geometry", "Geometry")
                        .WithMany()
                        .HasForeignKey("GeometryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.LocationDetail", null)
                        .WithMany("Results")
                        .HasForeignKey("LocationDetailId");

                    b.Navigation("Geometry");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.ProfilePhoto", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.Profile.UserProfile", null)
                        .WithMany("Photos")
                        .HasForeignKey("UserProfileId");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.UserProfile", b =>
                {
                    b.HasOne("Fliq.Domain.Entities.Profile.EducationStatus", "EducationStatus")
                        .WithMany()
                        .HasForeignKey("EducationStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Ethnicity", "Ethnicity")
                        .WithMany()
                        .HasForeignKey("EthnicityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Gender", "Gender")
                        .WithMany()
                        .HasForeignKey("GenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.HaveKids", "HaveKids")
                        .WithMany()
                        .HasForeignKey("HaveKidsId");

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Fliq.Domain.Entities.Profile.Occupation", "Occupation")
                        .WithMany()
                        .HasForeignKey("OccupationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.Religion", "Religion")
                        .WithMany()
                        .HasForeignKey("ReligionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.SexualOrientation", "SexualOrientation")
                        .WithMany()
                        .HasForeignKey("SexualOrientationId");

                    b.HasOne("ConnectVibe.Domain.Entities.User", "User")
                        .WithOne("UserProfile")
                        .HasForeignKey("ConnectVibe.Domain.Entities.Profile.UserProfile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ConnectVibe.Domain.Entities.Profile.WantKids", "WantKids")
                        .WithMany()
                        .HasForeignKey("WantKidsId");

                    b.Navigation("EducationStatus");

                    b.Navigation("Ethnicity");

                    b.Navigation("Gender");

                    b.Navigation("HaveKids");

                    b.Navigation("Location");

                    b.Navigation("Occupation");

                    b.Navigation("Religion");

                    b.Navigation("SexualOrientation");

                    b.Navigation("User");

                    b.Navigation("WantKids");
                });

            modelBuilder.Entity("Fliq.Domain.Entities.Payment", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.User", null)
                        .WithMany("Payments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Fliq.Domain.Entities.Subscription", b =>
                {
                    b.HasOne("ConnectVibe.Domain.Entities.User", null)
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.Location", b =>
                {
                    b.Navigation("LocationDetail")
                        .IsRequired();
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.LocationDetail", b =>
                {
                    b.Navigation("Results");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.LocationResult", b =>
                {
                    b.Navigation("AddressComponents");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.Profile.UserProfile", b =>
                {
                    b.Navigation("Photos");
                });

            modelBuilder.Entity("ConnectVibe.Domain.Entities.User", b =>
                {
                    b.Navigation("Payments");

                    b.Navigation("Subscriptions");

                    b.Navigation("UserProfile");
                });
#pragma warning restore 612, 618
        }
    }
}
