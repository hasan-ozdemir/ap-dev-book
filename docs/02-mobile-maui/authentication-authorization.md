---
title: Authentication & Authorization in .NET MAUI
description: Implement secure sign-in, token management, and access control for MAUI apps.
last_reviewed: 2025-10-30
owners:
  - @prodyum/security-guild
---

# Authentication & Authorization in .NET MAUI

Modern MAUI apps often connect to cloud services that require secure authentication and role-based authorization. This guide covers recommended approaches—from Microsoft Entra ID (Azure AD) to custom OAuth2/OIDC flows—and shows how to manage tokens, secure storage, and API access with .NET 9.

---

## 1. Strategy overview

| Scenario | Recommended approach | Notes |
| --- | --- | --- |
| Enterprise / Microsoft 365 access | MSAL.NET with Entra ID (Azure AD) | Supports single sign-on, Conditional Access, broker integration.[^msal-overview] |
| Consumer apps | OAuth2/OIDC via third-party (Auth0, Okta, IdentityServer) | Use WebAuthenticator or custom browser flow.[^webauth] |
| Offline or custom backend | JWT issued by own API, stored securely | Combine with refresh token rotation and secure storage.[^secure-storage] |
| Multi-tenant apps | MSAL B2C or Auth0 multi-tenant flows | Configure policies/user journeys.[^msal-overview] |

---

## 2. MSAL.NET for MAUI

### 2.1 Setup

1. Register app in Azure portal (mobile/native). Set redirect URI `msal{ClientId}://auth`.
2. Add MSAL package:

```bash
dotnet add package Microsoft.Identity.Client --version 4.64.0
```

3. Create `IPublicClientApplication`:

```csharp
public static class AuthenticationService
{
    private static readonly string[] Scopes = ["User.Read"];

    private static readonly IPublicClientApplication Pca =
        PublicClientApplicationBuilder.Create(Constants.ClientId)
            .WithRedirectUri($"msal{Constants.ClientId}://auth")
            .WithIosKeychainSecurityGroup("com.contoso.shared")
            .Build();

    public static async Task<AuthenticationResult> SignInAsync()
    {
        var accounts = await Pca.GetAccountsAsync();
        try
        {
            return await Pca.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            return await Pca.AcquireTokenInteractive(Scopes)
                            .WithParentActivityOrWindow(App.ParentWindow)
                            .ExecuteAsync();
        }
    }
}
```

4. Handle platform integration:
   - Android: implement `IMsalAndroidActivity`, update manifest with `BrowserTabActivity`.
   - iOS/Mac Catalyst: register URL scheme, handle `OpenUrl`.
   - Windows: configure `WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")`.

### 2.2 Token cache

- Use secure storage to persist tokens:

```csharp
await SecureStorage.Default.SetAsync("refresh_token", result.RefreshToken);
```

- MSAL provides cross-platform cache helpers; on iOS use Keychain; on Android use broker or encrypted shared preferences.[^msal-cache]
- Store refresh tokens in `SecureStorage` and clear them if access is revoked or the device lock is removed.[^secure-storage]

### 2.3 Conditional Access & Broker

- For managed devices, enable broker apps (Authenticator, Company Portal). Configure `WithBroker()` and add broker redirect URIs.[^msal-broker]
- Test on Android 15/iOS 18 to ensure system browsers bring users back (watch for preview OS issues).[^android15]

---

## 3. OAuth2 / OIDC with WebAuthenticator

```csharp
var authResult = await WebAuthenticator.Default.AuthenticateAsync(
    new Uri("https://auth.contoso.com/authorize?client_id=..."),
    new Uri("myapp://callback"));

var code = authResult?.Properties?["code"];
```

- Exchange authorization code for tokens using `HttpClient`.
- Store tokens securely; implement refresh token rotation.[^secure-storage]
- For PKCE flows, generate code verifier/challenge.

Handle edge cases:

