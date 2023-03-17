using System.ComponentModel;

namespace VacationAPI.Models
{
    public enum Status
    {
        [Description("Pending")]
        Pending = 0,

        [Description("Approved")]
        Approved = 1,

        [Description("Rejected")]
        Rejected = 2
    }
}
