/**
 * Represents the HMAC signature details for a request.
 */
type HmacSignature = {
    /** The content used to generate the HMAC signature, often a hashed string of request data. */
    signingContent: string;

    /** The generated HMAC signature based on the signing content and private key. */
    signature: string;
}

export default HmacSignature;