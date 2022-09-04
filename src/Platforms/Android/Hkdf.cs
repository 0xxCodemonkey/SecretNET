using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace SecretNET.Crypto
{
    /// <summary>
    /// Implements HMAC-based Extract-and-Expand Key Derivation Function (HKDF) - Fixed for Android.
    /// </summary>
    /// <remarks>
    /// For more information about HKDF, please refer to the following resources:
    /// <list type="number">
    ///     <item><see href="https://tools.ietf.org/html/rfc5869">RFC 5869</see>.</item>
    ///     <item><see href="https://eprint.iacr.org/2010/264.pdf">Cryptographic Extraction and Key Derivation: The HKDF Scheme - Hugo Krawczyk, 2010</see>.</item>
    ///     <item><see href="https://webee.technion.ac.il/~hugo/kdf/kdf.pdf">On Extract-then-Expand Key Derivation Functions and an HMAC-based KDF - Hugo Krawczyk, 2008</see>.</item>
    /// </list>
    /// </remarks>
    public static class Hkdf
    {
        /// <summary>
        /// The multiplier that is used to determine the maximum length of the output generated in the Expand stage
        /// (see <see href="https://tools.ietf.org/html/rfc5869#section-2.3">RFC 5869 section 2.3</see>).
        /// </summary>
        private const int maxOutputLengthCoef = 255;

        /// <summary>
        /// The maximum size of a temporary buffer allocated on the stack in bytes.
        /// </summary>
        private const int maxStackallocBufferSize = 256;


        /// <summary>
        /// Extracts a pseudorandom key from the provided input key material using an optional salt value.
        /// </summary>
        /// <param name="hashAlgorithmName">The hash algorithm to be used by the HMAC primitive. Supported hash functions: MD5, SHA1, SHA256, SHA384, SHA512.</param>
        /// <param name="ikm">The input key material.</param>
        /// <param name="salt">The optional salt value. If the argument is omitted or its value is set to <c>null</c>, the extraction is performed without salt.</param>
        /// <returns>The pseudorandom key of the size of hash algorithm output, i.e.: MD5 - 16 bytes, SHA1 - 20 bytes, SHA256 - 32 bytes, SHA384 - 48 bytes, SHA512 - 64 bytes.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="ikm"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The hash algorithm specified in the parameter <paramref name="hashAlgorithmName"/> is not supported.</exception>

        public static byte[] Extract(HashAlgorithmName hashAlgorithmName, byte[] ikm, byte[]? salt = null)
        {
            if (!IsHashAlgorithmSupported(hashAlgorithmName))
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), "The specified hash algorithm is not supported.");
            if (ikm == null)
                throw new ArgumentNullException(nameof(ikm), "The input key material cannot be null.");

            return PerformExtraction(hashAlgorithmName, ikm, salt);
        }


        /// <summary>
        /// Extracts a pseudorandom key from the provided input key material using the specified salt value.
        /// </summary>
        /// <param name="hashAlgorithmName">The hash algorithm to be used by the HMAC primitive. Supported hash functions: MD5, SHA1, SHA256, SHA384, SHA512.</param>
        /// <param name="ikm">The input key material.</param>
        /// <param name="salt">The salt value.</param>
        /// <param name="prk">The buffer to receive the generated pseudorandom key. Must be at least the size of the hash output to accommodate the pseudorandom key, i.e.: for MD5 - minimum 16 bytes, SHA1 - 20, SHA256 - 32, SHA384 - 48, SHA512 - 64.</param>
        /// <returns>The size of the extracted pseudorandom key in bytes.</returns>
        /// <exception cref="ArgumentException">The size of the <paramref name="prk"/> is smaller than the size of the output of hash algorithm.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The hash algorithm specified in the parameter <paramref name="hashAlgorithmName"/> is not supported.</exception>
        public static int Extract(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
        {
            if (!TryGetHashOutputLength(hashAlgorithmName, out int hashOutputLength))
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), "The specified hash algorithm is not supported.");
            if (prk.Length < hashOutputLength)
                throw new ArgumentException($"The supplied pseudorandom key buffer is too small. It must be large enough to accommodate the extracted pseudorandom key, that is, at least {hashOutputLength} bytes in case of {hashAlgorithmName}.", nameof(prk));

            return PerformExtraction(hashAlgorithmName, ikm, salt, prk);
        }



        /// <summary>
        /// Expands the provided pseudorandom key into an output keying material of the desired length using optional context information.
        /// </summary>
        /// <param name="hashAlgorithmName">The hash algorithm to be used by the HMAC primitive. Supported hash functions: MD5, SHA1, SHA256, SHA384, SHA512.</param>
        /// <param name="prk">The pseudorandom key. Must be at least as long as the output of the hash algorithm, i.e.: MD5 - 16 bytes, SHA1 - 20, SHA256 - 32, SHA384 - 48, SHA512 - 64.</param>
        /// <param name="outputLength">The desired length of the generated output keying material in bytes. Minimum value - 1. Maximum value - 255 times the size of the hash algorithm output, i.e.: MD5 - 4080, SHA1 - 5100, SHA256 - 8160, SHA384 - 12240, SHA512 - 16320.</param>
        /// <param name="info">The optional context-specific information. If the argument is omitted or its value is set to <c>null</c>, the expansion is performed without context information.</param>
        /// <returns>The output keying material.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="prk"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The hash algorithm specified in the parameter <paramref name="hashAlgorithmName"/> is not supported or the value of the parameter <paramref name="outputLength"/> is invalid (either too small or too large).</exception>
        /// <exception cref="ArgumentException">The length of the <paramref name="prk"/> is less than the length of the output of the hash algorithm.</exception>

        public static byte[] Expand(HashAlgorithmName hashAlgorithmName, byte[] prk, int outputLength, byte[]? info = null)
        {
            if (!TryGetHashOutputLength(hashAlgorithmName, out int hashOutputLength))
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), "The specified hash algorithm is not supported.");
            if (prk == null)
                throw new ArgumentNullException(nameof(prk), "The pseudorandom key cannot be null.");
            if (prk.Length < hashOutputLength)
                throw new ArgumentException($"The supplied pseudorandom key is too short. It must be at least as long as the hash-function output, i.e. {hashOutputLength} bytes in case of {hashAlgorithmName}.", nameof(prk));
            if (outputLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(outputLength), "The specified output length is too small. The minimum size of the output is 1 byte.");
            if (outputLength > hashOutputLength * maxOutputLengthCoef)
                throw new ArgumentOutOfRangeException(nameof(outputLength), $"The specified output length is too large. The maximum size of the output is {maxOutputLengthCoef} times the hash-output-length, i.e. {hashOutputLength * maxOutputLengthCoef} bytes in case of {hashAlgorithmName}.");

            return PerformExpansion(hashAlgorithmName, hashOutputLength, prk, info, outputLength);
        }


        /// <summary>
        /// Expands the provided pseudorandom key into an output keying material of the desired length using optional context information.
        /// </summary>
        /// <param name="hashAlgorithmName">The hash algorithm to be used by the HMAC primitive. Supported hash functions: MD5, SHA1, SHA256, SHA384, SHA512.</param>
        /// <param name="prk">The pseudorandom key. Must be at least as long as the output of the hash algorithm, i.e.: MD5 - 16 bytes, SHA1 - 20, SHA256 - 32, SHA384 - 48, SHA512 - 64.</param>
        /// <param name="output">The buffer to receive the generated output keying material (OKM). The OKM produced is of the same size as the buffer. Minimum buffer size - 1 byte, maximum buffer size - 255 times the size of the hash algorithm output in bytes (i.e., MD5 - 4080, SHA1 - 5100, SHA256 - 8160, SHA384 - 12240, SHA512 - 16320).</param>
        /// <param name="info">The optional context-specific information. If the argument is an empty span, the expansion is performed without context information.</param>
        /// <exception cref="ArgumentException">The size of the <paramref name="prk"/> is smaller than the size of the output of hash algorithm or the size of the <paramref name="output"/> is invalid (either too small or too large).</exception>
        /// <exception cref="ArgumentOutOfRangeException">The hash algorithm specified in the parameter <paramref name="hashAlgorithmName"/> is not supported.</exception>
        public static void Expand(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> prk, Span<byte> output, ReadOnlySpan<byte> info)
        {
            if (!TryGetHashOutputLength(hashAlgorithmName, out int hashOutputLength))
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), "The specified hash algorithm is not supported.");
            if (prk.Length < hashOutputLength)
                throw new ArgumentException($"The supplied pseudorandom key is too short. It must be at least as long as the hash-function output, i.e. {hashOutputLength} bytes in case of {hashAlgorithmName}.", nameof(prk));
            if (output.Length <= 0)
                throw new ArgumentException("The supplied output buffer is too small. The minimum size of the output buffer is 1 byte.", nameof(output));
            if (output.Length > hashOutputLength * maxOutputLengthCoef)
                throw new ArgumentException($"The supplied output buffer is too large. The maximum size of the output buffer is {maxOutputLengthCoef} times the hash-output-length, i.e. {hashOutputLength * maxOutputLengthCoef} bytes in case of {hashAlgorithmName}.", nameof(output));

            // If the output span and the info span overlap, writing to the output will modify the info;
            // the following output blocks (if any) will be computed for a different info, rendering the output invalid.
            // We overcome this by using a temporary buffer the size of the info or the output (whichever is the smallest).

            if (output.Overlaps(info))
            {
                int bufferSize = Math.Min(output.Length, info.Length);
                if (bufferSize <= maxStackallocBufferSize)
                {
                    Span<byte> buffer = stackalloc byte[bufferSize];
                    PerformExpansionWithOverlappingInfoAndOutput(hashAlgorithmName, hashOutputLength, prk, info, output, buffer);
                }
                else
                {
                    Span<byte> buffer = new byte[bufferSize];
                    unsafe
                    {
                        fixed (byte* bufferPointer = buffer)
                        {
                            PerformExpansionWithOverlappingInfoAndOutput(hashAlgorithmName, hashOutputLength, prk, info, output, buffer);
                        }
                    }
                }
            }
            else
            {
                PerformExpansion(hashAlgorithmName, hashOutputLength, prk, info, output);
            }

            static void PerformExpansionWithOverlappingInfoAndOutput(HashAlgorithmName hashAlgorithmName, int hmacOutputLength, ReadOnlySpan<byte> prk, ReadOnlySpan<byte> info, Span<byte> output, Span<byte> buffer)
            {
                try
                {
                    if (output.Length < info.Length)
                    {
                        PerformExpansion(hashAlgorithmName, hmacOutputLength, prk, info, buffer);
                        buffer.CopyTo(output);
                    }
                    else
                    {
                        info.CopyTo(buffer);
                        PerformExpansion(hashAlgorithmName, hmacOutputLength, prk, buffer, output);
                    }
                }
                finally
                {
                    Clear(buffer);
                }
            }
        }



        /// <summary>
        /// Derives the output keying material of the desired length from the input key material using the optional salt and context information.
        /// </summary>
        /// <remarks>
        /// This method performs the full HKDF cycle: first extracts a pseudorandom key from the input key material, then expands it into an output keying material.
        /// </remarks>
        /// <param name="hashAlgorithmName">The hash algorithm to be used by the HMAC primitive. Supported hash functions: MD5, SHA1, SHA256, SHA384, SHA512.</param>
        /// <param name="ikm">The input key material.</param>
        /// <param name="outputLength">The desired length of the generated output keying material in bytes. Minimum value - 1. Maximum value - 255 times the size of the hash algorithm output, i.e.: MD5 - 4080, SHA1 - 5100, SHA256 - 8160, SHA384 - 12240, SHA512 - 16320.</param>
        /// <param name="salt">The optional salt value. If the argument is omitted or its value is set to <c>null</c>, the key derivation is performed without a salt.</param>
        /// <param name="info">The optional context-specific information. If the argument is omitted or its value is set to <c>null</c>, the key derivation is performed without context information.</param>
        /// <returns>The output keying material.</returns>
        /// <exception cref="ArgumentNullException">The argument <paramref name="ikm"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The hash algorithm specified in the parameter <paramref name="hashAlgorithmName"/> is not supported or the value of the parameter <paramref name="outputLength"/> is invalid (either too small or too large).</exception>

        public static byte[] DeriveKey(HashAlgorithmName hashAlgorithmName, byte[] ikm, int outputLength, byte[]? salt = null, byte[]? info = null)

        {
            if (!TryGetHashOutputLength(hashAlgorithmName, out int hashOutputLength))
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), "The specified hash algorithm is not supported.");
            if (ikm == null)
                throw new ArgumentNullException(nameof(ikm), "The input key material cannot be null.");
            if (outputLength <= 0)
                throw new ArgumentOutOfRangeException(nameof(outputLength), "The specified output length is too small. The minimum size of the output is 1 byte.");
            if (outputLength > hashOutputLength * maxOutputLengthCoef)
                throw new ArgumentOutOfRangeException(nameof(outputLength), $"The specified output length is too large. The maximum size of the output is {maxOutputLengthCoef} times the hash-output-length, i.e. {hashOutputLength * maxOutputLengthCoef} bytes in case of {hashAlgorithmName}.");

            return PerformKeyDerivation(hashAlgorithmName, hashOutputLength, ikm, salt, info, outputLength);
        }


        /// <summary>
        /// Derives the output keying material of the desired length from the input key material using the provided salt and context information.
        /// </summary>
        /// <remarks>
        /// This method performs the full HKDF cycle: first extracts a pseudorandom key from the input key material, then expands it into an output keying material.
        /// </remarks>
        /// <param name="hashAlgorithmName">The hash algorithm to be used by the HMAC primitive. Supported hash functions: MD5, SHA1, SHA256, SHA384, SHA512.</param>
        /// <param name="ikm">The input key material.</param>
        /// <param name="output">The buffer to receive the generated output keying material (OKM). The OKM produced is of the same size as the buffer. The minimum buffer size - 1 byte, maximum buffer size - 255 times the size of the hash algorithm output in bytes (i.e., MD5 - 4080, SHA1 - 5100, SHA256 - 8160, SHA384 - 12240, SHA512 - 16320).</param>
        /// <param name="salt">The salt value.</param>
        /// <param name="info">The optional context-specific information. If the argument is an empty span, the key derivation is performed without context information.</param>
        /// <exception cref="ArgumentException">The length of the <paramref name="output"/> is either too small or too large.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The hash algorithm specified in the parameter <paramref name="hashAlgorithmName"/> is not supported.</exception>
        public static void DeriveKey(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, Span<byte> output, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info)
        {
            if (!TryGetHashOutputLength(hashAlgorithmName, out int hashOutputLength))
                throw new ArgumentOutOfRangeException(nameof(hashAlgorithmName), "The specified hash algorithm is not supported.");
            if (output.Length <= 0)
                throw new ArgumentException("The supplied output buffer is too small. The minimum size of the output buffer is 1 byte.", nameof(output));
            if (output.Length > hashOutputLength * maxOutputLengthCoef)
                throw new ArgumentException($"The supplied output buffer is too large. The maximum size of the output buffer is {maxOutputLengthCoef} times the hash-output-length, i.e. {hashOutputLength * maxOutputLengthCoef} bytes in case of {hashAlgorithmName}.", nameof(output));

            // If the output span and the info span overlap, writing to the output will modify the info;
            // the following output blocks (if any) will be computed for a different info, rendering the output invalid.
            // We overcome this by using a temporary buffer the size of the info or the output (whichever is the smallest).

            if (output.Overlaps(info))
            {
                int bufferSize = Math.Min(output.Length, info.Length);
                if (bufferSize <= maxStackallocBufferSize)
                {
                    Span<byte> buffer = stackalloc byte[bufferSize];
                    PerformKeyDerivationWithOverlappingInfoAndOutput(hashAlgorithmName, hashOutputLength, ikm, salt, info, output, buffer);
                }
                else
                {
                    Span<byte> buffer = new byte[bufferSize];
                    unsafe
                    {
                        fixed (byte* bufferPointer = buffer)
                        {
                            PerformKeyDerivationWithOverlappingInfoAndOutput(hashAlgorithmName, hashOutputLength, ikm, salt, info, output, buffer);
                        }
                    }
                }
            }
            else
            {
                PerformKeyDerivation(hashAlgorithmName, hashOutputLength, ikm, salt, info, output);
            }

            static void PerformKeyDerivationWithOverlappingInfoAndOutput(HashAlgorithmName hashAlgorithmName, int hmacOutputLength, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info, Span<byte> output, Span<byte> buffer)
            {
                try
                {
                    if (output.Length < info.Length)
                    {
                        PerformKeyDerivation(hashAlgorithmName, hmacOutputLength, ikm, salt, info, buffer);
                        buffer.CopyTo(output);
                    }
                    else
                    {
                        info.CopyTo(buffer);
                        PerformKeyDerivation(hashAlgorithmName, hmacOutputLength, ikm, salt, buffer, output);
                    }
                }
                finally
                {
                    Clear(buffer);
                }
            }
        }





        private static byte[] PerformExtraction(HashAlgorithmName hashAlgorithmName, byte[] ikm, byte[]? salt)
        {
            // RFC 5869 section 2.2 states that if the salt is not provided,
            // it should be set to a HashLen (length of the output of the hash function) of zeros,
            // i.e., a byte[HashLen] should be allocated.
            // However, since under the hood the salt is used as the HMAC key
            // and internally HMAC pads the supplied key with zeros for it to fit the length of the block of the underlying hash function (RFC 2104 section 2),
            // which is larger than the HashLen for all the supported hash algorithms (MD5, SHA1, SHA256, SHA384 and SHA512),
            // then the allocation of byte[HashLen] is not necessary and instead an empty byte array can be used.

            if (salt == null)
                salt = Array.Empty<byte>();

            // We're using the incremental version of HMAC because it is faster than the non-incremental one.

            using (var hmac = IncrementalHash.CreateHMAC(hashAlgorithmName, salt))
            {
                hmac.AppendData(ikm);
                return hmac.GetHashAndReset();
            }
        }


        private static int PerformExtraction(HashAlgorithmName hashAlgorithmName, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, Span<byte> prk)
        {
            // We're using the incremental version of HMAC because it is faster than the non-incremental one.

            using var hmac = IncrementalHash.CreateHMAC(hashAlgorithmName, salt);
            hmac.AppendData(ikm);

            // The method is supposed to always return true, because the destination buffer will always
            // be large enough to accommodate the HMAC value. See the docs:
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.incrementalhash.trygethashandreset?view=net-5.0
            // However, we still check the returned value just to be on the safe side.

            if (!hmac.TryGetHashAndReset(prk, out int bytesExtracted))
                throw new CryptographicException("Failed to compute the MAC during the Extract stage of HKDF.");

            return bytesExtracted;
        }




        private static byte[] PerformExpansion(HashAlgorithmName hashAlgorithmName, int hmacOutputLength, byte[] prk, byte[]? info, int outputLength)
        {
            byte[] output = new byte[outputLength];
            try
            {
                PerformExpansion(hashAlgorithmName, hmacOutputLength, prk, info, output);
            }
            catch
            {
                Clear(output);
                throw;
            }

            return output;
        }

        private static void PerformExpansion(HashAlgorithmName hashAlgorithmName, int hmacOutputLength, ReadOnlySpan<byte> prk, ReadOnlySpan<byte> info, Span<byte> output)
        {
            Span<byte> counter = stackalloc byte[1];
            Span<byte> previousBlock = Span<byte>.Empty;

            using (var hmac = IncrementalHash.CreateHMAC(hashAlgorithmName, prk))
            {
                int wholeBlockCount = output.Length / hmacOutputLength;
                for (int i = 1; i <= wholeBlockCount; i++)
                {
                    counter[0] = (byte)i;
                    var currentBlock = output.Slice((i - 1) * hmacOutputLength, hmacOutputLength);
                    GenerateOutputBlock(hmac, previousBlock, info, counter, currentBlock);
                    previousBlock = currentBlock;
                }

                int bytesWritten = wholeBlockCount * hmacOutputLength;
                int bytesLeft = output.Length - bytesWritten;
                if (bytesLeft > 0)
                {
                    counter[0]++;
                    Span<byte> partialBlock = stackalloc byte[hmacOutputLength];
                    try
                    {
                        GenerateOutputBlock(hmac, previousBlock, info, counter, partialBlock);
                        partialBlock.Slice(0, bytesLeft)
                                    .CopyTo(output.Slice(bytesWritten));
                    }
                    finally
                    {
                        Clear(partialBlock);
                    }
                }
            }
        }

        private static byte[] PerformKeyDerivation(HashAlgorithmName hashAlgorithmName, int hmacOutputLength, byte[] ikm, byte[]? salt, byte[]? info, int outputLength)
        {
            byte[] output = new byte[outputLength];
            try
            {
                PerformKeyDerivation(hashAlgorithmName, hmacOutputLength, ikm, salt, info, output);
            }
            catch
            {
                Clear(output);
                throw;
            }

            return output;
        }

        private static void PerformKeyDerivation(HashAlgorithmName hashAlgorithmName, int hmacOutputLength, ReadOnlySpan<byte> ikm, ReadOnlySpan<byte> salt, ReadOnlySpan<byte> info, Span<byte> output)
        {
            Span<byte> prk = stackalloc byte[hmacOutputLength];
            try
            {
                PerformExtraction(hashAlgorithmName, ikm, salt, prk);
                PerformExpansion(hashAlgorithmName, hmacOutputLength, prk, info, output);
            }
            finally
            {
                Clear(prk);
            }
        }



        private static bool TryGetHashOutputLength(HashAlgorithmName hashAlgorithmName, out int outputLength)
        {
            if (hashAlgorithmName == HashAlgorithmName.SHA256)
            {
                outputLength = 32;
                return true;
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA384)
            {
                outputLength = 48;
                return true;
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA512)
            {
                outputLength = 64;
                return true;
            }
            else if (hashAlgorithmName == HashAlgorithmName.SHA1)
            {
                outputLength = 20;
                return true;
            }
            else if (hashAlgorithmName == HashAlgorithmName.MD5)
            {
                outputLength = 16;
                return true;
            }
            else
            {
                outputLength = 0;
                return false;
            }
        }

        private static bool IsHashAlgorithmSupported(HashAlgorithmName hashAlgorithmName)
        {
            return TryGetHashOutputLength(hashAlgorithmName, out _);
        }


        private static int DividePositiveIntegersRoundingUp(int dividend, int divisor)
        {
            if (dividend <= 0)
                throw new ArgumentOutOfRangeException(nameof(dividend));
            if (divisor <= 0)
                throw new ArgumentOutOfRangeException(nameof(divisor));

            int quotient = dividend / divisor;
            int remainder = dividend % divisor;

            if (remainder == 0)
                return quotient;
            else
                return quotient + 1;
        }


        private static byte[] GenerateOutputBlock(IncrementalHash hmac, byte[] previousOutputBlock, byte[] info, byte[] counter)
        {
            hmac.AppendData(previousOutputBlock);
            hmac.AppendData(info);
            hmac.AppendData(counter);

            return hmac.GetHashAndReset();
        }


        private static void GenerateOutputBlock(IncrementalHash hmac, ReadOnlySpan<byte> previousOutputBlock, ReadOnlySpan<byte> info, ReadOnlySpan<byte> counter, Span<byte> outputBlock)
        {
            if (previousOutputBlock.Length > 0)
            {
                hmac.AppendData(previousOutputBlock);
            }
            if (info.Length > 0)
            {
                hmac.AppendData(info);
            }
            if (counter.Length > 0)
            {
                hmac.AppendData(counter);
            }          

            // The method is supposed to always return true, because the destination buffer will always
            // be large enough to accommodate the HMAC value. See the docs:
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.incrementalhash.trygethashandreset?view=netstandard-2.1
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.incrementalhash.trygethashandreset?view=net-5.0
            // However, we still check the returned value just to be on the safe side.

            if (!hmac.TryGetHashAndReset(outputBlock, out _))
                throw new CryptographicException("Failed to compute the MAC for the current HKDF block.");
        }



        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Clear(byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Clear(Span<byte> buffer)
        {
            buffer.Clear();
        }


    }
}