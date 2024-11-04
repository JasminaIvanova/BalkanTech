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
        }
    }
}
