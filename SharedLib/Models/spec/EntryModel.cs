////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SharedLib;

/// <summary>
/// Базовая DB модель объекта с поддержкой -> int:Id +string:Name
/// </summary>
[Index(nameof(Name))]
public class EntryModel
{
    /// <summary>
    /// Идентификатор/Key
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Имя объекта
    /// </summary>
    [NameValid]
    public virtual required string Name { get; set; }

    /// <inheritdoc/>
    public static EntryModel Build(string name) => new() { Name = name };

    /// <inheritdoc/>
    public static EntryModel Build(EntryModel sender) => new() { Name = sender.Name };

    /// <inheritdoc/>
    public static EntryModel BuildEmpty() => new() { Name = "" };

    /// <inheritdoc/>
    public void Update(EntryModel elementObjectEdit)
    {
        Name = elementObjectEdit.Name;
        Id = elementObjectEdit.Id;
    }

    /// <inheritdoc/>
    public static bool operator ==(EntryModel? e1, EntryModel? e2)
        => (e1 is null && e2 is null) || (e1?.Id == e2?.Id && e1?.Name == e2?.Name);

    /// <inheritdoc/>
    public static bool operator !=(EntryModel? e1, EntryModel? e2)
        => !(e1 == e2);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is EntryModel _e)
            return Id == _e.Id && Name == _e.Name;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode() 
        => $"{Id} {Name}".GetHashCode();
}