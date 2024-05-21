////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using StackExchange.Redis;


namespace SharedLib.MemCash
{
    /// <summary>
    /// Redis утилита
    /// </summary>
    public class RedisUtil : IDisposable
    {
        private static string GetRedisKey(MemCashePrefixModel pref, string id = "") => new MemCasheComplexKeyModel(id, pref).ToString();
        static RedisConfigModel? _config;
        /// <summary>
        /// Адрес сервера Redis
        /// </summary>
        public static string RedisServerAddress => _config?.EndPoint ?? "localhost:6379";
        ConnectionMultiplexer? connectionMultiplexer = null;
        private Lazy<ConnectionMultiplexer> LazyConnection => new(() =>
        {
            ConfigurationOptions co = new()
            {
                SyncTimeout = _config?.SyncTimeout ?? 500000,
                EndPoints = { { RedisServerAddress } },
                AbortOnConnectFail = _config?.AbortOnConnectFail ?? false,
                ConnectTimeout = _config?.ConnectTimeout ?? 10000,
                AllowAdmin = _config?.AllowAdmin ?? true,
                ConnectRetry = _config?.ConnectRetry ?? 5,
                ResolveDns = _config?.ResolveDns ?? true,
                User = _config?.User ?? string.Empty,
                Password = _config?.Password ?? string.Empty,
                KeepAlive = _config?.KeepAlive ?? 5,
                ConfigurationChannel = _config?.ConfigurationChannel ?? string.Empty,
                ClientName = _config?.ClientName ?? string.Empty,
                Ssl = _config?.Ssl ?? true,
                SslHost = _config?.SslHost ?? string.Empty
            };

            if (connectionMultiplexer is null)
                connectionMultiplexer = ConnectionMultiplexer.Connect(co);

            return connectionMultiplexer;
        });

        /// <summary>
        /// Представляет взаимосвязанную группу подключений к серверам Redis. Ссылку на это следует сохранить и использовать повторно.
        /// </summary>
        public ConnectionMultiplexer Connection
        {
            get
            {
                return LazyConnection.Value;
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="set_config"></param>
        public RedisUtil(RedisConfigModel set_config)
        {
            _config = set_config;
        }

        /// <summary>
        /// Утилизировать объект
        /// </summary>
        public void Dispose()
        {
            Connection?.Close();
            Connection?.Dispose();
        }

        /// <summary>
        /// Найти ключи по шаблону имени
        /// </summary>
        /// <param name="pattern">Шаблон имени для поиска ключей</param>
        /// <returns>Ключи, совпавшие шаблону</returns>
        public List<RedisKey> FindKeys(string pattern)
        {
            List<RedisKey> res = new();
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();
            foreach (System.Net.EndPoint? server in rc.GetEndPoints())
            {
                res.AddRange(rc.GetServer(server).Keys(db.Database, pattern));
            }

            return res;
        }

        /// <summary>
        /// Найти ключи по шаблону имени
        /// </summary>
        /// <param name="pref">Префикс/Шаблон имени для поиска ключей</param>
        /// <returns>Ключи, совпавшие шаблону</returns>
        public List<RedisKey> FindKeys(MemCashePrefixModel pref) => FindKeys(pref.ToString());

        /// <summary>
        /// Проверка существования ключа
        /// </summary>
        /// <param name="key">Ключ для проверки</param>
        /// <param name="flags">Флаги, используемые для этой операции.</param>
        /// <returns>true, если ключ существует. false, если ключ не существует.</returns>
        public async Task<bool> KeyExistsAsync(MemCasheComplexKeyModel key, CommandFlags flags = CommandFlags.None)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();

            return await db.KeyExistsAsync(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty), flags);
        }

        /// <summary>
        /// Установить тайм-аут на ключ. По истечении тайм-аута ключ будет автоматически удален. В терминологии Redis ключ с соответствующим тайм-аутом называется изменчивым.
        /// </summary>
        /// <param name="key">Ключ, для которого устанавливается срок действия.</param>
        /// <param name="expiry">Тайм-аут, который нужно установить.</param>
        /// <param name="flags">Флаги, используемые для этой операции.</param>
        /// <returns>true, если тайм-аут был установлен. false, если ключ не существует или не удалось установить время ожидания.</returns>
        public async Task<bool> KeyExpireAsync(MemCasheComplexKeyModel key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();

            return await db.KeyExpireAsync(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty), expiry, flags);
        }

