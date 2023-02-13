﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using batch_webapi.Data;

#nullable disable

namespace batchwebapi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230213131934_remove acl id key")]
    partial class removeaclidkey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("batch_webapi.Data.Models.ACL", b =>
                {
                    b.Property<int>("AclId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AclId"));

                    b.HasKey("AclId");

                    b.ToTable("ACL");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.Attributes", b =>
                {
                    b.Property<int>("AttributesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttributesId"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AttributesId");

                    b.ToTable("Attributes");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.Batch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AclId")
                        .HasColumnType("int");

                    b.Property<int>("AttributesId")
                        .HasColumnType("int");

                    b.Property<Guid>("BatchId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("BatchPublishedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("BusinessUnit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AclId")
                        .IsUnique();

                    b.HasIndex("AttributesId")
                        .IsUnique();

                    b.ToTable("Batch");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Level")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LogEvent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Properties")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.ReadGroups", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<int>("AclId")
                        .HasColumnType("int");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GroupId");

                    b.HasIndex("AclId");

                    b.ToTable("ReadGroups");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.ReadUsers", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<int>("AclId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("AclId");

                    b.ToTable("ReadUsers");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.Batch", b =>
                {
                    b.HasOne("batch_webapi.Data.Models.ACL", "ACL")
                        .WithOne("Batch")
                        .HasForeignKey("batch_webapi.Data.Models.Batch", "AclId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("batch_webapi.Data.Models.Attributes", "Attributes")
                        .WithOne("Batch")
                        .HasForeignKey("batch_webapi.Data.Models.Batch", "AttributesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ACL");

                    b.Navigation("Attributes");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.ReadGroups", b =>
                {
                    b.HasOne("batch_webapi.Data.Models.ACL", "ACL")
                        .WithMany("ReadGroups")
                        .HasForeignKey("AclId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ACL");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.ReadUsers", b =>
                {
                    b.HasOne("batch_webapi.Data.Models.ACL", "ACL")
                        .WithMany("ReadUsers")
                        .HasForeignKey("AclId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ACL");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.ACL", b =>
                {
                    b.Navigation("Batch");

                    b.Navigation("ReadGroups");

                    b.Navigation("ReadUsers");
                });

            modelBuilder.Entity("batch_webapi.Data.Models.Attributes", b =>
                {
                    b.Navigation("Batch");
                });
#pragma warning restore 612, 618
        }
    }
}
