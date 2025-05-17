namespace pis2.Models
{
    public class User
    {
        public string Login { get; private set; }
        public int Citizenship { get; set; }
        public int[] Flags { get; set; }
        public DateTime? Entry { get; set; }

        public User(string login, int citizenship, int[] flags, DateTime? entry)
        {
            Login = login;
            Citizenship = citizenship;
            Flags = flags;
            Entry = entry;
        }
    }
}