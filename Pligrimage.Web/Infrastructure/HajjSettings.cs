namespace Pligrimage.Web.Infrastructure
{
    /// <summary>
    /// Strongly-typed config bound from appsettings.json HajjSettings section.
    /// Injected via IOptions&lt;HajjSettings&gt;.
    /// </summary>
    public class HajjSettings
    {
        /// <summary>
        /// The current active Hajj year. All queries filter on this.
        /// Change in appsettings.json at the start of each Hajj cycle.
        /// </summary>
        public int ActiveHajjYear { get; set; }

        /// <summary>Base URL for the HRMS REST API.</summary>
        public string HrmsBaseUrl { get; set; }

        /// <summary>Relative endpoint path for employee lookup.</summary>
        public string HrmsEmployeeEndpoint { get; set; }

        /// <summary>
        /// Regex pattern for valid military service numbers.
        /// Validated before calling HRMS to avoid unnecessary API calls.
        /// </summary>
        public string ServiceNumberPattern { get; set; }
    }
}
