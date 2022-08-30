using StackExchange.Redis;

namespace Hzg.Tool;

public static class RedisTool
{
    private static ConnectionMultiplexer _redis;
    public static ConnectionMultiplexer Redis
    {
        get
        {
            if (_redis == null)
            {
                _redis = ConnectionMultiplexer.Connect("localhost");
            }

            return _redis;
        }
    }

    /// <summary>
    /// 设置字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public bool SetStringValue(string key, string value)
    {
        ConnectionMultiplexer redis = RedisTool.Redis;
        IDatabase db = redis.GetDatabase();

        var result = db.StringSet(key, value, new TimeSpan(1, 0, 0));

        return result;
    }

    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public string GetStringValue(string key)
    {
        IDatabase db = Redis.GetDatabase();

        return db.StringGet(key);
    }
}