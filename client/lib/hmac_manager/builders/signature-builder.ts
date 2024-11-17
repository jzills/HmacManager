import { getByteArray, getKeyBytes, getSignature, getUnicodeForm } from "../utilities/hmac-utilities.js";
import { Algorithm } from "../components/algorithm.js";
import { HashAlgorithm } from "../hash-algorithm.js";

/**
 * Class responsible for building a signature using HMAC.
 */
export class SignatureBuilder {
    /**
     * Represents the private key used for HMAC signature generation.
     */
    private readonly privateKey: string;

    /**
     * The content to be signed during the HMAC signature process.
     */
    private readonly signingContent: string;

    /**
     * The hashing algorithm used for signing, such as SHA1, SHA256, or SHA512.
     */
    private readonly algorithm: Algorithm;

    /**
     * Initializes a new instance of the SignatureBuilder.
     * @param privateKey The private key used for signing.
     * @param signingContent The content to sign.
     * @param signatureHashAlgorithm The hash algorithm to use (default is "sha-256").
     */
    constructor(
        privateKey: string,
        signingContent: string,
        signatureHashAlgorithm: string = HashAlgorithm.SHA256
    ) {
        this.privateKey = privateKey;
        this.signingContent = signingContent;
        this.algorithm = { name: "HMAC", hash: signatureHashAlgorithm };
    }

    /**
     * Builds the HMAC signature asynchronously.
     * @returns A promise that resolves to the base64-encoded signature.
     */
    build = async () => {
        const signatureBytes = await getSignature(
            await getKeyBytes(this.privateKey, this.algorithm),
            getByteArray(this.signingContent)
        );

        const unicodeForm = getUnicodeForm(signatureBytes);
        return btoa(unicodeForm);
    }
}