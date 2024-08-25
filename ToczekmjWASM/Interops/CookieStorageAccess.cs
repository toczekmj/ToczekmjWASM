using Microsoft.JSInterop;

namespace ToczekmjWASM.Interops;

public class CookieStorageAccess(IJSRuntime runtime) : IAsyncDisposable, ICookieStorageAccess
{
    private Lazy<IJSObjectReference> _accessorJsRef = new();

    private async Task WaitForReference()
    {
        if (_accessorJsRef.IsValueCreated is false)
        {
            _accessorJsRef = new(await runtime.InvokeAsync<IJSObjectReference>("import", "/js/CookieStorageAccess.js"));
        }   
    }

    public async ValueTask DisposeAsync()
    {
        if (_accessorJsRef.IsValueCreated)
        {
            await _accessorJsRef.Value.DisposeAsync();
        }
    }
    
    public async Task<T> GetValueAsync<T>(string key)
    {
        await WaitForReference();
        return await _accessorJsRef.Value.InvokeAsync<T>("get", key);
    }

    public async Task SetValueAsync<T>(string key, T value, int duration = 365)
    {
        await WaitForReference();
        await _accessorJsRef.Value.InvokeVoidAsync("set", key, value, duration);
    }
}

public interface ICookieStorageAccess
{
    Task<T> GetValueAsync<T>(string key);
    Task SetValueAsync<T>(string key, T value, int duration = 365);
}