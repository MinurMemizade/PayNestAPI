﻿namespace PayNestAPI.Models.DTOs
{
    public class RegisterDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
