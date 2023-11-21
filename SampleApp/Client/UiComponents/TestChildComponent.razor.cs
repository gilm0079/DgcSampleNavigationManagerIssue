using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Data;

namespace SampleApp.Client.UiComponents {
    public partial class TestChildComponent : ComponentBase, IAsyncDisposable {

        private const string JavaScriptBlazorHelper = "./dgcBlazorHelpers.js";
        private Task<IJSObjectReference> _module;
        protected Task<IJSObjectReference> Module => _module ??= JS.InvokeAsync<IJSObjectReference>("import", JavaScriptBlazorHelper).AsTask();

        private void ChangeQueryStringTabValue() {
            Console.WriteLine("Change QS tab value button pressed");
            SetQueryStringType();
        }

        private void SetQueryStringType() {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            Console.WriteLine("Before navigating, NavigationManager thinks the current URL is '" + uri?.ToString() ?? string.Empty + "'");
            Random rnd = new();
            int randomValue = rnd.Next(1, 999);
            string newUrl = NavigationManager.GetUriWithQueryParameter("type", randomValue.ToString());
            Console.WriteLine("Changing browser URL to '" + newUrl + "' via NavigationManager");
            NavigationManager.NavigateTo(newUrl, false, false); //should be equivalant to history.pushState()
        }

        private async Task SetQueryStringTypeViaJs() {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            Console.WriteLine("Before navigating, NavigationManager thinks the current URL is '" + uri?.ToString() ?? string.Empty + "'");
            Random rnd = new();
            int randomValue = rnd.Next(1, 999);
            string newUrl = NavigationManager.GetUriWithQueryParameter("type", randomValue.ToString());
            Console.WriteLine("Changing browser URL to '" + newUrl + "' via Javascript");
            var module = await Module;
            await module.InvokeVoidAsync("silentGoToUrl", null, string.Empty, newUrl);
        }

        [Inject]
        protected IJSRuntime JS { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized() {
            Console.WriteLine("TestChildComponent OnInitialized executing");
            base.OnInitialized();
        }

        public async ValueTask DisposeAsync() {
            Console.WriteLine("TestChildComponent Disposing...");
            try {
                if (_module != null) {
                    var module = await Module;
                    await module.DisposeAsync();
                }
            } catch (Microsoft.JSInterop.JSDisconnectedException dex) {
                Console.WriteLine("Dispose threw Javascript Disconnected Exception. Circuit disposed before JSRuntime disposed. Exception is absorbed and can be ignored.");
            } catch { }
        }
    }
}
