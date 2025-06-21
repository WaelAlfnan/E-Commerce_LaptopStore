using Store.DAL.Data.Entities;

namespace Store.BLL.DTOs.ProfileDTO
{
    public class UserInfoDTO
    {
        #region Properties
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserGender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public int Age { get; set; }
        #endregion

        #region Methods
        public static UserInfoDTO FromUser(User user)
        {
            return new UserInfoDTO
            {
                Gender = user.Gender,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Age = user.Age,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }
        #endregion
    }
}