- Android: ensure custom URI scheme not intercepted by other apps.
- iOS: add scheme to `Info.plist` `CFBundleURLTypes`.
- Windows: WebAuthenticator is not supported; fall back to a browser-based login flow or a WinUI WebView implementation.[^webauth]
- If the system browser cannot return the authorization response, provide an embedded WebAuthenticator fallback with clear user messaging.[^webauth-fallback]

---

## 4. API access & authorization

### 4.1 Inject tokens into HTTP clients

```csharp
builder.Services.AddHttpClient<IWeatherApi, WeatherApi>(client =>
{
    client.BaseAddress = new Uri(Constants.ApiBaseUrl);
}).AddHttpMessageHandler<TokenHandler>();
```

```csharp
public class TokenHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await SecureStorage.Default.GetAsync("access_token");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
```

### 4.2 Role-based UI

- Store user claims in a service; expose `IsInRole`.
- Use `IValueConverter` or `DataTrigger` to toggle UI elements based on permissions.
- For offline mode, cache user profile claims locally and refresh on next sign-in.

---

## 5. Security best practices

- Use HTTPS everywhere; configure HSTS on backend.
- Rotate tokens frequently; for MSAL use incremental consent and conditional access.
- Never store plain tokens; use `SecureStorage`, Keychain, or Android EncryptedSharedPreferences.[^secure-storage]
- Consider device attestation (Play Integrity, DeviceCheck) for high-risk scenarios.
- Log authentication events (success/failure) using privacy-compliant analytics.

---

## 6. Testing authentication flows

- Mock MSAL using `IPublicClientApplication` interface or `IdentityModel.OidcClient` test harness.
- Automate WebAuthenticator flows with Playwright’s device simulation or Appium’s deep link launching.
- Use pipeline secrets for integration tests; avoid storing real client secrets in source.

---

## 7. Troubleshooting

- **AADSTS errors**: Check app registration redirect URIs, scopes, tenant settings.
- **Android browser doesn't return**: Confirm `TaskAffinity` and manifest entries; update to the latest MSAL (4.64+) for Android 15 compatibility fixes.[^android15]
- **Token cache persistence**: Ensure SecureStorage accessible (Android may clear if device lock disabled).
- **Expired certificates**: Renew Apple developer certificates annually and update provisioning profiles.

---

## Checklist

- [ ] App registration configured with correct redirect URIs.
- [ ] Token acquisition (interactive & silent) implemented.
- [ ] Secure token storage and refresh flow in place.
- [ ] API clients inject tokens automatically.
- [ ] Role-based UI/feature flags applied.
- [ ] Authentication tests (unit & UI) automated.

---

## Further reading

- [MSAL.NET guidance for MAUI apps](https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/desktop-mobile/mobile-applications).
- [Community threads on Android 15 browser return behavior and MSAL broker testing](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/releases/tag/4.61.0).[^android15]


[^msal-overview]: Microsoft Learn, "Acquire tokens in mobile applications using MSAL.NET," updated June 4, 2025. citeturn16search0
[^msal-cache]: Microsoft Learn, "Customize token cache serialization in MSAL.NET," updated August 12, 2024. citeturn17search0
[^msal-broker]: Microsoft Learn, "Use a broker with MSAL.NET," updated July 11, 2024. citeturn18search0
[^android15]: InfoQ, ".NET 9 MAUI Preview 5: New Blazor Project Template, Android 15 Beta 2 Support," July 6, 2024. citeturn6search0
[^webauth]: Microsoft Learn, "Web authenticator - .NET MAUI," accessed November 1, 2025. citeturn19search0
[^webauth-fallback]: Microsoft Learn TV, "Authenticating with WebAuthenticator in .NET MAUI," May 12, 2024. citeturn15search0
[^secure-storage]: Microsoft Learn, "Secure storage - .NET MAUI," accessed November 1, 2025. citeturn20search0


