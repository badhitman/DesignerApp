////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using DbcLib;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLib;

namespace RemoteCallLib;

/// <inheritdoc/>
public class SerializeStorageService(IDbContextFactory<CloudParametersContext> cloudParametersDbFactory, ILogger<SerializeStorageService> loggerRepo) : ISerializeStorage
{
    /// <inheritdoc/>
    public async Task<StorageCloudParameterModelDB[]> Find<T>(RequestStorageCloudParameterModel req)
    {
        CloudParametersContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        string _tn = typeof(T).FullName ?? throw new Exception();
        return await context
            .CloudProperties
            .Where(x => x.TypeName == _tn && x.ApplicationName == req.ApplicationName && x.Name == req.Name)
            .ToArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<T?> Read<T>(StorageCloudParameterModel req)
    {
        CloudParametersContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        string _tn = typeof(T).FullName ?? throw new Exception();

        StorageCloudParameterModelDB? pdb = await context
            .CloudProperties
            .Where(x => x.TypeName == _tn && x.OwnerPrimaryKey == req.OwnerPrimaryKey && x.PrefixPropertyName == req.PrefixPropertyName && x.ApplicationName == req.ApplicationName)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.Name == req.Name);

        if (pdb is null)
            return default;

        try
        {
            return JsonConvert.DeserializeObject<T>(pdb.SerializedDataJson);
        }
        catch (Exception ex)
        {
            loggerRepo.LogError(ex, $"Ошибка де-сериализации [{typeof(T).FullName}] из: {pdb.SerializedDataJson}");
            return default;
        }
    }

    /// <inheritdoc/>
    public async Task Save<T>(T obj, StorageCloudParameterModel set)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        string _tn = typeof(T).FullName ?? throw new Exception();

        CloudParametersContext context = await cloudParametersDbFactory.CreateDbContextAsync();

        StorageCloudParameterModelDB _set = new()
        {
            ApplicationName = set.ApplicationName,
            Name = set.Name,
            TypeName = _tn,
            SerializedDataJson = JsonConvert.SerializeObject(obj),
            OwnerPrimaryKey = set.OwnerPrimaryKey,
            PrefixPropertyName = set.PrefixPropertyName,
        };
        await context.AddAsync(_set);
        bool success;
        Random rnd = new();
        for (int i = 0; i < 5; i++)
        {
            success = false;
            try
            {
                await context.SaveChangesAsync();
                success = true;
            }
            finally
            {
                _set.CreatedAt = DateTime.UtcNow;
                await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(400, 700)));
            }

            if (success)
                break;
        }

        IQueryable<StorageCloudParameterModelDB> qf = context
                 .CloudProperties
                 .Where(x => x.TypeName == _tn && x.ApplicationName == set.ApplicationName && x.Name == set.Name && x.OwnerPrimaryKey == set.OwnerPrimaryKey && x.PrefixPropertyName == set.PrefixPropertyName)
                 .AsQueryable();

        int history_count = await qf.CountAsync();
        if (history_count > 150)
        {
            for (int i = 0; i < 5; i++)
            {
                success = false;
                try
                {
                    await qf
                        .OrderBy(x => x.CreatedAt)
                        .Take(50)
                        .ExecuteDeleteAsync();

                    success = true;
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(400, 700)));
                }

                if (success)
                    break;
            }
        }
    }
}