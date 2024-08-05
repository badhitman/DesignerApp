////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SharedLib;

/// <inheritdoc/>
public class ProjectViewModel : EntryDescriptionSwitchableModel
{
    /// <inheritdoc/>
    public static ProjectViewModel Build(ProjectViewModel other)
    {
        return new()
        {
            OwnerUserId = other.OwnerUserId,
            Name = other.Name,
            Description = other.Description,
            Id = other.Id,
            IsDisabled = other.IsDisabled,
            Members = other.Members,
        };
    }

    /// <summary>
    /// Владелец проекта (user id of Identity)
    /// </summary>
    public required string OwnerUserId { get; set; }

    /// <summary>
    /// Участники проекта
    /// </summary>
    public List<EntryAltModel>? Members { get; set; }

    /// <summary>
    /// Scheme: Last updated DateTime
    /// </summary>
    public DateTime SchemeLastUpdated { get; set; }


    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is ProjectViewModel other)
            return Id == other.Id && Name == other.Name && Description == other.Description && IsDisabled == other.IsDisabled;

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return $"{Id} {Name} {Description} {IsDisabled}".GetHashCode();
    }

    /// <inheritdoc/>
    public void Reload(ProjectViewModel other)
    {
        Id = other.Id;
        Name = other.Name;
        Description = other.Description;
        IsDisabled = other.IsDisabled;
        SchemeLastUpdated = other.SchemeLastUpdated;

        if (other.Members is null)
            Members = null;
        else
        {
            Members ??= [];
            int findMember_for_remove() => Members.FindIndex(x => !other.Members.Any(y => x.Id == y.Id));
            int i = findMember_for_remove();
            while (i != -1)
            {
                Members.RemoveAt(i);
                i = findMember_for_remove();
            }

            EntryAltModel? member_obj;
            foreach (EntryAltModel member_item in Members)
            {
                member_obj = other.Members.FirstOrDefault(x => x.Id == member_item.Id);
                if (member_obj is not null)
                    member_item.Update(member_obj);
            }
            EntryAltModel[] members_for_add = other.Members.Where(x => !Members.Any(y => y.Id == x.Id)).ToArray();
            if (members_for_add.Length != 0)
                Members.AddRange(members_for_add);
        }
    }
}