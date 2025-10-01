// En: SIGAD.BlazorApp/ApiAuthenticationStateProvider.cs
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SIGAD.BlazorApp.Abstractions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SIGAD.BlazorApp
{
    /// <summary>
    /// Provider de estado de autenticación refactorizado para usar ITokenProvider (Fase 2 SOLID).
    /// Principio DIP: Depende de la abstracción ITokenProvider en lugar de ILocalStorageService concreto.
    /// </summary>
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;

        public ApiAuthenticationStateProvider(HttpClient httpClient, ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Console.WriteLine("GetAuthenticationStateAsync: Verificando estado de autenticación...");
            var savedToken = await _tokenProvider.GetTokenAsync();

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                Console.WriteLine("GetAuthenticationStateAsync: No se encontró token. Devolviendo usuario anónimo.");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Aquí podría haber un problema si el token guardado tiene comillas extra. Las quitamos por seguridad.
            savedToken = savedToken.Trim('"');

            Console.WriteLine($"GetAuthenticationStateAsync: Token encontrado. Procediendo a parsear.");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt");
            var user = new ClaimsPrincipal(identity);

            // Esta es la prueba de fuego. ¿Está el usuario autenticado después de crear la identidad?
            Console.WriteLine($"GetAuthenticationStateAsync: Usuario creado. ¿Está autenticado? {user.Identity?.IsAuthenticated}");

            return new AuthenticationState(user);
        }

        public void MarkUserAsAuthenticated(string token)
        {
            Console.WriteLine("MarkUserAsAuthenticated: Marcando usuario como autenticado y notificando...");
            var tokenToParse = token.Trim('"');
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(tokenToParse), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
            Console.WriteLine("MarkUserAsAuthenticated: ¡Notificación enviada!");
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            Console.WriteLine("ParseClaimsFromJwt: Iniciando parseo de claims...");
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                // Manejo especial para el rol, que a veces viene como un array
                keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);
                if (roles != null)
                {
                    if (roles.ToString()?.Trim().StartsWith("[") ?? false)
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString()!);
                        foreach (var parsedRole in parsedRoles ?? Enumerable.Empty<string>())
                        {
                            claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString() ?? string.Empty));
                    }
                }

                // Añadir el resto de los claims
                claims.AddRange(keyValuePairs
                    .Where(kvp => kvp.Key != ClaimTypes.Role) // Evitar añadir el rol dos veces
                    .Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
            }

            Console.WriteLine($"ParseClaimsFromJwt: Parseo finalizado. {claims.Count} claims encontrados.");
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}