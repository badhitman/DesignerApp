﻿// <auto-generated />
using System;
using DbcLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Commerce
{
    [DbContext(typeof(CommerceContext))]
    partial class CommerceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SharedLib.AddressOrganizationModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Contacts")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<int>("ParentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OrganizationId");

                    b.ToTable("AddressesOrganizations");
                });

            modelBuilder.Entity("SharedLib.LockOffersAvailabilityModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("LockerId")
                        .HasColumnType("integer");

                    b.Property<string>("LockerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RubricId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LockerId", "LockerName", "RubricId")
                        .IsUnique();

                    b.ToTable("LockerOffersAvailability");
                });

            modelBuilder.Entity("SharedLib.NomenclatureModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BaseUnit")
                        .HasColumnType("integer");

                    b.Property<string>("ContextName")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedNameUpper")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int>("ProjectId")
                        .HasColumnType("integer");

                    b.Property<long>("SortIndex")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ContextName");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("Name");

                    b.HasIndex("NormalizedNameUpper");

                    b.HasIndex("SortIndex", "ParentId", "ContextName")
                        .IsUnique();

                    b.ToTable("Nomenclatures");
                });

            modelBuilder.Entity("SharedLib.OfferAvailabilityModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("NomenclatureId")
                        .HasColumnType("integer");

                    b.Property<int>("OfferId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<int>("WarehouseId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OfferId");

                    b.HasIndex("Quantity");

                    b.HasIndex("WarehouseId", "OfferId")
                        .IsUnique();

                    b.ToTable("OffersAvailability");
                });

            modelBuilder.Entity("SharedLib.OfferModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("Multiplicity")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NomenclatureId")
                        .HasColumnType("integer");

                    b.Property<int>("OfferUnit")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("QuantitiesTemplate")
                        .HasColumnType("text");

                    b.Property<string>("ShortName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("Name");

                    b.HasIndex("NomenclatureId");

                    b.ToTable("Offers");
                });

            modelBuilder.Entity("SharedLib.OrderDocumentModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthorIdentityUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ExternalDocumentId")
                        .HasColumnType("text");

                    b.Property<int?>("HelpdeskId")
                        .HasColumnType("integer");

                    b.Property<string>("Information")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<int>("StatusDocument")
                        .HasColumnType("integer");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrdersDocuments");
                });

            modelBuilder.Entity("SharedLib.OrganizationModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BankBIC")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CorrespondentAccount")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CurrentAccount")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("INN")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<string>("KPP")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LegalAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NewBankBIC")
                        .HasColumnType("text");

                    b.Property<string>("NewBankName")
                        .HasColumnType("text");

                    b.Property<string>("NewCorrespondentAccount")
                        .HasColumnType("text");

                    b.Property<string>("NewCurrentAccount")
                        .HasColumnType("text");

                    b.Property<string>("NewINN")
                        .HasColumnType("text");

                    b.Property<string>("NewKPP")
                        .HasColumnType("text");

                    b.Property<string>("NewLegalAddress")
                        .HasColumnType("text");

                    b.Property<string>("NewName")
                        .HasColumnType("text");

                    b.Property<string>("NewOGRN")
                        .HasColumnType("text");

                    b.Property<string>("OGRN")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("INN")
                        .IsUnique();

                    b.HasIndex("IsDisabled");

                    b.HasIndex("Name");

                    b.HasIndex("OGRN")
                        .IsUnique();

                    b.HasIndex("BankBIC", "CorrespondentAccount", "CurrentAccount")
                        .IsUnique();

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("SharedLib.PaymentDocumentModelDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("ExternalDocumentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OrderDocumentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("OrderDocumentId");

                    b.ToTable("PaymentsDocuments");
                });

            modelBuilder.Entity("SharedLib.PriceRuleForOfferModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("OfferId")
                        .HasColumnType("integer");

                    b.Property<decimal>("PriceRule")
                        .HasColumnType("numeric");

                    b.Property<decimal>("QuantityRule")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("Name");

                    b.HasIndex("OfferId", "QuantityRule")
                        .IsUnique();

                    b.ToTable("PricesRules");
                });

            modelBuilder.Entity("SharedLib.RowOfOrderDocumentModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AddressForOrderTabId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<int>("NomenclatureId")
                        .HasColumnType("integer");

                    b.Property<int>("OfferId")
                        .HasColumnType("integer");

                    b.Property<int?>("OrderDocumentId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AddressForOrderTabId");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OfferId");

                    b.HasIndex("OrderDocumentId");

                    b.HasIndex("Quantity");

                    b.HasIndex("AddressForOrderTabId", "OfferId")
                        .IsUnique();

                    b.ToTable("RowsOfOrdersDocuments");
                });

            modelBuilder.Entity("SharedLib.RowOfWarehouseDocumentModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("NomenclatureId")
                        .HasColumnType("integer");

                    b.Property<int>("OfferId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("numeric");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<int>("WarehouseDocumentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("NomenclatureId");

                    b.HasIndex("OfferId");

                    b.HasIndex("Quantity");

                    b.HasIndex("WarehouseDocumentId");

                    b.HasIndex("WarehouseDocumentId", "OfferId")
                        .IsUnique();

                    b.ToTable("RowsOfWarehouseDocuments");
                });

            modelBuilder.Entity("SharedLib.TabAddressForOrderModelDb", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AddressOrganizationId")
                        .HasColumnType("integer");

                    b.Property<int>("OrderDocumentId")
                        .HasColumnType("integer");

                    b.Property<int>("WarehouseId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AddressOrganizationId");

                    b.HasIndex("OrderDocumentId");

                    b.HasIndex("WarehouseId");

                    b.ToTable("TabsAddressesForOrders");
                });

            modelBuilder.Entity("SharedLib.UserOrganizationModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("integer");

                    b.Property<string>("UserPersonIdentityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserPersonIdentityId");

                    b.HasIndex("OrganizationId", "UserPersonIdentityId")
                        .IsUnique();

                    b.ToTable("OrganizationsUsers");
                });

            modelBuilder.Entity("SharedLib.WarehouseDocumentModelDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ExternalDocumentId")
                        .HasColumnType("text");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastAtUpdatedUTC")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUpperName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("uuid");

                    b.Property<int>("WarehouseId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryDate");

                    b.HasIndex("IsDisabled");

                    b.HasIndex("Name");

                    b.HasIndex("NormalizedUpperName");

                    b.HasIndex("WarehouseId");

                    b.ToTable("WarehouseDocuments");
                });

            modelBuilder.Entity("SharedLib.AddressOrganizationModelDB", b =>
                {
                    b.HasOne("SharedLib.OrganizationModelDB", "Organization")
                        .WithMany("Addresses")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("SharedLib.OfferAvailabilityModelDB", b =>
                {
                    b.HasOne("SharedLib.NomenclatureModelDB", "Nomenclature")
                        .WithMany()
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.OfferModelDB", "Offer")
                        .WithMany("Registers")
                        .HasForeignKey("OfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Nomenclature");

                    b.Navigation("Offer");
                });

            modelBuilder.Entity("SharedLib.OfferModelDB", b =>
                {
                    b.HasOne("SharedLib.NomenclatureModelDB", "Nomenclature")
                        .WithMany("Offers")
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Nomenclature");
                });

            modelBuilder.Entity("SharedLib.OrderDocumentModelDB", b =>
                {
                    b.HasOne("SharedLib.OrganizationModelDB", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("SharedLib.PaymentDocumentModelDb", b =>
                {
                    b.HasOne("SharedLib.OrderDocumentModelDB", "OrderDocument")
                        .WithMany()
                        .HasForeignKey("OrderDocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderDocument");
                });

            modelBuilder.Entity("SharedLib.PriceRuleForOfferModelDB", b =>
                {
                    b.HasOne("SharedLib.OfferModelDB", "Offer")
                        .WithMany("PricesRules")
                        .HasForeignKey("OfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Offer");
                });

            modelBuilder.Entity("SharedLib.RowOfOrderDocumentModelDB", b =>
                {
                    b.HasOne("SharedLib.TabAddressForOrderModelDb", "AddressForOrderTab")
                        .WithMany("Rows")
                        .HasForeignKey("AddressForOrderTabId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.NomenclatureModelDB", "Nomenclature")
                        .WithMany()
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.OfferModelDB", "Offer")
                        .WithMany()
                        .HasForeignKey("OfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.OrderDocumentModelDB", "OrderDocument")
                        .WithMany()
                        .HasForeignKey("OrderDocumentId");

                    b.Navigation("AddressForOrderTab");

                    b.Navigation("Nomenclature");

                    b.Navigation("Offer");

                    b.Navigation("OrderDocument");
                });

            modelBuilder.Entity("SharedLib.RowOfWarehouseDocumentModelDB", b =>
                {
                    b.HasOne("SharedLib.NomenclatureModelDB", "Nomenclature")
                        .WithMany()
                        .HasForeignKey("NomenclatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.OfferModelDB", "Offer")
                        .WithMany()
                        .HasForeignKey("OfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.WarehouseDocumentModelDB", "WarehouseDocument")
                        .WithMany("Rows")
                        .HasForeignKey("WarehouseDocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Nomenclature");

                    b.Navigation("Offer");

                    b.Navigation("WarehouseDocument");
                });

            modelBuilder.Entity("SharedLib.TabAddressForOrderModelDb", b =>
                {
                    b.HasOne("SharedLib.AddressOrganizationModelDB", "AddressOrganization")
                        .WithMany()
                        .HasForeignKey("AddressOrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SharedLib.OrderDocumentModelDB", "OrderDocument")
                        .WithMany("AddressesTabs")
                        .HasForeignKey("OrderDocumentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AddressOrganization");

                    b.Navigation("OrderDocument");
                });

            modelBuilder.Entity("SharedLib.UserOrganizationModelDB", b =>
                {
                    b.HasOne("SharedLib.OrganizationModelDB", "Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("SharedLib.NomenclatureModelDB", b =>
                {
                    b.Navigation("Offers");
                });

            modelBuilder.Entity("SharedLib.OfferModelDB", b =>
                {
                    b.Navigation("PricesRules");

                    b.Navigation("Registers");
                });

            modelBuilder.Entity("SharedLib.OrderDocumentModelDB", b =>
                {
                    b.Navigation("AddressesTabs");
                });

            modelBuilder.Entity("SharedLib.OrganizationModelDB", b =>
                {
                    b.Navigation("Addresses");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("SharedLib.TabAddressForOrderModelDb", b =>
                {
                    b.Navigation("Rows");
                });

            modelBuilder.Entity("SharedLib.WarehouseDocumentModelDB", b =>
                {
                    b.Navigation("Rows");
                });
#pragma warning restore 612, 618
        }
    }
}