        /// <summary>
        /// Переименовывает ключ в newKey. Он возвращает ошибку, если имена источника и назначения совпадают или если ключ не существует.
        /// </summary>
        /// <param name="key">Ключ для переименования.</param>
        /// <param name="newKey">Новый ключ, в который требуется переименовать</param>
        /// <param name="when">При каких условиях переименовывать (по умолчанию всегда).</param>
        /// <param name="flags">Флаги, используемые для этой операции.</param>
        /// <returns>true, если ключ был переименован, в противном случае — false.</returns>
        public async Task<bool> KeyRenameAsync(MemCasheComplexKeyModel key, MemCasheComplexKeyModel newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();

            return await db.KeyRenameAsync(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty), GetRedisKey(newKey.Pref, newKey.Id?.ToString() ?? string.Empty), when, flags);
        }

        /// <summary>
        /// Возвращает оставшееся время жизни ключа. Эта возможность позволяет клиенту Redis проверять, сколько секунд данный ключ будет оставаться частью набора данных.
        /// </summary>
        /// <param name="key">Ключ для проверки</param>
        /// <param name="flags">Флаги, используемые для этой операции.</param>
        /// <returns>TTL или ноль, если ключ не существует или не имеет тайм-аута.</returns>
        public async Task<TimeSpan?> KeyTimeToLiveAsync(MemCasheComplexKeyModel key, CommandFlags flags = CommandFlags.None)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();

            return await db.KeyTimeToLiveAsync(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty), flags);
        }

        #region get

        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        /// <param name="key">Ключ доступа к мемкеш данным</param>
        /// <returns>Данные, прочитанные из мемкеша по полному имени ключа</returns>
        public async Task<string?> GetStringValueAsync(RedisKey key)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();

            RedisValue res = await db.StringGetAsync(key);
            return res.IsNull ? null : res.ToString();
        }

        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        /// <param name="key">Ключ доступа к мемкеш данным</param>
        /// <returns>Данные, прочитанные из мемкеша по полному имени ключа</returns>
        public string? GetStringValue(RedisKey key)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();

            RedisValue res = db.StringGet(key);

            return res.IsNull ? null : res.ToString();
        }

        ///////////////////////////////////

        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        public async Task<string?> GetStringValueAsync(string key) => await GetStringValueAsync(new RedisKey(key));
        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        public string? GetStringValue(string key) => GetStringValue(new RedisKey(key));
        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        public async Task<string?> GetStringValueAsync(MemCasheComplexKeyModel key) => await GetStringValueAsync(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty));
        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        public string? GetStringValue(MemCasheComplexKeyModel key) => GetStringValue(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty));
        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        public async Task<string?> GetStringValueAsync(MemCashePrefixModel pref, string id = "") => await GetStringValueAsync(new MemCasheComplexKeyModel(id, pref));
        /// <summary>
        /// Прочитать из мемкеша данные в виде строки
        /// </summary>
        public string? GetStringValue(MemCashePrefixModel pref, string id = "") => GetStringValue(new MemCasheComplexKeyModel(id, pref));
        #endregion

        #region set/update
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public async Task<bool> UpdateValueAsync(RedisKey key, string value, TimeSpan? expiry = null)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();
            return await db.StringSetAsync(key, value, expiry);
        }
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public bool UpdateValue(RedisKey key, string value, TimeSpan? expiry = null)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();
            return db.StringSet(key, value, expiry);
        }

        ///////////////////////////////////
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public async Task<bool> UpdateValueAsync(string key, string value, TimeSpan? expiry = null) => await UpdateValueAsync(new RedisKey(key), value, expiry);
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public bool UpdateValue(string key, string value, TimeSpan? expiry = null) => UpdateValue(new RedisKey(key), value, expiry);
        //
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public async Task<bool> UpdateValueAsync(MemCasheComplexKeyModel key, string value, TimeSpan? expiry = null) => await UpdateValueAsync(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty), value, expiry);
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public bool UpdateValue(MemCasheComplexKeyModel key, string value, TimeSpan? expiry = null) => UpdateValue(GetRedisKey(key.Pref, key.Id?.ToString() ?? string.Empty), value, expiry);
        //
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public async Task<bool> UpdateValueAsync(MemCashePrefixModel pref, string id, string value, TimeSpan? expiry = null) => await UpdateValueAsync(GetRedisKey(pref, id), value, expiry);
        /// <summary>
        /// Записать строковое значение в Redis. Если ключ уже содержит значение, оно перезаписывается независимо от его типа.
        /// </summary>
        public bool UpdateValue(MemCashePrefixModel pref, string id, string value, TimeSpan? expiry = null) => UpdateValue(GetRedisKey(pref, id), value, expiry);
        #endregion

        #region remove
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public async Task<bool> RemoveKeyAsync(RedisKey key)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public bool RemoveKey(RedisKey key)
        {
            ConnectionMultiplexer rc = Connection;
            IDatabase db = rc.GetDatabase();
            return db.KeyDelete(key);
        }

        ///////////////////////////////////
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public async Task<bool> RemoveKeyAsync(string key) => await RemoveKeyAsync(new RedisKey(key));
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public bool RemoveKey(string key) => RemoveKey(new RedisKey(key));
        //
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public async Task<bool> RemoveKeyAsync(MemCasheComplexKeyModel key) => await RemoveKeyAsync(GetRedisKey(key.Pref, key.Id));
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public bool RemoveKey(MemCasheComplexKeyModel key) => RemoveKey(GetRedisKey(key.Pref, key.Id));
        //
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public async Task<bool> RemoveKeyAsync(MemCashePrefixModel pref, string id) => await RemoveKeyAsync(GetRedisKey(pref, id));
        /// <summary>
        /// Удаляет указанный ключ.
        /// </summary>
        public bool RemoveKey(MemCashePrefixModel pref, string id) => RemoveKey(GetRedisKey(pref, id));
        #endregion
    }
}
