/**
 * Enum representing the supported hashing algorithms for HMAC authentication.
 */
enum HashAlgorithm {
    /**
     * Secure Hash Algorithm 1 (SHA-1).
     * Considered less secure than newer algorithms; use with caution.
     */
    SHA1 = "sha-1",

    /**
     * Secure Hash Algorithm 256 (SHA-256).
     * Commonly used and recommended for most cryptographic operations.
     */
    SHA256 = "sha-256",

    /**
     * Secure Hash Algorithm 512 (SHA-512).
     * Provides a higher level of security at the cost of larger hash size.
     */
    SHA512 = "sha-512"
}

export default HashAlgorithm;