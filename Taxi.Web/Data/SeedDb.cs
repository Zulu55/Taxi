using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enums;
using Taxi.Web.Data.Entities;
using Taxi.Web.Helpers;

namespace Taxi.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly Random _random;

        public SeedDb(DataContext dataContext, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Juan", "Zuluaga", "jzuluaga55@hotmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.Admin, "Zulu.jpg");
            await CheckUsersAsync();
            await CheckTaxisAsync();
            await CheckUserGroups();
        }

        private async Task CheckUsersAsync()
        {
            List<Photo> photos = LoadPhotos();
            int i = 0;
            foreach (Photo photo in photos)
            {
                i++;
                await CheckUserAsync($"100{i}", photo.Firstname, photo.Lastname, $"user{i}@yopmail.com", "350 634 2747", "Calle Luna Calle Sol", UserType.User, photo.Image);
            }
        }

        private List<Photo> LoadPhotos()
        {
            return new List<Photo>
            {
                new Photo { Firstname = "Adala", Lastname = "Samir", Image = "Adala.jpg" },
                new Photo { Firstname = "Amalia", Lastname = "Lopez", Image = "Amalia.jpg" },
                new Photo { Firstname = "Camila", Lastname = "Cardona", Image = "Camila.jpg" },
                new Photo { Firstname = "Carolina", Lastname = "Echavarria", Image = "Carolina.jpg" },
                new Photo { Firstname = "Claudia", Lastname = "Sanchez", Image = "Claudia.jpg" },
                new Photo { Firstname = "Gilberto", Lastname = "Medez", Image = "Gilberto.jpg" },
                new Photo { Firstname = "Jhon", Lastname = "Smith", Image = "Jhon.jpg" },
                new Photo { Firstname = "Ken", Lastname = "Rogers", Image = "Ken.jpg" },
                new Photo { Firstname = "Laura", Lastname = "Zuluaga", Image = "Laura.jpg" },
                new Photo { Firstname = "Luisa", Lastname = "Zapata", Image = "Luisa.jpg" },
                new Photo { Firstname = "Manuel", Lastname = "Rodriguez", Image = "Manuel.jpg" },
                new Photo { Firstname = "Manuela", Lastname = "Ateortua", Image = "Manuela.jpg" },
                new Photo { Firstname = "Mario", Lastname = "Bedoya", Image = "Mario.jpg" },
                new Photo { Firstname = "Monica", Lastname = "Cano", Image = "Monica.jpg" },
                new Photo { Firstname = "Pedro", Lastname = "Correa", Image = "Pedro.jpg" },
                new Photo { Firstname = "Penelope", Lastname = "Arias", Image = "Penelope.jpg" },
                new Photo { Firstname = "Pepe", Lastname = "Lopez", Image = "Pepe.jpg" },
                new Photo { Firstname = "Raul", Lastname = "Matinez", Image = "Raul.jpg" },
                new Photo { Firstname = "Roberto", Lastname = "Rivas", Image = "Roberto.jpg" },
                new Photo { Firstname = "Rosa", Lastname = "Velasquez", Image = "Rosa.jpg" },
                new Photo { Firstname = "Rosario", Lastname = "Sandoval", Image = "Rosario.jpg" },
                new Photo { Firstname = "Sandra", Lastname = "Machado", Image = "Sandra.jpg" },
                new Photo { Firstname = "Sandro", Lastname = "Ruiz", Image = "Sandro.jpg" },
                new Photo { Firstname = "Teresa", Lastname = "Santamaria", Image = "Teresa.jpg" }
            };
        }

        private async Task CheckUserGroups()
        {
            if (!_dataContext.UserGroups.Any())
            {
                foreach (var owner in _dataContext.Users.Where(u => u.UserType == UserType.User))
                {
                    var users = new List<UserEntity>();

                    foreach (var user in _dataContext.Users.Where(u => u.UserType == UserType.User))
                    {
                        if (user != owner)
                        {
                            users.Add(user);
                        }
                    }

                    _dataContext.UserGroups.Add(new UserGroupEntity
                    {
                        User = owner,
                        Users = users.Select(u => new UserGroupDetailEntity {  User = u }).ToList()
                    });
                }

                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task<UserEntity> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType,
            string image)
        {
            UserEntity user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\Users", image);
                string imageId = await _blobHelper.UploadBlobAsync(path, "users");

                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType,
                    PicturePath = imageId
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Driver.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckTaxisAsync()
        {
            if (!_dataContext.Taxis.Any())
            {
                UserEntity driver = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserType == UserType.Admin);
                int i = 0;

                foreach (UserEntity user in _dataContext.Users.Where(u => u.UserType == UserType.User))
                {
                    i++;
                    string plaque = $"ABC{i:000}";
                    AddTaxi(plaque, user, driver);
                }

                await _dataContext.SaveChangesAsync();
            }
        }

        private void AddTaxi(string plaque, UserEntity user, UserEntity driver)
        {
            _dataContext.Taxis.Add(new TaxiEntity
            {
                Plaque = plaque,
                User = driver,
                Trips = new List<TripEntity>
                {
                    new TripEntity
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(30),
                        Qualification = _random.Next(0, 5),
                        Source = "ITM Fraternidad",
                        Target = "ITM Robledo",
                        Remarks = "Muy buen servicio",
                        User = user
                    },
                    new TripEntity
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMinutes(30),
                        Qualification = _random.Next(0, 5),
                        Source = "ITM Robledo",
                        Target = "ITM Fraternidad",
                        Remarks = "Conductor muy amable",
                        User = user
                    }
                }
            });
        }

        private class Photo
        {
            public string Firstname { get; set; }

            public string Lastname { get; set; }

            public string Image { get; set; }
        }
    }
}
