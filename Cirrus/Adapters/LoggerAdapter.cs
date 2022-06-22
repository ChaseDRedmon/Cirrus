namespace Cirrus.Adapters;

using Microsoft.Extensions.Logging;

public interface ICirrusLoggerAdapter<T>
{
    void Trace(string message);
    void Trace<T0>(string message, T0 arg0);
    void Trace<T0, T1>(string message, T0 arg0, T1 arg1);
    void Trace<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

    void Debug(string message);
    void Debug<T0>(string message, T0 arg0);
    void Debug<T0, T1>(string message, T0 arg0, T1 arg1);
    void Debug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
    void Debug<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);

    void Information(string message);
    void Information<T0>(string message, T0 arg0);
    void Information<T0, T1>(string message, T0 arg0, T1 arg1);
    void Information<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

    void Warning(string message);
    void Warning<T0>(string message, T0 arg0);
    void Warning<T0, T1>(string message, T0 arg0, T1 arg1);
    void Warning<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);

    void Error(string message);
    void Error<T0>(string message, T0 arg0);
    void Error<T0, T1>(string message, T0 arg0, T1 arg1);
    void Error<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
}

internal class CirrusLoggerAdapter<T> : ICirrusLoggerAdapter<T>
{
    private readonly ILogger<T> _logger;

    public CirrusLoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void Trace(string message)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(message);
        }
    }

    public void Trace<T0>(string message, T0 arg0)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(message, arg0);
        }
    }

    public void Trace<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(message, arg0, arg1);
        }
    }

    public void Trace<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(message, arg0, arg1, arg2);
        }
    }

    public void Debug(string message)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(message);
        }
    }

    public void Debug<T0>(string message, T0 arg0)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(message, arg0);
        }
    }

    public void Debug<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(message, arg0, arg1);
        }
    }

    public void Debug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(message, arg0, arg1, arg2);
        }
    }
    
    public void Debug<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(message, arg0, arg1, arg2, arg3);
        }
    }

    public void Information(string message)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message);
        }
    }

    public void Information<T0>(string message, T0 arg0)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message, arg0);
        }
    }

    public void Information<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message, arg0, arg1);
        }
    }

    public void Information<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message, arg0, arg1, arg2);
        }
    }

    public void Warning(string message)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning(message);
        }
    }

    public void Warning<T0>(string message, T0 arg0)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning(message, arg0);
        }
    }

    public void Warning<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning(message, arg0, arg1);
        }
    }

    public void Warning<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning(message, arg0, arg1, arg2);
        }
    }

    public void Error(string message)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(message);
        }
    }

    public void Error<T0>(string message, T0 arg0)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(message, arg0);
        }
    }

    public void Error<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(message, arg0, arg1);
        }
    }

    public void Error<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogLevel.Error))
        {
            _logger.LogError(message, arg0, arg1, arg2);
        }
    }
}