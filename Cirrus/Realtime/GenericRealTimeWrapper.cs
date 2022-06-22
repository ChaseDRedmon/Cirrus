using Cirrus.Adapters;
using Cirrus.Models;
using Microsoft.Extensions.Options;

namespace Cirrus.Realtime;

using System.Threading;
using System.Threading.Tasks;
using Cirrus.Wrappers;

public class CirrusRealtime<T> : CirrusRealtime
{
    public CirrusRealtime(IOptions<CirrusConfig> options, ICirrusLoggerAdapter<CirrusRealtime>? logger = null)
        : base(options, logger)
    {
    }
    
    
}