/**
 * Represents a cryptographic algorithm with its associated name and hash function.
 */
type Algorithm = {
    
    /** The name of the algorithm (e.g., "HMAC", "SHA-256"). */
    name: string;

    /** The hash function used by the algorithm (e.g., "SHA-256"). */
    hash: string;
};

export default Algorithm;