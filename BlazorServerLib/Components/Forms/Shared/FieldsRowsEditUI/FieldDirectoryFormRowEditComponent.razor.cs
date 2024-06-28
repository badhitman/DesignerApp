﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using BlazorLib;
using BlazorLib.Components;
using BlazorWebLib.Components.Forms.Pages;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib;

namespace BlazorWebLib.Components.Forms.Shared.FieldsRowsEditUI;

/// <summary>
/// Field directory form row edit
/// </summary>
public partial class FieldDirectoryFormRowEditComponent : BlazorBusyComponentBaseModel, IDomBaseComponent
{
    /// <inheritdoc/>
    [Inject]
    protected ISnackbar SnackbarRepo { get; set; } = default!;

    /// <inheritdoc/>
    [Inject]
    protected IFormsService FormsRepo { get; set; } = default!;

    ///// <inheritdoc/>
    //[CascadingParameter, EditorRequired]
    //public FormConstructorModelDB Form { get; set; } = default!;

    /// <inheritdoc/>
    [CascadingParameter, EditorRequired]
    public Action<LinkDirectoryToFormConstructorModelDB> StateHasChangedHandler { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter, EditorRequired]
    public LinkDirectoryToFormConstructorModelDB Field { get; set; } = default!;

    /// <summary>
    /// Форма
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required FormConstructorModelDB Form { get; set; }

    /// <summary>
    /// Родительская страница форм
    /// </summary>
    [CascadingParameter, EditorRequired]
    public required ConstrucnorPage ParentFormsPage { get; set; }

    /// <inheritdoc/>
    protected IEnumerable<SystemEntryModel> Entries = default!;

    /// <inheritdoc/>
    public string DomID => $"{Field.GetType().FullName}_{Field.Id}";

    /// <inheritdoc/>
    public int SelectedDirectoryField
    {
        get => Field.DirectoryId;
        private set
        {
            Field.DirectoryId = value;
            if (Field.Directory is not null)
                Field.Directory.Id = value;
            StateHasChangedHandler(Field);
        }
    }

    /// <inheritdoc/>
    public void Update(LinkDirectoryToFormConstructorModelDB field)
    {
        Field.Update(field);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        IsBusyProgress = true;
        TResponseStrictModel<SystemEntryModel[]> rest = await FormsRepo.GetDirectories(Form.ProjectId);
        IsBusyProgress = false;

        Entries = rest.Response;
        StateHasChanged();
    }
}