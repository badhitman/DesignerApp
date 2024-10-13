using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DbPostgreLib.Migrations.Constructor
{
    /// <inheritdoc />
    public partial class ConstructorContext001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContextName = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<string>(type: "text", nullable: false),
                    SchemeLastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Directories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSchemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSchemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSchemes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false),
                    Css = table.Column<string>(type: "text", nullable: true),
                    AddRowButtonTitle = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forms_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Manufactures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    Namespace = table.Column<string>(type: "text", nullable: false),
                    EnumDirectoryPath = table.Column<string>(type: "text", nullable: false),
                    DocumentsMastersDbDirectoryPath = table.Column<string>(type: "text", nullable: false),
                    AccessDataDirectoryPath = table.Column<string>(type: "text", nullable: false),
                    BlazorDirectoryPath = table.Column<string>(type: "text", nullable: false),
                    BlazorSplitFiles = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufactures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manufactures_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembersOfProjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersOfProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembersOfProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectsSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Token = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectsSnapshots_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectsUse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectsUse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectsUse_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectoriesOuterJoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DirectoryId = table.Column<int>(type: "integer", nullable: true),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoriesOuterJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectoriesOuterJoins_Directories_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "Directories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DirectoriesOuterJoins_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementsOfDirectories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentId = table.Column<int>(type: "integer", nullable: false),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementsOfDirectories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementsOfDirectories_Directories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Directories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsOuterJoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsOuterJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsOuterJoins_DocumentSchemes_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "DocumentSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentsOuterJoins_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NormalizedUpperName = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    AuthorUser = table.Column<string>(type: "text", nullable: false),
                    EmailsNotifications = table.Column<string>(type: "text", nullable: true),
                    Editors = table.Column<string>(type: "text", nullable: true),
                    SessionToken = table.Column<string>(type: "text", nullable: true),
                    SessionStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastDocumentUpdateActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ShowDescriptionAsStartPage = table.Column<bool>(type: "boolean", nullable: false),
                    DeadlineDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_DocumentSchemes_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DocumentSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabsOfDocumentsSchemes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabsOfDocumentsSchemes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabsOfDocumentsSchemes_DocumentSchemes_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DocumentSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Hint = table.Column<string>(type: "text", nullable: true),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    Css = table.Column<string>(type: "text", nullable: true),
                    TypeField = table.Column<int>(type: "integer", nullable: false),
                    MetadataValueType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fields_Forms_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormsOuterJoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsOuterJoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormsOuterJoins_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormsOuterJoins_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinksDirectoriesToForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    IsMultiSelect = table.Column<bool>(type: "boolean", nullable: false),
                    DirectoryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    Hint = table.Column<string>(type: "text", nullable: true),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    Css = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinksDirectoriesToForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinksDirectoriesToForms_Directories_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "Directories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinksDirectoriesToForms_Forms_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemNamesManufactures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeDataId = table.Column<int>(type: "integer", nullable: false),
                    TypeDataName = table.Column<string>(type: "text", nullable: false),
                    Qualification = table.Column<string>(type: "text", nullable: true),
                    SystemName = table.Column<string>(type: "text", nullable: true),
                    ManufactureId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemNamesManufactures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemNamesManufactures_Manufactures_ManufactureId",
                        column: x => x.ManufactureId,
                        principalTable: "Manufactures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DirectoriesSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    TokenUniqueRoute = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoriesSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectoriesSnapshots_ProjectsSnapshots_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "ProjectsSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    TokenUniqueRoute = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsSnapshots_ProjectsSnapshots_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "ProjectsSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabsJoinsForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    ShowTitle = table.Column<bool>(type: "boolean", nullable: false),
                    IsTable = table.Column<bool>(type: "boolean", nullable: false),
                    TabId = table.Column<int>(type: "integer", nullable: false),
                    FormId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabsJoinsForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabsJoinsForms_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TabsJoinsForms_TabsOfDocumentsSchemes_TabId",
                        column: x => x.TabId,
                        principalTable: "TabsOfDocumentsSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementsOfDirectoriesSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    TokenUniqueRoute = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementsOfDirectoriesSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementsOfDirectoriesSnapshots_DirectoriesSnapshots_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DirectoriesSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabsSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    TokenUniqueRoute = table.Column<string>(type: "text", nullable: false),
                    SortIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabsSnapshots_DocumentsSnapshots_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DocumentsSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValuesSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JoinFormToTabId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    RowNum = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValuesSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValuesSessions_Sessions_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValuesSessions_TabsJoinsForms_JoinFormToTabId",
                        column: x => x.JoinFormToTabId,
                        principalTable: "TabsJoinsForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormsSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    TokenUniqueRoute = table.Column<string>(type: "text", nullable: false),
                    SortIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormsSnapshots_TabsSnapshots_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "TabsSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldsSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Discriminator = table.Column<string>(type: "character varying(34)", maxLength: 34, nullable: false),
                    DirectoryId = table.Column<int>(type: "integer", nullable: true),
                    TypeField = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    SystemName = table.Column<string>(type: "text", nullable: false),
                    TokenUniqueRoute = table.Column<string>(type: "text", nullable: false),
                    SortIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldsSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldsSnapshots_DirectoriesSnapshots_DirectoryId",
                        column: x => x.DirectoryId,
                        principalTable: "DirectoriesSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldsSnapshots_FormsSnapshots_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "FormsSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directories_Name",
                table: "Directories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Directories_Name_ProjectId",
                table: "Directories",
                columns: new[] { "Name", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Directories_ProjectId",
                table: "Directories",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoriesOuterJoins_DirectoryId",
                table: "DirectoriesOuterJoins",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoriesOuterJoins_ProjectId",
                table: "DirectoriesOuterJoins",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoriesSnapshots_Name",
                table: "DirectoriesSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoriesSnapshots_OwnerId",
                table: "DirectoriesSnapshots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoriesSnapshots_TokenUniqueRoute",
                table: "DirectoriesSnapshots",
                column: "TokenUniqueRoute",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSchemes_Name",
                table: "DocumentSchemes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSchemes_Name_ProjectId",
                table: "DocumentSchemes",
                columns: new[] { "Name", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSchemes_ProjectId",
                table: "DocumentSchemes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsOuterJoins_DocumentId",
                table: "DocumentsOuterJoins",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsOuterJoins_ProjectId",
                table: "DocumentsOuterJoins",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsSnapshots_Name",
                table: "DocumentsSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsSnapshots_OwnerId",
                table: "DocumentsSnapshots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsSnapshots_TokenUniqueRoute",
                table: "DocumentsSnapshots",
                column: "TokenUniqueRoute",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectories_Name",
                table: "ElementsOfDirectories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectories_Name_ParentId",
                table: "ElementsOfDirectories",
                columns: new[] { "Name", "ParentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectories_ParentId",
                table: "ElementsOfDirectories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectories_SortIndex",
                table: "ElementsOfDirectories",
                column: "SortIndex");

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectoriesSnapshots_Name",
                table: "ElementsOfDirectoriesSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectoriesSnapshots_OwnerId",
                table: "ElementsOfDirectoriesSnapshots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementsOfDirectoriesSnapshots_TokenUniqueRoute",
                table: "ElementsOfDirectoriesSnapshots",
                column: "TokenUniqueRoute",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Name",
                table: "Fields",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_OwnerId",
                table: "Fields",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_Required",
                table: "Fields",
                column: "Required");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_SortIndex",
                table: "Fields",
                column: "SortIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Fields_TypeField",
                table: "Fields",
                column: "TypeField");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsSnapshots_DirectoryId",
                table: "FieldsSnapshots",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsSnapshots_Name",
                table: "FieldsSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FieldsSnapshots_OwnerId_SortIndex",
                table: "FieldsSnapshots",
                columns: new[] { "OwnerId", "SortIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldsSnapshots_TokenUniqueRoute",
                table: "FieldsSnapshots",
                column: "TokenUniqueRoute",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Name",
                table: "Forms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Name_ProjectId",
                table: "Forms",
                columns: new[] { "Name", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_ProjectId",
                table: "Forms",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsOuterJoins_FormId",
                table: "FormsOuterJoins",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsOuterJoins_ProjectId",
                table: "FormsOuterJoins",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsSnapshots_Name",
                table: "FormsSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_FormsSnapshots_OwnerId",
                table: "FormsSnapshots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsSnapshots_TokenUniqueRoute",
                table: "FormsSnapshots",
                column: "TokenUniqueRoute",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinksDirectoriesToForms_DirectoryId",
                table: "LinksDirectoriesToForms",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LinksDirectoriesToForms_Name",
                table: "LinksDirectoriesToForms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_LinksDirectoriesToForms_OwnerId",
                table: "LinksDirectoriesToForms",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LinksDirectoriesToForms_Required",
                table: "LinksDirectoriesToForms",
                column: "Required");

            migrationBuilder.CreateIndex(
                name: "IX_LinksDirectoriesToForms_SortIndex",
                table: "LinksDirectoriesToForms",
                column: "SortIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Manufactures_ProjectId",
                table: "Manufactures",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersOfProjects_ProjectId",
                table: "MembersOfProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersOfProjects_UserId",
                table: "MembersOfProjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsDisabled",
                table: "Projects",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Name",
                table: "Projects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerUserId",
                table: "Projects",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsSnapshots_Name",
                table: "ProjectsSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsSnapshots_ProjectId",
                table: "ProjectsSnapshots",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUse_ProjectId",
                table: "ProjectsUse",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUse_UserId",
                table: "ProjectsUse",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AuthorUser",
                table: "Sessions",
                column: "AuthorUser");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedAt",
                table: "Sessions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DeadlineDate",
                table: "Sessions",
                column: "DeadlineDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_LastDocumentUpdateActivity",
                table: "Sessions",
                column: "LastDocumentUpdateActivity");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Name",
                table: "Sessions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_OwnerId_ProjectId_NormalizedUpperName",
                table: "Sessions",
                columns: new[] { "OwnerId", "ProjectId", "NormalizedUpperName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ProjectId",
                table: "Sessions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionStatus",
                table: "Sessions",
                column: "SessionStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionToken",
                table: "Sessions",
                column: "SessionToken");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNamesManufactures_ManufactureId",
                table: "SystemNamesManufactures",
                column: "ManufactureId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemNamesManufactures_TypeDataName_ManufactureId_SystemNa~",
                table: "SystemNamesManufactures",
                columns: new[] { "TypeDataName", "ManufactureId", "SystemName", "TypeDataId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TabsJoinsForms_FormId",
                table: "TabsJoinsForms",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_TabsJoinsForms_IsTable",
                table: "TabsJoinsForms",
                column: "IsTable");

            migrationBuilder.CreateIndex(
                name: "IX_TabsJoinsForms_SortIndex",
                table: "TabsJoinsForms",
                column: "SortIndex");

            migrationBuilder.CreateIndex(
                name: "IX_TabsJoinsForms_TabId",
                table: "TabsJoinsForms",
                column: "TabId");

            migrationBuilder.CreateIndex(
                name: "IX_TabsOfDocumentsSchemes_Name",
                table: "TabsOfDocumentsSchemes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TabsOfDocumentsSchemes_Name_OwnerId",
                table: "TabsOfDocumentsSchemes",
                columns: new[] { "Name", "OwnerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TabsOfDocumentsSchemes_OwnerId",
                table: "TabsOfDocumentsSchemes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TabsOfDocumentsSchemes_SortIndex_OwnerId",
                table: "TabsOfDocumentsSchemes",
                columns: new[] { "SortIndex", "OwnerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TabsSnapshots_Name",
                table: "TabsSnapshots",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TabsSnapshots_OwnerId",
                table: "TabsSnapshots",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TabsSnapshots_TokenUniqueRoute",
                table: "TabsSnapshots",
                column: "TokenUniqueRoute",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValuesSessions_JoinFormToTabId",
                table: "ValuesSessions",
                column: "JoinFormToTabId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuesSessions_Name",
                table: "ValuesSessions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ValuesSessions_OwnerId",
                table: "ValuesSessions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuesSessions_RowNum",
                table: "ValuesSessions",
                column: "RowNum");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoriesOuterJoins");

            migrationBuilder.DropTable(
                name: "DocumentsOuterJoins");

            migrationBuilder.DropTable(
                name: "ElementsOfDirectories");

            migrationBuilder.DropTable(
                name: "ElementsOfDirectoriesSnapshots");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.DropTable(
                name: "FieldsSnapshots");

            migrationBuilder.DropTable(
                name: "FormsOuterJoins");

            migrationBuilder.DropTable(
                name: "LinksDirectoriesToForms");

            migrationBuilder.DropTable(
                name: "MembersOfProjects");

            migrationBuilder.DropTable(
                name: "ProjectsUse");

            migrationBuilder.DropTable(
                name: "SystemNamesManufactures");

            migrationBuilder.DropTable(
                name: "ValuesSessions");

            migrationBuilder.DropTable(
                name: "DirectoriesSnapshots");

            migrationBuilder.DropTable(
                name: "FormsSnapshots");

            migrationBuilder.DropTable(
                name: "Directories");

            migrationBuilder.DropTable(
                name: "Manufactures");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "TabsJoinsForms");

            migrationBuilder.DropTable(
                name: "TabsSnapshots");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "TabsOfDocumentsSchemes");

            migrationBuilder.DropTable(
                name: "DocumentsSnapshots");

            migrationBuilder.DropTable(
                name: "DocumentSchemes");

            migrationBuilder.DropTable(
                name: "ProjectsSnapshots");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
