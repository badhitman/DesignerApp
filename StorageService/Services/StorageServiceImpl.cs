////////////////////////////////////////////////
// © https://github.com/badhitman - @FakeGov 
////////////////////////////////////////////////

using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLib;
using DbcLib;

namespace StorageService;

/// <inheritdoc/>
public class StorageServiceImpl(
    IDbContextFactory<StorageContext> cloudParametersDbFactory,
    IMemoryCache cache,
    ILogger<StorageServiceImpl> loggerRepo) : ISerializeStorage
{
#if DEBUG
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(50);
#else
    static readonly TimeSpan _ts = TimeSpan.FromSeconds(5);
#endif

    /// <inheritdoc/>
    public async Task<T?[]> Find<T>(RequestStorageBaseModel req)
    {
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        string _tn = typeof(T).FullName ?? throw new Exception();
        StorageCloudParameterModelDB[] _dbd = await context
            .CloudProperties
            .Where(x => x.TypeName == _tn && x.ApplicationName == req.ApplicationName && x.Name == req.Name)
            .ToArrayAsync();

        return _dbd.Select(x => JsonConvert.DeserializeObject<T>(x.SerializedDataJson)).ToArray();
    }

    /// <inheritdoc/>
    public async Task<T?> Read<T>(StorageMetadataModel req)
    {
        string mem_key = $"{req.Name}/{req.OwnerPrimaryKey}/{req.PrefixPropertyName}/{req.ApplicationName}";
        if (cache.TryGetValue(mem_key, out T? sd))
            return sd;

        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
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
    public async Task Save<T>(T obj, StorageMetadataModel set, bool trimHistory = false)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        StorageCloudParameterModelDB _set = new()
        {
            ApplicationName = set.ApplicationName,
            Name = set.Name,
            TypeName = typeof(T).FullName ?? throw new Exception(),
            SerializedDataJson = JsonConvert.SerializeObject(obj),
            OwnerPrimaryKey = set.OwnerPrimaryKey,
            PrefixPropertyName = set.PrefixPropertyName,
        };
        ResponseBaseModel res = await FlushParameter(_set, trimHistory);
        if (res.Success())
        {
            string mem_key = $"{set.Name}/{set.OwnerPrimaryKey}/{set.PrefixPropertyName}/{set.ApplicationName}";
            cache.Set(mem_key, obj, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));
        }
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<int?>> FlushParameter(StorageCloudParameterModelDB _set, bool trimHistory = false)
    {
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        TResponseModel<int?> res = new();
        _set.Id = 0;
        await context.AddAsync(_set);
        bool success;
        Random rnd = new();
        for (int i = 0; i < 5; i++)
        {
            success = false;
            try
            {
                await context.SaveChangesAsync();
                string mem_key = $"{_set.Name}/{_set.OwnerPrimaryKey}/{_set.PrefixPropertyName}/{_set.ApplicationName}";
                cache.Remove(mem_key);
                success = true;
                res.AddSuccess($"Данные успешно сохранены{(i > 0 ? $" (на попытке [{i}])" : "")}: {_set.ApplicationName}/{_set.Name}");
                res.Response = _set.Id;
            }
            catch (Exception ex)
            {
                res.AddInfo($"Попытка записи [{i}]: {ex.Message}");
                _set.CreatedAt = DateTime.UtcNow;
                await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(100, 300)));
            }

            if (success)
                break;
        }

        IQueryable<StorageCloudParameterModelDB> qf = context
                 .CloudProperties
                 .Where(x => x.TypeName == _set.TypeName && x.ApplicationName == _set.ApplicationName && x.Name == _set.Name && x.OwnerPrimaryKey == _set.OwnerPrimaryKey && x.PrefixPropertyName == _set.PrefixPropertyName)
                 .AsQueryable();

        if (trimHistory)
        {
            await qf
                .Where(x => x.Id != _set.Id)
                .ExecuteDeleteAsync();
        }
        else if (await qf.CountAsync() > 50)
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
                    res.AddSuccess($"Ротация успешно выполнена на попытке [{i}]");
                    success = true;
                }
                catch (Exception ex)
                {
                    res.AddInfo($"Попытка записи [{i}]: {ex.Message}");
                    await Task.Delay(TimeSpan.FromMilliseconds(rnd.Next(100, 300)));
                }

                if (success)
                    break;
            }
        }

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<StorageCloudParameterPayloadModel?>> ReadParameter(StorageMetadataModel req)
    {
        string mem_key = $"{req.Name}/{req.OwnerPrimaryKey}/{req.PrefixPropertyName}/{req.ApplicationName}";
        TResponseModel<StorageCloudParameterPayloadModel?> res = new();
        if (cache.TryGetValue(mem_key, out StorageCloudParameterPayloadModel? sd))
        {
            res.Response = sd;
            return res;
        }

        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        StorageCloudParameterModelDB? parameter_db = await context
            .CloudProperties
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x =>
            x.OwnerPrimaryKey == req.OwnerPrimaryKey &&
            x.Name == req.Name &&
            x.ApplicationName == req.ApplicationName &&
            x.PrefixPropertyName == req.PrefixPropertyName);

        if (parameter_db is not null)
            res.Response = new StorageCloudParameterPayloadModel()
            {
                ApplicationName = parameter_db.ApplicationName,
                Name = parameter_db.Name,
                OwnerPrimaryKey = parameter_db.OwnerPrimaryKey,
                PrefixPropertyName = parameter_db.PrefixPropertyName,
                TypeName = parameter_db.TypeName,
                SerializedDataJson = parameter_db.SerializedDataJson,
            };
        else
            res.AddWarning($"Параметр не найден");

        cache.Set(mem_key, res.Response, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_ts));

        return res;
    }

    /// <inheritdoc/>
    public async Task<TResponseModel<FoundParameterModel[]?>> Find(RequestStorageBaseModel req)
    {
        TResponseModel<FoundParameterModel[]?> res = new();
        using StorageContext context = await cloudParametersDbFactory.CreateDbContextAsync();
        StorageCloudParameterModelDB[] prop_db = await context
            .CloudProperties
            .Where(x => req.Name == x.Name && req.ApplicationName == x.ApplicationName)
            .ToArrayAsync();

        res.Response = prop_db
            .Select(x => new FoundParameterModel()
            {
                SerializedDataJson = JsonConvert.SerializeObject(x),
                CreatedAt = DateTime.UtcNow,
                OwnerPrimaryKey = x.OwnerPrimaryKey,
                PrefixPropertyName = x.PrefixPropertyName,
            })
            .ToArray();

        return res;
    }
}