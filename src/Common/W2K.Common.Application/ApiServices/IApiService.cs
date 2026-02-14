namespace DFI.Common.Application.ApiServices;

public interface IApiService
{
    #region Get

    Task<TResponse?> GetAsync<TResponse>(string serviceType, string url, CancellationToken cancel = default);

    Task<TResponse?> GetAsync<TResponse>(string serviceType, string url, Dictionary<string, object?> queryParameters, CancellationToken cancel = default);

    #endregion

    #region Post

    Task PostAsync<TRequest>(string serviceType, string url, TRequest request, CancellationToken cancel = default);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string serviceType, string url, TRequest request, CancellationToken cancel = default);

    Task<TResponse?> PostContentAsync<TResponse>(string serviceType, string url, MultipartFormDataContent content, CancellationToken cancel = default);

    #endregion

    #region Put

    Task PutAsync<TRequest>(string serviceType, string url, TRequest request, CancellationToken cancel = default);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string serviceType, string url, TRequest request, CancellationToken cancel = default);

    #endregion

    #region Delete

    Task<TResponse?> DeleteAsync<TResponse>(string serviceType, string url, CancellationToken cancel = default);

    Task DeleteAsync(string serviceType, string url, CancellationToken cancel = default);

    #endregion

}
