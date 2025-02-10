﻿// <auto-generated />
using System;
using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Storage
{
    [DbContext(typeof(StorageContext))]
    partial class StorageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SharedLib.AccessFileRuleModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessRuleType")
                        .HasColumnType("integer");

                    b.Property<string>("Option")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StoreFileId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("StoreFileId");

                    b.ToTable("RulesFilesAccess");
                });

            modelBuilder.Entity("SharedLib.AltnameKLADRModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("LEVEL")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("NEWCODE")
                        .IsRequired()
                        .HasMaxLength(19)
                        .HasColumnType("character varying(19)");

                    b.Property<string>("OLDCODE")
                        .IsRequired()
                        .HasMaxLength(19)
                        .HasColumnType("character varying(19)");

                    b.HasKey("Id");

                    b.HasIndex("LEVEL");

                    b.HasIndex("NEWCODE");

                    b.HasIndex("OLDCODE");

                    b.ToTable("AltnamesKLADR");
                });

            modelBuilder.Entity("SharedLib.NameMapKLADRModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CODE")
                        .IsRequired()
                        .HasMaxLength(17)
                        .HasColumnType("character varying(17)");

                    b.Property<string>("NAME")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("SCNAME")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("SHNAME")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.HasKey("Id");

                    b.HasIndex("CODE");

                    b.HasIndex("NAME");

                    b.HasIndex("SCNAME");

                    b.HasIndex("SHNAME");

                    b.ToTable("NamesMapsKLADR");
                });

            modelBuilder.Entity("SharedLib.ObjectKLADRModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CODE")
                        .IsRequired()
                        .HasMaxLength(17)
                        .HasColumnType("character varying(17)");

                    b.Property<string>("GNINMB")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.Property<string>("INDEX")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.Property<string>("NAME")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("OCATD")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("SOCR")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("STATUS")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("UNO")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.HasKey("Id");

                    b.HasIndex("CODE");

                    b.HasIndex("GNINMB");

                    b.HasIndex("INDEX");

                    b.HasIndex("NAME");

                    b.HasIndex("OCATD");

                    b.HasIndex("SOCR");

                    b.HasIndex("STATUS");

                    b.HasIndex("UNO");

                    b.ToTable("ObjectsKLADR");
                });

            modelBuilder.Entity("SharedLib.SocrbaseKLADRModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("KOD_T_ST")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("LEVEL")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.Property<string>("SCNAME")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("SOCRNAME")
                        .IsRequired()
                        .HasMaxLength(29)
                        .HasColumnType("character varying(29)");

                    b.HasKey("Id");

                    b.HasIndex("KOD_T_ST");

                    b.HasIndex("LEVEL");

                    b.HasIndex("SCNAME");

                    b.HasIndex("SOCRNAME");

                    b.ToTable("SocrbasesKLADR");
                });

            modelBuilder.Entity("SharedLib.StorageCloudParameterModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("OwnerPrimaryKey")
                        .HasColumnType("integer");

                    b.Property<string>("PrefixPropertyName")
                        .HasColumnType("text");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SerializedDataJson")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TypeName");

                    b.HasIndex("ApplicationName", "PropertyName");

                    b.HasIndex("PrefixPropertyName", "OwnerPrimaryKey");

                    b.ToTable("CloudProperties");
                });

            modelBuilder.Entity("SharedLib.StorageFileModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AuthorIdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("FileLength")
                        .HasColumnType("bigint");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedFileNameUpper")
                        .HasColumnType("text");

                    b.Property<int?>("OwnerPrimaryKey")
                        .HasColumnType("integer");

                    b.Property<string>("PointId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PrefixPropertyName")
                        .HasColumnType("text");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReferrerMain")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorIdentityId");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("FileName");

                    b.HasIndex("NormalizedFileNameUpper");

                    b.HasIndex("PointId");

                    b.HasIndex("ReferrerMain");

                    b.HasIndex("ApplicationName", "PropertyName");

                    b.HasIndex("PrefixPropertyName", "OwnerPrimaryKey");

                    b.ToTable("CloudFiles");
                });

            modelBuilder.Entity("SharedLib.StreetKLADRModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CODE")
                        .IsRequired()
                        .HasMaxLength(17)
                        .HasColumnType("character varying(17)");

                    b.Property<string>("GNINMB")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.Property<string>("INDEX")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.Property<string>("NAME")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<string>("OCATD")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("character varying(11)");

                    b.Property<string>("SOCR")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("UNO")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.HasKey("Id");

                    b.HasIndex("CODE");

                    b.HasIndex("GNINMB");

                    b.HasIndex("INDEX");

                    b.HasIndex("NAME");

                    b.HasIndex("OCATD");

                    b.HasIndex("SOCR");

                    b.HasIndex("UNO");

                    b.ToTable("StreetsKLADR");
                });

            modelBuilder.Entity("SharedLib.TagModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ApplicationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedTagNameUpper")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("OwnerPrimaryKey")
                        .HasColumnType("integer");

                    b.Property<string>("PrefixPropertyName")
                        .HasColumnType("text");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("NormalizedTagNameUpper");

                    b.HasIndex("ApplicationName", "PropertyName");

                    b.HasIndex("PrefixPropertyName", "OwnerPrimaryKey");

                    b.HasIndex(new[] { "NormalizedTagNameUpper", "OwnerPrimaryKey", "ApplicationName" }, "IX_TagNameOwnerKeyUnique")
                        .IsUnique();

                    b.ToTable("CloudTags");
                });

            modelBuilder.Entity("SharedLib.AccessFileRuleModelDB", b =>
                {
                    b.HasOne("SharedLib.StorageFileModelDB", "StoreFile")
                        .WithMany("AccessRules")
                        .HasForeignKey("StoreFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StoreFile");
                });

            modelBuilder.Entity("SharedLib.StorageFileModelDB", b =>
                {
                    b.Navigation("AccessRules");
                });
#pragma warning restore 612, 618
        }
    }
}
