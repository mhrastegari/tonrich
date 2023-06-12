﻿namespace Tonrich.Client.Shared;

public partial class Footer
{
#if MultilingualEnabled
    protected async override Task OnAfterFirstRenderAsync()
    {
#if BlazorHybrid
        var preferredCultureCookie = Preferences.Get(".AspNetCore.Culture", null);
#else
        var preferredCultureCookie = await JSRuntime.InvokeAsync<string?>("window.App.getCookie", ".AspNetCore.Culture");
#endif
        SelectedCulture = CultureInfoManager.GetCurrentCulture(preferredCultureCookie);

        StateHasChanged();

        await base.OnAfterFirstRenderAsync();
    }
#endif

#pragma warning disable CS0649 // Field 'Footer.SelectedCulture' is never assigned to, and will always have its default value null
    private string? SelectedCulture;
#pragma warning restore CS0649 // Field 'Footer.SelectedCulture' is never assigned to, and will always have its default value null

    private async Task OnCultureChanged()
    {
        var cultureCookie = $"c={SelectedCulture}|uic={SelectedCulture}";

#if BlazorHybrid
        Preferences.Set(".AspNetCore.Culture", cultureCookie);
#else
        await JSRuntime.InvokeVoidAsync("window.App.setCookie", ".AspNetCore.Culture", cultureCookie, 30 * 24 * 3600);
#endif

        NavigationManager.ForceReload();
    }

    private static List<BitDropDownItem> GetCultures() =>
        CultureInfoManager.SupportedCultures.Select(sc => new BitDropDownItem { Value = sc.code, Text = sc.name }).ToList();
}
