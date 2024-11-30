using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalkanTech.Common
{
    public static class ErrorMessages
    {
        public static class Rooms
        {
            public const string AddRoomsErrorMessage =
                "Unexpected error occurred! Make sure this room is not already added!";
            public const string NoResultFound =
                "We couldn't find any matches for your search";
            public const string ErrorRoomNumber =
                "Unexpected error occurred! Make sure room with this number exists!";
            
        }
        public static class Admin
        {
            public const string UserDeletedSuccess = "User deleted successfully.";
            public const string ErrorDeletingUser = "Error occurred while trying to delete the user.";

            public const string ChangeRoleSuccess = "User role changed successfully.";
            public const string ErrorChangingRoleUser = "Error occurred while trying to change the role of the user.";

            public const string AdminSuccess = "AdminSuccess";
            public const string AdminError = "AdminError";
        }
        public static class Tasks
        {
            public const string ErrorData = "Invalid data entered";
            public const string SuccessData = "Success";
            public const string SuccessfullyDeletedTask = "Successfully deleted task";
            public const string ErrorDeleteTask = "Error occured while trying to delete the task";
        }
     }
}
