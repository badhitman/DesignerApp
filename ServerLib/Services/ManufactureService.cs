using Microsoft.EntityFrameworkCore;
using SharedLib;
using DbcLib;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ServerLib;

/// <inheritdoc/>
public class ManufactureService(
    IDbContextFactory<MainDbAppContext> mainDbFactory,
    IUsersProfilesService usersProfilesRepo) : IManufactureService
{
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
    public async Task<ResponseBaseModel> Update(ManageManufactureModelDB manufacture)
    {
        (bool IsValid, List<ValidationResult> ValidationResults) = GlobalTools.ValidateObject(manufacture);
        if (!IsValid)
            return ResponseBaseModel.CreateError(ValidationResults);

        if (!string.IsNullOrEmpty(manufacture.ControllersDirectoryPath) && !Regex.IsMatch(manufacture.ControllersDirectoryPath, GlobalStaticConstants.FOLDER_NAME_TEMPLATE))
            return ResponseBaseModel.CreateError($"Не корректное имя папки контроллеров: {CodeGeneratorConfigModel.MessageErrorTemplateNameFolder}");

        string?[] folder_names = [manufacture.ControllersDirectoryPath, manufacture.AccessDataDirectoryPath, manufacture.EnumDirectoryPath, manufacture.DocumentsMastersDbDirectoryPath];
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