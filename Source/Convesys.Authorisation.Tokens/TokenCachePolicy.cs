using System;
using Twilight.Kernel.Caching;
using Twilight.Kernel.Web.Authorisation;

namespace Twilight.Platform.Web.Tokens
{
    public class TokenCachePolicy : ICacheEntryOptions
    {
        public TokenCachePolicy(TokenDescriptor tokenDescriptor)
        {
            ((ICacheEntryOptions)this).AbsoluteExpiration = tokenDescriptor.ExpireOn;
            ((ICacheEntryOptions)this).SlidingExpiration = tokenDescriptor.ExpireOn.Subtract(tokenDescriptor.IssuedAt);
        }

        DateTimeOffset ICacheEntryOptions.AbsoluteExpiration { get; set; }
        
        TimeSpan ICacheEntryOptions.SlidingExpiration { get; set; }
    }
}
