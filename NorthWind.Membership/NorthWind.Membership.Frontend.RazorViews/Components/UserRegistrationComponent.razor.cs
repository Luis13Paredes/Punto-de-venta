namespace NorthWind.Membership.Frontend.RazorViews.Components;

public partial class UserRegistrationComponent
{
    [Parameter]
    public UserRegistrationViewModel ViewModel { get; set; } = default!;

    private void SanitizeFirstName() =>
        ViewModel.FirstName =
            InputFilters.FilterName(ViewModel.FirstName);

    private void SanitizeLastName() =>
        ViewModel.LastName =
            InputFilters.FilterName(ViewModel.LastName);

    private void SanitizeEmail() =>
        ViewModel.Email =
            InputFilters.FilterEmail(ViewModel.Email);
}
