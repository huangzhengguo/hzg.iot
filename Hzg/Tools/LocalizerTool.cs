using Hzg.Const;

namespace Hzg.Tool;

/// <summary>
/// 语言工具类
/// </summary>
public static class LocalizerTool
{
    private static string currentLanguage = "en";
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Value(string key, string language = "en")
    {
        return ErrorMessage.Messages[key];
    }
}