namespace PMS_MVC.Models
{
    public class NotificationMessages
    {
        //common notification string
        public string savedSuccessToaster = "{1} has been saved Successfully!";
        public string createSuccessToaster = "has been created Successfully!";
        public string editSuccessToaster = "{1} has been edited Successfully!";
        public string deleteSuccessToaster = "{1} has been deleted Successfully!";
        public string unabledeleteToaster = "Selected Category have already products so you can't delete this category!";
        //System error common notification string
        public string systemErrorToaster = "Something went wrong!";
        //Category Controller common notification string
        public string categoryWarningToaster = "Category name or code is already Exist!";
        //product controller common notification string
        public string productWarningToaster = "Product is already exist in selected category!";
        public string productErrorToaster = "Error occurred while deleting product!";
        //Login controller common notification string
        //Create account common notification string
        public string accountCreatedSuccessToaster = "Account has been created successfully!";
        public string accountCreatedErrorToaster = "Email is already exist!";
        //Login user common notification string
        public string loginSuccessToaster = "User logged in successfully!";
        public string loginErrorToaster = "Email or password is incorrect!";
        public string loginWarningToaster = "Account doesn't exist with entered Email. Kindly create an account.";
    }

    public enum NotificationTypeEnum
    {
        success,
        warning,
        info,
        error
    }
}
