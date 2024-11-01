import { HmacScheme } from "./hmac-scheme.js";

/**
 * Represents an HMAC policy configuration.
 */
export type HmacPolicy = {
    /** The name of the HMAC policy. */
    name: string;

    /** The public key used for HMAC signing. */
    publicKey: string;

    /** The private key used for HMAC signing. */
    privateKey: string;

    /** The algorithm used to compute the content hash. */
    contentHashAlgorithm: string;

    /** The algorithm used to compute the signature hash. */
    signatureHashAlgorithm: string;

    /** The schemes associated with this HMAC policy. */
    schemes: HmacScheme[];
};