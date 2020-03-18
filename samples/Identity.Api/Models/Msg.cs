namespace Identity.Api
{
    public static class Msg
    {
        public const string UserNotExistByMail =  "user_not_exist_by_email";
        public const string InvalidEmailAddress = "invalid_email_address";
        public const string UserAlreadyExistByEmail = "user_alread_exist_by_email";
        public const string PasswordDoesNotMeetRequirments = "password_doesnot_meet_requirments";
        public const string LastNameIsRequiredFieldForUser = "last_name_is_required";
        public const string NameIsRequiredFieldForUser = "name_is_required";
    }
}