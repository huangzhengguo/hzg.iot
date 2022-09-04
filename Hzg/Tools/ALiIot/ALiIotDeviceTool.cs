using AlibabaCloud.SDK.Iot20180120;
using AlibabaCloud.SDK.Iot20180120.Models;
using Hzg.Iot.Dto;

namespace Hzg.Tool;

public class ALiIotDeviceTool
{
    /// <summary>
    /// 阿里云物联网配置
    /// </summary>
    private readonly HzgAliIotConfig hzgAliIotConfig;

    public ALiIotDeviceTool(HzgAliIotConfig config)
    {
        this.hzgAliIotConfig = config;
    }

    /// <summary>
    /// 初始化阿里云物联网客户端
    /// </summary>
    /// <returns></returns>
    private Client Common()
    {
        if (hzgAliIotConfig == null)
        {
            throw new ArgumentNullException("阿里云物联网配置错误");
        }

        return new Client(hzgAliIotConfig.Config);
    }

    /// <summary>
    /// 阿里云物联网注册设备
    /// </summary>
    /// <param name="deviceInfo"></param>
    /// <returns></returns>
    public RegisterDeviceResponse CreateDevice(SubscribeDto deviceInfo)
    {
        var client = Common();

        var request = new RegisterDeviceRequest();

        request.ProductKey = deviceInfo.ProductKey;
        request.DeviceName = deviceInfo.DeviceName;
        RegisterDeviceResponse response = null;
        
        try
        {
            response = client.RegisterDevice(request);
        }
        catch(Exception e)
        {
            throw new Exception("CreateDevice 注册设备失败!");
        }
        

        return response;
    }
}