using Blazored.LocalStorage;

namespace SIGAD.BlazorApp.Abstractions
{
    /// <summary>
    /// Implementación del proveedor de tokens usando LocalStorage.
    /// Principio DIP: Implementa la abstracción ITokenProvider.
    /// Principio SRP: Solo se encarga de la persistencia del token, no de su generación o validación.
    /// </summary>
    public class LocalStorageTokenProvider : ITokenProvider
    {
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";

        public LocalStorageTokenProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _localStorage.GetItemAsStringAsync(TokenKey);
            }
            catch
            {
                return null;
            }
        }

        public async Task SetTokenAsync(string token)
        {
            await _localStorage.SetItemAsStringAsync(TokenKey, token);
        }

        public async Task RemoveTokenAsync()
        {
            await _localStorage.RemoveItemAsync(TokenKey);
        }

        public async Task<bool> HasTokenAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrWhiteSpace(token);
        }
    }
}
