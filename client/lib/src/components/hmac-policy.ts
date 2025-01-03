import SigningContentAccessor from "../builders/signing-content-accessor";
import HashAlgorithm from "../hash-algorithm";
import HmacScheme from "./hmac-scheme";

/**
 * Represents an HMAC policy configuration.
 */
type HmacPolicy = {
    /** The name of the HMAC policy. */
    name: string;

    /** The public key used for HMAC signing. */
    publicKey: string;

    /** The private key used for HMAC signing. */
    privateKey: string;

    /** The algorithm used to compute the content hash. */
    contentHashAlgorithm: HashAlgorithm;

    /** The algorithm used to compute the signature hash. */
    signatureHashAlgorithm: HashAlgorithm;

    /** The schemes associated with this HMAC policy. */
    schemes: HmacScheme[];

    signingContentAccessor?: SigningContentAccessor;
};

export default HmacPolicy;