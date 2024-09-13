﻿// <auto-generated />
using System;
using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DbSqliteLib.Migrations.Storage
{
    [DbContext(typeof(StorageContext))]
    [Migration("StorageContext001")]
    partial class StorageContext001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("SharedLib.StorageCloudParameterModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApplicationName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnerPrimaryKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PrefixPropertyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedDataJson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TypeName");

                    b.HasIndex("ApplicationName", "Name");

                    b.HasIndex("PrefixPropertyName", "OwnerPrimaryKey");

                    b.ToTable("CloudProperties");
                });
#pragma warning restore 612, 618
        }
    }
}