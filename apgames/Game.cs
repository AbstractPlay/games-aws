using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace apgames
{
    public class Game
    {
        public string version;
        public string description;
        public int[] playercounts;
        public string changelog;
        public string state;

        public string genState()
        {
            SHA256 mysha = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(this.version + this.description + this.changelog);
            byte[] hash = mysha.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hash);
        }
    }
}
