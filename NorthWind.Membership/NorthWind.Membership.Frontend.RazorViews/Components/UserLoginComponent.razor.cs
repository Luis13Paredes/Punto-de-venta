namespace NorthWind.Membership.Frontend.RazorViews.Components;

public partial class UserLoginComponent
{
    [Parameter]
    public UserLoginViewModel ViewModel { get; set; } = default!;

    private void SanitizeEmail() =>
        ViewModel.Email =
            InputFilters.FilterEmail(ViewModel.Email);
}
