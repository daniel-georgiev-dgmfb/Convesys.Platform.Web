using Convesys.Kernel.Caching;
using Convesys.Kernel.Web.Authorisation;
using System;

namespace Convesys.Platform.Web.Tokens
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
