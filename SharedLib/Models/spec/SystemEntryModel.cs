////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedLib;

/// <summary>
/// Базовая модель с поддержкой -> int:Id +string:Name +string:SystemName +bool:IsDeleted
/// </summary>
public class SystemEntryModel : EntryModel
{
    /// <summary>
    /// System name
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(GlobalStaticConstants.NAME_SPACE_TEMPLATE, ErrorMessage = GlobalStaticConstants.NAME_SPACE_TEMPLATE_MESSAGE)]
    public required string SystemName { get; set; }


    /// <inheritdoc/>
    public void Update(SystemEntryModel other)
    {
        Id = other.Id;
        IsDisabled = other.IsDisabled;
        Name = other.Name;
        SystemName = other.SystemName;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return base.Equals(obj);

        if (obj is SystemEntryModel se)
            return Id == se.Id && IsDisabled == se.IsDisabled && Name == se.Name && SystemName == se.SystemName;

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public static SystemEntryModel BuildEmpty()
        => new() { Name = "", SystemName = "" };

    /// <inheritdoc/>
    public static SystemEntryModel Build(SystemEntryModel sender)
        => new()
        {
            Id = sender.Id,
            IsDisabled = sender.IsDisabled,
            Name = sender.Name,
            SystemName = sender.SystemName
        };

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id} {IsDisabled} {Name} /{SystemName}".GetHashCode();
    }
}