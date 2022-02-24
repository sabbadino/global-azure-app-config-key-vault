using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Linq;
using System.Threading.Tasks;


namespace key_vault_core
{

    public class ClaimsFilterSettings
    {
        public string[] RequiredClaims { get; set; }
    }

    [FilterAlias("Claims")] // How we will refer to the filter in configuration
    public class ClaimsFeatureFilter : IFeatureFilter
    {
        // Used to access HttpContext
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClaimsFeatureFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Evaluate(FeatureFilterEvaluationContext context)
        {
            // Get the ClaimsFilterSettings from configuration
            var settings = context.Parameters.Get<ClaimsFilterSettings>();

            // Retrieve the current user (ClaimsPrincipal)
            var user = _httpContextAccessor.HttpContext.User;

            // Only enable the feature if the user has ALL the required claims
            var isEnabled = settings.RequiredClaims.All(claimType => user.HasClaim(claim => claim.Type == claimType));

            return isEnabled;
        }

        public Task<bool> EvaluateAsync(Microsoft.FeatureManagement.FeatureFilterEvaluationContext context)
        {
            return Task.FromResult(Evaluate(context));
        }
    }
}
