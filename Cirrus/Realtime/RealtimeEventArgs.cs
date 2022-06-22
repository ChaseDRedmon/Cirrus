namespace Cirrus.Realtime;

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Cirrus.Models;

#if NET5_0_OR_GREATER
public record ApiKeyWrapper([property: JsonPropertyName("apiKeys")] IEnumerable<string> ApiKeys);

public record OnSubscribeEventArgs(Root UserDevice);
public record OnDataReceivedEventArgs(Device Device);

#else
public class ApiKeyWrapper
{
    public ApiKeyWrapper(IEnumerable<string> keys)
    {
        ApiKeys = keys;
    }

    [JsonPropertyName("apiKeys")]
    public IEnumerable<string> ApiKeys { get; }
}

public class OnSubscribeEventArgs
{
    public OnSubscribeEventArgs(Root userDevice)
    {
        UserDevice = userDevice;
    }

    public Root UserDevice { get; }
}

public class OnDataReceivedEventArgs
{
    public OnDataReceivedEventArgs(Device device)
    {
        Device = device;
    }

    public Device Device { get; }
}
#endif