using CoffeeAppWebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeAppWebApi.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<User> WithoutPrivateInfos(this IEnumerable<User> users)
        {
            if (users == null) return null;

            return users.Select(x => x.WithoutPrivateInfo());
        }

        public static User WithoutPrivateInfo(this User user)
        {
            if (user == null) return null;

            user.Password = null;
            user.Token = null;
            return user;
        }
    }
}
