using Microsoft.AspNetCore.Mvc;

namespace backenddb_c.Extensions
{
    public static class NotificationExtensions
    {
        public static void ShowError(this Controller controller, string message, string title = "Error")
        {
            controller.TempData["ErrorTitle"] = title;
            controller.TempData["ErrorMessage"] = message;
            controller.TempData["ErrorType"] = "error";
        }

        public static void ShowSuccess(this Controller controller, string message, string title = "Éxito")
        {
            controller.TempData["ErrorTitle"] = title;
            controller.TempData["ErrorMessage"] = message;
            controller.TempData["ErrorType"] = "success";
        }

        public static void ShowWarning(this Controller controller, string message, string title = "Advertencia")
        {
            controller.TempData["ErrorTitle"] = title;
            controller.TempData["ErrorMessage"] = message;
            controller.TempData["ErrorType"] = "warning";
        }
    }
}