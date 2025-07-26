using System;

namespace bgbahasajerman_DataAccessLibrary.Models
{
    /// <summary>
    /// Represents a user in the system.
    /// This is a simple Plain Old C# Object (POCO) that maps directly to a database table.
    /// It contains properties that represent the columns in the 'Users' table.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The unique identifier for the user.
        /// This typically corresponds to the primary key in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The user's username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The date and time the user was registered.
        /// </summary>
        public DateTime DateRegistered { get; set; }
    }
}
