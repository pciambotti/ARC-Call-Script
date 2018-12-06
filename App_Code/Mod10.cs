// Copyright (c) 2007 David Pinch (www.thoughtproject.com/Snippets/Mod10/).
// Free for any use including commercial and closed source.
//
// Do not use this code on production systems until you have fully tested it.
// The MOD10 algorithm is commonly used in financial and e-commerce systems.
// You downloaded this code from the Internet.  Be careful, OK?
using System;

public static class Mod10
{

    /// <summary>
    ///     Validates a string of digits with the MOD10 algorithm.
    /// </summary>
    /// <returns>
    ///     True if digits pass the checksum test.
    /// </returns>
    public static bool Validate(string digits)
    {
        // Zero can be passed to minimumLength and maximumLength.
        // It instructs the function to ignore length checking.
        return Validate(digits, 0, 0);
    }
    /// <summary>
    ///     Validates a string of digits with the MOD10 algorithm.
    /// </summary>
    /// <param name="digits">
    ///     The string of digits to check.
    /// </param>
    /// <param name="length">
    ///     The expected length of the string.
    /// </param>
    /// <returns>
    ///     True if the string passes the checksum length and is also
    ///     exactly the specified length.
    /// </returns>
    public static bool Validate(string digits, int length)
    {
        return Validate(digits, length, length);
    }
    /// <summary>
    ///     Validates a string of digits with the MOD10 algorithm.
    /// </summary>
    /// <param name="digits">
    ///     The digits to check.
    /// </param>
    /// <param name="minimumLength">
    ///     The minimum allow length of the string.
    /// </param>
    /// <param name="maximumLength">
    ///     The maximum allowed length of the string.
    /// </param>
    /// <returns>
    ///     True if the string passes the checksum test and is also
    ///     between the minimum and maximum lengths, inclusively.
    /// </returns>
    public static bool Validate(string digits, int minimumLength, int maximumLength)
    {
        // Raise an exception if the minimum length is wrong.
        // No exceptions are raised if the digits parameter
        // is null or otherwise wrong, however, because the parameter
        // is based on user input.

        if (minimumLength < 0)
            throw new ArgumentOutOfRangeException("minimumLength");

        if ((maximumLength < 0) || (maximumLength < minimumLength))
            throw new ArgumentOutOfRangeException("maximumLength");

        // For simplicity in the rest of the code, just
        // convert a null string into an empty string.

        digits = digits == null ? string.Empty : digits;

        if (digits.Length == 0)
            return false;

        if (digits.Length < minimumLength)
            return false;

        if ((maximumLength > 0) && (digits.Length > maximumLength))
            return false;

        int parity = digits.Length % 2;
        int sum = 0;

        for (int index = 0; index < digits.Length; index++)
        {

            // First, make sure this character is a digit.
            // This algorithm requires a string to contain
            // all digits (such as credit card numbers).

            if (!char.IsDigit(digits[index]))
                return false;

            int digit = Convert.ToInt32(char.GetNumericValue(digits[index]));

            if ((index % 2) == parity)
            {

                digit *= 2;

                if (digit > 9)
                {
                    digit -= 9;
                }

            }


            sum += digit;
        }

        return (sum % 10) == 0;

    }

}

