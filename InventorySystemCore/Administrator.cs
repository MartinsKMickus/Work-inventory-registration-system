using System;
using System.ComponentModel.DataAnnotations;

namespace InventorySystemCore
{
    public class Administrator
    {
        [Key]
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public bool Registered { get; set; }
        public Administrator() { }
        public Administrator(string email, string name, string password)
        {
            Tools tools = new Tools();
            Password = tools.CreatePassHash(password);
            Name = name;
            Registered = false;
            if(tools.CheckEmail(email))
            {
                Email = email;
            }
            else
            {
                Exception WrongEmail = new Exception("Email not correct");
                throw WrongEmail;
            }
        }
    }
}
