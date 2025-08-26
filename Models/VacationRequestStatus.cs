namespace VacationPlanner.Models;

public enum VacationRequestStatus
{
    Submitted = 0, // eingereicht/offen
    Approved  = 1, // genehmigt
    Rejected  = 2, // abgelehnt
    Cancelled = 3  // vom Mitarbeitenden/Team storniert
}