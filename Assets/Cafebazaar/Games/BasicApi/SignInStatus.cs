namespace CafeBazaar.Games.BasicApi
{
    public enum SignInStatus
    {
        Success,
        UiSignInRequired,
        DeveloperError,
        NetworkError,
        InternalError,
        Canceled,
        AlreadyInProgress,
        Failed,
        NotAuthenticated,
    }
}