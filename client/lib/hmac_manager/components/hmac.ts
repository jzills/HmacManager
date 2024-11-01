/**
 * Represents the HMAC data used for signing requests.
 */
export type Hmac = {
    /** The date and time when the request was created. */
    dateRequested: Date;
    
    /** A unique string for each request to prevent replay attacks. */
    nonce: string;
    
    /** The content to be signed, typically a hash of the request body. */
    signingContent: string;
    
    /** The list of headers included in the signature, or null if none. */
    signedHeaders: string[] | null;
    
    /** The generated HMAC signature. */
    signature: string;
}