using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PersistenceNet.Interfaces;
using PersistenceNet.MessagesProvider.Locations;

namespace PersistenceNet.MessagesProvider
{
    public class MessagesProvider(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider) 
        : IMessagesProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public Messages Current
        {
            get
            {
                var culture = GetRequestCulture();

                return culture switch
                {
                    "en-US" => _serviceProvider.GetRequiredService<MessagesEnUs>(),
                    "pt-BR" => _serviceProvider.GetRequiredService<MessagesPtBr>(),
                    _ => _serviceProvider.GetRequiredService<Messages>()
                };
            }
        }

        private string? GetRequestCulture()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var culture = request?.Headers["Accept-Language"].ToString();

            if (!string.IsNullOrWhiteSpace(culture))
            {
                var parts = culture.Split(',', StringSplitOptions.RemoveEmptyEntries);
                return parts.FirstOrDefault()?.Trim();
            }

            return "en-US";
        }
    }
}