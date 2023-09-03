namespace SSTM.Helpers.Common
{
    public static class AppMessages
    {
        public static string InValidCredentials
        {
            get { return "Your credentials are not valid. Please insert valid credentials."; }
        }

        public static string AccountInactive
        {
            get { return "Your account is inactive, please contact our system administrator."; }
        }

        public static string NotAdministrator
        {
            get { return "You are not authorized to access the administration modules."; }
        }

        public static string Exception
        {
            get { return "Something went wrong! Please refresh the page and try again or contact our site administrator."; }
        }

        public static string OTPMessage
        {
            get { return "Verify your login using the following OTP for SSTM Login: {0}. It is valid only for 60 seconds from now ({1}). DO NOT disclose it to anyone."; }
        }

        public static string ExpiredOTPMessage
        {
            get { return "Your OTP (One Time Password) is expired. You can get new OTP via Resend button."; }
        }

        public static string InvalidOTPMessage
        {
            get { return "OTP is not valid. Please enter valid OTP and try again."; }
        }

        public static string InvalidFileExtention
        {
            get { return "You can only upload files with the extensions like *.doc, *.docx, *.pptx, *.xlsx, *.xls, *.ppt.,*.mp4,*.ogg Please select valid file and try again."; }
        }

        public static string NoFileSelected
        {
            get { return "No file selected."; }
        }

        public static string BlankDocumentName
        {
            get { return "Document name cannot be blank."; }
        }

        public static string NoData
        {
            get { return "No data found for selected record."; }
        }

        public static string NotAuthorized
        {
            get { return "You are not authorized to access this website."; }
        }
    }
}