using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;

namespace ServerLib;

/// <inheritdoc/>
public class ManufactureService(
    IDbContextFactory<MainDbAppContext> mainDbFactory,
    IUsersProfilesService usersProfilesRepo) : IManufactureService
{
    /// <inheritdoc/>
    public async Task<List<SystemNameEntryModel>> GetSystemNames(int manufactureId)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        return await context_forms
            .SystemNamesManufactures
            .Where(x => x.ManufactureId == manufactureId)
            .Select(x => new SystemNameEntryModel() { Qualification = x.Qualification, TypeDataName = x.TypeDataName, SystemName = x.SystemName, TypeDataId = x.TypeDataId })
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<ManageManufactureModelDB>> ReadManufactureConfig(int projectId, string? userId = null)
    {
        TResponseModel<ManageManufactureModelDB> res = new();
        TResponseModel<UserInfoModel?> user = await usersProfilesRepo.FindByIdAsync(userId);

        if (!user.Success() || user.Response is null)
        {
            res.AddRangeMessages(user.Messages);
            return res;
        }

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();

        res.Response = await context_forms
            .Manufactures
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.UserId == user.Response.UserId);

        if (res.Response is null)
        {
            var project_db = await context_forms
                        .Projects
                        .Where(x => x.Id == projectId)
                        .Select(x => new { x.Id, x.Name })
                        .FirstAsync();

            res.Response = new ManageManufactureModelDB() { UserId = user.Response.UserId, Namespace = GlobalTools.TranslitToSystemName(project_db.Name), ProjectId = project_db.Id };
            await context_forms.AddAsync(res.Response);
            await context_forms.SaveChangesAsync();
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> SetOrDeleteSystemName(UpdateSystemNameModel request)
    {
        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ManufactureSystemNameModelDB? snMan = await context_forms
            .SystemNamesManufactures
            .FirstOrDefaultAsync(x => x.Qualification == request.Qualification && x.TypeDataName == request.TypeDataName && x.ManufactureId == request.ManufactureId && x.TypeDataId == request.TypeDataId);

        if (string.IsNullOrWhiteSpace(request.SystemName))
        {
            if (snMan == null)
                return ResponseBaseModel.CreateInfo("Значение отсутствует. Удаление не требуется.");
            else
            {
                context_forms.Remove(snMan);
                await context_forms.SaveChangesAsync();
                return ResponseBaseModel.CreateInfo("Значение удалено.");
            }
        }
        else if (!Regex.IsMatch(request.SystemName, GlobalStaticConstants.SYSTEM_NAME_TEMPLATE))
            return ResponseBaseModel.CreateError(GlobalStaticConstants.SYSTEM_NAME_TEMPLATE_MESSAGE);
        else
        {
            if (snMan == null)
            {
                if (await context_forms.SystemNamesManufactures.AnyAsync(x => x.SystemName == request.SystemName && x.TypeDataName == request.TypeDataName && x.ManufactureId == request.ManufactureId && x.TypeDataId == request.TypeDataId))
                    return ResponseBaseModel.CreateError("Имя не уникально. Задайте другое имя");

                await context_forms.AddAsync(ManufactureSystemNameModelDB.Build(request));
                await context_forms.SaveChangesAsync();
                return ResponseBaseModel.CreateInfo("Значение создано.");
            }
            else
            {
                if (snMan.SystemName == request.SystemName)
                    return ResponseBaseModel.CreateInfo("Обновления системного имени не требуется.");

                if (await context_forms.SystemNamesManufactures.AnyAsync(x => x.Id != snMan.Id && x.SystemName == request.SystemName && x.TypeDataName == request.TypeDataName && x.ManufactureId == request.ManufactureId && x.TypeDataId == request.TypeDataId))
                    return ResponseBaseModel.CreateError("Имя не уникально. Задайте другое имя");

                snMan.SystemName = request.SystemName;
                context_forms.Update(snMan);
                await context_forms.SaveChangesAsync();
                return ResponseBaseModel.CreateInfo("Значение обновлено.");
            }
        }
    }

    /// <inheritdoc/>
    public async Task<ResponseBaseModel> UpdateManufactureConfig(ManageManufactureModelDB manufacture)
    {
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(manufacture);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        string?[] folder_names = [manufacture.AccessDataDirectoryPath, manufacture.EnumDirectoryPath, manufacture.DocumentsMastersDbDirectoryPath];
        folder_names = folder_names
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (folder_names.Length != 0)
            return ResponseBaseModel.CreateError($"Имена папок должны быть уникальные. Есть дубликаты: {string.Join(";", folder_names)}");

        using MainDbAppContext context_forms = mainDbFactory.CreateDbContext();
        ManageManufactureModelDB manufacture_db = await context_forms.Manufactures.FirstAsync(x => x.Id == manufacture.Id);
        if (manufacture_db.Equals(manufacture))
            return ResponseBaseModel.CreateInfo("Обновление не требуется. Объекты равны");

        manufacture_db.Reload(manufacture);
        context_forms.Update(manufacture_db);
        await context_forms.SaveChangesAsync();

        return ResponseBaseModel.CreateSuccess("Обновление успешно выполнено");
    }
}