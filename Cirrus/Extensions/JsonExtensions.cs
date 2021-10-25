using System;
using System.Buffers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cirrus.Extensions
{
    // From: https://github.com/dotnet/runtime/issues/31274#issuecomment-804360901
    public static class JsonExtensions
    {
        public static async Task<T?> ToObjectAsync<T>(this JsonElement element, JsonSerializerOptions? options = null,
            CancellationToken token = default)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            await using var writer = new Utf8JsonWriter(bufferWriter);
            element.WriteTo(writer);
        
            // NOTE: call Flush on the writer before Deserializing since Dispose is called at the end of the scope of the method
            await writer.FlushAsync(token);
        
            return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
        }

        public static async Task<T?> ToObjectAsync<T>(this JsonDocument document, JsonSerializerOptions? options = null,
            CancellationToken token = default)
        {
            if (document == null) throw new ArgumentNullException(nameof(document), "JsonDocument cannot be null");
            return await document.RootElement.ToObjectAsync<T>(options, token: token);
        }
    }
}