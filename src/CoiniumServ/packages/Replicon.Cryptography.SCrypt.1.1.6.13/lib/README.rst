Replicon.Cryptography.SCrypt
----------------------------

This library is a wrapper for the scrypt key-deriviation function (http://www.tarsnap.com/scrypt.html) created by
Colin Percival.  The core of the library is a copy of the scrypt KDF routines written in C and distributed by Colin.
We've added a .NET wrapper class and done a bit of work to compile this into a mixed-mode .NET assembly.

Download Binaries
~~~~~~~~~~~~~~~~~

Pre-compiled binaries are available for download:

`scrypt-net35-1.1.6.13.zip <https://dl.dropbox.com/s/53a4a6efl6hnb45/scrypt-net35-1.1.6.13.zip?dl=1>`_
    Binary build of version 1.1.6.13 for .NET 3.5.

`scrypt-net40-1.1.6.13.zip <https://dl.dropbox.com/s/kppdambw3wz6xix/scrypt-net40-1.1.6.13.zip?dl=1>`_
    Binary build of version 1.1.6.13 for .NET 4.0.

Why the weird mixed-mode assembly, and C++/CLI stuff?
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Well, a few reasons:

* I don't want to write the scrypt algorithm myself to port it to another language.  This release directly
  incorporates Colin Percival's scrypt implementation, and can be easily updated when he releases updates.

* It's fast.  At the time of writing, it executes the scrypt KDF is about 1/12th the execution time of a
  pure C# implementation (namely, CryptSharp's).  That's a pretty significant time difference.

* Even with an ideal C# implementation, you couldn't use SIMD (eg. SSE) instructions.

* A mixed-mode assembly is easier than PInvoke when you want to dynamically load either a Win32 or
  x64 native library.

So, all those reasons combine to create this library in this form.  I would eventually like to make the backend
implementation dynamically selected so that a C# implementation could be used in environments where native
is not possible (eg. Mono), but I think that the mixed-mode approach will probably stick around as the fastest
implementation.


API Documentation
~~~~~~~~~~~~~~~~~

API description::

    namespace Replicon.Cryptography.SCrypt
    {
        // Summary:
        //     A .NET wrapper for a native implementation of the scrypt key-derivation function.
        //     In addition to exposing the raw key-derivation function (DerivePassword),
        //     SCrypt also contains helper functions for a common use-case of scrypt as
        //     a password hashing algorithm.
        public static class SCrypt
        {
            // Summary:
            //     Default value for N used by parameterless GenerateSalt, currently 2^14.
            public static readonly ulong Default_N;

            // Summary:
            //     Default value for p used by parameterless GenerateSalt, currently 1.
            public static readonly uint Default_p;

            // Summary:
            //     Default value for r used by parameterless GenerateSalt, currently 8.
            public static readonly uint Default_r;

            // Summary:
            //     Default value for hashLengthBytes used by parameterless GenerateSalt, currently
            //     32 bytes.
            public static readonly uint DefaultHashLengthBytes;

            // Summary:
            //     Default value for saltLengthBytes used by parameterless GenerateSalt, currently
            //     16 bytes.
            public static readonly uint DefaultSaltLengthBytes;

            // Summary:
            //     The 'raw' scrypt key-derivation function.
            //
            // Parameters:
            //   password:
            //     The password bytes to generate the key based upon.
            //
            //   salt:
            //     Random salt bytes to make the derived key unique.
            //
            //   N:
            //     CPU/memory cost parameter. Must be a value 2^N. 2^14 (16384) causes a calculation
            //     time of approximately 50-70ms on 2010 era hardware; each successive value
            //     (eg. 2^15, 2^16, ...) should double the amount of CPU time and memory required.
            //
            //   r:
            //     scrypt 'r' tuning parameter
            //
            //   p:
            //     scrypt 'p' tuning parameter (parallelization parameter); a large value of
            //     p can increase computational cost of scrypt without increasing the memory
            //     usage.
            //
            //   derivedKeyLengthBytes:
            //     The number of bytes of key to derive.
            public static byte[] DeriveKey(byte[] password, byte[] salt, ulong N, uint r, uint p, uint derivedKeyLengthBytes);

            // Summary:
            //     Generate a salt for use with HashPassword, selecting reasonable default values
            //     for scrypt parameters that are appropriate for an interactive login verification
            //     workflow.
            //
            // Remarks:
            //     Uses the default values in DefaultSaltLengthBytes, Default_N, Default_r,
            //     Default_r, and DefaultHashLengthBytes.
            public static string GenerateSalt();

            // Summary:
            //     Generate a random salt for use with HashPassword. In addition to the random
            //     salt, the salt value also contains the tuning parameters to use with the
            //     scrypt algorithm, as well as the size of the password hash to generate.
            //
            // Parameters:
            //   saltLengthBytes:
            //     The number of bytes of random salt to generate. The goal for the salt is
            //     to be unique. 16 bytes gives a 2^128 possible salt options, and roughly an
            //     N in 2^64 chance of a salt collision for N salts, which seems reasonable.
            //     A larger salt requires more storage space, but doesn't affect the scrypt
            //     performance significantly.
            //
            //   N:
            //     CPU/memory cost parameter. Must be a value 2^N. 2^14 (16384) causes a calculation
            //     time of approximately 50-70ms on 2010 era hardware; each successive value
            //     (eg. 2^15, 2^16, ...) should double the amount of CPU time and memory required.
            //
            //   r:
            //     scrypt 'r' tuning parameter
            //
            //   p:
            //     scrypt 'p' tuning parameter (parallelization parameter); a large value of
            //     p can increase computational cost of scrypt without increasing the memory
            //     usage.
            //
            //   hashLengthBytes:
            //     The number of bytes to store the password hash in.
            public static string GenerateSalt(uint saltLengthBytes, ulong N, uint r, uint p, uint hashLengthBytes);

            // Summary:
            //     Generate a password hash using a newly generated salt, with default salt
            //     parameters.
            //
            // Parameters:
            //   password:
            //     A password to hash.
            public static string HashPassword(string password);

            // Summary:
            //     Generate a password hash using a specific password salt.
            //
            // Parameters:
            //   password:
            //     A password to hash.
            //
            //   salt:
            //     Salt to hash the password with. This is often a password hash from a previous
            //     HashPassword call, which contains the salt of the original password call;
            //     in that case, the returned hash will be identical to the salt parameter if
            //     the password is the same password as the original.
            public static string HashPassword(string password, string salt);

            // Summary:
            //     Parse the salt component of a salt or password and return the tuning parameters
            //     embedded in the salt.
            //
            // Parameters:
            //   salt:
            //     Salt or hashed password to parse.
            //
            //   saltBytes:
            //     The randomly generated salt data. The length will match saltLengthBytes from
            //     GenerateSalt.
            //
            //   N:
            //     Matching value for GenerateSalt's N parameter.
            //
            //   r:
            //     Matching value for GenerateSalt's r parameter.
            //
            //   p:
            //     Matching value for GenerateSalt's p parameter.
            //
            //   hashLengthBytes:
            //     The number of bytes to store the password hash in.
            //
            // Exceptions:
            //   Replicon.Cryptography.SCrypt.SaltParseException:
            //     Throws SaltParseException if an error occurs while parsing the salt.
            public static void ParseSalt(string salt, out byte[] saltBytes, out ulong N, out uint r, out uint p, out uint hashLengthBytes);

            // Summary:
            //     Attempt to parse the salt component of a salt or password and return the
            //     tuning parameters embedded in the salt.
            //
            // Parameters:
            //   salt:
            //     Salt or hashed password to parse.
            //
            //   saltBytes:
            //     The randomly generated salt data. The length will match saltLengthBytes from
            //     GenerateSalt.
            //
            //   N:
            //     Matching value for GenerateSalt's N parameter.
            //
            //   r:
            //     Matching value for GenerateSalt's r parameter.
            //
            //   p:
            //     Matching value for GenerateSalt's p parameter.
            //
            //   hashLengthBytes:
            //     The number of bytes to store the password hash in.
            //
            // Returns:
            //     True if the parsing was successful, false otherwise.
            public static bool TryParseSalt(string salt, out byte[] saltBytes, out ulong N, out uint r, out uint p, out uint hashLengthBytes);

            // Summary:
            //     Verify that a given password matches a given hash.
            public static bool Verify(string password, string hash);
        }
    }
