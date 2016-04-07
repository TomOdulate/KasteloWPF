using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Tao.CredentialStore
{
    [Serializable]
    public class PasswordGenerator
    {    
        /// <summary>
        /// Defines a default length of generated passwords.
        /// </summary>
        [NonSerialized]
        private const short DefaultLength = 8;

        /// <summary>
        /// Generate a string consisting of a randomly generated password characters. 
        /// </summary>
        /// <param name="allowedCharacters">Characters permitted for the generation of the string</param>
        /// <param name="size">The length of the password</param>
        /// <returns>A string consisting of a randomly generated password</returns>
        public static string GeneratePassword(char[] allowedCharacters, int size = DefaultLength )
        {
            var disallowedCharacters = new List<char>();
            disallowedCharacters.AddRange(" ".ToCharArray());

            var rnd = new Random();
            var sb = new StringBuilder();
            for (var i = 1; i <= size; i++)
            {
                char newChar;
                do
                {
                    newChar = allowedCharacters[rnd.Next(0, allowedCharacters.Length)];
                } while (disallowedCharacters.Contains(newChar));               
                
                sb.Append(newChar);
            }
            return sb.ToString();
        }

        // Only accurate for passwords in ASCII.
        public static double CalculatePasswordStrength(string password)
        {
            var cardinality = 0;

            // Password contains lowercase letters.
            if (password.Any(char.IsLower)) cardinality += 26;

            // Password contains uppercase letters.
            if (password.Any(char.IsUpper)) cardinality += 26;

            // Password contains numbers.
            if (password.Any(char.IsDigit)) cardinality += 10;


            // Password contains brackets.
            if (password.IndexOfAny(@"[]{}()<>".ToCharArray()) >= 0)
                cardinality += 18;

            // Password contains Punctuation.
            if (password.IndexOfAny(@",.!?".ToCharArray()) >= 0)
                cardinality += 8;

            // Password contains SpecialChars.
            if (password.IndexOfAny(@"£$%^&*_+#@~\/`|".ToCharArray()) >= 0)
                cardinality += 30;
            
            return cardinality * password.Length;
        }

    }
}
