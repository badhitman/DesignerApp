﻿// <auto-generated />
using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ToolsMauiAppMigration.Migrations
{
    [DbContext(typeof(ToolsAppContext))]
    [Migration("20250218072707_ToolsAppContext001")]
    partial class ToolsAppContext001
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("SharedLib.ApiRestConfigModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AddressBaseUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("HeaderName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenAccess")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("SharedLib.ExeCommandModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Arguments")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("ParentId");

                    b.ToTable("ExeCommands");
                });

            modelBuilder.Entity("SharedLib.SyncDirectoryModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocalDirectory")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ParentId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RemoteDirectory")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("ParentId");

                    b.ToTable("SyncDirectories");
                });

            modelBuilder.Entity("SharedLib.ExeCommandModelDB", b =>
                {
                    b.HasOne("SharedLib.ApiRestConfigModelDB", "Parent")
                        .WithMany("CommandsRemote")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("SharedLib.SyncDirectoryModelDB", b =>
                {
                    b.HasOne("SharedLib.ApiRestConfigModelDB", "Parent")
                        .WithMany("SyncDirectories")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("SharedLib.ApiRestConfigModelDB", b =>
                {
                    b.Navigation("CommandsRemote");

                    b.Navigation("SyncDirectories");
                });
#pragma warning restore 612, 618
        }
    }
}
