////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace SharedLib.MemCash
{
    /// <summary>
    /// Сервис мемкеша Redis
    /// </summary>
    public class RedisMemoryCasheService : IManualMemoryCashe, IDisposable
    {
        /// <summary>
        /// Адрес сервреа Redis
        /// </summary>
        public string RedisServerAddress => _config?.Value?.RedisConfig?.EndPoint ?? "localhost:6379";
        private readonly ILogger<RedisMemoryCasheService> _logger;
        private readonly RedisUtil _redis;
        private readonly IOptions<ServerConfigModel> _config;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="set_config"></param>
        /// <param name="set_logger"></param>
        public RedisMemoryCasheService(IOptions<ServerConfigModel> set_config, ILogger<RedisMemoryCasheService> set_logger)
        {
            _config = set_config;
            _logger = set_logger;
            _redis = new RedisUtil(_config.Value.RedisConfig);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _redis.Dispose();
        }

        /// <inheritdoc/>
        public IEnumerable<RedisKey>? FindKeys(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return null;

            try
            {
                return _redis.FindKeys(pattern);
            }
            catch (Exception ex)
            {
                string msg = $"error '{nameof(FindKeys)}' by string pattern:{pattern}";
                _logger.LogError(ex, msg);
                return null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<RedisKey>? FindKeys(MemCashePrefixModel pref) => FindKeys(pref.ToString());

        /// <inheritdoc/>
        public async Task<bool> KeyExistsAsync(MemCasheComplexKeyModel mem_key)
        {
            return await _redis.KeyExistsAsync(mem_key);
        }

        /// <inheritdoc/>
        public async Task<bool> KeyExpireAsync(MemCasheComplexKeyModel mem_key, TimeSpan? expiry)
        {
            return await _redis.KeyExpireAsync(mem_key, expiry);
        }

        /// <inheritdoc/>
        public async Task<bool> KeyRenameAsync(MemCasheComplexKeyModel mem_key, MemCasheComplexKeyModel new_mem_key)
        {
            return await _redis.KeyRenameAsync(mem_key, new_mem_key);
        }

        /// <inheritdoc/>
        public async Task<TimeSpan?> KeyTimeToLiveAsync(MemCasheComplexKeyModel mem_key)
        {
            return await _redis.KeyTimeToLiveAsync(mem_key);
        }

        #region get
        /// <inheritdoc/>
        public async Task<string?> GetStringValueAsync(MemCasheComplexKeyModel mem_key)
        {
            return await _redis.GetStringValueAsync(mem_key);
        }

        /// <inheritdoc/>
        public string? GetStringValue(MemCasheComplexKeyModel mem_key)
        {
            return _redis.GetStringValue(mem_key);
        }

        /// <inheritdoc/>
        public async Task<string?> GetStringValueAsync(MemCashePrefixModel pref, string id = "")
        {
            return await _redis.GetStringValueAsync(pref, id);
        }

        /// <inheritdoc/>
        public string? GetStringValue(MemCashePrefixModel pref, string id = "")
        {
            return _redis.GetStringValue(pref, id);
        }

        /// <inheritdoc/>
        public async Task<string?> GetStringValueAsync(string mem_key)
        {
            return await _redis.GetStringValueAsync(mem_key);
        }

        /// <inheritdoc/>
        public string? GetStringValue(string mem_key)
        {
            return _redis.GetStringValue(mem_key);
        }

        #endregion

        #region set/update
        /// <inheritdoc/>
        public async Task<bool> UpdateValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            return await _redis.UpdateValueAsync(key, value, expiry);
        }

        /// <inheritdoc/>
        public bool UpdateValue(string key, string value, TimeSpan? expiry = null)
        {
            return _redis.UpdateValue(key, value, expiry);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateValueAsync(MemCasheComplexKeyModel key, string value, TimeSpan? expiry = null)
        {
            return await _redis.UpdateValueAsync(key, value, expiry);
        }

        /// <inheritdoc/>
        public bool UpdateValue(MemCasheComplexKeyModel key, string value, TimeSpan? expiry = null)
        {
            return _redis.UpdateValue(key, value, expiry);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateValueAsync(MemCashePrefixModel pref, string id, string value, TimeSpan? expiry = null)
        {
            return await _redis.UpdateValueAsync(pref, id, value, expiry);
        }

        /// <inheritdoc/>
        public bool UpdateValue(MemCashePrefixModel pref, string id, string value, TimeSpan? expiry = null)
        {
            return _redis.UpdateValue(pref, id, value, expiry);
        }
        #endregion

        #region remove
        /// <inheritdoc/>
        public async Task<bool> RemoveKeyAsync(string key)
        {
            return await _redis.RemoveKeyAsync(key);
        }

        /// <inheritdoc/>
        public bool RemoveKey(string key)
        {
            return _redis.RemoveKey(key);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveKeyAsync(MemCasheComplexKeyModel key)
        {
            return await _redis.RemoveKeyAsync(key);
        }

        /// <inheritdoc/>
        public bool RemoveKey(MemCasheComplexKeyModel key)
        {
            return _redis.RemoveKey(key);
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveKeyAsync(MemCashePrefixModel pref, string id)
        {
            return await _redis.RemoveKeyAsync(pref, id);
        }

        /// <inheritdoc/>
        public bool RemoveKey(MemCashePrefixModel pref, string id)
        {
            return _redis.RemoveKey(pref, id);
        }
        #endregion
    }
}
