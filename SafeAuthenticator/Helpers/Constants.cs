namespace SafeAuthenticator.Helpers
{
    internal static class Constants
    {
        // StringStrength
        internal const int AccStrengthVeryWeak = 4;
        internal const int AccStrengthWeak = 8;
        internal const int AccStrengthSomeWhatSecure = 10;

        // ContainerName
        internal const string AppContainer = "apps/";
        internal const string PublicNamesContainer = "_publicNames";
        internal const string PublicContainer = "_public";
        internal const string DocumentsContainer = "_documents";
        internal const string DownloadsContainer = "_downloads";
        internal const string MusicContainer = "_music";
        internal const string PicturesContainer = "_pictures";
        internal const string VideosContainer = "_videos";

        // ContainerImage
        internal const string PublicContainerImage = "PublicContainer";
        internal const string PublicNamesContainerImage = "PublicNames";
        internal const string AppContainerImage = "AppContainer";

        // FormattedContainerName
        internal const string AppOwnFormattedContainer = "App's own Container";
        internal const string PublicNamesFormattedContainer = "Public Names";
        internal const string PublicFormattedContainer = "Public";
        internal const string DocumentsFormattedContainer = "Documents";
        internal const string DownloadsFormattedContainer = "Downloads";
        internal const string MusicFormattedContainer = "Music";
        internal const string PicturesFormattedContainer = "Pictures";
        internal const string VideosFormattedContainer = "Videos";

        internal static readonly string AppName = "SAFE Authenticator";
        internal static readonly string IsTutorialComplete = "IsTutorialComplete";

        // Authentication PopupState
        internal static readonly string None = "None";
        internal static readonly string Error = "Error";
        internal static readonly string Loading = "Loading";

        // Dialogs
        internal static readonly string AutoReconnectInfoDialog = "Enable this feature to automatically reconnect to the network." +
            " Your credentials will be securely stored on your device. Logging out will clear the credentials from memory.";

        // URL
        internal static readonly string ClaimTokenUrl = @"https://invite.maidsafe.net/";
        internal static readonly string ForumLinkUrl = @"https://safenetforum.org/t/trust-level-1-basic-user-requirements/15200";
        internal static readonly string PrivacyInfoUrl = @"https://safenetwork.tech/privacy/";
        internal static readonly string FaqUrl = @"https://safenetforum.org/t/safe-authenticator-faq/26683";
    }
}
